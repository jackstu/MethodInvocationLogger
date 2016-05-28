# MethodInvocationLogger

This library allows to log invocations of specified methods declared in classes which instances are resolved using dependency injection. Currently `Castle.Windsor` is the only supported container, but I plan to add others. Logger could be usefull in application performance monitor (analyzing duration of method execution), creating events for BI and many others because logs output could be anything. 
The most important advantage is separation of concerns - logging is completly separated from business logic.

```csharp
// log execution of PutSomeData method in SomeApiClient
logger.LogInvocationOf<SomeApiClient>(t => t.PutSomeData(Input.Param<FormData>("formData"), Input.Param<Data>()))
				.WithInvocationTime() // log time of execution
				.WithExecutionDuration() // log duration of execution 
				.WithAllArguments() // log every argument of a method
				.WithMethodName() // log name of a method
				.WithAdditionalData<int, IUserContext>((context, data) => context.UserId, "UserId") // log userId from current user context
				.OnlyIfSucceeded() // log only if method succeeded
				.OnCondition((container, data) => data.Arguments.Get<FormData>("formData").FormType != FormType.FormABC); // log only for specifed input
```
## Usage

Install logger using following NuGet command:
```
  PM> Install-Package MethodInvocationLogger.Castle
  ```
This will install two packages: `MethodInvocationLogger` and `MethodInvocationLogger.Castle`

Let's say you've got a Component class with Test() and Test(int x) methods.
```csharp
interface IComponent
{
    void Test();
    void Test(int x);
}

class TestComponent : IComponent
{
    public void Test()
    {
        // some stuff
    }
    public void Test(int x)
    {
        // some stuff
    }
}
```
and you want log every invocation of `Test()` and `Test(int x)` methods.

First you need to pick a class which will carry information about method's invocations. You could create your own class or use universal `DictionaryLogData` supplied by the library. Actually, it is a simple `Dictionary<string, object>`

Then you have to pick a logger output. There are two sample writers in this repository.

- `ElasticSearchLogWriter`, which writes logs into `ElasticSearch` server. With `Kibana` you can create very useful real-time dashboards ( [http://i.imgur.com/ONPB1fe.png] - this is a performance monitor of a microservice in a big commercial system which I am working on)

- `SqlDataWriter` - usefull for giving business events to BI warehouse.

Although for this example it will be simple custom console writer
```csharp
class ConsoleOutput : ILogOutput<DictionaryLogData>
{
    public void WriteLog(DictionaryLogData logData)
    {
        Console.WriteLine(DateTime.Now + " - " + string.Join(" ", logData.Select(i => i.Key + ":" + i.Value)));
    }
}
```
Now you've got everything you need to create and initialize a logger
```csharp
var logger = LoggerFactory.Create<DictionaryLogData>()
		.WriteTo(new ConsoleOutput())
		.BindToWindsor(container.Kernel);
```
Configure logger to log every execution of `Test()` and `Test(int x)` methods in `Component`
```csharp
logger.LogInvocationOf<TestComponent>(c => c.Test());
logger.LogInvocationOf<TestComponent>(c => c.Test(Input.Param<int>()));
```
It is important to configure logger (set up methods you want to log) before you register components in a container. To be sure that everything is ok you should execute `Validate()` after you configured logger and registered components in a container.
```csharp
container.Register(Component.For<IComponent>().ImplementedBy<TestComponent>());
logger.Validate(container.Kernel);
```
This method will throw an exception if something is wrong with logger configuration.

Now if you resolve `TestComponent` from a container and execute `Test()`
```csharp
container.Resolve<IComponent>().Test();
```
a method `WriteLog()` in `ConsoleOutput` will be invoked and print a message:
```
2016-05-28 09:59:04 -
```
So, method's invocation was logged but `ConsoleOutput` was given empty `logData`. To fulfill this object you need to set `PrepareLogData` action in a logger configuration for specified method. For example:
```csharp
logger.LogInvocationOf<TestComponent>(c => c.Test())
        .PrepareLogData((container1, invocationData, logData) =>
        {
            logData["duration"] = invocationData.Raw.Duration;
        });
```
`PrepareLogData` action will be executed after specified method finished. It has 3 arguments.
- `container` - abstraction of a dependency injection container, which allows to resolving components.
- `invocationData` - contains informations about invoked method: passed arguments, duration of execution, exception if method failed, returned value, instance of an object on which method was executed and few others
- `logData` - an object which will be passed to `LogOutput`.

You could set as many `PrepareLogData` actions as you want. They will execute synchronously. If you discover that some of them are duplicated for many methods you want to log, it is useful to create extension methods from them. For example if you often want to log currently logged user
```csharp
logger.LogInvocationOf<TestComponent>(c => c.Test())				
				.PrepareLogData((container1, invocationData, logData) =>
				{
					IUserContext userContext = container1.Resolve<IUserContext>();
					logData["LoggedUser"] = userContext.UserName;
					container1.Release(userContext);
				});
logger.LogInvocationOf<TestComponent>(c => c.Test(Input.Param<int>())
				.PrepareLogData((container1, invocationData, logData) =>
				{
					IUserContext userContext = container1.Resolve<IUserContext>();
					logData["LoggedUser"] = userContext.UserName;
					container1.Release(userContext);
				});
```
You could create an extension method
```csharp
public static class ExtensionMethods
{
		public static MethodInvocationLoggingConfiguration<DictionaryLogData> WithCurrentUserName(
			this MethodInvocationLoggingConfiguration<DictionaryLogData> config)
		{
		    return config.PrepareLogData(((container, invocationData, logData) =>
			  {
  				IUserContext userContext = container.Resolve<IUserContext>();
				  logData["LoggedUser"] = userContext.UserName;
				  container.Release(userContext);
			  }));
		}
}
```
and use it like this
```csharp
logger.LogInvocationOf<TestComponent>(c => c.Test())
				.WithCurrentUserName()
logger.LogInvocationOf<TestComponent>(c => c.Test(Input.Param<int>())
				.WithCurrentUserName()
```
If you are using `DictionaryLogData` you could use many predefined extension methods from `MethodInvocationLogger.Extensions` namespace.

##Filters
If there is a need to log method invocation only in certain circumstances, you could use `OnCondition` function. Let say that you want to log invocation of method `Test(int x)` only if `x` is more than `10`
```csharp
logger.LogInvocationOf<TestComponent>(c => c.Test(Input.Param<int>())
	.OnCondition((container1, data) => data.Arguments.Get<int>("x") > 10);
```
Here `data` is the same object as in the `PrepareLogData` action and same as with `PrepareLogData` it's useful to aggregate `OnCondition` functions using extension methods, which makes configuring logger nice and fluent.

##Naming arguments

You could retrieve arguments by their names (like in the example above) or by index. Retrieving by names could be risky because if you change argument's name in a method signature and forget to update it in a logger configuration, there will be no compilation error but method will throw an exception. To prevent this you can name arguments in a logging scope using `Input.Param<int>(string name)`
```csharp
logger.LogInvocationOf<TestComponent>(c => c.Test(Input.Param<int>("ourParamName"))
	.OnCondition((container1, data) => data.Arguments.Get<int>("ourParamName") > 10);
```
Now it doesn't matter what names have parameters in method signature.

##Demo
There is a demo web aplication in a repository (`MethodInvocationLogger.Demo`), which is an example of using loggers as a performance tool and as business event generator. The logging code is in `global.asax.cs` file.


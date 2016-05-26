using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using MethodInvocationLogger.Castle;
using MethodInvocationLogger.Demo.Controllers;
using MethodInvocationLogger.Demo.Plumbing;
using MethodInvocationLogger.Extensions;
using Newtonsoft.Json;

namespace MethodInvocationLogger.Demo
{
	public class MvcApplication : System.Web.HttpApplication
	{
		readonly WindsorContainer _container = new WindsorContainer();

		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);

			BusinessEventsList businessEventsList = new BusinessEventsList();
			PerformanceItemsList performanceItemsList = new PerformanceItemsList();

			#region Logger Config

			// create and initialize logger for business events. We are using custom BusinessEvent class to store event's data
			var businessEventsLogger = LoggerFactory.Create<BusinessEvent>()
				.BindToWindsor(_container.Kernel)
				// logs will be written into businessEventsList collection
				.WriteTo(new BusinessEventLogWriter(businessEventsList));

			// Log event when user click "Enable cyclic api caller" button and controller's method would not throw exception
			// With event we want to store current time and user's name from current context
			// --- Plecase check .AsBusinessEvent extensions method implementation
			businessEventsLogger.LogInvocationOf<HomeController>(t => t.EnableCyclicApiCaller())
				.AsBusinessEvent("EnableCyclicApiCallerClickedEvent");

			// same as above but with "Disable cyclic api caller" button
			businessEventsLogger.LogInvocationOf<HomeController>(t => t.DisableCyclicApiCaller())
				.AsBusinessEvent("DisableCyclicApiCallerClickedEvent");

			// Log event when user send a form
			businessEventsLogger.LogInvocationOf<HomeController>(t => t.SendSomeForm(Input.Param<FormData>("model")))
				// Same as above but name of the event depends on selected radiobutton option
				.AsBusinessEvent((container, invocationData) =>
				{
					switch (invocationData.Arguments.Get<FormData>("model").FormType)
					{
						case FormType.SimpleForm:
							return "SimpleFormSentEvent";
						case FormType.AdvancedForm:
							return "AdvancedFormSendEvent";
						default:
							return null;
					}
				})
				// Also, we don't want log event if user select "FormABC" option
				.OnCondition((container, data) => data.Arguments.Get<FormData>("model").FormType != FormType.FormABC)
				// Except of standard event data (UserName and time of occurence) we want to log given formTitle
				.WithAdditionalData((container, data) => data.Arguments.Get<FormData>("model").Title, "FormTitle")
				// and url referrer
				.WithAdditionalData((container, data) => ((Controller) data.Raw.Target).Request.UrlReferrer, "UrlReferrer");

			// create and initialize logger for performance monitor. We are using DictionaryLogData supplied by MethodInvocationLogger.Extensions.
			var performanceLogger = LoggerFactory.Create<DictionaryLogData>()
				.BindToWindsor(_container.Kernel)
				.WriteTo(new PerformanceLogWriter(performanceItemsList)); // logs will be written into performanceItemsList collection 

			// log every execution of PutSomeData method in SomeApiClient
			performanceLogger.LogInvocationOf<SomeApiClient>(t => t.PutSomeData(Input.Param<string>(), Input.Param<Data>()))
				.WithInvocationTime() // log time of the exection
				.WithInvocationDuration() // log duration of execution 
				.WithAllArguments() // log every argument of the method
				.WithMethodName() // log method name
				.WithAdditionalData<int, IUserContext>((context, data) => context.UserId, "UserId") // log userId from current user context
				.WithExceptionIfThrown(); // log full exception if method failed

			// here we are logging every execution of GetSomeData method with same additional values as above, but we use 
			// one single configuration method "WithEverythingNecessaryForPerformanceMonitor" (please check its implementation)
			performanceLogger.LogInvocationOf<SomeApiClient>(t => t.GetSomeData())
				.WithEverythingNecessaryForPerformanceMonitor()
				.WithReturnedValue(); // and method's returned value

			#endregion


			ControllerBuilder.Current.SetControllerFactory(new WindsorControllerFactory(_container));
			_container.Register(Classes.FromThisAssembly().BasedOn<IController>().LifestyleTransient());

			_container.Register(
				Component.For<IUserContext>().ImplementedBy<UserContext>(),
				Component.For<BusinessEventsList>().Instance(businessEventsList),
				Component.For<PerformanceItemsList>().Instance(performanceItemsList),
				Component.For<IUserNameRetriever>().ImplementedBy<UserNameRetriever>().LifestyleSingleton(),
				Component.For<ISomeApiClient>().ImplementedBy<SomeApiClient>().LifestyleSingleton(),
				Component.For<CyclicApiCaller>().ImplementedBy<CyclicApiCaller>().LifestyleSingleton()
				);

			// Additional validation to check if everything is configured properly
			businessEventsLogger.Validate(_container.Kernel);
			performanceLogger.Validate(_container.Kernel);
		}
	}

	public static class PerformanceLoggingConfigurationExtensions
	{
		public static MethodInvocationLoggingConfiguration<DictionaryLogData> WithEverythingNecessaryForPerformanceMonitor(
			this MethodInvocationLoggingConfiguration<DictionaryLogData> config)
		{
			return config
				.WithInvocationTime()
				.WithInvocationDuration()
				.WithAllArguments()
				.WithMethodName()
				.WithAdditionalData<int, IUserContext>((context, data) => context.UserId, "UserId")
				.WithExceptionIfThrown();
		}
	}

	public static class BusinessEventLoggingConfigurationExtensions
	{
		public static MethodInvocationLoggingConfiguration<BusinessEvent> AsBusinessEvent(
			this MethodInvocationLoggingConfiguration<BusinessEvent> config, string eventName)
		{
			return config.AsBusinessEvent((container, invocationData) => eventName);
		}

		public static MethodInvocationLoggingConfiguration<BusinessEvent> AsBusinessEvent(
			this MethodInvocationLoggingConfiguration<BusinessEvent> config, Func<IContainer, InvocationData, string> eventNameRetrieveFunction)
		{
			return config
				.OnlyIfSucceded()
				.WithUserNameFromCurrentUserContext()
				.PrepareLogData(((container, invocationData, logData) =>
				{
					logData.EventName = eventNameRetrieveFunction(container, invocationData);
					logData.Time = DateTime.Now;
				}));
		}

		public static MethodInvocationLoggingConfiguration<BusinessEvent> WithUserNameFromCurrentUserContext(
			this MethodInvocationLoggingConfiguration<BusinessEvent> config)
		{
			return config.PrepareLogData(((container, invocationData, logData) =>
			{
				var currentUserContext = container.Resolve<IUserContext>();
				var userNameRetriever = container.Resolve<IUserNameRetriever>();

				logData.UserName = userNameRetriever.GetUserName(currentUserContext.UserId);

				container.Release(userNameRetriever);
				container.Release(currentUserContext);
			}));
		}

		public static MethodInvocationLoggingConfiguration<BusinessEvent> OnlyIfSucceded(
			this MethodInvocationLoggingConfiguration<BusinessEvent> config)
		{
			return config.OnCondition((container, invocationData) => invocationData.Raw.Exception == null);
		}

		public static MethodInvocationLoggingConfiguration<BusinessEvent> WithAdditionalData(
			this MethodInvocationLoggingConfiguration<BusinessEvent> config, Func<IContainer, InvocationData, object> additionalDataRetriever, string additionalDataKey)
		{
			return config.PrepareLogData((container, data, logData) =>
			{
				logData.AdditionalData.Add(additionalDataKey, additionalDataRetriever(container, data));
			});
		}
	}

	public class BusinessEvent
	{
		public string EventName { get; set; }
		public string UserName { get; set; }
		public DateTime Time { get; set; }
		public Dictionary<string, object> AdditionalData { get; set; } = new Dictionary<string, object>();
	}

	public class BusinessEventLogWriter : ILogWriter<BusinessEvent>
	{
		private readonly BusinessEventsList _eventStorage;

		public BusinessEventLogWriter(BusinessEventsList eventStorage)
		{
			_eventStorage = eventStorage;
		}

		public void WriteLog(BusinessEvent logData)
		{
			_eventStorage.AddEvent(logData);
		}
	}

	public class PerformanceLogWriter : ILogWriter<DictionaryLogData>
	{
		private readonly PerformanceItemsList _storage;

		public PerformanceLogWriter(PerformanceItemsList storage)
		{
			_storage = storage;
		}

		public void WriteLog(DictionaryLogData logData)
		{
			_storage.AddItem(logData);
		}
	}
}

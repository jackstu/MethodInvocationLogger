using MethodInvocationLogger.Exceptions;
using MethodInvocationLogger.Tests.Helpers;
using NUnit.Framework;

namespace MethodInvocationLogger.Tests
{
	public abstract class ArgumentsNamingTests : BaseTests<object>
	{
		[Test]
		public void GivingTwoArgumentsSameNameShouldThrowDuplicateParameterNameException()
		{
			RunTest((container, logger) =>
			{
				Assert.Throws<DuplicateParameterNameException>(() =>
				{
					logger.LogInvocationOf<InvocationTestClass>(
						t => t.TestMethod(Input.Param<string>("sameName"), Input.Param<string>("sameName"), Input.Param<string>()));
				});
			});
		}

		[Test]
		public void ArgumentsWithoutSpecifiedNamesShouldHaveNamesSameAsMethodParameteres()
		{
			RunTest((container, logger) =>
			{
				logger.LogInvocationOf<InvocationTestClass>(t => t.TestMethod(Input.Param<string>(), Input.Param<string>(), Input.Param<string>()))
					.PrepareLogData((container1, data, logData) =>
					{
						Assert.AreEqual("a", data.Arguments.Get<string>("first"));
						Assert.AreEqual("b", data.Arguments.Get<string>("second"));
						Assert.AreEqual("c", data.Arguments.Get<string>("last"));
					});

				RegisterAndResolveComponent<InvocationTestClass>(container).TestMethod("a", "b", "c");
			});
		}
		
		[Test]
		public void ArgumentsNamesShouldBeSameAsSpecifiedInInputParam()
		{
			RunTest((container, logger) =>
			{
				logger.LogInvocationOf<InvocationTestClass>(t => t.TestMethod(Input.Param<string>("param1"), Input.Param<string>("param2"), Input.Param<string>("param3")))
					.PrepareLogData((container1, data, logData) =>
					{
						Assert.AreEqual("a", data.Arguments.Get<string>("param1"));
						Assert.AreEqual("b", data.Arguments.Get<string>("param2"));
						Assert.AreEqual("c", data.Arguments.Get<string>("param3"));
					});

				RegisterAndResolveComponent<InvocationTestClass>(container).TestMethod("a", "b", "c");
			});
		}

		[Test]
		public void GettingNotExistedArgumentShouldThrowUnknownArgumentException()
		{
			RunTest((container, logger) =>
			{
				logger.LogInvocationOf<InvocationTestClass>(t => t.TestMethod(Input.Param<string>("param1"), Input.Param<string>("param2"), Input.Param<string>("param3")))
					.PrepareLogData((container1, data, logData) =>
					{
						Assert.Throws<UnknownArgumentException>(() => data.Arguments.Get<string>("param123"));
					});

				RegisterAndResolveComponent<InvocationTestClass>(container).TestMethod("a", "b", "c");
			});
		}

		[Test]
		public void ArgumentsNamesShouldBeCaseSensitive()
		{
			RunTest((container, logger) =>
			{
				logger.LogInvocationOf<InvocationTestClass>(t => t.TestMethod(Input.Param<string>("First"), Input.Param<string>(), Input.Param<string>()))
					.PrepareLogData((container1, data, logData) =>
					{
						Assert.AreEqual("a", data.Arguments.Get<string>("First"));
						Assert.Throws<UnknownArgumentException>(() => data.Arguments.Get<string>("first"));
					});

				RegisterAndResolveComponent<InvocationTestClass>(container).TestMethod("a", "b", "c");
			});
		}

		public class InvocationTestClass
		{
			public virtual void TestMethod(string first, string second, string last) { }
		}
	}
}
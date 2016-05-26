using System;
using System.Reflection;
using MethodInvocationLogger.Tests.Helpers;
using NUnit.Framework;

namespace MethodInvocationLogger.Tests
{
	public abstract class InvocationRawDataFillTests : BaseTests<object>
	{
		[Test]
		public void InvocationRawDataShouldContainCorrectMethodInfo()
		{
			RunTest((container, logger) =>
			{
				MethodInfo methodInfo = typeof (InvocationTestClass).GetMethod("TestMethod", new Type[0]);
				logger.LogInvocationOf<InvocationTestClass>(t => t.TestMethod())
					.PrepareLogData((container1, data, logData) =>
					{
						Assert.AreEqual(methodInfo, data.Raw.MethodInfo);
					});

				RegisterAndResolveComponent<InvocationTestClass>(container).TestMethod();
			});
		}

		[Test]
		public void IfMethodThrowsExceptionInvocationRawDataShouldContainThisException()
		{
			RunTest((container, logger) =>
			{
				string testMsg = "Test";
				logger.LogInvocationOf<InvocationTestClass>(t => t.ExceptionThrowingTestMethod(Input.Param<string>()))
					.PrepareLogData((container1, data, logData) =>
					{
						TestException exc = data.Raw.Exception as TestException;

						Assert.IsInstanceOf<TestException>(exc);
						Assert.AreEqual(testMsg, exc?.Message);
					});

				Assert.Catch<TestException>(() => RegisterAndResolveComponent<InvocationTestClass>(container).ExceptionThrowingTestMethod(testMsg));
			});
		}

		[Test]
		public void InvocationRawDataShouldContainArgumentsOfInvokedMethod()
		{
			RunTest((container, logger) =>
			{
				logger.LogInvocationOf<InvocationTestClass>(t => t.TestMethod(Input.Param<int>(), Input.Param<string>(), Input.Param<InvocationTestClass.TestArg>()))
					.PrepareLogData((container1, data, logData) =>
					{
						Assert.AreEqual(10, data.Raw.Arguments[0]);
						Assert.AreEqual("test", data.Raw.Arguments[1]);
						Assert.IsInstanceOf<InvocationTestClass.TestArg>(data.Raw.Arguments[2]);
						Assert.AreEqual("TestData", ((InvocationTestClass.TestArg)data.Raw.Arguments[2]).Data);
					});

				RegisterAndResolveComponent<InvocationTestClass>(container)
					.TestMethod(10, "test", new InvocationTestClass.TestArg { Data = "TestData" });
			});
		}

		[Test]
		public void InvocationRawDataShouldContainCorrectReturnValue()
		{
			RunTest((container, logger) =>
			{
				logger.LogInvocationOf<InvocationTestClass>(t => t.TestMethod(Input.Param<int>()))
					.PrepareLogData((container1, data, logData) =>
					{
						Assert.AreEqual("[10]", data.Raw.ReturnValue);
					});

				RegisterAndResolveComponent<InvocationTestClass>(container).TestMethod(10);
			});
		}

		[Test]
		public void InvocationRawDataShouldContainIntanceOfInvocationTarget()
		{
			RunTest((container, logger) =>
			{
				logger.LogInvocationOf<InvocationTestClass>(t => t.TestMethod())
					.PrepareLogData((container1, data, logData) =>
					{
						Assert.IsInstanceOf<InvocationTestClass>(data.Raw.Target);
						Assert.AreEqual("Test", ((InvocationTestClass)data.Raw.Target).TestProperty);
					});

				var component = RegisterAndResolveComponent<InvocationTestClass>(container);
				component.TestProperty = "Test";
				component.TestMethod();
			});
		}

		public class InvocationTestClass
		{
			public virtual void TestMethod() { }

			public virtual void ExceptionThrowingTestMethod(string exceptionMessage)
			{
				throw new TestException(exceptionMessage);
			}

			public virtual void TestMethod(int x, string y, TestArg z) { }

			public virtual string TestMethod(int input)
			{
				return "[" + input + "]";
			}

			public string TestProperty { get; set; }

			public class TestArg
			{
				public string Data { get; set; }
			}
		}

		public class TestException : Exception
		{
			public TestException(string message) : base(message) { }
		}
	}
}
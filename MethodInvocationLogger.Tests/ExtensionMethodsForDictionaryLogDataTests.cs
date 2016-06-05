using System;
using MethodInvocationLogger.Extensions;
using MethodInvocationLogger.Tests.Helpers;
using NUnit.Framework;

namespace MethodInvocationLogger.Tests
{
	public abstract class ExtensionMethodsForDictionaryLogDataTests : BaseTests<DictionaryLogData>
	{
		[Test]
		public void WithInvocationTime_WithDefaultDataKeyNameTest()
		{
			DateTime invocationTime = default(DateTime);
			RunTestAndCheckFirstLogDataWritten((container, logger) =>
			{
				logger.LogInvocationOf<InvocationTestClass>(m => m.TestMethod())
					.WithInvocationTime()
					.PrepareLogData((container1, data, logData) => invocationTime = data.Raw.Begin);
				RegisterAndResolveComponent<InvocationTestClass>(container).TestMethod();
			}
			, data =>
			{
				data.AssertContainsData("BeginTime");
				data.AssertDataAreEqual(invocationTime, "BeginTime");
			}
			);
		}

		[Test]
		public void WithInvocationTime_WithSpecifiedDataKeyName_Test()
		{
			DateTime invocationTime = default(DateTime);
			string dataKeyName = "test";
			RunTestAndCheckFirstLogDataWritten((container, logger) =>
			{
				logger.LogInvocationOf<InvocationTestClass>(m => m.TestMethod())
					.WithInvocationTime(dataKeyName)
					.PrepareLogData((container1, data, logData) => invocationTime = data.Raw.Begin);
				RegisterAndResolveComponent<InvocationTestClass>(container).TestMethod();
			}
			, data =>
			{
				data.AssertContainsData(dataKeyName);
				data.AssertDataAreEqual(invocationTime, dataKeyName);
			}
			);
		}

		[Test]
		public void WithInvocationDuration_WithDefaultDataKeyNameTest()
		{
			TimeSpan invocationDuration = default(TimeSpan);
			RunTestAndCheckFirstLogDataWritten((container, logger) =>
			{
				logger.LogInvocationOf<InvocationTestClass>(m => m.TestMethod())
					.WithExecutionDuration()
					.PrepareLogData((container1, data, logData) => invocationDuration = data.Raw.Duration);
				RegisterAndResolveComponent<InvocationTestClass>(container).TestMethod();
			}
			, data =>
			{
				data.AssertContainsData("Duration");
				data.AssertDataAreEqual(invocationDuration, "Duration");
			}
			);
		}

		[Test]
		public void WithInvocationDuration_WithSpecifiedDataKeyName_Test()
		{
			TimeSpan invocationDuration = default(TimeSpan);
			string dataKeyName = "test";
			RunTestAndCheckFirstLogDataWritten((container, logger) =>
			{
				logger.LogInvocationOf<InvocationTestClass>(m => m.TestMethod())
					.WithExecutionDuration(dataKeyName)
					.PrepareLogData((container1, data, logData) => invocationDuration = data.Raw.Duration);
				RegisterAndResolveComponent<InvocationTestClass>(container).TestMethod();
			}
			, data =>
			{
				data.AssertContainsData(dataKeyName);
				data.AssertDataAreEqual(invocationDuration, dataKeyName);
			}
			);
		}

		[Test]
		public void WithAdditionalData_Test()
		{
			string dataKeyName = "test";
			InvocationData invocationData = null;
			RunTestAndCheckFirstLogDataWritten((container, logger) =>
			{
				logger.LogInvocationOf<InvocationTestClass>(m => m.TestMethod())
					.WithAdditionalData((container1, data) => data, dataKeyName)
					.PrepareLogData((container1, data, logData) => invocationData = data);
				RegisterAndResolveComponent<InvocationTestClass>(container).TestMethod();
			}
			, data =>
			{
				data.AssertContainsData(dataKeyName);
				data.AssertDataAreEqual(invocationData, dataKeyName);
			}
			);
		}

		[Test]
		public void WithAdditionalData_WithComponentResolving()
		{
			string dataKeyName = "testKey";
			string componentPropValue = "testPropValue";

			RunTestAndCheckFirstLogDataWritten((container, logger) =>
			{
				container.RegisterComponent<TestComponent>();
				container.ResolveComponent<TestComponent>().TestProp = componentPropValue;

				logger.LogInvocationOf<InvocationTestClass>(m => m.TestMethod())
					.WithAdditionalData<string, TestComponent>((component, data) => component.TestProp, dataKeyName);

				RegisterAndResolveComponent<InvocationTestClass>(container).TestMethod();
			}
			, data =>
			{
				data.AssertContainsData(dataKeyName);
				data.AssertDataAreEqual(componentPropValue, dataKeyName);
			}
			);
		}

		[Test]
		public void WithArgument_Test()
		{
			RunTestAndCheckFirstLogDataWritten((container, logger) =>
			{
				logger.LogInvocationOf<InvocationTestClass>(m => m.TestMethod(Input.Param<int>(), Input.Param<string>(), Input.Param<bool>()))
					.WithArgument("y");

				RegisterAndResolveComponent<InvocationTestClass>(container).TestMethod(10, "test", true);
			}
			, data =>
			{
				data.AssertDoesntContainData("x");
				data.AssertContainsData("y");
				data.AssertDoesntContainData("z");
				data.AssertDataAreEqual("test", "y");
			}
			);
		}

		[Test]
		public void WithArguments_Test()
		{
			RunTestAndCheckFirstLogDataWritten((container, logger) =>
			{
				logger.LogInvocationOf<InvocationTestClass>(m => m.TestMethod(Input.Param<int>(), Input.Param<string>(), Input.Param<bool>()))
					.WithArguments("y","z");

				RegisterAndResolveComponent<InvocationTestClass>(container).TestMethod(10, "test", true);
			}
			, data =>
			{
				data.AssertDoesntContainData("x");
				data.AssertContainsData("y");
				data.AssertContainsData("z");
				data.AssertDataAreEqual("test", "y");
				data.AssertDataAreEqual(true, "z");
			}
			);
		}

		[Test]
		public void WithAllArguments_Test()
		{
			RunTestAndCheckFirstLogDataWritten((container, logger) =>
			{
				logger.LogInvocationOf<InvocationTestClass>(m => m.TestMethod(Input.Param<int>(), Input.Param<string>(), Input.Param<bool>()))
					.WithAllArguments();

				RegisterAndResolveComponent<InvocationTestClass>(container).TestMethod(10, "test", true);
			}
			, data =>
			{
				data.AssertContainsData("x");
				data.AssertContainsData("y");
				data.AssertContainsData("z");
				data.AssertDataAreEqual(10, "x");
				data.AssertDataAreEqual("test", "y");
				data.AssertDataAreEqual(true, "z");
			}
			);
		}

		[Test]
		public void WithExceptionIfThrown_WithSpecifiedDataKeyName_Test()
		{
			string dataKeyName = "test";

			RunTestAndCheckFirstLogDataWritten((container, logger) =>
			{
				logger.LogInvocationOf<InvocationWithExceptionTestClass>(m => m.TestMethod())
					.WithExceptionIfThrown(dataKeyName);

				Assert.Throws<TestException>(() => RegisterAndResolveComponent<InvocationWithExceptionTestClass>(container).TestMethod());
			}
			, data =>
			{
				data.AssertContainsData(dataKeyName);
				data.AssertDataIs<TestException>(dataKeyName);
			}
			);
		}

		[Test]
		public void WithExceptionIfThrown_WithDefaultDataKeyName_Test()
		{
			RunTestAndCheckFirstLogDataWritten((container, logger) =>
			{
				logger.LogInvocationOf<InvocationWithExceptionTestClass>(m => m.TestMethod())
					.WithExceptionIfThrown();

				Assert.Throws<TestException>(() => RegisterAndResolveComponent<InvocationWithExceptionTestClass>(container).TestMethod());
			}
			, data =>
			{
				data.AssertContainsData("Exception");
				data.AssertDataIs<TestException>("Exception");
			}
			);
		}

		[Test]
		public void OnlyIfSucceeded_ShouldNotLogIfExceptionThrown()
		{
			RunTestAndAssertNoLogDataWritten((container, logger) =>
			{
				logger.LogInvocationOf<InvocationWithExceptionTestClass>(m => m.TestMethod())
					.OnlyIfSucceeded();

				Assert.Throws<TestException>(() => RegisterAndResolveComponent<InvocationWithExceptionTestClass>(container).TestMethod());
			});
		}

		[Test]
		public void OnlyIfSucceeded_ShouldLogIfExceptionNotThrown()
		{
			RunTestAndAssertExactlyOneLogDataWritten((container, logger) =>
			{
				logger.LogInvocationOf<InvocationTestClass>(m => m.TestMethod())
					.OnlyIfSucceeded();

				RegisterAndResolveComponent<InvocationTestClass>(container).TestMethod();
			});
		}

		[Test]
		public void OnlyIfFailed_ShouldLogIfExceptionThrown()
		{
			RunTestAndAssertExactlyOneLogDataWritten((container, logger) =>
			{
				logger.LogInvocationOf<InvocationWithExceptionTestClass>(m => m.TestMethod())
					.OnlyIfFailed();

				Assert.Throws<TestException>(() => RegisterAndResolveComponent<InvocationWithExceptionTestClass>(container).TestMethod());
			});
		}

		[Test]
		public void OnlyIfFailed_ShouldNotLogIfExceptionNotThrown()
		{
			RunTestAndAssertNoLogDataWritten((container, logger) =>
			{
				logger.LogInvocationOf<InvocationTestClass>(m => m.TestMethod())
					.OnlyIfFailed();

				RegisterAndResolveComponent<InvocationTestClass>(container).TestMethod();
			});
		}

		[Test]
		public void WithConst_Test()
		{
			string dataKeyName = "test";
			TestData testData = new TestData();

			RunTestAndCheckFirstLogDataWritten((container, logger) =>
			{
				logger.LogInvocationOf<InvocationTestClass>(m => m.TestMethod())
					.WithConst(dataKeyName, testData);

				RegisterAndResolveComponent<InvocationTestClass>(container).TestMethod();
			}
			, data =>
			{
				data.AssertContainsData(dataKeyName);
				data.AssertDataAreEqual(testData, dataKeyName);
			});
		}

		[Test]
		public void WithMethodName_WithDefaultDataKeyName_Test()
		{
			RunTestAndCheckFirstLogDataWritten((container, logger) =>
			{
				logger.LogInvocationOf<InvocationTestClass>(m => m.TestMethod())
					.WithMethodName();

				RegisterAndResolveComponent<InvocationTestClass>(container).TestMethod();
			}
			, data =>
			{
				data.AssertContainsData("Method");
				data.AssertDataAreEqual(Method<InvocationTestClass>(m=>m.TestMethod()).ToString(), "Method");
			});
		}

		[Test]
		public void WithMethodName_WithSpecifiedDataKeyName_Test()
		{
			string dataKeyName = "test";
			RunTestAndCheckFirstLogDataWritten((container, logger) =>
			{
				logger.LogInvocationOf<InvocationTestClass>(m => m.TestMethod())
					.WithMethodName(dataKeyName);

				RegisterAndResolveComponent<InvocationTestClass>(container).TestMethod();
			}
			, data =>
			{
				data.AssertContainsData(dataKeyName);
				data.AssertDataAreEqual(Method<InvocationTestClass>(m => m.TestMethod()).ToString(), dataKeyName);
			});
		}

		public class TestComponent
		{
			public string TestProp { get; set; }
		}

		public class InvocationTestClass
		{
			public int TestProp { get; set; }
			public virtual void TestMethod() { }
			public virtual void TestMethod(int x, string y, bool z) { }
		}

		public class InvocationWithExceptionTestClass
		{
			public virtual void TestMethod()
			{
				throw new TestException();
			}
		}

		public class TestException : Exception
		{
		}

		public class TestData
		{
		}
	}
}

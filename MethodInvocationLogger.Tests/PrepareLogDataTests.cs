using MethodInvocationLogger.Tests.Helpers;
using NUnit.Framework;

namespace MethodInvocationLogger.Tests
{
	public abstract class PrepareLogDataTests : BaseTests<PrepareLogDataTests.LogDataTest>
	{
		[Test]
		public void PrepareLogDataActionShouldBeInvokedDuringMethodInterception()
		{
			bool actionInvoked = false;

			RunTest((container, logger) =>
			{
				logger.LogInvocationOf<InvocationTestClass>(t => t.TestMethod())
					.PrepareLogData((container1, data, logData) => actionInvoked = true);

				RegisterAndResolveComponent<InvocationTestClass>(container).TestMethod();
			});

			Assert.True(actionInvoked);
		}

		[Test]
		public void AllPrepareLogDataActionsShouldBeInvokedDuringMethodInterception()
		{
			bool firstActionInvoked = false;
			bool secondActionInvoked = false;
			bool thirdActionInvoked = false;

			RunTest((container, logger) =>
			{
				logger.LogInvocationOf<InvocationTestClass>(t => t.TestMethod())
					.PrepareLogData((container1, data, logData) => firstActionInvoked = true)
					.PrepareLogData((container1, data, logData) => secondActionInvoked = true)
					.PrepareLogData((container1, data, logData) => thirdActionInvoked = true);

				RegisterAndResolveComponent<InvocationTestClass>(container).TestMethod();
			});

			Assert.True(firstActionInvoked);
			Assert.True(secondActionInvoked);
			Assert.True(thirdActionInvoked);
		}

		[Test]
		public void DataPreparedDuringMethodInterceptionShouldBeLogged()
		{
			const string testData = "Test";

			RunTestAndCheckFirstLogDataWritten((container, logger) =>
			{
				logger.LogInvocationOf<InvocationTestClass>(t => t.TestMethod())
					.PrepareLogData((container1, data, logData) => logData.Data = testData);

				RegisterAndResolveComponent<InvocationTestClass>(container).TestMethod();
			},
			logData => Assert.AreEqual(testData, logData.Data));
		}

		[Test]
		public void PrepareLogDataActionsShouldBeInvokedSequentiallyDuringMethodInterception()
		{
			RunTestAndCheckFirstLogDataWritten((container, logger) =>
			{
				logger.LogInvocationOf<InvocationTestClass>(t => t.TestMethod())
					.PrepareLogData((container1, data, logData) => logData.Data = 0)
					.PrepareLogData((container1, data, logData) => logData.Data = (int) logData.Data + 1)
					.PrepareLogData((container1, data, logData) => logData.Data = (int) logData.Data + 1);

				RegisterAndResolveComponent<InvocationTestClass>(container).TestMethod();
			}, 
			logData => Assert.AreEqual(2, logData.Data));
		}

		public class LogDataTest
		{
			public object Data { get; set; }
		}

		public class InvocationTestClass
		{
			public virtual void TestMethod() { }
		}
	}
}
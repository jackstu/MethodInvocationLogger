using MethodInvocationLogger.Tests.Helpers;
using NUnit.Framework;

namespace MethodInvocationLogger.Tests
{
	public abstract class ConditionalLoggingTests : BaseTests<object>
	{
		[Test]
		public void MethodInvocationForFullfilledConditionShouldBeLogged()
		{
			RunTestAndAssertExactlyOneLogDataWritten((container, logger) =>
			{
				logger.LogInvocationOf<InvocationTestClass>(t => t.TestMethod(Input.Param<int>()))
					.OnCondition((container1, data) => (int)data.Arguments[0] > 0);

				RegisterAndResolveComponent<InvocationTestClass>(container).TestMethod(5);
			});
		}

		[Test]
		public void MethodInvocationForNotFullfilledConditionShouldNotBeLogged()
		{
			RunTestAndAssertNoLogDataWritten((container, logger) =>
			{
				logger.LogInvocationOf<InvocationTestClass>(t => t.TestMethod(Input.Param<int>()))
					.OnCondition((container1, data) => (int)data.Arguments[0] > 0);

				RegisterAndResolveComponent<InvocationTestClass>(container).TestMethod(-5);
			});
		}

		[Test]
		public void TwoSameMethodsWithDifferentConditions_NoneCondidationFulfilled()
		{
			RunTestAndAssertNumberOfLogDatasWritten((container, logger) =>
			{
				logger.LogInvocationOf<InvocationTestClass>(t => t.TestMethod(Input.Param<int>())).OnCondition((container1, data) => (int)data.Arguments[0] > 0);
				logger.LogInvocationOf<InvocationTestClass>(t => t.TestMethod(Input.Param<int>())).OnCondition((container1, data) => (int)data.Arguments[0] > 5);

				RegisterAndResolveComponent<InvocationTestClass>(container).TestMethod(-10);
			}, 0);
		}

		[Test]
		public void TwoSameMethodsWithDifferentConditions_OneCondidationFulfilled()
		{
			RunTestAndAssertNumberOfLogDatasWritten((container, logger) =>
			{
				logger.LogInvocationOf<InvocationTestClass>(t => t.TestMethod(Input.Param<int>())).OnCondition((container1, data) => (int)data.Arguments[0] > 0);
				logger.LogInvocationOf<InvocationTestClass>(t => t.TestMethod(Input.Param<int>())).OnCondition((container1, data) => (int)data.Arguments[0] > 5);

				RegisterAndResolveComponent<InvocationTestClass>(container).TestMethod(4);
			}, 1);
		}

		[Test]
		public void TwoSameMethodsWithDifferentConditions_BothCondidationFulfilled()
		{
			RunTestAndAssertNumberOfLogDatasWritten((container, logger) =>
			{
				logger.LogInvocationOf<InvocationTestClass>(t => t.TestMethod(Input.Param<int>())).OnCondition((container1, data) => (int)data.Arguments[0] > 0);
				logger.LogInvocationOf<InvocationTestClass>(t => t.TestMethod(Input.Param<int>())).OnCondition((container1, data) => (int)data.Arguments[0] > 5);

				RegisterAndResolveComponent<InvocationTestClass>(container).TestMethod(10);
			}, 2);
		}

		[Test]
		public void ForMethodWithManyConditionsAllMustBeFulfilledToLogInvocation()
		{
			RunTestAndAssertExactlyOneLogDataWritten((container, logger) =>
			{
				logger.LogInvocationOf<InvocationTestClass>(t => t.TestMethod(Input.Param<int>(), Input.Param<int>()))
					.OnCondition((container1, data) => (int)data.Arguments[0] > 0)
					.OnCondition((container1, data) => (int)data.Arguments[1] > 0);

				var component = RegisterAndResolveComponent<InvocationTestClass>(container);

				component.TestMethod(1, 0);
				component.TestMethod(0, 1);
				component.TestMethod(0, 0);
				component.TestMethod(1, 1);
			});
		}

		public class InvocationTestClass
		{
			public virtual void TestMethod(int input) { }
			public virtual void TestMethod(int firstInput, int secondInput) { }
		}
	}
}
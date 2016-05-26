using MethodInvocationLogger.Tests.Helpers;
using NUnit.Framework;

namespace MethodInvocationLogger.Tests
{
	public abstract class MethodInfoMatchingTests : BaseTests<object>
	{
		[Test]
		public void ParameterlessMethodInvocationLogging()
		{
			RunTestAndAssertExactlyOneLogDataWritten((container, logger) =>
			{
				logger.LogInvocationOf<InvocationTestClass>(t => t.TestMethod());
				RegisterAndResolveComponent<InvocationTestClass>(container).TestMethod();
			});
		}

		[Test]
		public void MethodWithOneParamInvocationLogging()
		{
			RunTestAndAssertExactlyOneLogDataWritten((container, logger) =>
			{
				logger.LogInvocationOf<InvocationTestClass>(t => t.TestMethod(Input.Param<int>()));
				RegisterAndResolveComponent<InvocationTestClass>(container).TestMethod(0);
			});
		}

		[Test]
		public void ParameterlessGenericMethodInvocationLogging()
		{
			RunTestAndAssertExactlyOneLogDataWritten((container, logger) =>
			{
				logger.LogInvocationOf<InvocationTestClass>(t => t.TestMethod<int>());
				RegisterAndResolveComponent<InvocationTestClass>(container).TestMethod<int>();
			});
		}

		[Test]
		public void GenericMethodWithOneParamInvocationLogging()
		{
			RunTestAndAssertExactlyOneLogDataWritten((container, logger) =>
			{
				logger.LogInvocationOf<InvocationTestClass>(t => t.TestMethod<string>(Input.Param<string>()));
				RegisterAndResolveComponent<InvocationTestClass>(container).TestMethod<string>(string.Empty);
			});
		}

		[Test]
		public void MethodWithUnspecifiedParamsCountInvocationLogging()
		{
			RunTest(
				(container, logger) =>
				{
					logger.LogInvocationOf<InvocationTestClass>(t => t.TestMethod(Input.Param<string[]>()));
					var component = RegisterAndResolveComponent<InvocationTestClass>(container);

					component.TestMethod(string.Empty);
					component.TestMethod(string.Empty, string.Empty);
					component.TestMethod(string.Empty, string.Empty, string.Empty);
					component.TestMethod(string.Empty, string.Empty, string.Empty, string.Empty);
					component.TestMethod(new string[100]);
				},
				writer =>
				{
					Assert.AreEqual(5, writer.LogDataCount);
				});
		}

		[Test]
		public void MethodWithInheritedParamInvocationLogging()
		{
			RunTestAndAssertExactlyOneLogDataWritten((container, logger) =>
			{
				logger.LogInvocationOf<InvocationTestClass>(t => t.TestMethod(Input.Param<InvocationTestClass.BaseArg>()));
				var component = RegisterAndResolveComponent<InvocationTestClass>(container);

				component.TestMethod(new InvocationTestClass.InheritedArg());
				component.TestMethod((InvocationTestClass.BaseArg)new InvocationTestClass.InheritedArg());
			});
		}

		public class InvocationTestClass
		{
			public class BaseArg
			{
			}

			public class InheritedArg : BaseArg
			{
			}

			public virtual void TestMethod() { }
			public virtual void TestMethod(int input) { }
			public virtual void TestMethod<T>() { }
			public virtual void TestMethod<T>(T input) { }
			public virtual void TestMethod(params string[] args) { }
			public virtual void TestMethod(InheritedArg inheritedArg) { }
			public virtual void TestMethod(BaseArg inheritedArg) { }
		}
	}
}
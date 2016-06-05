using System;
using System.Linq.Expressions;
using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using MethodInvocationLogger.Castle.MethodInfoRequirements;
using MethodInvocationLogger.Castle.Tests.Helpers;
using MethodInvocationLogger.Exceptions;
using NUnit.Framework;

namespace MethodInvocationLogger.Castle.Tests.MethodInfoRequirements
{
	[TestFixture]
	public class DeclaringNonGenericTypeMustBeRegisteredInContainerAfterLoggerConfiguredTest
	{
		[Test]
		public void MethodInTypeRegisteredInContainerBeforeLoggerConfiguredShouldNotSatisfyRequirement()
		{
			RunTest((container, logger, requirement) =>
			{
				container.Register(Component.For<TestClass>().ImplementedBy<TestClass>());
				logger.LogInvocationOf<TestClass>(m => m.TestMethod());
				Method<TestClass>(m => m.TestMethod()).ShouldNotSatisfy(requirement);
			});
		}

		[Test]
		public void MethodInTypeRegisteredInContainerAfterLoggerConfiguredShouldSatisfyRequirement()
		{
			RunTest((container, logger, requirement) =>
			{
				logger.LogInvocationOf<TestClass>(m => m.TestMethod());
				container.Register(Component.For<TestClass>().ImplementedBy<TestClass>());
				Method<TestClass>(m => m.TestMethod()).ShouldSatisfy(requirement);
			});
		}

		[Test]
		public void MethodInGenericTypeRegisteredInContainerAfterLoggerConfiguredShouldSatisfyRequirement()
		{
			RunTest((container, logger, requirement) =>
			{
				Type genericType = typeof(TestClassGeneric<>);

				logger.LogInvocationOf<TestClassGeneric<object>>(m => m.TestMethod());
				container.Register(Component.For(genericType).ImplementedBy(genericType));
				Method<TestClassGeneric<object>>(m => m.TestMethod()).ShouldSatisfy(requirement);
			});
		}

		[Test]
		public void MethodInGenericTypeRegisteredInContainerBeforeLoggerConfiguredShouldSatisfyRequirement()
		{
			RunTest((container, logger, requirement) =>
			{
				Type genericType = typeof(TestClassGeneric<>);
				container.Register(Component.For(genericType).ImplementedBy(genericType));
				logger.LogInvocationOf<TestClassGeneric<object>>(m => m.TestMethod());
				Method<TestClassGeneric<object>>(m => m.TestMethod()).ShouldSatisfy(requirement);
			});
		}

		[Test]
		public void CheckingRequirementWhenLoggerNotBoundToWindsorShouldThrowUnableToCheckRequirementException()
		{
			Assert.Catch<UnableToCheckRequirementException>(() =>
			{
				WindsorContainer container = new WindsorContainer();
				container.Register(Component.For<TestClass>().ImplementedBy<TestClass>());
				DeclaringNonGenericTypeMustBeRegisteredInContainerAfterLoggerConfigured requirement = new DeclaringNonGenericTypeMustBeRegisteredInContainerAfterLoggerConfigured(container.Kernel);
				var result = requirement.SatisfiedBy(Method<TestClass>(m => m.TestMethod()));
			});
		}

		private void RunTest(Action<WindsorContainer, ILogger<object>, DeclaringNonGenericTypeMustBeRegisteredInContainerAfterLoggerConfigured> testAction)
		{
			WindsorContainer container = new WindsorContainer();
			var logger = LoggerFactory.Create<object>().BindToWindsor(container.Kernel);
			DeclaringNonGenericTypeMustBeRegisteredInContainerAfterLoggerConfigured requirement = new DeclaringNonGenericTypeMustBeRegisteredInContainerAfterLoggerConfigured(container.Kernel);

			testAction(container, logger, requirement);
		}

		private MethodInfo Method<T>(Expression<Action<T>> expression)
		{
			return ((MethodCallExpression)expression.Body).Method;
		}

		public class TestClass
		{
			public void TestMethod() { }
		}

		public class TestClassGeneric<T>
		{
			public T TestMethod()
			{
				return default(T);
			}
		}
	}
}
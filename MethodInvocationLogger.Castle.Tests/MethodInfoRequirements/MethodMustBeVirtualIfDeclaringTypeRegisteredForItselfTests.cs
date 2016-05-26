using System;
using System.Linq.Expressions;
using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using MethodInvocationLogger.Castle.MethodInfoRequirements;
using MethodInvocationLogger.Castle.Tests.Helpers;
using NUnit.Framework;

namespace MethodInvocationLogger.Castle.Tests.MethodInfoRequirements
{
	[TestFixture]
	public class MethodMustBeVirtualIfDeclaringTypeRegisteredForItselfTests
	{
		[Test]
		public void PublicVirtualMethodInPublicClassShouldSatisfyRequirement()
		{
			var requirement = ConfigureLoggerAndRegisterComponentForItself<PublicClass>(t => t.PublicVirtualMethod());
			Method<PublicClass>(t=>t.PublicVirtualMethod()).ShouldSatisfy(requirement);
		}
		
		[Test]
		public void InternalVirtualMethodInPublicClassShouldSatisfyRequirement()
		{
			var requirement = ConfigureLoggerAndRegisterComponentForItself<PublicClass>(t => t.InternalVirtualMethod());
			Method<PublicClass>(t => t.InternalVirtualMethod()).ShouldSatisfy(requirement);
		}

		[Test]
		public void PublicNonVirtualMethodInPublicClassShouldNotSatisfyRequirement()
		{
			var requirement = ConfigureLoggerAndRegisterComponentForItself<PublicClass>(t => t.PublicNonVirtualMethod());
			Method<PublicClass>(t => t.PublicNonVirtualMethod()).ShouldNotSatisfy(requirement);
		}

		[Test]
		public void InternalNonVirtualMethodInPublicClassShouldNotSatisfyRequirement()
		{
			var requirement = ConfigureLoggerAndRegisterComponentForItself<PublicClass>(t => t.InternalNonVirtualMethod());
			Method<PublicClass>(t => t.InternalNonVirtualMethod()).ShouldNotSatisfy(requirement);
		}

		[Test]
		public void PublicVirtualMethodInInternalClassShouldSatisfyRequirement()
		{
			var requirement = ConfigureLoggerAndRegisterComponentForItself<InternalClass>(t => t.PublicVirtualMethod());
			Method<InternalClass>(t => t.PublicVirtualMethod()).ShouldSatisfy(requirement);
		}

		[Test]
		public void InternalVirtualMethodInInternalClassShouldSatisfyRequirement()
		{
			var requirement = ConfigureLoggerAndRegisterComponentForItself<InternalClass>(t => t.InternalVirtualMethod());
			Method<InternalClass>(t => t.InternalVirtualMethod()).ShouldSatisfy(requirement);
		}

		[Test]
		public void PublicNonVirtualMethodInInternalClassShouldNotSatisfyRequirement()
		{
			var requirement = ConfigureLoggerAndRegisterComponentForItself<InternalClass>(t => t.PublicNonVirtualMethod());
			Method<InternalClass>(t => t.PublicNonVirtualMethod()).ShouldNotSatisfy(requirement);
		}

		[Test]
		public void InternalNonVirtualMethodInInternalClassShouldNotSatisfyRequirement()
		{
			var requirement = ConfigureLoggerAndRegisterComponentForItself<InternalClass>(t => t.InternalNonVirtualMethod());
			Method<InternalClass>(t => t.InternalNonVirtualMethod()).ShouldNotSatisfy(requirement);
		}

		[Test]
		public void NonMethodVirtualShouldSatisfyRequirementIfDeclaringTypeRegisteredForInterface()
		{
			WindsorContainer container = new WindsorContainer();
			var logger = LoggerFactory.Create<object>().BindToWindsor(container.Kernel);
			logger.LogInvocationOf<TestClass>(t => t.PublicNonVirtualMethod());
			MethodMustBeVirtualIfDeclaringTypeRegisteredOnlyForItself requirement = new MethodMustBeVirtualIfDeclaringTypeRegisteredOnlyForItself(container.Kernel);
			container.Register(Component.For<ITestClass>().ImplementedBy<TestClass>());

			Method<TestClass>(t => t.PublicNonVirtualMethod()).ShouldSatisfy(requirement);
		}

		public class PublicClass 
		{
			public virtual void PublicVirtualMethod() { }
			internal virtual void InternalVirtualMethod() { }
			public void PublicNonVirtualMethod() { }
			internal void InternalNonVirtualMethod() { }
		}

		internal class InternalClass
		{
			public virtual void PublicVirtualMethod() { }
			internal virtual void InternalVirtualMethod() { }
			public void PublicNonVirtualMethod() { }
			internal void InternalNonVirtualMethod() { }
		}

		public class TestClass : ITestClass
		{
			public void PublicNonVirtualMethod() { }
		}

		public interface ITestClass
		{
			void PublicNonVirtualMethod();
		}

		private MethodMustBeVirtualIfDeclaringTypeRegisteredOnlyForItself ConfigureLoggerAndRegisterComponentForItself<TComponent>(Expression<Action<TComponent>> loggedMethod)
			where TComponent : class
		{
			WindsorContainer container = new WindsorContainer();
			var logger = LoggerFactory.Create<object>().BindToWindsor(container.Kernel);
			logger.LogInvocationOf(loggedMethod);
			MethodMustBeVirtualIfDeclaringTypeRegisteredOnlyForItself requirement = new MethodMustBeVirtualIfDeclaringTypeRegisteredOnlyForItself(container.Kernel);
			container.Register(Component.For<TComponent>().ImplementedBy<TComponent>());
			return requirement;
		}

		private MethodInfo Method<T>(Expression<Action<T>> expression)
		{
			return ((MethodCallExpression)expression.Body).Method;
		}
	}
}
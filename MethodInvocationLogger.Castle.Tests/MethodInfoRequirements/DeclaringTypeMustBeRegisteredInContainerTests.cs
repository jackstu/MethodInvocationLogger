using System;
using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using MethodInvocationLogger.Castle.MethodInfoRequirements;
using MethodInvocationLogger.Castle.Tests.Helpers;
using NUnit.Framework;

namespace MethodInvocationLogger.Castle.Tests.MethodInfoRequirements
{
	[TestFixture]
	public class DeclaringTypeMustBeRegisteredInContainerTests
	{
		[Test]
		public void MethodInTypeNotRegisteredShouldNotSatisfyRequirement()
		{
			WindsorContainer container = new WindsorContainer();
			DeclaringTypeMustBeRegisteredInContainer requirement = new DeclaringTypeMustBeRegisteredInContainer(container.Kernel);
			GetTestMethodInfo().ShouldNotSatisfy(requirement);
		}

		[Test]
		public void MethodInTypeRegisteredForInterfaceShouldSpecifyRequirement()
		{
			WindsorContainer container = new WindsorContainer();
			DeclaringTypeMustBeRegisteredInContainer requirement = new DeclaringTypeMustBeRegisteredInContainer(container.Kernel);
			container.Register(Component.For<ITestClass>().ImplementedBy<TestClass>());
			GetTestMethodInfo().ShouldSatisfy(requirement);
		}

		[Test]
		public void MethodInGenericTypeRegisteredForGenericDefInterfaceShouldSpecifyRequirement()
		{
			WindsorContainer container = new WindsorContainer();
			DeclaringTypeMustBeRegisteredInContainer requirement = new DeclaringTypeMustBeRegisteredInContainer(container.Kernel);
			Type interfaceType = typeof (ITestClassGeneric<>);
			Type classType = typeof(TestClassGeneric<>);
			container.Register(Component.For(interfaceType).ImplementedBy(classType));
			GetGenericClassTestMethodInfo().ShouldSatisfy(requirement);
			GetGenericClassTestMethodInfo<int>().ShouldSatisfy(requirement);
		}

		[Test]
		public void MethodInTypeRegisteredForItselfShouldSpecifyRequirement()
		{
			WindsorContainer container = new WindsorContainer();
			DeclaringTypeMustBeRegisteredInContainer requirement = new DeclaringTypeMustBeRegisteredInContainer(container.Kernel);
			container.Register(Component.For<TestClass>().ImplementedBy<TestClass>());
			GetTestMethodInfo().ShouldSatisfy(requirement);
		}

		[Test]
		public void MethodInGenericTypeRegisteredForItselfShouldSpecifyRequirement()
		{
			WindsorContainer container = new WindsorContainer();
			DeclaringTypeMustBeRegisteredInContainer requirement = new DeclaringTypeMustBeRegisteredInContainer(container.Kernel);
			Type classType = typeof(TestClassGeneric<>);
			container.Register(Component.For(classType).ImplementedBy(classType));
			GetGenericClassTestMethodInfo().ShouldSatisfy(requirement);
			GetGenericClassTestMethodInfo<int>().ShouldSatisfy(requirement);
		}

		private MethodInfo GetTestMethodInfo()
		{
			return typeof(TestClass).GetMethod("TestMethod");
		}

		private MethodInfo GetGenericClassTestMethodInfo()
		{
			return typeof(TestClassGeneric<>).GetMethod("TestMethod");
		}

		private MethodInfo GetGenericClassTestMethodInfo<T>()
		{
			return typeof(TestClassGeneric<T>).GetMethod("TestMethod");
		}

		public class TestClass : ITestClass
		{
			public void TestMethod() { }
		}

		public interface ITestClass
		{
			void TestMethod();
		}

		public class TestClassGeneric<T> : ITestClassGeneric<T>
		{
			public T TestMethod()
			{
				return default(T);
			}
		}

		public interface ITestClassGeneric<T>
		{
			T TestMethod();
		}
	}
}

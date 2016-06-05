using System;
using MethodInvocationLogger.Castle.MethodInfoRequirements;
using MethodInvocationLogger.Castle.Tests.Helpers;
using NUnit.Framework;

namespace MethodInvocationLogger.Castle.Tests.MethodInfoRequirements
{
	[TestFixture]
	public class DeclaringTypeCannotBeOpenGenericTests : MethodInfoRequirementTest<DeclaringTypeCannotBeOpenGeneric>
	{
		[Test]
		public void MethodInOpenGenericTypeShouldNotSatisfyRequirement([Values(
			"TestMethod", 
			"TestMethodWithGenericArg", 
			"TestMethodWithGenericReturnType")] string methodName)
		{
			typeof(OpenGenericType<>).GetMethod(methodName).ShouldNotSatisfy(Requirement);
		}

		[Test]
		public void MethodInGenericTypeShouldSatisfyRequirement([Values(
			"TestMethod",
			"TestMethodWithGenericArg",
			"TestMethodWithGenericReturnType")] string methodName, [Values(
			typeof(object),
			typeof(int),
			typeof(Dummy))] Type type)
		{
			typeof(OpenGenericType<>).MakeGenericType(type).GetMethod("TestMethod").ShouldSatisfy(Requirement);
		}

		[Test]
		public void MethodInCustomTypeShoudSatisfyRequirement()
		{
			Method<Dummy>(t=>t.TestMethod()).ShouldSatisfy(Requirement);
		}

		[Test]
		public void MethodInSystemTypeShoudSatisfyRequirement()
		{
			Method<int>(t => t.ToString()).ShouldSatisfy(Requirement);
		}

		public class Dummy
		{
			public void TestMethod() { }
		}

		public class OpenGenericType<T>
		{
			public void TestMethod() { }
			public void TestMethodWithGenericArg(T t) { }
			public T TestMethodWithGenericReturnType() { return default(T); }
		}
	}
}
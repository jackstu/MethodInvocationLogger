using MethodInvocationLogger.Castle.MethodInfoRequirements;
using MethodInvocationLogger.Castle.Tests.Helpers;
using NUnit.Framework;

namespace MethodInvocationLogger.Castle.Tests.MethodInfoRequirements
{
	[TestFixture]
	public class DeclaringTypeMustBeAccessibleForProxyCreatingTests : MethodInfoRequirementTest<DeclaringTypeMustBeAccessibleForProxyCreating>
	{
		[Test]
		public void MethodInPublicTypeShouldSatisfyRequirement()
		{
			Method<PublicTestClass>(m=>m.TestMethod()).ShouldSatisfy(Requirement);
		}

		[Test]
		public void MethodInNestedPublicTypeShouldSatisfyRequirement()
		{
			Method<PublicTestClass.NestedPublicClass>(m=>m.TestMethod()).ShouldSatisfy(Requirement);
		}

		[Test]
		public void MethodInPrivateTypeShouldNotSatisfyRequirement()
		{
			Method<PrivateTestClass>(m=>m.TestMethod()).ShouldNotSatisfy(Requirement);
		}

		[Test]
		public void MethodInInternalTypeInAssemblyWithInternalVisibleAttributeShoudSatisfyRequirement()
		{
			Method<MethodInvocationLogger.Castle.Tests.InternalsVisible.InternalTestClass>(m=>m.TestMethod()).ShouldSatisfy(Requirement);
		}

		[Test]
		public void MethodInInternalTypeNestedInPublicTypeInAssemblyWithInternalVisibleAttributeShoudSatisfyRequirement()
		{
			Method<MethodInvocationLogger.Castle.Tests.InternalsVisible.PublicTestClass.InternalNestedClass>(m => m.TestMethod()).ShouldSatisfy(Requirement);
		}

		[Test]
		public void MethodInPublicTypeNestedInInternalTypeInAssemblyWithInternalVisibleAttributeShoudSatisfyRequirement()
		{
			Method<MethodInvocationLogger.Castle.Tests.InternalsVisible.InternalTestClass.PublicNestedClass>(m => m.TestMethod()).ShouldSatisfy(Requirement);
		}

		[Test]
		public void MethodInInternalTypeInAssemblyWithoutInternalVisibleAttributeShoudNotSatisfyRequirement()
		{
			Method<InternalTestClass>(m=>m.TestMethod()).ShouldNotSatisfy(Requirement);
		}

		private class PrivateTestClass
		{
			public void TestMethod() { }
		}
	}

	internal class InternalTestClass
	{
		public void TestMethod() { }
	}

	public class PublicTestClass
	{
		public class NestedPublicClass
		{
			public void TestMethod() { }
		}

		public void TestMethod() { }
	}
}
using MethodInvocationLogger.Castle.MethodInfoRequirements;
using MethodInvocationLogger.Castle.Tests.Helpers;
using NUnit.Framework;

namespace MethodInvocationLogger.Castle.Tests.MethodInfoRequirements
{
	[TestFixture]
	public class DeclaringTypeCannotBeInterfaceTests : MethodInfoRequirementTest<DeclaringTypeCannotBeInterface>
	{
		[Test]
		public void MethodInInterfaceShouldNotSatisfyRequirement()
		{
			Method<ITestClass>(m=>m.TestMethod()).ShouldNotSatisfy(Requirement);
		}

		[Test]
		public void MethodInClassShouldSatisfyRequirement()
		{
			Method<TestClass>(m => m.TestMethod()).ShouldSatisfy(Requirement);
		}

		public class TestClass
		{
			public void TestMethod() { }
		}

		public interface ITestClass
		{
			void TestMethod();
		}
	}
}
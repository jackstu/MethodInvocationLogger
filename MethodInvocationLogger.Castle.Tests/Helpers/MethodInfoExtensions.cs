using System.Reflection;
using NUnit.Framework;

namespace MethodInvocationLogger.Castle.Tests.Helpers
{
	public static class MethodInfoExtensions
	{
		public static void ShouldSatisfy(this MethodInfo methodInfo, IMethodInfoRequirement requirement)
		{
			Assert.True(requirement.SatisfiedBy(methodInfo), $"{methodInfo} don't satisfied requirement: {requirement}, but it should.");
		}

		public static void ShouldNotSatisfy(this MethodInfo methodInfo, IMethodInfoRequirement requirement)
		{
			Assert.False(requirement.SatisfiedBy(methodInfo), $"{methodInfo} satisfied requirement: {requirement}, but it shouldn't.");
		}
	}
}
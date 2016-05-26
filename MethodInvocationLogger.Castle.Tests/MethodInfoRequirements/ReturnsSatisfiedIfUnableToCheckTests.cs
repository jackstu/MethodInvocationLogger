using System;
using System.Reflection;
using MethodInvocationLogger.Castle.MethodInfoRequirements;
using MethodInvocationLogger.Exceptions;
using Moq;
using NUnit.Framework;

namespace MethodInvocationLogger.Castle.Tests.MethodInfoRequirements
{
	[TestFixture]
	public class ReturnsSatisfiedIfUnableToCheckTests
	{
		[Test]
		public void ShouldReturnTrueIfInternalRequirementThrowUnableToCheckException()
		{
			Mock<IMethodInfoRequirement> internalRequirement = new Mock<IMethodInfoRequirement>();
			internalRequirement.Setup(r => r.SatisfiedBy(It.IsAny<MethodInfo>())).Throws(new UnableToCheckRequirementException(null, (Exception)null));
			Assert.True(new ReturnsSatisfiedIfUnableToCheck(internalRequirement.Object).SatisfiedBy(GetTestMethodInfo()));
		}
		
		[Test]
		public void ShouldReturnTrueIfInternalRequirementReturnsTrue()
		{
			Mock<IMethodInfoRequirement> internalRequirement = new Mock<IMethodInfoRequirement>();
			internalRequirement.Setup(r => r.SatisfiedBy(It.IsAny<MethodInfo>())).Returns(true);
			Assert.True(new ReturnsSatisfiedIfUnableToCheck(internalRequirement.Object).SatisfiedBy(GetTestMethodInfo()));
		}

		[Test]
		public void ShouldReturnFalseIfInternalRequirementReturnsFalse()
		{
			Mock<IMethodInfoRequirement> internalRequirement = new Mock<IMethodInfoRequirement>();
			internalRequirement.Setup(r => r.SatisfiedBy(It.IsAny<MethodInfo>())).Returns(false);
			Assert.False(new ReturnsSatisfiedIfUnableToCheck(internalRequirement.Object).SatisfiedBy(GetTestMethodInfo()));
		}

		[Test]
		public void ShouldThrowSpecifiedExceptionIfInternalRequirementThrowSpecifiedException([Values(
			typeof(Exception),
			typeof(TestException),
			typeof(NullReferenceException))] Type exceptionType)
		{
			Mock<IMethodInfoRequirement> internalRequirement = new Mock<IMethodInfoRequirement>();
			internalRequirement.Setup(r => r.SatisfiedBy(It.IsAny<MethodInfo>())).Throws((Exception)Activator.CreateInstance(exceptionType));
			Assert.Throws(exceptionType, () =>
			{
				new ReturnsSatisfiedIfUnableToCheck(internalRequirement.Object).SatisfiedBy(GetTestMethodInfo());
			});
		}

		private MethodInfo GetTestMethodInfo()
		{
			return typeof(TestClass).GetMethod("TestMethod");
		}

		private class TestClass
		{
			public void TestMethod() { }
		}

		public class TestException : Exception { }
	}
}
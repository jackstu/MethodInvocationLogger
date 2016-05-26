using System;
using System.Linq.Expressions;
using System.Reflection;

namespace MethodInvocationLogger.Castle.Tests.Helpers
{
	public class MethodInfoRequirementTest<TRequirement>
	{
		protected readonly TRequirement Requirement;

		public MethodInfoRequirementTest()
		{
			Requirement = Activator.CreateInstance<TRequirement>();
		}

		public MethodInfoRequirementTest(TRequirement requirement)
		{
			Requirement = requirement;
		}

		public MethodInfo Method<T>(Expression<Action<T>> expression)
		{
			return ((MethodCallExpression)expression.Body).Method;
		}
	}
}
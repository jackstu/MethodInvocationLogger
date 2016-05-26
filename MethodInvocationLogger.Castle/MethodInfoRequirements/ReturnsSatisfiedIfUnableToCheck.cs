using System.Reflection;
using MethodInvocationLogger.Exceptions;

namespace MethodInvocationLogger.Castle.MethodInfoRequirements
{
	public class ReturnsSatisfiedIfUnableToCheck : IMethodInfoRequirement
	{
		private readonly IMethodInfoRequirement _requirement;

		public ReturnsSatisfiedIfUnableToCheck(IMethodInfoRequirement requirement)
		{
			_requirement = requirement;
		}

		public bool SatisfiedBy(MethodInfo method)
		{
			try
			{
				return _requirement.SatisfiedBy(method);
			}
			catch (UnableToCheckRequirementException)
			{
				return true;
			}
		}

		public string GetFriendlyMessage(MethodInfo methodInfo)
		{
			return _requirement.GetFriendlyMessage(methodInfo);
		}
	}
}
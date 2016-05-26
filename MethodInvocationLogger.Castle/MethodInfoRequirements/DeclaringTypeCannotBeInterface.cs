using System.Reflection;

namespace MethodInvocationLogger.Castle.MethodInfoRequirements
{
	public class DeclaringTypeCannotBeInterface : IMethodInfoRequirement
	{
		public bool SatisfiedBy(MethodInfo method)
		{
			return !method.DeclaringType.IsInterface;
		}

		public string GetFriendlyMessage(MethodInfo methodInfo)
		{
			return $"{methodInfo.DeclaringType} is an interface. Interfaces are not supported.";
		}
	}
}
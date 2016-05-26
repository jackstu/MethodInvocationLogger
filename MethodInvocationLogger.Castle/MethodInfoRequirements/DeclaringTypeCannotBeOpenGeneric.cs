using System.Reflection;

namespace MethodInvocationLogger.Castle.MethodInfoRequirements
{
	public class DeclaringTypeCannotBeOpenGeneric : IMethodInfoRequirement
	{
		public bool SatisfiedBy(MethodInfo method)
		{
			return !method.DeclaringType.IsGenericTypeDefinition;
		}

		public string GetFriendlyMessage(MethodInfo methodInfo)
		{
			return $"{methodInfo.DeclaringType} is an open generic type. Open generic type are not supported";
		}
	}
}
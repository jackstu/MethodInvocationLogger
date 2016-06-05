using System.Reflection;

namespace MethodInvocationLogger
{
	public interface IMethodInfoRequirement
	{
		bool SatisfiedBy(MethodInfo method);
		string GetFriendlyMessage(MethodInfo methodInfo);
	}
}
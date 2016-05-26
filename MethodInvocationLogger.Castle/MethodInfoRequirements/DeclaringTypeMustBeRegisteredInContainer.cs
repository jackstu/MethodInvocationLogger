using System.Linq;
using System.Reflection;
using Castle.Core;
using Castle.MicroKernel;

namespace MethodInvocationLogger.Castle.MethodInfoRequirements
{
	public class DeclaringTypeMustBeRegisteredInContainer : IMethodInfoRequirement
	{
		private readonly IKernel _kernel;

		public DeclaringTypeMustBeRegisteredInContainer(IKernel kernel)
		{
			_kernel = kernel;
		}

		public bool SatisfiedBy(MethodInfo method)
		{
			return CastleHelper.GetComponentModelForImplementation(_kernel, method.DeclaringType) != null;
		}

		public string GetFriendlyMessage(MethodInfo methodInfo)
		{
			return $"{methodInfo.DeclaringType} is not registered in a castle container.";
		}
	}
}
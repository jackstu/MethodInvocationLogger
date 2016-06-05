using System.Linq;
using System.Reflection;
using Castle.MicroKernel;
using MethodInvocationLogger.Exceptions;

namespace MethodInvocationLogger.Castle.MethodInfoRequirements
{
	public class DeclaringNonGenericTypeMustBeRegisteredInContainerAfterLoggerConfigured : IMethodInfoRequirement
	{
		private readonly LogInterceptorAppenderContributor _logInterceptorAppenderContributor;

		public DeclaringNonGenericTypeMustBeRegisteredInContainerAfterLoggerConfigured(IKernel kernel)
		{
			_logInterceptorAppenderContributor = kernel.ComponentModelBuilder.Contributors.FirstOrDefault(c => c.GetType() == typeof (LogInterceptorAppenderContributor)) as LogInterceptorAppenderContributor;
		}

		public bool SatisfiedBy(MethodInfo method)
		{
			if (_logInterceptorAppenderContributor == null)
				throw new UnableToCheckRequirementException(this.GetType(), "LogInterceptorAppenderContributor not found");

			return !_logInterceptorAppenderContributor.ProcessedImplementations.ContainsKey(method.DeclaringType)
			       || _logInterceptorAppenderContributor.ProcessedImplementations[method.DeclaringType].InterceptorAdded;
		}

		public string GetFriendlyMessage(MethodInfo methodInfo)
		{
			return $"{methodInfo.DeclaringType} was registered in Castle before logger configured. Logger configuration must be done before Castle registration.";
		}
	}
}
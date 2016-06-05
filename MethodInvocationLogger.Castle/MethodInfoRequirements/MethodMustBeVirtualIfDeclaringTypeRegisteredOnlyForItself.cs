using System;
using System.Linq;
using System.Reflection;
using Castle.MicroKernel;
using MethodInvocationLogger.Exceptions;

namespace MethodInvocationLogger.Castle.MethodInfoRequirements
{
	public class MethodMustBeVirtualIfDeclaringTypeRegisteredOnlyForItself : IMethodInfoRequirement
	{
		private readonly LogInterceptorAppenderContributor _logInterceptorAppenderContributor;

		public MethodMustBeVirtualIfDeclaringTypeRegisteredOnlyForItself(IKernel kernel)
		{
			_logInterceptorAppenderContributor = kernel.ComponentModelBuilder.Contributors.FirstOrDefault(c => c.GetType() == typeof(LogInterceptorAppenderContributor)) as LogInterceptorAppenderContributor;
		}

		public bool SatisfiedBy(MethodInfo method)
		{
			if (_logInterceptorAppenderContributor == null)
				throw new UnableToCheckRequirementException(this.GetType(), "LogInterceptorAppenderContributor not found");

			if (!IsComponentRegisteredOnlyForItself(method.DeclaringType))
				return true;

			return method.IsVirtual;
		}

		private bool IsComponentRegisteredOnlyForItself(Type implementationType)
		{
			LogInterceptorAppenderContributor.ImplementationInfo implementationInfo;
			if (!_logInterceptorAppenderContributor.ProcessedImplementations.TryGetValue(implementationType, out implementationInfo))
				throw new UnableToCheckRequirementException(this.GetType(), $"{implementationType} not exists in a collection of processed and intercepted implementations in LogInterceptorAppenderContributor.");

			return !implementationInfo.Services.Any(t => t.IsInterface);
		}

		public string GetFriendlyMessage(MethodInfo methodInfo)
		{
			return $"{methodInfo} has to be virtual because it's declaring type {methodInfo.DeclaringType} is registered in Castle for itself. If you register declaring type for interface then method doesn't have to be virtual.";
		}
	}
}

using System;
using System.Reflection;

namespace MethodInvocationLogger
{
	public interface ILoggerInternal
	{
		void ProcessInvocationData(InvocationRawData invocationRawData);
		bool MethodRegistered(MethodInfo method);
		bool TypeRegistered(Type type);
		ConfigurationValidationResult ValidateConfiguration(params IMethodInfoRequirement[] methodRequirements);
	}
}
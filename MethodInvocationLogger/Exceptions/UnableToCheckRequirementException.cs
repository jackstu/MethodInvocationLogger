using System;

namespace MethodInvocationLogger.Exceptions
{
	public class UnableToCheckRequirementException : Exception
	{
		public UnableToCheckRequirementException(Type requirementType, Exception innerException) 
			: base($"Unable to check requirement {requirementType} ", innerException)
		{ }

		public UnableToCheckRequirementException(Type requirementType, string message)
			: base($"Unable to check requirement {requirementType}. {message}")
		{ }

		public UnableToCheckRequirementException(Type requirementType, Exception innerException, string message)
			: base($"Unable to check requirement {requirementType}. {message}",innerException)
		{ }
	}
}
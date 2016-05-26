using System;

namespace MethodInvocationLogger.Exceptions
{
	public class LoggerValidationException : Exception
	{
		public ConfigurationValidationResult ValidationResult { get; }

		public LoggerValidationException(ConfigurationValidationResult validationResult)
			: base("Logger is invalid.\n " + validationResult)
		{
			ValidationResult = validationResult;
		}
	}
}
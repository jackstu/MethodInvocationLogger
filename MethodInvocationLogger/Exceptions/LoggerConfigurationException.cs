using System;

namespace MethodInvocationLogger.Exceptions
{
	public class LoggerConfigurationException : Exception
	{
		public LoggerConfigurationException(string message) : base(message) { }
	}
}
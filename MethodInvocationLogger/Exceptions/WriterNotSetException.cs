using System;

namespace MethodInvocationLogger.Exceptions
{
	public class WriterNotSetException : Exception
	{
		public WriterNotSetException() : base("You have to specified ILogWriter in Logger")
		{
		}
	}
}
using System;

namespace MethodInvocationLogger.Exceptions
{
	public class UnknownArgumentException : Exception
	{
		public UnknownArgumentException(string message) : base(message) { }
	}
}
using System;
using System.Reflection;

namespace MethodInvocationLogger
{
	public class InvocationRawData
	{
		public MethodInfo MethodInfo { get; }
		public DateTime Begin { get; }
		public TimeSpan Duration { get; }
		public object[] Arguments { get; }
		public object ReturnValue { get; }
		public Exception Exception { get; }
		public object Target { get; }

		public InvocationRawData(MethodInfo methodInfo, DateTime begin, TimeSpan duration, object[] arguments, object target, object returnValue, Exception exception)
		{
			MethodInfo = methodInfo;
			Begin = begin;
			Duration = duration;
			Arguments = arguments;
			Target = target;
			ReturnValue = returnValue;
			Exception = exception;
		}
	}
}
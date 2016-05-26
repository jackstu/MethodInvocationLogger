using System;
using System.Diagnostics;
using Castle.DynamicProxy;

namespace MethodInvocationLogger.Castle
{
	internal class LoggerInterceptor : IInterceptor
	{
		private readonly ICompositeLoggerInternal _compositeLoggerInternal;

		public LoggerInterceptor(ICompositeLoggerInternal compositeLoggerInternal)
		{
			_compositeLoggerInternal = compositeLoggerInternal;
		}

		public void Intercept(IInvocation invocation)
		{
			if (_compositeLoggerInternal.MethodRegistered(invocation.MethodInvocationTarget))
			{
				DateTime begin = DateTime.Now;
				Stopwatch stopWatch = new Stopwatch();
				Exception exception = null;
				stopWatch.Start();
				try
				{
					invocation.Proceed();
				}
				catch (Exception exc)
				{
					exception = exc;
					throw;
				}
				finally
				{
					stopWatch.Stop();

					InvocationRawData info = new InvocationRawData(invocation.MethodInvocationTarget, begin, stopWatch.Elapsed, invocation.Arguments,
						invocation.InvocationTarget, invocation.ReturnValue, exception);

					_compositeLoggerInternal.ProcessInvocationData(info);
				}
			}
			else
				invocation.Proceed();
		}
	}
}

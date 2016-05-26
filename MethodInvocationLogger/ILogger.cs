using System;
using System.Linq.Expressions;

namespace MethodInvocationLogger
{
	public interface ILogger<TLogData>
	{
		MethodInvocationLoggingConfiguration<TLogData> LogInvocationOf<TLogedClass>(Expression<Action<TLogedClass>> expression);
		ILogger<TLogData> SetContainer(IContainer container);
		ILogger<TLogData> WriteTo(ILogWriter<TLogData> logWriter);
	}
}
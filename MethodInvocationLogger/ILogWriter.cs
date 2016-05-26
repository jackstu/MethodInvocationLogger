namespace MethodInvocationLogger
{
	public interface ILogWriter<in TLogData>
	{
		void WriteLog(TLogData logData);
	}
}
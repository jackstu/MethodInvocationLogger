namespace MethodInvocationLogger
{
	public interface ILogOutput<in TLogData>
	{
		void WriteLog(TLogData logData);
	}
}
using System.Collections.Generic;

namespace MethodInvocationLogger.Tests.Helpers
{
	public class TestLogWriter<TLogData> : ILogWriter<TLogData>
	{
		readonly List<TLogData> _logs = new List<TLogData>();

		public void WriteLog(TLogData logData)
		{
			_logs.Add(logData);
		}

		public int LogDataCount => _logs.Count;

		public TLogData this[int i]
		{
			get { return _logs[i]; }
			set { _logs[i] = value; }
		}
	}
}
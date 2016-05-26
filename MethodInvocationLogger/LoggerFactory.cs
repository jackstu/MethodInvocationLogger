namespace MethodInvocationLogger
{
	public static class LoggerFactory
	{
		public static ILogger<TLogData> Create<TLogData>()
		{
			return new Logger<TLogData>();
		}
	}
}
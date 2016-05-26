namespace MethodInvocationLogger.Castle
{
	internal interface ICompositeLoggerInternal : ILoggerInternal
	{
		void Add(ILoggerInternal loggerInternal);
		bool Has(ILoggerInternal loggerInternal);
	}
}
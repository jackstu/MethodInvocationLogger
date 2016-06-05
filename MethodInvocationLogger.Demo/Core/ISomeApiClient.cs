namespace MethodInvocationLogger.Demo.Core
{
	public interface ISomeApiClient
	{
		Data GetSomeData();
		void PutSomeData(string dataStr, Data dataComplex);
	}
}
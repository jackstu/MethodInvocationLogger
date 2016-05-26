namespace MethodInvocationLogger.Demo.Controllers
{
	public interface ISomeApiClient
	{
		Data GetSomeData();
		void PutSomeData(string dataStr, Data dataComplex);
	}
}
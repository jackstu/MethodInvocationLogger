using System.Threading;
using System.Threading.Tasks;

namespace MethodInvocationLogger.Demo.Core
{
	public class CyclicApiCaller
	{
		public bool Enabled { get; set; }
		private readonly ISomeApiClient _apiClient;

		public CyclicApiCaller(ISomeApiClient apiClient)
		{
			_apiClient = apiClient;
			Task.Run(() => Run());
		}

		private void Run()
		{
			while (true)
			{
				if (Enabled)
				{
					Data data = _apiClient.GetSomeData();
				}

				Thread.Sleep(3000);
			}
		}
	}
}
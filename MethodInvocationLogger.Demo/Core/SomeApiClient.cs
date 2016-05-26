using System;
using System.Threading;

namespace MethodInvocationLogger.Demo.Controllers
{
	public class SomeApiClient : ISomeApiClient
	{
		readonly Random _random = new Random();

		public void PutSomeData(string dataStr, Data dataComplex)
		{
			// fake implementation
			Thread.Sleep(_random.Next(20, 60));
		}

		public Data GetSomeData()
		{
			// fake implementation
			Thread.Sleep(_random.Next(10, 30));
			return new Data {Data1 = "test", Data2 = DateTime.Now.Ticks.ToString()};
		}
	}
}
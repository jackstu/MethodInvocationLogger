using System;
using System.Collections.Generic;
using MethodInvocationLogger.Extensions;
using Nest;

namespace MethodInvocationLogger.SampleWriters
{
	public class ElasticSearchLogWriter : ILogOutput<DictionaryLogData>
	{
		readonly ElasticClient _client;

		public ElasticSearchLogWriter(Uri uri)
		{
			_client = new ElasticClient(uri);
		}

		public void WriteLog(DictionaryLogData logData)
		{
			DictionaryLogData data = new DictionaryLogData();
			string logDataName = null;

			foreach (KeyValuePair<string, object> pair in logData)
			{
				if (pair.Key == "Name")
				{
					logDataName = pair.Value.ToString().ToLower();
					continue;
				}

				if (pair.Value is TimeSpan)
				{
					data[pair.Key] = (decimal)((TimeSpan)pair.Value).TotalSeconds;
				}
				else
				{
					data[pair.Key] = pair.Value;
				}
			}

			_client.Index<DictionaryLogData>(data, (indexDescriptior) => indexDescriptior.Index("invocation_" + logDataName));
		}
	}
}
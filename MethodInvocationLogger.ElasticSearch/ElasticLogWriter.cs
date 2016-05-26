using System;
using MethodInvocationLogger.Extensions;
using Nest;

namespace MethodInvocationLogger.ElasticSearch
{
    public class ElasticLogWriter : ILogWriter<DictionaryLogData>
    {
	    private readonly Uri _uri;

	    public ElasticLogWriter(Uri uri)
	    {
		    _uri = uri;
	    }

	    public void WriteLog(DictionaryLogData logData)
	    {
		    ElasticClient client = new ElasticClient(new ConnectionSettings(_uri));

			var copy = new DictionaryLogData(logData);
			string name = (string)copy["Name"];
			copy.Remove("Name");

			var result = client.Index(copy, descriptor => descriptor.Index(name));
	    }
    }
}
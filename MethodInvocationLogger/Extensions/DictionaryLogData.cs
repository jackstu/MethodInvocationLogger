using System.Collections.Generic;

namespace MethodInvocationLogger.Extensions
{
	public class DictionaryLogData : Dictionary<string, object>
	{
		public DictionaryLogData()
		{
		}

		public DictionaryLogData(IDictionary<string, object> dictionary) : base(dictionary)
		{
		}
	}
}
using System.Collections.Generic;

namespace MethodInvocationLogger
{
	public class InvocationData
	{
		public InvocationRawData Raw { get; }
		public ArgumentsCollection Arguments { get; }

		public InvocationData(InvocationRawData raw, Dictionary<string, int> paramNameToIndexMapping)
		{
			Raw = raw;
			Arguments = new ArgumentsCollection(raw, paramNameToIndexMapping);
		}
	}
}
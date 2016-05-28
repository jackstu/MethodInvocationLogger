using System;
using MethodInvocationLogger.Extensions;

namespace MethodInvocationLogger.SampleWriters
{
	public class EventLogData : DictionaryLogData
	{
		public DateTime CreateDateTime { get; set; }
		public string EventType { get; set; }
		public int TenantId { get; set; }
	}
}
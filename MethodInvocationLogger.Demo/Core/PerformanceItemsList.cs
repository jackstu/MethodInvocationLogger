using System;
using System.Collections.Generic;
using System.Linq;
using MethodInvocationLogger.Extensions;

namespace MethodInvocationLogger.Demo.Core
{
	public class PerformanceItemsList
	{
		private readonly List<DictionaryLogData> _list = new List<DictionaryLogData>();

		public IEnumerable<DictionaryLogData> GetItems()
		{
			return _list;
		}

		public IEnumerable<DictionaryLogData> GetItems(DateTime min, DateTime max)
		{
			return _list.Where(item => (DateTime) item["BeginTime"] > min && (DateTime) item["BeginTime"] < max);
		}

		public void AddItem(DictionaryLogData item)
		{
			_list.Add(item);
		}
	}
}
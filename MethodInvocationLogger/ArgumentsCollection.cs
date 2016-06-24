using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MethodInvocationLogger.Exceptions;

namespace MethodInvocationLogger
{
	public class ArgumentsCollection : IEnumerable<KeyValuePair<string, object>>
	{
		private readonly InvocationRawData _invocationRawData;
		private readonly Dictionary<string, int> _paramNameToIndexMapping;

		public ArgumentsCollection(InvocationRawData invocationRawData, Dictionary<string, int> paramNameToIndexMapping)
		{
			_invocationRawData = invocationRawData;
			_paramNameToIndexMapping = paramNameToIndexMapping;
		}

		public object this[string name] => Get(name);
		public object this[int index] => _invocationRawData.Arguments[index];

		public object Get(string name)
		{
			if (string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));

			int index;
			if (!_paramNameToIndexMapping.TryGetValue(name, out index))
				throw new UnknownArgumentException($"Unknown argument {name}");

			return _invocationRawData.Arguments[index];
		}

		public T Get<T>(string name)
		{
			return (T)Get(name);
		}

		public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
		{
			return _paramNameToIndexMapping.Select(paramNameAndIndex => new KeyValuePair<string, object>(paramNameAndIndex.Key, this[paramNameAndIndex.Value])).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
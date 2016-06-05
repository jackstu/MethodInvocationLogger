using MethodInvocationLogger.Extensions;
using NUnit.Framework;

namespace MethodInvocationLogger.Tests
{
	public static class DictionaryLogDataHelperExtensions
	{
		public static void AssertContainsData(this DictionaryLogData logData, string itemKeyName)
		{
			Assert.True(logData.ContainsKey(itemKeyName));
		}

		public static void AssertDoesntContainData(this DictionaryLogData logData, string itemKeyName)
		{
			Assert.False(logData.ContainsKey(itemKeyName));
		}

		public static void AssertDataAreEqual(this DictionaryLogData logData, object expectedValue, string itemKeyName)
		{
			if (logData.ContainsKey(itemKeyName))
				Assert.AreEqual(expectedValue, logData[itemKeyName]);
			else
				Assert.Fail($"Can't check value of item {itemKeyName} because it's not exist");
		}

		public static void AssertDataIs<TDataType>(this DictionaryLogData logData, string itemKeyName)
		{
			if (!logData.ContainsKey(itemKeyName))
				Assert.Fail($"Can't check type of item {itemKeyName} because it's not exist");

			if (logData[itemKeyName] == null)
				Assert.Fail($"Can't check type of item {itemKeyName} because it's null");

			Assert.AreEqual(typeof(TDataType), logData[itemKeyName].GetType());
		}
	}
}
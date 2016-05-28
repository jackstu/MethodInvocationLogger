using System;
using System.Collections.Generic;

namespace MethodInvocationLogger.Extensions
{
	public static class MethodInvocationLoggingConfigurationExtensions
	{
		public static MethodInvocationLoggingConfiguration<DictionaryLogData> WithInvocationTime(this MethodInvocationLoggingConfiguration<DictionaryLogData> config)
		{
			return WithInvocationTime(config, "BeginTime");
		}

		public static MethodInvocationLoggingConfiguration<DictionaryLogData> WithInvocationTime(
			this MethodInvocationLoggingConfiguration<DictionaryLogData> config, string dataKeyName)
		{
			return config.PrepareLogData(((container, invocationData, logData) => logData[dataKeyName] = invocationData.Raw.Begin));
		}

		public static MethodInvocationLoggingConfiguration<DictionaryLogData> WithExecutionDuration(this MethodInvocationLoggingConfiguration<DictionaryLogData> config)
		{
			return WithExecutionDuration(config, "Duration");
		}

		public static MethodInvocationLoggingConfiguration<DictionaryLogData> WithExecutionDuration(
			this MethodInvocationLoggingConfiguration<DictionaryLogData> config, string dataKeyName)
		{
			return config.PrepareLogData(((container, invocationData, logData) => logData[dataKeyName] = invocationData.Raw.Duration));
		}
		
		public static MethodInvocationLoggingConfiguration<DictionaryLogData> WithAdditionalData<TDataValue>(this MethodInvocationLoggingConfiguration<DictionaryLogData> config,
			Func<IContainer, InvocationData, TDataValue> valueRetrieveFunction, string dataKeyName)
		{
			return config.PrepareLogData(((container, invocationData, logData) => logData[dataKeyName] = valueRetrieveFunction(container, invocationData)));
		}

		public static MethodInvocationLoggingConfiguration<DictionaryLogData> WithAdditionalData<TDataValue, TComponent>(
			this MethodInvocationLoggingConfiguration<DictionaryLogData> config,
			Func<TComponent, InvocationData, TDataValue> valueRetrieveFunction, string dataKeyName) where TComponent : class
		{
			return config.PrepareLogData(((container, invocationData, logData) =>
			{
				TComponent component = null;
				try
				{
					component = container.Resolve<TComponent>();
					logData[dataKeyName] = valueRetrieveFunction(component, invocationData);
				}
				finally
				{
					if (component != null)
						container.Release(component);
				}
			}));
		}

		public static MethodInvocationLoggingConfiguration<DictionaryLogData> WithArgument(this MethodInvocationLoggingConfiguration<DictionaryLogData> config,
			string argumentName)
		{
			return WithArguments(config, argumentName);
		}

		public static MethodInvocationLoggingConfiguration<DictionaryLogData> WithArguments(this MethodInvocationLoggingConfiguration<DictionaryLogData> config,
			params string[] argumentsNames)
		{
			return config.PrepareLogData((container, data, logData) =>
			{
				foreach (string argName in argumentsNames)
				{
					logData[argName] = data.Arguments[argName];
				}
			});
		}

		public static MethodInvocationLoggingConfiguration<DictionaryLogData> WithAllArguments(this MethodInvocationLoggingConfiguration<DictionaryLogData> config)
		{
			return config.PrepareLogData((container, data, logData) =>
			{
				foreach(KeyValuePair<string, object> arg in data.Arguments)
				{
					logData[arg.Key] = arg.Value;
				}
			});
		}
		
		public static MethodInvocationLoggingConfiguration<DictionaryLogData> WithExceptionIfThrown(this MethodInvocationLoggingConfiguration<DictionaryLogData> config,
			string dataKeyName)
		{
			return config.PrepareLogData(((container, invocationData, logData) =>
			{
				if (invocationData.Raw.Exception != null)
					logData[dataKeyName] = invocationData.Raw.Exception;
			}));
		}

		public static MethodInvocationLoggingConfiguration<DictionaryLogData> WithExceptionIfThrown(this MethodInvocationLoggingConfiguration<DictionaryLogData> config)
		{
			return WithExceptionIfThrown(config, "Exception");
		}

		public static MethodInvocationLoggingConfiguration<DictionaryLogData> OnlyIfSucceeded(this MethodInvocationLoggingConfiguration<DictionaryLogData> config)
		{
			return config.OnCondition((container, data) => data.Raw.Exception == null);
		}

		public static MethodInvocationLoggingConfiguration<DictionaryLogData> OnlyIfFailed(this MethodInvocationLoggingConfiguration<DictionaryLogData> config)
		{
			return config.OnCondition((container, data) => data.Raw.Exception != null);
		}

		public static MethodInvocationLoggingConfiguration<DictionaryLogData> WithConst<T>(
			this MethodInvocationLoggingConfiguration<DictionaryLogData> config, string dataKeyName, T dataValue)
		{
			return config.PrepareLogData(((container, invocationData, logData) => logData[dataKeyName] = dataValue));
		}

		public static MethodInvocationLoggingConfiguration<DictionaryLogData> WithMethodName(
		this MethodInvocationLoggingConfiguration<DictionaryLogData> config, string dataKeyName)
		{
			return config.WithAdditionalData((container, invocationData) => invocationData.Raw.MethodInfo.ToString(), dataKeyName);
		}

		public static MethodInvocationLoggingConfiguration<DictionaryLogData> WithMethodName(
		this MethodInvocationLoggingConfiguration<DictionaryLogData> config)
		{
			return config.WithMethodName("Method");
		}

		public static MethodInvocationLoggingConfiguration<DictionaryLogData> WithReturnedValue(
		this MethodInvocationLoggingConfiguration<DictionaryLogData> config, string dataKeyName)
		{
			return config.WithAdditionalData((container, invocationData) => invocationData.Raw.ReturnValue, dataKeyName);
		}

		public static MethodInvocationLoggingConfiguration<DictionaryLogData> WithReturnedValue(
		this MethodInvocationLoggingConfiguration<DictionaryLogData> config)
		{
			return config.WithReturnedValue("ReturnedValue");
		}
	}
}

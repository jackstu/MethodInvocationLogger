using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace MethodInvocationLogger
{
	internal class LoggerConfiguration<TLogData>
	{
		internal ILogWriter<TLogData> LogWriter { get; private set; }

		private readonly Dictionary<MethodInfo, List<MethodInvocationLoggingConfiguration<TLogData>>> _methodConfigs = new Dictionary<MethodInfo, List<MethodInvocationLoggingConfiguration<TLogData>>>();
		private readonly HashSet<Type> _registeredTypes = new HashSet<Type>();

		public void SetWriter(ILogWriter<TLogData> logWriter)
		{
			LogWriter = logWriter;
		}

		internal bool TypeRegistered(Type type)
		{
			return _registeredTypes.Contains(type);
		}

		internal bool MethodRegistered(MethodInfo method)
		{
			return _methodConfigs.ContainsKey(method);
		}

		internal IEnumerable<MethodInfo> GetRegisteredMethods()
		{
			return _methodConfigs.Keys;
		}

		internal IEnumerable<MethodInvocationLoggingConfiguration<TLogData>> GetConfigsForMethod(MethodInfo methodInfo)
		{
			List<MethodInvocationLoggingConfiguration<TLogData>> ret;
			if (!_methodConfigs.TryGetValue(methodInfo, out ret))
				ret = new List<MethodInvocationLoggingConfiguration<TLogData>>();

			return ret;
		}

		public MethodInvocationLoggingConfiguration<TLogData> LogInvocationOf<TLoggedClass>(Expression<Action<TLoggedClass>> expression)
		{
			Type type = typeof(TLoggedClass);
			MethodCallExpression methodCallExpression = (MethodCallExpression) expression.Body;
			MethodInfo methodInfo = methodCallExpression.Method;

			MethodInvocationLoggingConfiguration<TLogData> config = new MethodInvocationLoggingConfiguration<TLogData>(methodCallExpression);
			_registeredTypes.Add(type);
			
			if (!_methodConfigs.ContainsKey(methodInfo))
				_methodConfigs.Add(methodInfo, new List<MethodInvocationLoggingConfiguration<TLogData>>());

			_methodConfigs[methodInfo].Add(config);
			return config;
		}
	}
}
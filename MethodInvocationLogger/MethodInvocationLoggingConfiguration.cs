using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using MethodInvocationLogger.Exceptions;

namespace MethodInvocationLogger
{
	public class MethodInvocationLoggingConfiguration<TLogData>
	{
		public MethodInfo MethodInfo { get; }

		private readonly List<Func<IContainer, InvocationData, bool>> _conditions = new List<Func<IContainer, InvocationData, bool>>();
		private Action<IContainer, InvocationData, TLogData> _logDataPrepareAction;
		private readonly Dictionary<string, int> _paramNameToIndexMapping = new Dictionary<string, int>();

		public MethodInvocationLoggingConfiguration(MethodCallExpression methodCallExpression)
		{
			MethodInfo = methodCallExpression.Method;
			var args = methodCallExpression.Arguments.ToList();

			for (int i = 0; i < args.Count; i++)
			{
				using (ArgumentOptionsFluentContext context = new ArgumentOptionsFluentContext())
				{
					Expression.Lambda(args[i]).Compile().DynamicInvoke();

					var paramName = context.ArgumentInternalName ?? MethodInfo.GetParameters()[i].Name;

					if (_paramNameToIndexMapping.ContainsKey(paramName))
						throw new DuplicateParameterNameException(paramName);

					_paramNameToIndexMapping.Add(paramName, i);
				}
			}
		}

		internal bool ShouldBeLogged(IContainer container, InvocationRawData invocationRawData)
		{
			return _conditions.All(c => c(container, new InvocationData(invocationRawData, _paramNameToIndexMapping)));
		}

		internal TLogData CreateLogData(IContainer container, InvocationRawData invocationRawData)
		{
			TLogData logData = Activator.CreateInstance<TLogData>();
			_logDataPrepareAction?.Invoke(container, new InvocationData(invocationRawData, _paramNameToIndexMapping), logData);
			return logData;
		}

		public MethodInvocationLoggingConfiguration<TLogData> OnCondition(Func<IContainer, InvocationData, bool> condition)
		{
			_conditions.Add(condition);
			return this;
		}

		public MethodInvocationLoggingConfiguration<TLogData> PrepareLogData(Action<IContainer, InvocationData, TLogData> action)
		{
			if (_logDataPrepareAction == null)
				_logDataPrepareAction = action;
			else
				_logDataPrepareAction += action;

			return this;
		}
	}
}

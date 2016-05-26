using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MethodInvocationLogger.Castle
{
	internal class CompositeLoggerInternal : ICompositeLoggerInternal
	{
		readonly List<ILoggerInternal> _loggers = new List<ILoggerInternal>();

		public void Add(ILoggerInternal loggerInternal)
		{
			if (Has(loggerInternal))
				throw new Exception("Logger already added");

			_loggers.Add(loggerInternal);
		}

		public bool Has(ILoggerInternal loggerInternal)
		{
			return _loggers.Contains(loggerInternal);
		}

		public void ProcessInvocationData(InvocationRawData invocationRawData)
		{
			_loggers.ForEach(l => l.ProcessInvocationData(invocationRawData));
		}

		public bool MethodRegistered(MethodInfo method)
		{
			return _loggers.Any(l => l.MethodRegistered(method));
		}

		public bool TypeRegistered(Type type)
		{
			return _loggers.Any(l => l.TypeRegistered(type));
		}

		public ConfigurationValidationResult ValidateConfiguration(params IMethodInfoRequirement[] methodRequirements)
		{
			var result = new ConfigurationValidationResult();
			_loggers.ForEach(l => result.CopyErrorsFrom(l.ValidateConfiguration(methodRequirements)));
			return result;
		}
	}
}
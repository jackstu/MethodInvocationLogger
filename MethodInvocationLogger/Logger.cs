using System;
using System.Linq.Expressions;
using System.Reflection;
using MethodInvocationLogger.Exceptions;

namespace MethodInvocationLogger
{
	public class Logger<TLogData> : ILogger<TLogData>, ILoggerInternal
	{
        private readonly LoggerConfiguration<TLogData> _config = new LoggerConfiguration<TLogData>();
	    private IContainer _container;

		public MethodInvocationLoggingConfiguration<TLogData> LogInvocationOf<TLoggedClass>(Expression<Action<TLoggedClass>> expression)
		{
			return _config.LogInvocationOf(expression);
		}

		public ILogger<TLogData> SetContainer(IContainer container)
		{
			_container = container;
			return this;
		}

		public ILogger<TLogData> WriteTo(ILogOutput<TLogData> logOutput)
		{
			_config.SetWriter(logOutput);
			return this;
		}

		public bool MethodRegistered(MethodInfo methodInfo)
		{
			return _config.MethodRegistered(methodInfo);
		}

		public bool TypeRegistered(Type type)
		{
			return _config.TypeRegistered(type);
		}

		public ConfigurationValidationResult ValidateConfiguration(params IMethodInfoRequirement[] methodRequirements)
		{
			var result = new ConfigurationValidationResult();

			if (_container == null)
				result.AddError("Container not set.");

			if (_config.LogOutput == null)
				result.AddError("Writer not set.");

			foreach (MethodInfo method in _config.GetRegisteredMethods())
			{
				foreach (var methodRequirement in methodRequirements)
				{
					if (!methodRequirement.SatisfiedBy(method))
						result.AddError($"Logging invocation of {method} in {method.DeclaringType} is not possible. {methodRequirement.GetFriendlyMessage(method)}");
				}
			}

			return result;
		}

		public void ProcessInvocationData(InvocationRawData invocationRawData)
	    {
		    if (_config.LogOutput == null)
			    throw new WriterNotSetException();

		    foreach (var methodConfig in _config.GetConfigsForMethod(invocationRawData.MethodInfo))
		    {
				if (methodConfig.ShouldBeLogged(_container, invocationRawData))
					_config.LogOutput.WriteLog(methodConfig.CreateLogData(_container, invocationRawData));
		    }
		}
    }
}




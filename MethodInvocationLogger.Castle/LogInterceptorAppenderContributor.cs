using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core;
using Castle.Core.Internal;
using Castle.MicroKernel;
using Castle.MicroKernel.ModelBuilder;

namespace MethodInvocationLogger.Castle
{
	internal class LogInterceptorAppenderContributor : IContributeComponentModelConstruction
	{
		private readonly ICompositeLoggerInternal _compositeLoggerInternal;
		private readonly string _loggerInterceptorComponentName;
		internal Dictionary<Type, ImplementationInfo> ProcessedImplementations = new Dictionary<Type, ImplementationInfo>();

		public LogInterceptorAppenderContributor(ICompositeLoggerInternal compositeLoggerInternal, string loggerInterceptorComponentName)
		{
			_compositeLoggerInternal = compositeLoggerInternal;
			_loggerInterceptorComponentName = loggerInterceptorComponentName;
		}

		public void ProcessModel(IKernel kernel, ComponentModel model)
		{
			if (ProcessedImplementations.ContainsKey(model.Implementation))
				return;

			ImplementationInfo implementationInfo = new ImplementationInfo(model.Implementation);

			if ((_compositeLoggerInternal.TypeRegistered(model.Implementation))
			    || model.Services.Any(_compositeLoggerInternal.TypeRegistered))
			{
				model.Interceptors.Add(InterceptorReference.ForKey(_loggerInterceptorComponentName));
				implementationInfo.InterceptorAdded = true;
				model.Services.ForEach(implementationInfo.AddService);
			}
			else
			{
				implementationInfo.InterceptorAdded = false;
			}

			ProcessedImplementations.Add(model.Implementation, implementationInfo);
		}

		internal class ImplementationInfo
		{
			public Type Implementation { get; }
			public bool InterceptorAdded { get; set; }
			public HashSet<Type> Services { get; } = new HashSet<Type>();

			public ImplementationInfo(Type implementation)
			{
				Implementation = implementation;
			}

			public void AddService(Type type)
			{
				Services.Add(type);
			}
		}
	}
}
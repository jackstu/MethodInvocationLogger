using System;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using MethodInvocationLogger.Castle.MethodInfoRequirements;
using MethodInvocationLogger.Exceptions;

namespace MethodInvocationLogger.Castle
{
	public static class LoggerExtensions
	{
		internal static string InterceptorComponentName = "MethodInvocationLoggerInterceptor_3595848BB7AF4E4C8433B76AF53A0E97";

		public static ILogger<TDataItem> BindToWindsor<TDataItem>(this ILogger<TDataItem> logger, IKernel kernel)
		{
			ILoggerInternal loggerInternal = logger as ILoggerInternal;

			if (loggerInternal == null)
				throw new ArgumentException("logger must implement ILoggerInternal");

			if (!InterceptionEnabled(kernel))
				EnableInterception(kernel);

			ICompositeLoggerInternal compositeLoggerInternal = kernel.Resolve<ICompositeLoggerInternal>();

			if (compositeLoggerInternal.Has((ILoggerInternal) logger))
				throw new InvalidOperationException("Logger already bound");

			compositeLoggerInternal.Add((ILoggerInternal) logger);

			return logger.SetContainer(new WindsorContainerWrapper(kernel));
		}

		public static ILogger<TDataItem> Validate<TDataItem>(this ILogger<TDataItem> logger, IKernel kernel)
		{
			ILoggerInternal loggerInternal = logger as ILoggerInternal;

			if (loggerInternal == null)
				throw new ArgumentException("logger must implement ILoggerInternal");

			ConfigurationValidationResult result = new ConfigurationValidationResult();

			if (!InterceptionEnabled(kernel))
				result.AddError("Castle binding not set. Call BindToWindsor method.");

			ConfigurationValidationResult innerValidationResult = ((ILoggerInternal) logger).ValidateConfiguration(
				new DeclaringTypeMustBeAccessibleForProxyCreating(),
				new DeclaringTypeCannotBeOpenGeneric(),
				new DeclaringTypeCannotBeInterface(),
				new ReturnsSatisfiedIfUnableToCheck(
					new MethodMustBeVirtualIfDeclaringTypeRegisteredOnlyForItself(kernel)),
				new DeclaringTypeMustBeRegisteredInContainer(kernel),
				new ReturnsSatisfiedIfUnableToCheck(
					new DeclaringNonGenericTypeMustBeRegisteredInContainerAfterLoggerConfigured(kernel)));

			result.CopyErrorsFrom(innerValidationResult);

			if (!result.IsValid)
				throw new LoggerValidationException(result);

			return logger;
		}

		private static bool InterceptionEnabled(IKernel kernel)
		{
			return kernel.HasComponent(typeof (ICompositeLoggerInternal));
		}

		private static void EnableInterception(IKernel kernel)
		{
			ICompositeLoggerInternal compositeLoggerInternal = new CompositeLoggerInternal();

			kernel.Register(
				Component.For<ICompositeLoggerInternal>()
					.Instance(compositeLoggerInternal)
					.LifestyleSingleton());

			kernel.Register(Component.For<LoggerInterceptor>()
				.ImplementedBy<LoggerInterceptor>()
				.Named(InterceptorComponentName)
				.LifestyleSingleton());

			kernel.ComponentModelBuilder.AddContributor(new LogInterceptorAppenderContributor(compositeLoggerInternal,
				InterceptorComponentName));
		}
	}
}

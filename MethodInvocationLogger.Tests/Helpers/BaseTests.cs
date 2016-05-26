using System;
using System.Linq.Expressions;
using System.Reflection;
using NUnit.Framework;

namespace MethodInvocationLogger.Tests.Helpers
{
	public abstract class BaseTests<TLogData>
	{
		protected abstract Func<ILogger<TLogData>, IContainer> PrepareLoggerAndGetContainer { get; }

		protected void RunTest(Action<IContainer, ILogger<TLogData>> runAction, Action<TestLogWriter<TLogData>> checkResults = null)
		{
			TestLogWriter<TLogData> logWriter = new TestLogWriter<TLogData>();
			ILogger<TLogData> logger = LoggerFactory.Create<TLogData>().WriteTo(logWriter);

			using (IContainer container = PrepareLoggerAndGetContainer(logger))
			{
				runAction(container, logger);
				checkResults?.Invoke(logWriter);
			}
		}

		protected void RunTestAndAssertNumberOfLogDatasWritten(Action<IContainer, ILogger<TLogData>> testAction, int logDataCount)
		{
			RunTest(
				testAction,
				writer =>
				{
					Assert.AreEqual(logDataCount, writer.LogDataCount);
				});
		}

		protected void RunTestAndAssertExactlyOneLogDataWritten(Action<IContainer, ILogger<TLogData>> testAction)
		{
			RunTestAndAssertNumberOfLogDatasWritten(testAction, 1);
		}

		protected void RunTestAndAssertNoLogDataWritten(Action<IContainer, ILogger<TLogData>> testAction)
		{
			RunTestAndAssertNumberOfLogDatasWritten(testAction, 0);
		}

		protected void RunTestAndCheckFirstLogDataWritten(Action<IContainer, ILogger<TLogData>> testAction,
			Action<TLogData> checkFirstLogDataAction)
		{
			RunTest(testAction, writer => checkFirstLogDataAction(writer[0]));
		}

		protected T RegisterAndResolveComponent<T>(IContainer container) where T : class
		{
			container.RegisterComponent<T>();
			return container.ResolveComponent<T>();
		}

		protected MethodInfo Method<T>(Expression<Action<T>> expression)
		{
			return ((MethodCallExpression)expression.Body).Method;
		}
	}
}

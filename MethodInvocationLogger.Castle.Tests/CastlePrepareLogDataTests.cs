using System;
using Castle.Windsor;
using MethodInvocationLogger.Castle.Tests.Helpers;
using MethodInvocationLogger.Tests;
using NUnit.Framework;

namespace MethodInvocationLogger.Castle.Tests
{
	[TestFixture]
	public class CastlePrepareLogDataTests : PrepareLogDataTests
	{
		protected override Func<ILogger<LogDataTest>, MethodInvocationLogger.Tests.Helpers.IContainer> PrepareLoggerAndGetContainer
		{
			get
			{
				return logger =>
				{
					WindsorContainer container = new WindsorContainer();
					logger.BindToWindsor(container.Kernel);
					return new CastleContainer(container);
				};
			}
		}
	}
}

using System;
using Castle.Windsor;
using MethodInvocationLogger.Castle.Tests.Helpers;
using MethodInvocationLogger.Extensions;
using MethodInvocationLogger.Tests;
using NUnit.Framework;

namespace MethodInvocationLogger.Castle.Tests
{
	[TestFixture]
	public class CastleExtensionMethodsForDictionaryLogDataTests : ExtensionMethodsForDictionaryLogDataTests
	{
		protected override Func<ILogger<DictionaryLogData>, MethodInvocationLogger.Tests.Helpers.IContainer> PrepareLoggerAndGetContainer
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
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using MethodInvocationLogger.Tests.Helpers;
using NUnit.Framework;

namespace MethodInvocationLogger.Castle.Tests
{
	[TestFixture]
	public class DecoratorsTests
	{
		[Test]
		public void DecoratorsMethodsShouldBeLoggable()
		{
			WindsorContainer container = new WindsorContainer();
			TestLogWriter<object> logWriter = new TestLogWriter<object>();
			ILogger<object> logger = LoggerFactory.Create<object>().WriteTo(logWriter);
			logger.BindToWindsor(container.Kernel);

			logger.LogInvocationOf<TestClass>(t => t.TestMethod());
			logger.LogInvocationOf<TestClassDecorator>(t => t.TestMethod());

			container.Register(Component.For<ITest>().ImplementedBy<TestClassDecorator>());
			container.Register(Component.For<ITest>().ImplementedBy<TestClass>());

			container.Resolve<ITest>().TestMethod();

			Assert.AreEqual(2, logWriter.LogDataCount);
		}

		public class TestClass : ITest
		{
			public void TestMethod()
			{
			}
		}

		public class TestClassDecorator : ITest
		{
			private readonly ITest _test;

			public TestClassDecorator(ITest test)
			{
				_test = test;
			}

			public void TestMethod()
			{
				_test.TestMethod();
			}
		}

		public interface ITest
		{
			void TestMethod();
		}
	}
}
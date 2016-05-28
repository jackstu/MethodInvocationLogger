using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using MethodInvocationLogger.Tests.Helpers;
using NUnit.Framework;

namespace MethodInvocationLogger.Castle.Tests
{
	[TestFixture]
	public class DifferentComponentRegistraionMethodsTests
	{
		[Test]
		public void MethodsInComponentRegisteredForInterfaceShouldBeLoggable()
		{
			RunTestAndCheckIfOneItemLogged((logger, windsorContainer) =>
			{
				logger.LogInvocationOf<InvocationTestClass>(m => m.TestMethod());
				windsorContainer.Register(Component.For<IInvocationTestClass>().ImplementedBy<InvocationTestClass>());
				windsorContainer.Resolve<IInvocationTestClass>().TestMethod();
			});
		}

		[Test]
		public void MethodsInComponentRegisteredForItselfShouldBeLoggable()
		{
			RunTestAndCheckIfOneItemLogged((logger, windsorContainer) =>
			{
				logger.LogInvocationOf<InvocationTestClassWithoutImplementingInterface>(m => m.TestMethod());
				windsorContainer.Register(Component.For<InvocationTestClassWithoutImplementingInterface>().ImplementedBy<InvocationTestClassWithoutImplementingInterface>());
				windsorContainer.Resolve<InvocationTestClassWithoutImplementingInterface>().TestMethod();
			});
		}

		[Test]
		public void MethodsInComponentRegisteredUsingFromDescriptorShouldBeLoggable()
		{
			RunTestAndCheckIfOneItemLogged((logger, windsorContainer) =>
			{
				logger.LogInvocationOf<InvocationTestClassWithoutImplementingInterface>(m => m.TestMethod());
				windsorContainer.Register(Classes.FromThisAssembly().Where(t => t == typeof (InvocationTestClassWithoutImplementingInterface)).WithServiceSelf());
				windsorContainer.Resolve<InvocationTestClassWithoutImplementingInterface>().TestMethod();
			});
		}

		private void RunTestAndCheckIfOneItemLogged(Action<ILogger<object>, WindsorContainer> testAction)
		{
			WindsorContainer container = new WindsorContainer();
			TestLogOutput<object> logOutput = new TestLogOutput<object>();
			ILogger<object> logger = LoggerFactory.Create<object>().WriteTo(logOutput);
			logger.BindToWindsor(container.Kernel);

			testAction(logger, container);

			Assert.AreEqual(1, logOutput.LogDataCount);
		}

		public class InvocationTestClass : IInvocationTestClass
		{
			public void TestMethod() { }
		}

		public class InvocationTestClassWithoutImplementingInterface
		{
			public virtual void  TestMethod() { }
		}

		public interface IInvocationTestClass
		{
			void TestMethod();
		}
	}
}
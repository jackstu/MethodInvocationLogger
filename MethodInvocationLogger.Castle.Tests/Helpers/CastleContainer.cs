using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace MethodInvocationLogger.Castle.Tests.Helpers
{
	public class CastleContainer : MethodInvocationLogger.Tests.Helpers.IContainer
	{
		private readonly WindsorContainer _container;

		public CastleContainer(WindsorContainer container)
		{
			_container = container;
		}

		public void Dispose()
		{
			_container.Dispose();
		}

		public void RegisterComponent<T>(string componentName = null) where T : class
		{
			ComponentRegistration<T> componentRegistration = Component.For<T>().ImplementedBy<T>();
			if (componentName != null)
				componentRegistration.Named(componentName);

			_container.Register(componentRegistration);
		}

		public T ResolveComponent<T>(string componentName = null) where T : class
		{
			return componentName != null ? (T)_container.Resolve(componentName, typeof(T)) : _container.Resolve<T>();
		}
	}
}
using System;

namespace MethodInvocationLogger.Tests.Helpers
{
	public interface IContainer : IDisposable
	{
		void RegisterComponent<T>(string componentName = null) where T : class;
		T ResolveComponent<T>(string componentName = null) where T : class;
	}
}
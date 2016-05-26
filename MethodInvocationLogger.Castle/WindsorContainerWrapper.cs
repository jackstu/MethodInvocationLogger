using System;
using Castle.MicroKernel;

namespace MethodInvocationLogger.Castle
{
	internal class WindsorContainerWrapper : IContainer
	{
		private readonly IKernel _kernel;

		public WindsorContainerWrapper(IKernel kernel)
		{
			_kernel = kernel;
		}

		public T Resolve<T>()
		{
			return _kernel.Resolve<T>();
		}

		public object Resolve(Type type)
		{
			return _kernel.Resolve(type);
		}

		public void Release(object obj)
		{
			_kernel.ReleaseComponent(obj);
		}
	}
}
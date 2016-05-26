using System;

namespace MethodInvocationLogger
{
	public interface IContainer
	{
		T Resolve<T>();
		object Resolve(Type type);
		void Release(object obj);
	}
}
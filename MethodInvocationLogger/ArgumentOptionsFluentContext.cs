using System;

namespace MethodInvocationLogger
{
	internal class ArgumentOptionsFluentContext : IDisposable
	{
		[ThreadStatic]
		private static ArgumentOptionsFluentContext _current;

		public static ArgumentOptionsFluentContext Current => _current;
		public static bool IsActive => _current != null;

		public ArgumentOptionsFluentContext()
		{
			_current = this;
		}

		public string ArgumentInternalName { get; set; }

		public void Dispose()
		{
			_current = null;
		}
	}
}
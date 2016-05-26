namespace MethodInvocationLogger
{
	public static class Input
	{
		public static T Param<T>(string internalName)
		{
			if (ArgumentOptionsFluentContext.IsActive)
			{
				ArgumentOptionsFluentContext.Current.ArgumentInternalName = internalName;
			}
			return default(T);
		}

		public static T Param<T>()
		{
			if (ArgumentOptionsFluentContext.IsActive)
			{
				ArgumentOptionsFluentContext.Current.ArgumentInternalName = null;
			}
			return default(T);
		}
	}
}
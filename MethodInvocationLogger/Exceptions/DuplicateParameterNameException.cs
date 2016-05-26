namespace MethodInvocationLogger.Exceptions
{
	public class DuplicateParameterNameException : LoggerConfigurationException
	{
		public DuplicateParameterNameException(string paramName)
			: base($"Parameter names must be unique. Duplicate name: {paramName}")
		{
		}
	}
}
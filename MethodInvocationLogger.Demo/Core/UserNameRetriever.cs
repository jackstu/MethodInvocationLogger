namespace MethodInvocationLogger.Demo.Core
{
	public class UserNameRetriever : IUserNameRetriever
	{
		public string GetUserName(int userId)
		{
			// fake implementation
			return "Jack";
		}
	}
}
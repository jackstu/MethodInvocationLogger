namespace MethodInvocationLogger.Demo.Controllers
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
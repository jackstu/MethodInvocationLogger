using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
[assembly: InternalsVisibleTo("MethodInvocationLogger.Castle.Tests")]

namespace MethodInvocationLogger.Castle.Tests.InternalsVisible
{
    internal class InternalTestClass
    {
	    public class PublicNestedClass
	    {
		    public void TestMethod() { }
	    }

	    internal void TestMethod() { }
    }
}

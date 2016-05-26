using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

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

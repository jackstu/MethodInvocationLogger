using System;
using System.Reflection;
using Castle.DynamicProxy.Internal;

namespace MethodInvocationLogger.Castle.MethodInfoRequirements
{
	public class DeclaringTypeMustBeAccessibleForProxyCreating : IMethodInfoRequirement
	{
		public bool SatisfiedBy(MethodInfo method)
		{
			return IsPublic(method.DeclaringType) ||
			       (IsInternal(method.DeclaringType) && method.DeclaringType.Assembly.IsInternalToDynamicProxy());
		}

		#region Code from Castle DefaultProxyBuilder

		private static bool IsPublic(Type target)
		{
			return target.GetTypeInfo().IsPublic || target.GetTypeInfo().IsNestedPublic;
		}

		private static bool IsInternal(Type target)
		{
			var isTargetNested = target.IsNested;
			var isNestedAndInternal = isTargetNested && (target.GetTypeInfo().IsNestedAssembly || target.GetTypeInfo().IsNestedFamORAssem);
			var isInternalNotNested = target.IsVisible == false && isTargetNested == false;

			return isInternalNotNested || isNestedAndInternal;
		}

		#endregion

		public string GetFriendlyMessage(MethodInfo methodInfo)
		{
			return
				$"{methodInfo.DeclaringType} must be public or internal within assembly marked with [assembly: InternalsVisibleTo(\"DynamicProxyGenAssembly2\")] attribute.";
		}
	}
}
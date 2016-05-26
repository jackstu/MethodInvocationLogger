using System;
using Castle.Core;
using Castle.MicroKernel;

namespace MethodInvocationLogger.Castle.MethodInfoRequirements
{
	internal static class CastleHelper
	{
		public static ComponentModel GetComponentModelForImplementation(IKernel kernel, Type implementationType)
		{
			foreach (var node in kernel.GraphNodes)
			{
				var model = node as ComponentModel;

				if (model == null)
					continue;

				if ((model.Implementation.IsGenericTypeDefinition) && (implementationType.IsGenericType))
				{
					if (model.Implementation == implementationType.GetGenericTypeDefinition())
						return model;
				}
				else
				{
					if (model.Implementation == implementationType)
						return model;
				}
			}

			return null;
		}
	}
}
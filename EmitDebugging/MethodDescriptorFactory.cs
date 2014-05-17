using System;
using System.Reflection;
using System.Reflection.Emit;

namespace EmitDebugging
{
	internal static class MethodDescriptorFactory
	{
		internal static MethodDescriptor Create(MethodBase method)
		{
			return MethodDescriptorFactory.Create(method, method.DeclaringType.Assembly, false);
		}

		internal static MethodDescriptor Create(MethodBase method, bool isDeclaration)
		{
			return MethodDescriptorFactory.Create(method, method.DeclaringType.Assembly, isDeclaration);
		}

		internal static MethodDescriptor Create(MethodBase method, Assembly containingAssembly)
		{
			return MethodDescriptorFactory.Create(method, containingAssembly, false);
		}

		internal static MethodDescriptor Create(MethodBase method, Assembly containingAssembly, bool isDeclaration)
		{
			MethodDescriptor descriptor = null;

			if(method is MethodBuilder)
			{
				if(method.IsGenericMethod)
				{
					descriptor = new MethodGenericDeclarationDescriptor(method, containingAssembly);
				}
				else
				{
					descriptor = new MethodDescriptor(method, containingAssembly, isDeclaration);
				}
			}
			else if(method.IsGenericMethodDefinition)
			{
				descriptor = new MethodGenericDeclarationDescriptor(method, containingAssembly);
			}
			else if(method.IsGenericMethod)
			{
				descriptor = new MethodGenericInvocationDescriptor(method, containingAssembly);
			}
			else
			{
				descriptor = new MethodDescriptor(method, containingAssembly, isDeclaration);
			}

			return descriptor;
		}
	}
}

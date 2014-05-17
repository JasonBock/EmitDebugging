using System;
using System.Reflection;

namespace EmitDebugging.Extensions
{
	internal static class MemberInfoExtensions
	{
		internal static string GetName(this MemberInfo @this, Assembly containingAssembly, 
			bool isDeclaration)
		{
			var methodName = string.Empty;

			if(!isDeclaration)
			{
				methodName += new TypeDescriptor(
					@this.DeclaringType, containingAssembly, 
					@this.DeclaringType.IsGenericType).Value + "::";
			}
			
			return methodName + @this.Name;
		}
	}
}

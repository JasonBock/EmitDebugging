using EmitDebugging.Extensions;
using Spackle.Extensions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace EmitDebugging
{
	internal abstract class MethodGenericDescriptor : MethodDescriptor
	{
		protected MethodGenericDescriptor()
			: base()
		{
		}

		protected static string GetGenericDeclarationInformation(MethodInfo method, Assembly containingAssembly, bool isDeclaration)
		{
			var builder = new StringBuilder();

			if(method != null)
			{
				var genericArguments = method.GetGenericArguments();
				builder.Append("<");

				var descriptors = new List<string>();

				if(genericArguments != null)
				{
					foreach(var genericArgument in genericArguments)
					{
						var elementType = genericArgument.GetRootElementType();

						if(isDeclaration)
						{
							descriptors.Add(elementType.Name);
						}
						else
						{
							descriptors.Add(new TypeDescriptor(elementType, containingAssembly, true).Value);
						}
					}				
				}

				builder.Append(string.Join(", ", descriptors.ToArray()));
				builder.Append(">");
			}

			return builder.ToString();
		}
	}
}

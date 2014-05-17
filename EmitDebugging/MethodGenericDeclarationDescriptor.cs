using EmitDebugging.Extensions;
using Spackle.Extensions;
using Spackle.Reflection.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace EmitDebugging
{
	internal sealed class MethodGenericDeclarationDescriptor : MethodGenericDescriptor
	{
		internal MethodGenericDeclarationDescriptor(MethodBase method, Assembly containingAssembly)
			: base()
		{
			var descriptors = new List<string>();

			MethodGenericDeclarationDescriptor.AddAttributes(method, descriptors);
			MethodGenericDeclarationDescriptor.AddLocation(method, descriptors);
			MethodGenericDeclarationDescriptor.AddCallingConventions(method, descriptors);
			MethodGenericDeclarationDescriptor.AddReturnType(method, containingAssembly, descriptors);

			descriptors.Add(method.GetName(containingAssembly, true) +
				MethodGenericDescriptor.GetGenericDeclarationInformation(
					(method as MethodInfo), containingAssembly, true) +
				MethodGenericDeclarationDescriptor.GetMethodArgumentInformation(method, containingAssembly));
			descriptors.Add("cil managed");

			this.Value = string.Join(" ", descriptors.ToArray());
		}

		private static void AddReturnType(MethodBase method, Assembly containingAssembly, List<string> descriptors)
		{
			descriptors.Add(new TypeDescriptor(
				(method as MethodInfo).ReturnType, containingAssembly).Value);
		}

		private static void AddCallingConventions(MethodBase method, List<string> descriptors)
		{
			string callingConventions = method.GetCallingConventions();

			if(callingConventions.Length > 0)
			{
				descriptors.Add(callingConventions);
			}
		}

		private static void AddLocation(MethodBase method, List<string> descriptors)
		{
			if(method.IsStatic)
			{
				descriptors.Add("static");
			}
			else
			{
				descriptors.Add("instance");
			}
		}

		private static void AddAttributes(MethodBase method, List<string> descriptors)
		{
			descriptors.Add(".method");
			descriptors.Add(method.GetAttributes());
		}

		private static string GetMethodArgumentInformation(MethodBase method, Assembly containingAssembly)
		{
			var info = new StringBuilder();

			info.Append("(");
			var num = 0;

			var argumentTypes = method.GetParameterTypes();

			if(argumentTypes.Length > 0)
			{
				var list = new List<string>();
				var methodParameterDeclarationNames = method.GetGenericParameterDeclarationNames();

				foreach(var type in argumentTypes)
				{
					var arguments = new List<string>();

					var elementType = type.GetRootElementType();

					if(elementType.IsGenericParameter)
					{
						if(methodParameterDeclarationNames.Contains(elementType.Name))
						{
							arguments.Add("!!" + type.Name);
						}
						else
						{
							arguments.Add("!" + type.Name);
						}
					}
					else
					{
						arguments.Add(new TypeDescriptor(type, containingAssembly).Value);
					}

					arguments.Add("V_" + num.ToString(CultureInfo.CurrentCulture));
					list.Add(string.Join(" ", arguments.ToArray()));

					num++;
				}
				info.Append(string.Join(", ", list.ToArray()));
			}

			info.Append(")");

			return info.ToString();
		}
	}
}

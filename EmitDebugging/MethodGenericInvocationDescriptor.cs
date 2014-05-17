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
	internal sealed class MethodGenericInvocationDescriptor : MethodGenericDescriptor
	{
		private MethodGenericInvocationDescriptor()
			: base()
		{
		}

		internal MethodGenericInvocationDescriptor(MethodBase method, Assembly containingAssembly)
			: this()
		{
			var parts = new List<string>();

			MethodGenericInvocationDescriptor.AddLocation(method, parts);
			MethodGenericInvocationDescriptor.AddCallingConventions(method, parts);
			MethodGenericInvocationDescriptor.AddReturnValue(method, containingAssembly, parts);

			parts.Add(method.GetName(containingAssembly, false) +
				MethodGenericDescriptor.GetGenericDeclarationInformation(
					method as MethodInfo, containingAssembly, false) +
				MethodGenericInvocationDescriptor.GetMethodArgumentInformation(method, containingAssembly));

			this.Value = string.Join(" ", parts.ToArray());
		}

		private static void AddReturnValue(MethodBase method, Assembly containingAssembly, List<string> parts)
		{
			parts.Add(new TypeDescriptor(
				(method as MethodInfo).ReturnType, containingAssembly).Value);
		}

		private static void AddCallingConventions(MethodBase method, List<string> parts)
		{
			var callingConventions = method.GetCallingConventions();

			if(callingConventions.Length > 0)
			{
				parts.Add(callingConventions);
			}
		}

		private static void AddLocation(MethodBase method, List<string> parts)
		{
			if(!method.IsStatic)
			{
				parts.Add("instance");
			}
		}

		private static string GetMethodArgumentInformation(MethodBase method, Assembly containingAssembly)
		{
			var info = new StringBuilder();

			info.Append("(");
			var num = 0;

			var descriptors = new List<string>();
			var genericMethodDefinition = (method as MethodInfo).GetGenericMethodDefinition();

			if(genericMethodDefinition.DeclaringType.IsGenericType)
			{
				var genericTypeDefinition = genericMethodDefinition.DeclaringType.GetGenericTypeDefinition();
				genericMethodDefinition = MethodBase.GetMethodFromHandle(
					genericMethodDefinition.MethodHandle, genericTypeDefinition.TypeHandle) as MethodInfo;
			}

			var arguments = genericMethodDefinition.GetParameterTypes();

			var methodParameterDeclarationNames = genericMethodDefinition.GetGenericParameterDeclarationNames();
			var typeParameterDeclarationNames = GetTypeParameterDeclarationNames(method);

			foreach(var argument in arguments)
			{
				string descriptor = null;
				var elementType = argument.GetRootElementType();

				if(methodParameterDeclarationNames.Contains(elementType.Name))
				{
					descriptor = "!!" + methodParameterDeclarationNames.IndexOf(elementType.Name).ToString(CultureInfo.CurrentCulture) +
						argument.Name.Replace(elementType.Name, string.Empty);
				}
				else if(typeParameterDeclarationNames.Contains(elementType.Name))
				{
					descriptor = "!" + typeParameterDeclarationNames.IndexOf(elementType.Name).ToString(CultureInfo.CurrentCulture) +
						argument.Name.Replace(elementType.Name, string.Empty);
				}
				else
				{
					descriptor = new TypeDescriptor(argument, containingAssembly).Value;
				}

				descriptors.Add(descriptor);
				num++;
			}

			info.Append(string.Join(", ", descriptors.ToArray()));
			info.Append(")");

			return info.ToString();
		}

		private static List<string> GetTypeParameterDeclarationNames(MethodBase method)
		{
			var list = new List<string>();

			if(method.DeclaringType.IsGenericType)
			{
				var genericArguments = method.DeclaringType.GetGenericTypeDefinition().GetGenericArguments();
				
				if(genericArguments != null)
				{
					foreach(var type in genericArguments)
					{
						list.Add(type.Name);
					}				
				}
			}

			return list;
		}
	}
}

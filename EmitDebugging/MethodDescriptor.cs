using EmitDebugging.Extensions;
using Spackle.Reflection.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace EmitDebugging
{
	internal class MethodDescriptor : Descriptor
	{
		protected MethodDescriptor()
			: base()
		{
		}

		internal MethodDescriptor(MethodBase method, Assembly containingAssembly, bool isDeclaration)
			: base()
		{
			var parts = new List<string>();

			MethodDescriptor.AddAttributes(method, isDeclaration, parts);
			MethodDescriptor.AddLocation(method, isDeclaration, parts);
			MethodDescriptor.AddCallingConventions(method, parts);
			MethodDescriptor.AddReturnValue(method, containingAssembly, parts);

			parts.Add(method.GetName(containingAssembly, isDeclaration) +
				MethodDescriptor.GetMethodArgumentInformation(method, containingAssembly, isDeclaration));

			if(isDeclaration)
			{
				parts.Add("cil managed");
			}

			this.Value = string.Join(" ", parts.ToArray());
		}

		private static void AddReturnValue(MethodBase method, Assembly containingAssembly, List<string> parts)
		{
			var info = method as MethodInfo;

			if(info != null && info.ReturnType != null)
			{
				parts.Add(new TypeDescriptor(info.ReturnType, containingAssembly).Value);
			}
			else
			{
				parts.Add("void");
			}
		}

		private static void AddLocation(MethodBase method, bool isDeclaration, List<string> parts)
		{
			if(method.IsStatic)
			{
				if(isDeclaration)
				{
					parts.Add("static");
				}
			}
			else
			{
				parts.Add("instance");
			}
		}

		private static void AddCallingConventions(MethodBase method, List<string> parts)
		{
			var callingConventions = method.GetCallingConventions();

			if(callingConventions.Length > 0)
			{
				parts.Add(callingConventions);
			}
		}

		private static void AddAttributes(MethodBase method, bool isDeclaration, List<string> parts)
		{
			if(isDeclaration)
			{
				parts.Add(".method");
				parts.Add(method.GetAttributes());
			}
		}

		private static string GetMethodArgumentInformation(MethodBase method, Assembly containingAssembly, bool isDeclaration)
		{
			var information = new StringBuilder();

			information.Append("(");
			var i = 0;

			var argumentTypes = method.GetParameterTypes();

			if(argumentTypes.Length > 0)
			{
				var descriptors = new List<string>();

				foreach(var type in argumentTypes)
				{
					var argumentDescriptor = new List<string>() {
						new TypeDescriptor(type, containingAssembly).Value
					};

					if(isDeclaration)
					{
						argumentDescriptor.Add("V_" + i.ToString(CultureInfo.CurrentCulture));
					}

					descriptors.Add(string.Join(" ", argumentDescriptor.ToArray()));

					i++;
				}

				if(!isDeclaration &&
					((method.CallingConvention & CallingConventions.VarArgs) == CallingConventions.VarArgs))
				{
					descriptors.Add("...");
				}

				information.Append(string.Join(", ", descriptors.ToArray()));
			}

			information.Append(")");
			return information.ToString();
		}
	}
}

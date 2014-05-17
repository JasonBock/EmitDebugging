using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace EmitDebugging
{
	internal sealed class ManagedCalliDescriptor : CalliDescriptor
	{
		internal ManagedCalliDescriptor(CallingConventions callingConvention, Type returnType, Type[] parameterTypes, Type[] optionalParameterTypes)
			: base()
		{
			this.Value = string.Format(CultureInfo.CurrentCulture, "{0} {1}({2})",
				ManagedCalliDescriptor.GetConventions(callingConvention),
				new TypeDescriptor(returnType).Value,
				ManagedCalliDescriptor.GetArguments(callingConvention, parameterTypes, optionalParameterTypes));		
		}
				
		private static string GetArguments(CallingConventions callingConvention, Type[] parameterTypes, Type[] optionalParameterTypes)
		{
			var arguments = new List<string>();
			arguments.AddRange(CalliDescriptor.GetArguments(parameterTypes));
			
			if((callingConvention & CallingConventions.VarArgs) == CallingConventions.VarArgs)
			{
				arguments.Add("...");
				arguments.AddRange(CalliDescriptor.GetArguments(optionalParameterTypes));				
			}
			
			return string.Join(", ", arguments.ToArray());
		}
		
		private static string GetConventions(CallingConventions callingConvention)
		{
			var conventions = new List<string>();
			
			foreach(CallingConventions convention in Enum.GetValues(typeof(CallingConventions)))
			{
				if((callingConvention & convention) == convention)
				{
					conventions.Add(convention.ToString().ToLower(CultureInfo.CurrentCulture));
				}
			}
			
			return string.Join(" ", conventions.ToArray());
		}
	}
}

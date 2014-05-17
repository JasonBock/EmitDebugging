using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace EmitDebugging
{
	internal sealed class UnmanagedCalliDescriptor : CalliDescriptor
	{
		internal UnmanagedCalliDescriptor(CallingConvention unmanagedCallConv, Type returnType, Type[] parameterTypes)
			: base()
		{
			this.Value = string.Format(CultureInfo.CurrentCulture, "unmanaged {0} {1}({2})",
				unmanagedCallConv.ToString().ToLower(CultureInfo.CurrentCulture), 
				new TypeDescriptor(returnType).Value,
				string.Join(", ", CalliDescriptor.GetArguments(parameterTypes).ToArray()));
		}
	}
}

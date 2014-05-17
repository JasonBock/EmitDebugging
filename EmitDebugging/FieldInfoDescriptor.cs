using System;
using System.Globalization;
using System.Reflection;

namespace EmitDebugging
{
	internal sealed class FieldInfoDescriptor : Descriptor
	{
		internal FieldInfoDescriptor(FieldInfo field)
			: base()
		{
			this.Value = string.Format(CultureInfo.CurrentCulture, "{0} {1}::{2}",
				new TypeDescriptor(field.FieldType, false).Value,
				new TypeDescriptor(field.DeclaringType, false).Value,
				field.Name);
		}
	}
}

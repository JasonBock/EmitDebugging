using System;

namespace EmitDebugging
{
	internal abstract class Descriptor
	{
		protected Descriptor()
		{
		}

		protected internal string Value
		{
			get;
			protected set;
		}
	}
}

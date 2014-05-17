using System;
using System.Collections.Generic;

namespace EmitDebugging
{
	internal abstract class CalliDescriptor : Descriptor
	{
		protected static List<string> GetArguments(Type[] argumentTypes)
		{
			var arguments = new List<string>();

			if(argumentTypes != null && argumentTypes.Length > 0)
			{
				foreach(var argumentType in argumentTypes)
				{
					if(argumentType != null)
					{
						arguments.Add(new TypeDescriptor(argumentType).Value);					
					}
				}
			}

			return arguments;
		}
	}
}

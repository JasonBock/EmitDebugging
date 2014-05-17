using System;
using System.Collections.Generic;
using System.Reflection;

namespace EmitDebugging.Extensions
{
	internal static class TypeExtensions
	{
		internal static string GetAttributes(this Type @this)
		{
			var attributes = new List<string>();
			
			if(@this.IsPublic)
			{
				attributes.Add("public");
			}
			else if(@this.IsNotPublic)
			{
				attributes.Add("private");
			}
			
			if(@this.IsAbstract)
			{
				attributes.Add("abstract");
			}
			
			if(@this.IsSealed)
			{
				attributes.Add("sealed");
			}

			if(@this.IsAutoClass)
			{
				attributes.Add("auto");
			}

			if(@this.IsAnsiClass)
			{
				attributes.Add("ansi");
			}

			if(@this.IsSerializable)
			{
				attributes.Add("serializable");
			}

			if((@this.Attributes & TypeAttributes.BeforeFieldInit) == TypeAttributes.BeforeFieldInit)
			{
				attributes.Add("beforefieldinit");
			}
						
			return string.Join(" ", attributes.ToArray());
		}
	}
}

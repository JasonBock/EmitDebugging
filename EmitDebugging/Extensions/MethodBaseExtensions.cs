using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace EmitDebugging.Extensions
{
	internal static class MethodBaseExtensions
	{
		internal static string GetAttributes(this MethodBase @this)
		{
			var attributes = new List<string>();

			if(@this.IsPublic)
			{
				attributes.Add("public");
			}
			else if(@this.IsFamily)
			{
				attributes.Add("family");
			}
			else if(@this.IsFamilyAndAssembly)
			{
				attributes.Add("famandassembly");
			}
			else if(@this.IsFamilyOrAssembly)
			{
				attributes.Add("familyorassembly");
			}
			else if(@this.IsPrivate)
			{
				attributes.Add("private");
			}
			
			if((@this.Attributes & MethodAttributes.VtableLayoutMask) > MethodAttributes.ReuseSlot)
			{
				attributes.Add("newslot");
			}
			
			if(@this.IsAbstract)
			{
				attributes.Add("abstract");
			}
			
			if(@this.IsFinal)
			{
				attributes.Add("final");
			}
			
			if(@this.IsVirtual)
			{
				attributes.Add("virtual");
			}
			
			if(@this.IsHideBySig)
			{
				attributes.Add("hidebysig");
			}
			
			if(@this.IsSpecialName)
			{
				attributes.Add("specialname");
			}
			
			return string.Join(" ", attributes.ToArray());
		}

		internal static string GetCallingConventions(this MethodBase @this)
		{
			var callingConventions = new List<string>();

			if((@this.CallingConvention & CallingConventions.VarArgs) == CallingConventions.VarArgs)
			{
				callingConventions.Add("vararg");
			}
			
			if((@this.CallingConvention & CallingConventions.ExplicitThis) == CallingConventions.ExplicitThis)
			{
				callingConventions.Add("explicit");
			}
			
			return string.Join(" ", callingConventions.ToArray()).Trim();
		}

		internal static List<string> GetGenericParameterDeclarationNames(this MethodBase @this)
		{
			return new List<string>(Array.ConvertAll<Type, string>(
				@this.GetGenericArguments(), target => target.Name));
		}
	}
}

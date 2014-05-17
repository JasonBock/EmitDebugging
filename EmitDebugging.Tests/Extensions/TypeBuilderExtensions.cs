using System;
using System.Reflection;
using System.Reflection.Emit;

namespace EmitDebugging.Tests.Extensions
{
	internal static class TypeBuilderExtensions
	{
		internal static void GenerateConstructor(this TypeBuilder @this)
		{
			var constructor = @this.DefineConstructor(
				 MethodAttributes.Public | MethodAttributes.SpecialName |
				 MethodAttributes.RTSpecialName,
				 CallingConventions.Standard, Type.EmptyTypes);

			var constructorMethod = typeof(object).GetConstructor(Type.EmptyTypes);
			var constructorGenerator = constructor.GetILGenerator();

			constructorGenerator.Emit(OpCodes.Ldarg_0);
			constructorGenerator.Emit(OpCodes.Call, constructorMethod);
			constructorGenerator.Emit(OpCodes.Ret);
		}
	}
}

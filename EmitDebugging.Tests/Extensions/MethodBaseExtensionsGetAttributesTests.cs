using System;
using System.Reflection;
using System.Reflection.Emit;
using AssemblyVerifier;
using EmitDebugging.Extensions;
using Xunit;

namespace EmitDebugging.Tests.Extensions
{
	public static class MethodBaseExtensionsGetCallingConventionsTests
	{
		private const string AssemblyName = "MethodBaseExtensionsGetCallingConventionsTests";
		private const string MethodExplicitThisName = "ExplicitThis";
		private const string MethodVarArgName = "VarArg";
		private const string TypeName = "MethodBaseExtensionsGetCallingConventions";

		static MethodBaseExtensionsGetCallingConventionsTests()
		{
			var name = new AssemblyName();
			name.Name = MethodBaseExtensionsGetCallingConventionsTests.AssemblyName;
			name.Version = new Version(1, 0, 0, 0);
			var fileName = name.Name + ".dll";

			var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
				 name, AssemblyBuilderAccess.Save);

			var moduleBuilder = assemblyBuilder.DefineDynamicModule(name.Name, fileName, false);

			var typeName = MethodBaseExtensionsGetCallingConventionsTests.GenerateType(name, moduleBuilder);

			assemblyBuilder.Save(fileName);
			AssemblyVerification.Verify(assemblyBuilder);

			MethodBaseExtensionsGetCallingConventionsTests.Type = assemblyBuilder.GetType(typeName);
		}

		private static string GenerateType(AssemblyName name, ModuleBuilder moduleBuilder)
		{
			var typeName = name.Name + "." + MethodBaseExtensionsGetCallingConventionsTests.TypeName;

			var typeBuilder = moduleBuilder.DefineType(
				 typeName, TypeAttributes.Class | TypeAttributes.Public,
				 typeof(object));

			typeBuilder.GenerateConstructor();
			MethodBaseExtensionsGetCallingConventionsTests.GenerateMethods(typeBuilder);
			typeBuilder.CreateType();
			return typeName;
		}

		private static void GenerateMethods(TypeBuilder typeBuilder)
		{
			MethodBaseExtensionsGetCallingConventionsTests.GenerateVarArgMethod(typeBuilder);
			MethodBaseExtensionsGetCallingConventionsTests.GenerateExplicitThisMethod(typeBuilder);
		}

		private static void GenerateExplicitThisMethod(TypeBuilder typeBuilder)
		{
			var methodBuilder = typeBuilder.DefineMethod(
				 MethodBaseExtensionsGetCallingConventionsTests.MethodExplicitThisName,
				 MethodAttributes.Public | MethodAttributes.HideBySig |
					MethodAttributes.NewSlot | MethodAttributes.Virtual,
				CallingConventions.HasThis | CallingConventions.ExplicitThis, null,
				new Type[] { typeBuilder.UnderlyingSystemType });

			var methodGenerator = methodBuilder.GetILGenerator();
			methodGenerator.Emit(OpCodes.Ret);
		}

		private static void GenerateVarArgMethod(TypeBuilder typeBuilder)
		{
			var methodBuilder = typeBuilder.DefineMethod(
				 MethodBaseExtensionsGetCallingConventionsTests.MethodVarArgName,
				 MethodAttributes.Public | MethodAttributes.HideBySig |
					MethodAttributes.NewSlot | MethodAttributes.Virtual, CallingConventions.VarArgs);

			var methodGenerator = methodBuilder.GetILGenerator();
			methodGenerator.Emit(OpCodes.Ret);
		}

		[Fact]
		public static void GetCallingConventionsForConstructor()
		{
			var attributes = MethodBaseExtensionsGetCallingConventionsTests.Type.GetConstructor(
				Type.EmptyTypes).GetCallingConventions();
			Assert.Equal(string.Empty, attributes);
		}

		//[Fact]
		public static void GetCallingConventionsForExplicitThisMethod()
		{
			var attributes = MethodBaseExtensionsGetCallingConventionsTests.Type.GetMethod(
				MethodBaseExtensionsGetCallingConventionsTests.MethodExplicitThisName,
				BindingFlags.Public | BindingFlags.Instance).GetCallingConventions();
			Assert.Equal("explicit", attributes);
		}

		[Fact]
		public static void GetCallingConventionsForVarArgMethod()
		{
			var attributes = MethodBaseExtensionsGetCallingConventionsTests.Type.GetMethod(
				MethodBaseExtensionsGetCallingConventionsTests.MethodVarArgName,
				BindingFlags.Public | BindingFlags.Instance).GetCallingConventions();
			Assert.Equal("vararg", attributes);
		}

		private static Type Type { get; set; }
	}
}

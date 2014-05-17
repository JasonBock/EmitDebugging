using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using AssemblyVerifier;
using EmitDebugging.Extensions;
using Xunit;

namespace EmitDebugging.Tests.Extensions
{
	public static class MethodBaseExtensionsGetAttributesTests
	{
		private const string AssemblyName = "MethodBaseExtensionsGetAttributesTests";
		private const string MethodFamilyAndAssemblyInstanceHideBySigAbstractNewSlotVirtualName = "FamilyAndAssemblyInstanceHideBySigAbstractNewSlotVirtual";
		private const string MethodFamilyInstanceHideBySigAbstractNewSlotVirtualName = "FamilyInstanceHideBySigAbstractNewSlotVirtual";
		private const string MethodFamilyOrAssemblyInstanceHideBySigAbstractNewSlotVirtualName = "FamilyOrAssemblyInstanceHideBySigAbstractNewSlotVirtual";
		private const string MethodPrivateStaticName = "PrivateStatic";
		private const string MethodPublicHideBySigNewSlotFinalName = "PublicHideBySigNewSlotFinal";
		private const string MethodPublicHideBySigNewSlotVirtualName = "PublicHideBySigNewSlotVirtual";
		private const string TypeAbstractName = "AbstractMethodVariations";
		private const string TypeName = "MethodBaseExtensionsGetAttributesVariations";

		static MethodBaseExtensionsGetAttributesTests()
		{
			var name = new AssemblyName();
			name.Name = MethodBaseExtensionsGetAttributesTests.AssemblyName;
			name.Version = new Version(1, 0, 0, 0);
			var fileName = name.Name + ".dll";

			var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
				 name, AssemblyBuilderAccess.Save);

			var moduleBuilder = assemblyBuilder.DefineDynamicModule(name.Name, fileName, false);

			var typeName = MethodBaseExtensionsGetAttributesTests.GenerateType(name, moduleBuilder);
			var typeAbstractName = MethodBaseExtensionsGetAttributesTests.GenerateAbstractType(name, moduleBuilder);

			assemblyBuilder.Save(fileName);
			AssemblyVerification.Verify(assemblyBuilder);

			MethodBaseExtensionsGetAttributesTests.AbstractType = assemblyBuilder.GetType(typeAbstractName);
			MethodBaseExtensionsGetAttributesTests.Type = assemblyBuilder.GetType(typeName);
		}

		private static string GenerateAbstractType(AssemblyName name, ModuleBuilder moduleBuilder)
		{
			var typeName = name.Name + "." + MethodBaseExtensionsGetAttributesTests.TypeAbstractName;

			var typeBuilder = moduleBuilder.DefineType(
				 typeName, TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Abstract,
				 typeof(object));

			typeBuilder.GenerateConstructor();
			MethodBaseExtensionsGetAttributesTests.GenerateAbstractMethods(typeBuilder);
			typeBuilder.CreateType();
			return typeName;
		}

		private static string GenerateType(AssemblyName name, ModuleBuilder moduleBuilder)
		{
			var typeName = name.Name + "." + MethodBaseExtensionsGetAttributesTests.TypeName;

			var typeBuilder = moduleBuilder.DefineType(
				 typeName, TypeAttributes.Class | TypeAttributes.Public,
				 typeof(object));

			typeBuilder.GenerateConstructor();
			MethodBaseExtensionsGetAttributesTests.GenerateMethods(typeBuilder);
			typeBuilder.CreateType();
			return typeName;
		}

		private static void GenerateAbstractMethods(TypeBuilder typeBuilder)
		{
			MethodBaseExtensionsGetAttributesTests.GenerateFamilyInstanceHideBySigAbstractNewSlotVirtualMethod(typeBuilder);
			MethodBaseExtensionsGetAttributesTests.GenerateFamilyAndAssemblyInstanceHideBySigAbstractNewSlotVirtualMethod(typeBuilder);
			MethodBaseExtensionsGetAttributesTests.GenerateFamilyOrAssemblyInstanceHideBySigAbstractNewSlotVirtualMethod(typeBuilder);
		}

		private static void GenerateMethods(TypeBuilder typeBuilder)
		{
			MethodBaseExtensionsGetAttributesTests.GeneratePrivateStaticMethod(typeBuilder);
			MethodBaseExtensionsGetAttributesTests.GeneratePublicInstanceHideBySigNewSlotVirtualMethod(typeBuilder);
			MethodBaseExtensionsGetAttributesTests.GeneratePublicInstanceHideBySigNewSlotFinalMethod(typeBuilder);
		}

		private static void GeneratePrivateStaticMethod(TypeBuilder typeBuilder)
		{
			var methodBuilder = typeBuilder.DefineMethod(
				 MethodBaseExtensionsGetAttributesTests.MethodPrivateStaticName,
				 MethodAttributes.Private | MethodAttributes.Static);

			var methodGenerator = methodBuilder.GetILGenerator();
			methodGenerator.Emit(OpCodes.Ret);
		}

		private static void GenerateFamilyAndAssemblyInstanceHideBySigAbstractNewSlotVirtualMethod(TypeBuilder typeBuilder)
		{
			var methodBuilder = typeBuilder.DefineMethod(
				 MethodBaseExtensionsGetAttributesTests.MethodFamilyAndAssemblyInstanceHideBySigAbstractNewSlotVirtualName,
				 MethodAttributes.FamANDAssem | MethodAttributes.HideBySig | MethodAttributes.Abstract |
					MethodAttributes.NewSlot | MethodAttributes.Virtual);

			var methodGenerator = methodBuilder.GetILGenerator();
			methodGenerator.Emit(OpCodes.Ret);
		}

		private static void GenerateFamilyInstanceHideBySigAbstractNewSlotVirtualMethod(TypeBuilder typeBuilder)
		{
			var methodBuilder = typeBuilder.DefineMethod(
				 MethodBaseExtensionsGetAttributesTests.MethodFamilyInstanceHideBySigAbstractNewSlotVirtualName,
				 MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.Abstract |
					MethodAttributes.NewSlot | MethodAttributes.Virtual);

			var methodGenerator = methodBuilder.GetILGenerator();
			methodGenerator.Emit(OpCodes.Ret);
		}

		private static void GenerateFamilyOrAssemblyInstanceHideBySigAbstractNewSlotVirtualMethod(TypeBuilder typeBuilder)
		{
			var methodBuilder = typeBuilder.DefineMethod(
				 MethodBaseExtensionsGetAttributesTests.MethodFamilyOrAssemblyInstanceHideBySigAbstractNewSlotVirtualName,
				 MethodAttributes.FamORAssem | MethodAttributes.HideBySig | MethodAttributes.Abstract |
					MethodAttributes.NewSlot | MethodAttributes.Virtual);

			var methodGenerator = methodBuilder.GetILGenerator();
			methodGenerator.Emit(OpCodes.Ret);
		}

		private static void GeneratePublicInstanceHideBySigNewSlotVirtualMethod(TypeBuilder typeBuilder)
		{
			var methodBuilder = typeBuilder.DefineMethod(
				 MethodBaseExtensionsGetAttributesTests.MethodPublicHideBySigNewSlotVirtualName,
				 MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual);

			var methodGenerator = methodBuilder.GetILGenerator();
			methodGenerator.Emit(OpCodes.Ret);
		}

		private static void GeneratePublicInstanceHideBySigNewSlotFinalMethod(TypeBuilder typeBuilder)
		{
			var methodBuilder = typeBuilder.DefineMethod(
				 MethodBaseExtensionsGetAttributesTests.MethodPublicHideBySigNewSlotFinalName,
				 MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot |
					MethodAttributes.Final | MethodAttributes.Virtual);

			var methodGenerator = methodBuilder.GetILGenerator();
			methodGenerator.Emit(OpCodes.Ret);
		}

		[Fact]
		public static void GetAttributesForConstructor()
		{
			var attributes = MethodBaseExtensionsGetAttributesTests.AbstractType.GetConstructor(
				Type.EmptyTypes).GetAttributes();
			Assert.Equal("public specialname", attributes);
		}

		[Fact]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sig")]
		public static void GetAttributesForFamilyAndAssemblyInstanceHideBySigAbstractNewSlotVirtualMethod()
		{
			var attributes = MethodBaseExtensionsGetAttributesTests.AbstractType.GetMethod(
				MethodBaseExtensionsGetAttributesTests.MethodFamilyAndAssemblyInstanceHideBySigAbstractNewSlotVirtualName,
				BindingFlags.NonPublic | BindingFlags.Instance).GetAttributes();
			Assert.Equal("famandassembly newslot abstract virtual hidebysig", attributes);
		}

		[Fact]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sig")]
		public static void GetAttributesForFamilyInstanceHideBySigAbstractNewSlotVirtualMethod()
		{
			var attributes = MethodBaseExtensionsGetAttributesTests.AbstractType.GetMethod(
				MethodBaseExtensionsGetAttributesTests.MethodFamilyInstanceHideBySigAbstractNewSlotVirtualName,
				BindingFlags.NonPublic | BindingFlags.Instance).GetAttributes();
			Assert.Equal("family newslot abstract virtual hidebysig", attributes);
		}

		[Fact]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sig")]
		public static void GetAttributesForFamilyOrAssemblyInstanceHideBySigAbstractNewSlotVirtualMethod()
		{
			var attributes = MethodBaseExtensionsGetAttributesTests.AbstractType.GetMethod(
				MethodBaseExtensionsGetAttributesTests.MethodFamilyOrAssemblyInstanceHideBySigAbstractNewSlotVirtualName,
				BindingFlags.NonPublic | BindingFlags.Instance).GetAttributes();
			Assert.Equal("familyorassembly newslot abstract virtual hidebysig", attributes);
		}

		[Fact]
		public static void GetAttributesForPrivateStaticMethod()
		{
			var attributes = MethodBaseExtensionsGetAttributesTests.Type.GetMethod(
				MethodBaseExtensionsGetAttributesTests.MethodPrivateStaticName,
				BindingFlags.NonPublic | BindingFlags.Static).GetAttributes();
			Assert.Equal("private", attributes);
		}

		[Fact]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sig")]
		public static void GetAttributesForPublicInstanceHideBySigNewSlotFinalMethod()
		{
			var attributes = MethodBaseExtensionsGetAttributesTests.Type.GetMethod(
				MethodBaseExtensionsGetAttributesTests.MethodPublicHideBySigNewSlotFinalName,
				BindingFlags.Public | BindingFlags.Instance).GetAttributes();
			Assert.Equal("public newslot final virtual hidebysig", attributes);
		}

		[Fact]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sig")]
		public static void GetAttributesForPublicInstanceHideBySigNewSlotVirtualMethod()
		{
			var attributes = MethodBaseExtensionsGetAttributesTests.Type.GetMethod(
				MethodBaseExtensionsGetAttributesTests.MethodPublicHideBySigNewSlotVirtualName,
				BindingFlags.Public | BindingFlags.Instance).GetAttributes();
			Assert.Equal("public newslot virtual hidebysig", attributes);
		}

		private static Type AbstractType { get; set; }

		private static Type Type { get; set; }
	}
}

using System;
using System.Reflection;
using System.Reflection.Emit;
using AssemblyVerifier;
using EmitDebugging.Extensions;
using Xunit;

namespace EmitDebugging.Tests.Extensions
{
	public static class TypeExtensionsTests
	{
		private const string AssemblyName = "TypeExtensionsTests";
		private const string TypePublicAbstractSerializableAutoName = "PublicAbstractSerializableAuto";
		private const string TypeInternalSealedAnsiBeforeFieldInitName = "InternalSealedAnsiBeforeFieldInit";

		static TypeExtensionsTests()
		{
			var name = new AssemblyName();
			name.Name = TypeExtensionsTests.AssemblyName;
			name.Version = new Version(1, 0, 0, 0);
			var fileName = name.Name + ".dll";

			var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
				 name, AssemblyBuilderAccess.Save);

			var moduleBuilder = assemblyBuilder.DefineDynamicModule(name.Name, fileName, false);

			var typePublicName = TypeExtensionsTests.GeneratePublicAbstractSerializableAutoType(name, moduleBuilder);
			var typeInternalName = TypeExtensionsTests.GenerateInternalSealedAnsiBeforeFieldInitType(name, moduleBuilder);

			assemblyBuilder.Save(fileName);
			AssemblyVerification.Verify(assemblyBuilder);

			TypeExtensionsTests.TypePublicAbstractSerializableAuto = assemblyBuilder.GetType(typePublicName);
			TypeExtensionsTests.TypeInternalSealedAnsiBeforeFieldInit = assemblyBuilder.GetType(typeInternalName);
		}

		private static string GeneratePublicAbstractSerializableAutoType(AssemblyName name, ModuleBuilder moduleBuilder)
		{
			var typeName = name.Name + "." + TypeExtensionsTests.TypePublicAbstractSerializableAutoName;

			var typeBuilder = moduleBuilder.DefineType(
				 typeName, TypeAttributes.Public | TypeAttributes.Abstract |
					TypeAttributes.Serializable | TypeAttributes.AutoClass,
				 typeof(object));

			typeBuilder.GenerateConstructor();
			typeBuilder.CreateType();
			return typeName;
		}

		private static string GenerateInternalSealedAnsiBeforeFieldInitType(AssemblyName name, ModuleBuilder moduleBuilder)
		{
			var typeName = name.Name + "." + TypeExtensionsTests.TypeInternalSealedAnsiBeforeFieldInitName;

			var typeBuilder = moduleBuilder.DefineType(
				 typeName, TypeAttributes.NotPublic | TypeAttributes.Sealed |
					TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit,
				 typeof(object));

			typeBuilder.GenerateConstructor();
			typeBuilder.CreateType();
			return typeName;
		}

		[Fact]
		public static void GetAttributesForPublicAbstractSerializableClass()
		{
			Assert.Equal("public abstract auto serializable",
				TypeExtensionsTests.TypePublicAbstractSerializableAuto.GetAttributes());
		}

		[Fact]
		public static void GetAttributesForInternalSealedAnsiBeforeFieldInitClass()
		{
			Assert.Equal("private sealed ansi beforefieldinit",
				TypeExtensionsTests.TypeInternalSealedAnsiBeforeFieldInit.GetAttributes());
		}

		private static Type TypeInternalSealedAnsiBeforeFieldInit { get; set; }

		private static Type TypePublicAbstractSerializableAuto { get; set; }
	}
}

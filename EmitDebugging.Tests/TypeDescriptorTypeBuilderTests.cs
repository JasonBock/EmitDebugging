using System;
using System.Reflection;
using System.Reflection.Emit;
using Xunit;

namespace EmitDebugging.Tests
{
	public static class TypeDescriptorTypeBuilderTests
	{
		static TypeDescriptorTypeBuilderTests()
		{
			var assemblyName = new AssemblyName();
			assemblyName.Name = "TypeDescriptorTypeBuilderTests";
			var assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(
				assemblyName, AssemblyBuilderAccess.Save);

			var module = assembly.DefineDynamicModule(assemblyName.Name,
				assemblyName.Name + ".dll", true);

			var genericType = module.DefineType(
				assembly.GetName().Name + ".GenericType",
				TypeAttributes.Class | TypeAttributes.Sealed |
				TypeAttributes.Public, typeof(object));
			genericType.DefineGenericParameters("T", "U", "V");
			genericType.CreateType();

			var standardType = module.DefineType(
				assembly.GetName().Name + ".StandardType",
				TypeAttributes.Class | TypeAttributes.Sealed |
				TypeAttributes.Public, typeof(object));
			standardType.CreateType();

			TypeDescriptorTypeBuilderTests.GenericType = genericType;
			TypeDescriptorTypeBuilderTests.StandardType = standardType;
		}

		[Fact]
		public static void GetGenericTypeDescription()
		{
			Assert.Equal(
				"class TypeDescriptorTypeBuilderTests.GenericType`3<T, U, V>",
				new TypeDescriptor(TypeDescriptorTypeBuilderTests.GenericType).Value);
		}

		[Fact]
		public static void GetStandardTypeDescription()
		{
			Assert.Equal("class TypeDescriptorTypeBuilderTests.StandardType",
				new TypeDescriptor(TypeDescriptorTypeBuilderTests.StandardType).Value);
		}

		private static TypeBuilder GenericType { get; set; }

		private static TypeBuilder StandardType { get; set; }
	}
}

using System;
using System.Reflection;
using EmitDebugging.Tests.Types;
using Xunit;

namespace EmitDebugging.Tests
{
	public sealed class TypeDescriptorTests
		: AssemblyCreationTests
	{
		[Fact]
		public static void GetGenericTypeBuilderDescription()
		{
			using (var assembly = AssemblyCreationTests.CreateDebuggingAssembly("GenericTypeBuilderDescriptor"))
			{
				using (var type = AssemblyCreationTests.CreateDebuggingType(
					assembly, assembly.Builder.GetDynamicModule(assembly.Builder.GetName().Name), "Holder"))
				{
					type.Builder.DefineGenericParameters("T", "U", "V");

					var descriptor = new TypeDescriptor(type.Builder);
					Assert.Equal("class GenericTypeBuilderDescriptor.Holder`3<T, U, V>",
						descriptor.Value);
				}
			}
		}

		[Fact]
		public static void GetTypeBuilderDescription()
		{
			using (var assembly = AssemblyCreationTests.CreateDebuggingAssembly("TypeBuilderDescriptor"))
			{
				using (var type = AssemblyCreationTests.CreateDebuggingType(
					assembly, assembly.Builder.GetDynamicModule(assembly.Builder.GetName().Name), "Holder"))
				{
					var descriptor = new TypeDescriptor(type.Builder);
					Assert.Equal("class TypeBuilderDescriptor.Holder",
						descriptor.Value);
				}
			}
		}

		[Fact]
		public static void GetDerivedGenericTypeDescription()
		{
			var target = typeof(SimpleDerivedGenericType<Uri>);
			Assert.Equal(
				"class EmitDebugging.Tests.Types.SimpleDerivedGenericType`1<class [System]System.Uri>",
				new TypeDescriptor(target).Value);
			Assert.Equal(
				"EmitDebugging.Tests.Types.SimpleDerivedGenericType`1<class [System]System.Uri>",
				new TypeDescriptor(target, false).Value);
			Assert.Equal(
				"class EmitDebugging.Tests.Types.SimpleDerivedGenericType`1<class [System]System.Uri>&",
				new TypeDescriptor(target.MakeByRefType()).Value);
			Assert.Equal(
				"class EmitDebugging.Tests.Types.SimpleDerivedGenericType`1<class [System]System.Uri>[]",
				new TypeDescriptor(target.MakeArrayType()).Value);
			Assert.Equal(
				"class EmitDebugging.Tests.Types.SimpleDerivedGenericType`1<class [System]System.Uri>[]&",
				new TypeDescriptor(target.MakeArrayType().MakeByRefType()).Value);
		}

		[Fact]
		public static void GetExternalSimpleReferenceTypeScenarios()
		{
			var target = typeof(SimpleReferenceType);
			var external = typeof(TypeDescriptor);
			Assert.Equal(
				"class [EmitDebugging.Tests]EmitDebugging.Tests.Types.SimpleReferenceType",
				new TypeDescriptor(target, external.Assembly).Value);
			Assert.Equal(
				"[EmitDebugging.Tests]EmitDebugging.Tests.Types.SimpleReferenceType",
				new TypeDescriptor(target, external.Assembly, false).Value);
			Assert.Equal(
				"class [EmitDebugging.Tests]EmitDebugging.Tests.Types.SimpleReferenceType&",
				new TypeDescriptor(target.MakeByRefType(), external.Assembly).Value);
			Assert.Equal(
				"class [EmitDebugging.Tests]EmitDebugging.Tests.Types.SimpleReferenceType[]",
				new TypeDescriptor(target.MakeArrayType(), external.Assembly).Value);
			Assert.Equal(
				"class [EmitDebugging.Tests]EmitDebugging.Tests.Types.SimpleReferenceType[]&",
				new TypeDescriptor(target.MakeArrayType().MakeByRefType(), external.Assembly).Value);
		}

		[Fact]
		public static void GetExternalValueTypeDescription()
		{
			var target = typeof(SimpleValueType);
			var external = typeof(TypeDescriptor);
			Assert.Equal(
				"valuetype [EmitDebugging.Tests]EmitDebugging.Tests.Types.SimpleValueType",
				new TypeDescriptor(target, external.Assembly).Value);
			Assert.Equal(
				"[EmitDebugging.Tests]EmitDebugging.Tests.Types.SimpleValueType",
				new TypeDescriptor(target, external.Assembly, false).Value);
			Assert.Equal(
				"valuetype [EmitDebugging.Tests]EmitDebugging.Tests.Types.SimpleValueType&",
				new TypeDescriptor(target.MakeByRefType(), external.Assembly).Value);
			Assert.Equal(
				"valuetype [EmitDebugging.Tests]EmitDebugging.Tests.Types.SimpleValueType[]",
				new TypeDescriptor(target.MakeArrayType(), external.Assembly).Value);
			Assert.Equal(
				"valuetype [EmitDebugging.Tests]EmitDebugging.Tests.Types.SimpleValueType[]&",
				new TypeDescriptor(target.MakeArrayType().MakeByRefType(), external.Assembly).Value);
		}

		[Fact]
		public static void GetFloat32Scenarios()
		{
			var target = typeof(float);
			Assert.Equal(
				"float32", new TypeDescriptor(target).Value);
			Assert.Equal(
				"float32&", new TypeDescriptor(target.MakeByRefType()).Value);
			Assert.Equal(
				"float32[]", new TypeDescriptor(target.MakeArrayType()).Value);
			Assert.Equal(
				"float32[]&", new TypeDescriptor(target.MakeArrayType().MakeByRefType()).Value);
		}

		[Fact]
		public static void GetFloat64Scenarios()
		{
			var target = typeof(double);
			Assert.Equal(
				"float64", new TypeDescriptor(target).Value);
			Assert.Equal(
				"float64&", new TypeDescriptor(target.MakeByRefType()).Value);
			Assert.Equal(
				"float64[]", new TypeDescriptor(target.MakeArrayType()).Value);
			Assert.Equal(
				"float64[]&", new TypeDescriptor(target.MakeArrayType().MakeByRefType()).Value);
		}

		[Fact]
		public static void GetGenericTypeDescription()
		{
			var target = typeof(SimpleGenericType<MethodInfo, Uri>);
			Assert.Equal(
				"class EmitDebugging.Tests.Types.SimpleGenericType`2<class [mscorlib]System.Reflection.MethodInfo, class [System]System.Uri>",
				new TypeDescriptor(target).Value);
			Assert.Equal(
				"EmitDebugging.Tests.Types.SimpleGenericType`2<class [mscorlib]System.Reflection.MethodInfo, class [System]System.Uri>",
				new TypeDescriptor(target, false).Value);
			Assert.Equal(
				"class EmitDebugging.Tests.Types.SimpleGenericType`2<class [mscorlib]System.Reflection.MethodInfo, class [System]System.Uri>&",
				new TypeDescriptor(target.MakeByRefType()).Value);
			Assert.Equal(
				"class EmitDebugging.Tests.Types.SimpleGenericType`2<class [mscorlib]System.Reflection.MethodInfo, class [System]System.Uri>[]",
				new TypeDescriptor(target.MakeArrayType()).Value);
			Assert.Equal(
				"class EmitDebugging.Tests.Types.SimpleGenericType`2<class [mscorlib]System.Reflection.MethodInfo, class [System]System.Uri>[]&",
				new TypeDescriptor(target.MakeArrayType().MakeByRefType()).Value);
		}

		[Fact]
		public static void GetOpenGenericTypeDescription()
		{
			var target = typeof(SimpleGenericType<,>);
			Assert.Equal(
				"class EmitDebugging.Tests.Types.SimpleGenericType`2<T, U>",
				new TypeDescriptor(target).Value);
			Assert.Equal(
				"EmitDebugging.Tests.Types.SimpleGenericType`2<T, U>",
				new TypeDescriptor(target, false).Value);
			Assert.Equal(
				"class EmitDebugging.Tests.Types.SimpleGenericType`2<T, U>&",
				new TypeDescriptor(target.MakeByRefType()).Value);
			Assert.Equal(
				"class EmitDebugging.Tests.Types.SimpleGenericType`2<T, U>[]",
				new TypeDescriptor(target.MakeArrayType()).Value);
			Assert.Equal(
				"class EmitDebugging.Tests.Types.SimpleGenericType`2<T, U>[]&",
				new TypeDescriptor(target.MakeArrayType().MakeByRefType()).Value);
		}

		[Fact]
		public static void GetInt16Scenarios()
		{
			var target = typeof(short);
			Assert.Equal("int16", new TypeDescriptor(target).Value);
			Assert.Equal("int16&", new TypeDescriptor(target.MakeByRefType()).Value);
			Assert.Equal("int16[]", new TypeDescriptor(target.MakeArrayType()).Value);
			Assert.Equal("int16[]&", new TypeDescriptor(target.MakeArrayType().MakeByRefType()).Value);
		}

		[Fact]
		public static void GetInt32Scenarios()
		{
			var target = typeof(int);
			Assert.Equal("int32", new TypeDescriptor(target).Value);
			Assert.Equal("int32&", new TypeDescriptor(target.MakeByRefType()).Value);
			Assert.Equal("int32[]", new TypeDescriptor(target.MakeArrayType()).Value);
			Assert.Equal("int32[]&", new TypeDescriptor(target.MakeArrayType().MakeByRefType()).Value);
		}

		[Fact]
		public static void GetInt64Scenarios()
		{
			var target = typeof(long);
			Assert.Equal("int64", new TypeDescriptor(target).Value);
			Assert.Equal("int64&", new TypeDescriptor(target.MakeByRefType()).Value);
			Assert.Equal("int64[]", new TypeDescriptor(target.MakeArrayType()).Value);
			Assert.Equal("int64[]&", new TypeDescriptor(target.MakeArrayType().MakeByRefType()).Value);
		}

		[Fact]
		public static void GetInt8Scenarios()
		{
			var target = typeof(sbyte);
			Assert.Equal("int8", new TypeDescriptor(target).Value);
			Assert.Equal("int8&", new TypeDescriptor(target.MakeByRefType()).Value);
			Assert.Equal("int8[]", new TypeDescriptor(target.MakeArrayType()).Value);
			Assert.Equal("int8[]&", new TypeDescriptor(target.MakeArrayType().MakeByRefType()).Value);
		}

		[Fact]
		public static void GetObjectScenarios()
		{
			var target = typeof(object);
			Assert.Equal("object", new TypeDescriptor(target).Value);
			Assert.Equal("object&", new TypeDescriptor(target.MakeByRefType()).Value);
			Assert.Equal("object[]", new TypeDescriptor(target.MakeArrayType()).Value);
			Assert.Equal("object[]&", new TypeDescriptor(target.MakeArrayType().MakeByRefType()).Value);
		}

		[Fact]
		public static void GetSimpleReferenceTypeScenarios()
		{
			var target = typeof(SimpleReferenceType);
			Assert.Equal(
				"class EmitDebugging.Tests.Types.SimpleReferenceType",
				new TypeDescriptor(target).Value);
			Assert.Equal(
				"EmitDebugging.Tests.Types.SimpleReferenceType",
				new TypeDescriptor(target, false).Value);
			Assert.Equal(
				"class EmitDebugging.Tests.Types.SimpleReferenceType&",
				new TypeDescriptor(target.MakeByRefType()).Value);
			Assert.Equal(
				"class EmitDebugging.Tests.Types.SimpleReferenceType[]",
				new TypeDescriptor(target.MakeArrayType()).Value);
			Assert.Equal(
				"class EmitDebugging.Tests.Types.SimpleReferenceType[]&",
				new TypeDescriptor(target.MakeArrayType().MakeByRefType()).Value);
		}

		[Fact]
		public static void GetSimpleValueTypeScenarios()
		{
			var target = typeof(SimpleValueType);
			Assert.Equal(
				"valuetype EmitDebugging.Tests.Types.SimpleValueType",
				new TypeDescriptor(target).Value);
			Assert.Equal(
				"EmitDebugging.Tests.Types.SimpleValueType",
				new TypeDescriptor(target, false).Value);
			Assert.Equal(
				"valuetype EmitDebugging.Tests.Types.SimpleValueType&",
				new TypeDescriptor(target.MakeByRefType()).Value);
			Assert.Equal(
				"valuetype EmitDebugging.Tests.Types.SimpleValueType[]",
				new TypeDescriptor(target.MakeArrayType()).Value);
			Assert.Equal(
				"valuetype EmitDebugging.Tests.Types.SimpleValueType[]&",
				new TypeDescriptor(target.MakeArrayType().MakeByRefType()).Value);
		}

		[Fact]
		public static void GetStringScenarios()
		{
			var target = typeof(string);
			Assert.Equal("string", new TypeDescriptor(target).Value);
			Assert.Equal("string&", new TypeDescriptor(target.MakeByRefType()).Value);
			Assert.Equal("string[]", new TypeDescriptor(target.MakeArrayType()).Value);
			Assert.Equal("string[]&", new TypeDescriptor(target.MakeArrayType().MakeByRefType()).Value);
		}

		[Fact]
		public static void GetUInt16Scenarios()
		{
			var target = typeof(ushort);
			Assert.Equal("uint16", new TypeDescriptor(target).Value);
			Assert.Equal("uint16&", new TypeDescriptor(target.MakeByRefType()).Value);
			Assert.Equal("uint16[]", new TypeDescriptor(target.MakeArrayType()).Value);
			Assert.Equal("uint16[]&", new TypeDescriptor(target.MakeArrayType().MakeByRefType()).Value);
		}

		[Fact]
		public static void GetUInt32Scenarios()
		{
			var target = typeof(uint);
			Assert.Equal("uint32", new TypeDescriptor(target).Value);
			Assert.Equal("uint32&", new TypeDescriptor(target.MakeByRefType()).Value);
			Assert.Equal("uint32[]", new TypeDescriptor(target.MakeArrayType()).Value);
			Assert.Equal("uint32[]&", new TypeDescriptor(target.MakeArrayType().MakeByRefType()).Value);
		}

		[Fact]
		public static void GetUInt64Scenarios()
		{
			var target = typeof(ulong);
			Assert.Equal("uint64", new TypeDescriptor(target).Value);
			Assert.Equal("uint64&", new TypeDescriptor(target.MakeByRefType()).Value);
			Assert.Equal("uint64[]", new TypeDescriptor(target.MakeArrayType()).Value);
			Assert.Equal("uint64[]&", new TypeDescriptor(target.MakeArrayType().MakeByRefType()).Value);
		}

		[Fact]
		public static void GetUInt8Scenarios()
		{
			var target = typeof(byte);
			Assert.Equal("uint8", new TypeDescriptor(target).Value);
			Assert.Equal("uint8&", new TypeDescriptor(target.MakeByRefType()).Value);
			Assert.Equal("uint8[]", new TypeDescriptor(target.MakeArrayType()).Value);
			Assert.Equal("uint8[]&", new TypeDescriptor(target.MakeArrayType().MakeByRefType()).Value);
		}

		[Fact]
		public static void GetNullScenario()
		{
			Assert.Equal("void", new TypeDescriptor(null).Value);
		}

		[Fact]
		public static void GetVoidScenarios()
		{
			Assert.Equal("void", new TypeDescriptor(typeof(void)).Value);
		}

		[Fact]
		public static void PassNullAssembly()
		{
			Assert.Throws<ArgumentNullException>(() => new TypeDescriptor(typeof(long), (null as Assembly)));
		}
	}
}

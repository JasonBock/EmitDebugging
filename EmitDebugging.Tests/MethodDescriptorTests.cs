using System;
using System.Reflection;
using Xunit;

namespace EmitDebugging.Tests.Types
{
	public static class MethodDescriptorTests
	{
		[Fact]
		public static void GetConstructorAsDeclaration()
		{
			MethodBase constructor = typeof(SimpleReferenceType).GetConstructor(Type.EmptyTypes);
			Assert.Equal(".method public hidebysig specialname instance void .ctor() cil managed",
				MethodDescriptorFactory.Create(constructor, true).Value);
		}

		[Fact]
		public static void GetConstructorAsInvocation()
		{
			MethodBase constructor = typeof(SimpleReferenceType).GetConstructor(Type.EmptyTypes);
			Assert.Equal(
				"instance void EmitDebugging.Tests.Types.SimpleReferenceType::.ctor()",
				MethodDescriptorFactory.Create(constructor).Value);
		}

		[Fact]
		public static void GetConstructorAsInvocationViaMethodAndAssembly()
		{
			MethodBase constructor = typeof(SimpleReferenceType).GetConstructor(Type.EmptyTypes);
			Assert.Equal("instance void EmitDebugging.Tests.Types.SimpleReferenceType::.ctor()",
				MethodDescriptorFactory.Create(constructor, constructor.DeclaringType.Assembly).Value);
		}

		[Fact]
		public static void GetConstructorWithArgumentAsDeclaration()
		{
			MethodBase constructor = typeof(SimpleReferenceType).GetConstructor(
				new Type[] { typeof(string) });
			Assert.Equal(
				".method public hidebysig specialname instance void .ctor(string V_0) cil managed",
				MethodDescriptorFactory.Create(constructor, true).Value);
		}

		[Fact]
		public static void GetConstructorWithArgumentsAsInvocation()
		{
			MethodBase constructor = typeof(SimpleReferenceType).GetConstructor(
				new Type[] { typeof(string) });
			Assert.Equal(
				"instance void EmitDebugging.Tests.Types.SimpleReferenceType::.ctor(string)",
				MethodDescriptorFactory.Create(constructor).Value);
		}

		[Fact]
		public static void GetGenericMethodOneAsDeclaration()
		{
			MethodInfo method = typeof(SimpleGenericType<,>).GetMethod("MethodOne");
			Assert.Equal(
				".method public hidebysig instance void MethodOne<V>(!T V_0, !U V_1, !!V V_2) cil managed",
				MethodDescriptorFactory.Create(method, true).Value);
		}

		[Fact]
		public static void GetGenericMethodOneAsInvocation()
		{
			MethodInfo method = typeof(SimpleGenericType<MethodInfo, Uri>).GetMethod(
				"MethodOne").MakeGenericMethod(new Type[] { typeof(string) });
			Assert.Equal(
				"instance void class EmitDebugging.Tests.Types.SimpleGenericType`2<class [mscorlib]System.Reflection.MethodInfo, class [System]System.Uri>::MethodOne<string>(!0, !1, !!0)",
				MethodDescriptorFactory.Create(method).Value);
		}

		[Fact]
		public static void GetGenericMethodOneOnValueTypeAsDeclaration()
		{
			MethodInfo method = typeof(SimpleGenericValueType<,>).GetMethod("MethodOne");
			Assert.Equal(
				".method public hidebysig instance void MethodOne<V>(!T V_0, !!V V_1, !U V_2) cil managed",
				MethodDescriptorFactory.Create(method, true).Value);
		}

		[Fact]
		public static void GetGenericMethodOneOnValueTypeAsInvocation()
		{
			MethodInfo method = typeof(SimpleGenericValueType<Guid, string>).GetMethod(
				"MethodOne").MakeGenericMethod(new Type[] { typeof(int) });
			Assert.Equal(
				"instance void valuetype EmitDebugging.Tests.Types.SimpleGenericValueType`2<valuetype [mscorlib]System.Guid, string>::MethodOne<int32>(!0, !!0, !1)",
				MethodDescriptorFactory.Create(method).Value);
		}

		[Fact]
		public static void GetGenericMethodTwoAsDeclaration()
		{
			MethodInfo method = typeof(SimpleGenericType<,>).GetMethod("MethodTwo");
			Assert.Equal(
				".method public hidebysig instance void MethodTwo<V>(string V_0, !!V V_1) cil managed",
				MethodDescriptorFactory.Create(method, true).Value);
		}

		[Fact]
		public static void GetGenericMethodTwoAsInvocation()
		{
			MethodInfo method = typeof(SimpleGenericType<MethodInfo, Uri>).GetMethod(
				"MethodTwo").MakeGenericMethod(new Type[] { typeof(string) });
			Assert.Equal(
				"instance void class EmitDebugging.Tests.Types.SimpleGenericType`2<class [mscorlib]System.Reflection.MethodInfo, class [System]System.Uri>::MethodTwo<string>(string, !!0)",
				MethodDescriptorFactory.Create(method).Value);
		}

		[Fact]
		public static void GetMethodFiveAsDeclaration()
		{
			MethodBase method = typeof(SimpleReferenceType).GetMethod("MethodFive");
			Assert.Equal(
				".method public hidebysig instance void MethodFive(string[] V_0, string[]& V_1, valuetype EmitDebugging.Tests.Types.SimpleValueType[] V_2, valuetype EmitDebugging.Tests.Types.SimpleValueType[]& V_3) cil managed",
				MethodDescriptorFactory.Create(method, true).Value);
		}

		[Fact]
		public static void GetMethodFiveAsInvocation()
		{
			MethodBase method = typeof(SimpleReferenceType).GetMethod("MethodFive");
			Assert.Equal(
				"instance void EmitDebugging.Tests.Types.SimpleReferenceType::MethodFive(string[], string[]&, valuetype EmitDebugging.Tests.Types.SimpleValueType[], valuetype EmitDebugging.Tests.Types.SimpleValueType[]&)",
				MethodDescriptorFactory.Create(method).Value);
		}

		[Fact]
		public static void GetMethodFourAsDeclaration()
		{
			MethodBase method = typeof(SimpleReferenceType).GetMethod("MethodFour");
			Assert.Equal(
				".method public newslot virtual hidebysig instance void MethodFour<T, U, V>(!!T V_0, !!U V_1, !!V V_2) cil managed",
				MethodDescriptorFactory.Create(method, true).Value);
		}

		[Fact]
		public static void GetMethodFourAsInvocation()
		{
			MethodInfo method = typeof(SimpleReferenceType).GetMethod(
				"MethodFour").MakeGenericMethod(new Type[] { typeof(Guid), typeof(string), typeof(int) });
			Assert.Equal(
				"instance void EmitDebugging.Tests.Types.SimpleReferenceType::MethodFour<valuetype [mscorlib]System.Guid, string, int32>(!!0, !!1, !!2)",
				MethodDescriptorFactory.Create(method).Value);
		}

		[Fact]
		public static void GetMethodOneAsDeclaration()
		{
			MethodBase method = typeof(SimpleReferenceType).GetMethod("MethodOne");
			Assert.Equal(
				".method public hidebysig instance string MethodOne(int32 V_0, valuetype EmitDebugging.Tests.Types.SimpleValueType V_1) cil managed",
				MethodDescriptorFactory.Create(method, true).Value);
		}

		[Fact]
		public static void GetMethodOneAsInvocation()
		{
			MethodBase method = typeof(SimpleReferenceType).GetMethod("MethodOne");
			Assert.Equal(
				"instance string EmitDebugging.Tests.Types.SimpleReferenceType::MethodOne(int32, valuetype EmitDebugging.Tests.Types.SimpleValueType)",
				MethodDescriptorFactory.Create(method).Value);
		}

		[Fact]
		public static void GetMethodSevenAsDeclaration()
		{
			MethodBase method = typeof(SimpleReferenceType).GetMethod("MethodSeven");
			Assert.Equal(
				".method public hidebysig static void MethodSeven<T>(!!T V_0) cil managed",
				MethodDescriptorFactory.Create(method, true).Value);
		}

		[Fact]
		public static void GetMethodSevenAsInvocation()
		{
			MethodBase method = typeof(SimpleReferenceType).GetMethod(
				"MethodSeven").MakeGenericMethod(new Type[] { typeof(string) });
			Assert.Equal(
				"void EmitDebugging.Tests.Types.SimpleReferenceType::MethodSeven<string>(!!0)",
				MethodDescriptorFactory.Create(method).Value);
		}

		[Fact]
		public static void GetMethodSixAsDeclaration()
		{
			MethodBase method = typeof(SimpleReferenceType).GetMethod("MethodSix");
			Assert.Equal(
				".method public hidebysig instance void MethodSix<T, U>(!!T& V_0, !!U V_1) cil managed",
				MethodDescriptorFactory.Create(method, true).Value);
		}

		[Fact]
		public static void GetMethodSixAsInvocation()
		{
			MethodInfo method = typeof(SimpleReferenceType).GetMethod(
				"MethodSix").MakeGenericMethod(new Type[] { typeof(string), typeof(string) });
			Assert.Equal(
				"instance void EmitDebugging.Tests.Types.SimpleReferenceType::MethodSix<string, string>(!!0&, !!1)",
				MethodDescriptorFactory.Create(method).Value);
		}

		[Fact]
		public static void GetMethodThreeAsDeclaration()
		{
			MethodBase method = typeof(SimpleReferenceType).GetMethod("MethodThree");
			Assert.Equal(
				".method public hidebysig static void MethodThree(int64 V_0, valuetype EmitDebugging.Tests.Types.SimpleValueType[] V_1) cil managed",
				MethodDescriptorFactory.Create(method, true).Value);
		}

		[Fact]
		public static void GetMethodThreeAsInvocation()
		{
			MethodBase method = typeof(SimpleReferenceType).GetMethod("MethodThree");
			Assert.Equal(
				"void EmitDebugging.Tests.Types.SimpleReferenceType::MethodThree(int64, valuetype EmitDebugging.Tests.Types.SimpleValueType[])",
				MethodDescriptorFactory.Create(method).Value);
		}

		[Fact]
		public static void GetMethodTwoAsDeclaration()
		{
			MethodBase method = typeof(SimpleReferenceType).GetMethod("MethodTwo");
			Assert.Equal(
				".method public hidebysig instance void MethodTwo(string& V_0, valuetype [mscorlib]System.Guid& V_1, valuetype EmitDebugging.Tests.Types.SimpleValueType& V_2) cil managed",
				MethodDescriptorFactory.Create(method, true).Value);
		}

		[Fact]
		public static void GetMethodTwoAsInvocation()
		{
			MethodBase method = typeof(SimpleReferenceType).GetMethod("MethodTwo");
			Assert.Equal(
				"instance void EmitDebugging.Tests.Types.SimpleReferenceType::MethodTwo(string&, valuetype [mscorlib]System.Guid&, valuetype EmitDebugging.Tests.Types.SimpleValueType&)",
				MethodDescriptorFactory.Create(method).Value);
		}

		[Fact]
		public static void GetVarArgAsDeclaration()
		{
			MethodBase method = typeof(VarArgMethod).GetMethod("VarArg");
			Assert.Equal(
				".method public hidebysig static vararg void VarArg(int32 V_0) cil managed",
				MethodDescriptorFactory.Create(method, true).Value);
		}

		[Fact]
		public static void GetVarArgAsInvocation()
		{
			MethodBase method = typeof(VarArgMethod).GetMethod("VarArg");
			Assert.Equal(
				"vararg void EmitDebugging.Tests.Types.VarArgMethod::VarArg(int32, ...)",
				MethodDescriptorFactory.Create(method).Value);
		}
	}
}

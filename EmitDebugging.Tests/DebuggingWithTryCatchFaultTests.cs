using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using System.Xml.XPath;
using Xunit;

namespace EmitDebugging.Tests
{
	public sealed class DebuggingWithTryCatchFaultTests 
		: DebuggingTests
	{
		[SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
		protected override void AssertSequencePoints(XPathNavigator navigator)
		{
			var entries = navigator.Select(
				"./symbols/methods/method[@name=\"MethodWithTryCatchFault.SimpleClass.Divide\"]/sequencepoints/entry");
			Assert.Equal(12, entries.Count);

			int visitedCount = 0;

			foreach (XPathNavigator entry in entries)
			{
				if (entry.GetAttribute("il_offset", string.Empty) == "0x0")
				{
					Assert.Equal("20", entry.GetAttribute("start_row", string.Empty));
					Assert.Equal("20", entry.GetAttribute("end_row", string.Empty));
					Assert.Equal("6", entry.GetAttribute("start_column", string.Empty));
					Assert.Equal("23", entry.GetAttribute("end_column", string.Empty));
					visitedCount++;
				}
				else if (entry.GetAttribute("il_offset", string.Empty) == "0x2c")
				{
					Assert.Equal("42", entry.GetAttribute("start_row", string.Empty));
					Assert.Equal("42", entry.GetAttribute("end_row", string.Empty));
					Assert.Equal("4", entry.GetAttribute("start_column", string.Empty));
					Assert.Equal("17", entry.GetAttribute("end_column", string.Empty));
					visitedCount++;
				}
			}

			Assert.Equal(2, visitedCount);
		}

		protected override AssemblyDebugging CreateAssembly()
		{
			using (var assembly = DebuggingTests.CreateDebuggingAssembly("MethodWithTryCatchFault"))
			{
				using (var type = DebuggingTests.CreateDebuggingType(
					assembly, assembly.Builder.GetDynamicModule(assembly.Builder.GetName().Name),
					"SimpleClass"))
				{
					using (var ctor = type.GetMethodDebugging(
						type.Builder.DefineConstructor(
							MethodAttributes.Public | MethodAttributes.SpecialName |
							MethodAttributes.RTSpecialName | MethodAttributes.HideBySig,
							CallingConventions.Standard, Type.EmptyTypes)))
					{
						ctor.Emit(OpCodes.Ldarg_0);
						var objectCtor = typeof(object).GetConstructor(Type.EmptyTypes);
						ctor.Emit(OpCodes.Call, objectCtor);
						ctor.Emit(OpCodes.Ret);
					}

					using (var method = type.GetMethodDebugging(
						type.Builder.DefineMethod("Divide",
						MethodAttributes.HideBySig | MethodAttributes.Public,
						typeof(int), new Type[] { typeof(int), typeof(int) })))
					{
						method.DeclareLocal(typeof(int));
						method.BeginExceptionBlock();
						method.BeginExceptionBlock();
						method.Emit(OpCodes.Ldarg_1);
						method.Emit(OpCodes.Ldarg_2);
						method.Emit(OpCodes.Div);
						method.Emit(OpCodes.Stloc_0);
						method.BeginCatchBlock(typeof(DivideByZeroException));
						method.Emit(OpCodes.Ldc_I4_S, 22);
						method.Emit(OpCodes.Stloc_0);
						method.BeginCatchBlock(typeof(ArgumentException));
						method.Emit(OpCodes.Ldc_I4_S, 20);
						method.Emit(OpCodes.Stloc_0);
						method.EndExceptionBlock();
						method.BeginFaultBlock();
						method.Emit(OpCodes.Ldc_I4_S, 21);
						method.Emit(OpCodes.Stloc_0);
						method.EndExceptionBlock();
						method.Emit(OpCodes.Ldloc_0);
						method.Emit(OpCodes.Ret);
					}

					type.Builder.CreateType();
				}

				return assembly;
			}
		}

		[Fact]
		public void CreateAssemblyAndAssert()
		{
			this.RunTest();
		}

		protected override string[] GetExpectedFileLines()
		{
			return new string[] 
			{
				".assembly MethodWithTryCatchFault",
				"{",
				"	.class public sealed ansi MethodWithTryCatchFault.SimpleClass extends object",
				"	{",
				"		.method public hidebysig specialname instance void .ctor() cil managed",
				"		{",
				"			.locals init ()",
				"			IL_0000:  ldarg.0",
				"			IL_0001:  call instance void object::.ctor()",
				"			IL_0006:  ret",
				"		}",
				"		",
				"		.method public hidebysig instance int32 Divide() cil managed",
				"		{",
				"			.locals init ([0] int32 V_0)",
				"			.try",
				"			{",
				"				.try",
				"				{",
				"					IL_0000:  ldarg.1",
				"					IL_0001:  ldarg.2",
				"					IL_0002:  div",
				"					IL_0003:  stloc.0",
				"				}",
				"				catch class [mscorlib]System.DivideByZeroException",
				"				{",
				"					IL_0004:  ldc.i4.s 22",
				"					IL_0009:  stloc.0",
				"				}",
				"				catch class [mscorlib]System.ArgumentException",
				"				{",
				"					IL_000A:  ldc.i4.s 20",
				"					IL_000F:  stloc.0",
				"				}",
				"			}",
				"			fault",
				"			{",
				"				IL_0010:  ldc.i4.s 21",
				"				IL_0015:  stloc.0",
				"			}",
				"			IL_0016:  ldloc.0",
				"			IL_0017:  ret",
				"		}",
				"	}",
				"}",
			};
		}
	}
}

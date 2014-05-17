using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using System.Xml.XPath;
using Xunit;

namespace EmitDebugging.Tests
{
	public sealed class DebuggingWithMethodThatStoresIntegersTests
		: DebuggingTests
	{
		[SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
		protected override void AssertSequencePoints(XPathNavigator navigator)
		{
			var entries = navigator.Select(
				"./symbols/methods/method[@name=\"MethodWithStoringIntegers.SimpleClass.StoreIntegers\"]/sequencepoints/entry");
			Assert.Equal(9, entries.Count);

			int visitedCount = 0;

			foreach (XPathNavigator entry in entries)
			{
				if (entry.GetAttribute("il_offset", string.Empty) == "0x0")
				{
					Assert.Equal("16", entry.GetAttribute("start_row", string.Empty));
					Assert.Equal("16", entry.GetAttribute("end_row", string.Empty));
					Assert.Equal("4", entry.GetAttribute("start_column", string.Empty));
					Assert.Equal("24", entry.GetAttribute("end_column", string.Empty));
					visitedCount++;
				}
				else if (entry.GetAttribute("il_offset", string.Empty) == "0x17")
				{
					Assert.Equal("24", entry.GetAttribute("start_row", string.Empty));
					Assert.Equal("24", entry.GetAttribute("end_row", string.Empty));
					Assert.Equal("4", entry.GetAttribute("start_column", string.Empty));
					Assert.Equal("17", entry.GetAttribute("end_column", string.Empty));
					visitedCount++;
				}
			}

			Assert.Equal(2, visitedCount);
		}

		protected override AssemblyDebugging CreateAssembly()
		{
			using (var assembly = DebuggingTests.CreateDebuggingAssembly("MethodWithStoringIntegers"))
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
						type.Builder.DefineMethod("StoreIntegers",
						MethodAttributes.HideBySig | MethodAttributes.Public)))
					{
						method.DeclareLocal(typeof(short));
						method.DeclareLocal(typeof(int));
						method.DeclareLocal(typeof(long));
						method.DeclareLocal(typeof(byte));
						method.Emit(OpCodes.Ldc_I4_S, (short)3);
						method.Emit(OpCodes.Stloc_0);
						method.Emit(OpCodes.Ldc_I4, 33);
						method.Emit(OpCodes.Stloc_1);
						method.Emit(OpCodes.Ldc_I8, 33L);
						method.Emit(OpCodes.Stloc_2);
						method.Emit(OpCodes.Ldc_I4_S, (byte)3);
						method.Emit(OpCodes.Stloc_3);
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
				".assembly MethodWithStoringIntegers",
				"{",
				"	.class public sealed ansi MethodWithStoringIntegers.SimpleClass extends object",
				"	{",
				"		.method public hidebysig specialname instance void .ctor() cil managed",
				"		{",
				"			.locals init ()",
				"			IL_0000:  ldarg.0",
				"			IL_0001:  call instance void object::.ctor()",
				"			IL_0006:  ret",
				"		}",
				"		",
				"		.method public hidebysig instance void StoreIntegers() cil managed",
				"		{",
				"			.locals init ([0] int16 V_0, [1] int32 V_1, [2] int64 V_2, [3] uint8 V_3)",
				"			IL_0000:  ldc.i4.s 3",
				"			IL_0003:  stloc.0",
				"			IL_0004:  ldc.i4 33",
				"			IL_0009:  stloc.1",
				"			IL_000A:  ldc.i8 33",
				"			IL_0013:  stloc.2",
				"			IL_0014:  ldc.i4.s 3",
				"			IL_0016:  stloc.3",
				"			IL_0017:  ret",
				"		}",
				"	}",
				"}"
			};
		}
	}
}

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using System.Xml.XPath;
using Xunit;

namespace EmitDebugging.Tests
{
	public sealed class DebuggingWithBreakingToLabelTests
		: DebuggingTests
	{
		[SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
		protected override void AssertSequencePoints(XPathNavigator navigator)
		{
			var entries = navigator.Select(
				"./symbols/methods/method[@name=\"MethodWithBreakingToLabel.SimpleClass.BreakToLabel\"]/sequencepoints/entry");
			Assert.Equal(6, entries.Count);

			int visitedCount = 0;

			foreach (XPathNavigator entry in entries)
			{
				if (entry.GetAttribute("il_offset", string.Empty) == "0x0")
				{
					Assert.Equal("16", entry.GetAttribute("start_row", string.Empty));
					Assert.Equal("16", entry.GetAttribute("end_row", string.Empty));
					Assert.Equal("4", entry.GetAttribute("start_column", string.Empty));
					Assert.Equal("21", entry.GetAttribute("end_column", string.Empty));
					visitedCount++;
				}
				else if (entry.GetAttribute("il_offset", string.Empty) == "0x7")
				{
					Assert.Equal("21", entry.GetAttribute("start_row", string.Empty));
					Assert.Equal("21", entry.GetAttribute("end_row", string.Empty));
					Assert.Equal("4", entry.GetAttribute("start_column", string.Empty));
					Assert.Equal("17", entry.GetAttribute("end_column", string.Empty));
					visitedCount++;
				}
			}

			Assert.Equal(2, visitedCount);
		}

		protected override AssemblyDebugging CreateAssembly()
		{
			using (var assembly = DebuggingTests.CreateDebuggingAssembly("MethodWithBreakingToLabel"))
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
						type.Builder.DefineMethod("BreakToLabel",
						MethodAttributes.HideBySig | MethodAttributes.Public,
						typeof(int), new Type[] { typeof(bool) })))
					{
						var trueValue = method.DefineLabel();
						var end = method.DefineLabel();
						method.Emit(OpCodes.Ldarg_1);
						method.Emit(OpCodes.Brtrue_S, trueValue);
						method.Emit(OpCodes.Ldc_I4_0);
						method.Emit(OpCodes.Br_S, end);
						method.MarkLabel(trueValue);
						method.Emit(OpCodes.Ldc_I4_1);
						method.MarkLabel(end);
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
				".assembly MethodWithBreakingToLabel",
				"{",
				"	.class public sealed ansi MethodWithBreakingToLabel.SimpleClass extends object",
				"	{",
				"		.method public hidebysig specialname instance void .ctor() cil managed",
				"		{",
				"			.locals init ()",
				"			IL_0000:  ldarg.0",
				"			IL_0001:  call instance void object::.ctor()",
				"			IL_0006:  ret",
				"		}",
				"		",
				"		.method public hidebysig instance int32 BreakToLabel() cil managed",
				"		{",
				"			.locals init ()",
				"			IL_0000:  ldarg.1",
				"			IL_0001:  brtrue.s IL_0006",
				"			IL_0003:  ldc.i4.0",
				"			IL_0004:  br.s IL_0007",
				"			IL_0006:  ldc.i4.1",
				"			IL_0007:  ret",
				"		}",
				"	}",
				"}"
			};
		}
	}
}

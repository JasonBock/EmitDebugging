using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using System.Xml.XPath;
using Xunit;

namespace EmitDebugging.Tests
{
	public sealed class DebuggingWithFilteredExceptionHandlerTests
		: DebuggingTests
	{
		[SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
		protected override void AssertSequencePoints(XPathNavigator navigator)
		{
			var entries = navigator.Select(
				"./symbols/methods/method[@name=\"MethodWithFilteredExceptionHandler.SimpleClass.DivideWithFilter\"]/sequencepoints/entry");
			Assert.Equal(14, entries.Count);

			int visitedCount = 0;

			foreach (XPathNavigator entry in entries)
			{
				if (entry.GetAttribute("il_offset", string.Empty) == "0x0")
				{
					Assert.Equal("18", entry.GetAttribute("start_row", string.Empty));
					Assert.Equal("18", entry.GetAttribute("end_row", string.Empty));
					Assert.Equal("5", entry.GetAttribute("start_column", string.Empty));
					Assert.Equal("22", entry.GetAttribute("end_column", string.Empty));
					visitedCount++;
				}
				else if (entry.GetAttribute("il_offset", string.Empty) == "0x1e")
				{
					Assert.Equal("38", entry.GetAttribute("start_row", string.Empty));
					Assert.Equal("38", entry.GetAttribute("end_row", string.Empty));
					Assert.Equal("4", entry.GetAttribute("start_column", string.Empty));
					Assert.Equal("17", entry.GetAttribute("end_column", string.Empty));
					visitedCount++;
				}
			}

			Assert.Equal(2, visitedCount);
		}

		protected override AssemblyDebugging CreateAssembly()
		{
			using (var assembly = DebuggingTests.CreateDebuggingAssembly("MethodWithFilteredExceptionHandler"))
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
						type.Builder.DefineMethod("DivideWithFilter",
						MethodAttributes.HideBySig | MethodAttributes.Public,
						typeof(int), new Type[] { typeof(int), typeof(int) })))
					{
						method.DeclareLocal(typeof(int));
						method.BeginExceptionBlock();
						method.Emit(OpCodes.Ldarg_1);
						method.Emit(OpCodes.Ldarg_2);
						method.Emit(OpCodes.Div);
						method.Emit(OpCodes.Stloc_0);
						method.BeginExceptFilterBlock();
						method.Emit(OpCodes.Pop);
						method.Emit(OpCodes.Ldarg_1);
						method.Emit(OpCodes.Ldc_I4_2);
						method.Emit(OpCodes.Rem);
						method.Emit(OpCodes.Ldc_I4_0);
						method.Emit(OpCodes.Ceq);
						method.BeginCatchBlock(null);
						method.Emit(OpCodes.Ldc_I4_S, 22);
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
				".assembly MethodWithFilteredExceptionHandler",
				"{",
				"	.class public sealed ansi MethodWithFilteredExceptionHandler.SimpleClass extends object",
				"	{",
				"		.method public hidebysig specialname instance void .ctor() cil managed",
				"		{",
				"			.locals init ()",
				"			IL_0000:  ldarg.0",
				"			IL_0001:  call instance void object::.ctor()",
				"			IL_0006:  ret",
				"		}",
				"		",
				"		.method public hidebysig instance int32 DivideWithFilter() cil managed",
				"		{",
				"			.locals init ([0] int32 V_0)",
				"			.try",
				"			{",
				"				IL_0000:  ldarg.1",
				"				IL_0001:  ldarg.2",
				"				IL_0002:  div",
				"				IL_0003:  stloc.0",
				"			}",
				"			filter",
				"			{",
				"				IL_0004:  pop",
				"				IL_0005:  ldarg.1",
				"				IL_0006:  ldc.i4.2",
				"				IL_0007:  rem",
				"				IL_0008:  ldc.i4.0",
				"				IL_0009:  ceq",
				"			}",
				"			catch",
				"			{",
				"				IL_000B:  ldc.i4.s 22",
				"				IL_0010:  stloc.0",
				"			}",
				"			IL_0011:  ldloc.0",
				"			IL_0012:  ret",
				"		}",
				"	}",
				"}",
			};
		}
	}
}

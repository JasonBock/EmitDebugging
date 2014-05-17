using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using System.Xml.XPath;
using Xunit;

namespace EmitDebugging.Tests
{
	public sealed class DebuggingWithMethodThatCallsVarArgsMethodTests
		: DebuggingTests
	{
		[SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
		protected override void AssertSequencePoints(XPathNavigator navigator)
		{
			var entries = navigator.Select(
				"./symbols/methods/method[@name=\"MethodThatCallsVarArgsMethod.SimpleClass.CallVarArgsMethod\"]/sequencepoints/entry");
			Assert.Equal(6, entries.Count);

			int visitedCount = 0;

			foreach (XPathNavigator entry in entries)
			{
				if (entry.GetAttribute("il_offset", string.Empty) == "0x0")
				{
					Assert.Equal("22", entry.GetAttribute("start_row", string.Empty));
					Assert.Equal("22", entry.GetAttribute("end_row", string.Empty));
					Assert.Equal("4", entry.GetAttribute("start_column", string.Empty));
					Assert.Equal("21", entry.GetAttribute("end_column", string.Empty));
					visitedCount++;
				}
				else if (entry.GetAttribute("il_offset", string.Empty) == "0x11")
				{
					Assert.Equal("27", entry.GetAttribute("start_row", string.Empty));
					Assert.Equal("27", entry.GetAttribute("end_row", string.Empty));
					Assert.Equal("4", entry.GetAttribute("start_column", string.Empty));
					Assert.Equal("17", entry.GetAttribute("end_column", string.Empty));
					visitedCount++;
				}
			}

			Assert.Equal(2, visitedCount);
		}

		protected override AssemblyDebugging CreateAssembly()
		{
			using (var assembly = DebuggingTests.CreateDebuggingAssembly("MethodThatCallsVarArgsMethod"))
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

					MethodInfo varMethod = null;

					using (var varArgsMethod = type.GetMethodDebugging(
						type.Builder.DefineMethod("VarArgsMethod",
						MethodAttributes.HideBySig | MethodAttributes.Public,
						CallingConventions.VarArgs,
						null, new Type[] { typeof(string) })))
					{
						varArgsMethod.Emit(OpCodes.Ret);
						varMethod = varArgsMethod.Builder as MethodInfo;
					}

					using (var method = type.GetMethodDebugging(
						type.Builder.DefineMethod("CallVarArgsMethod",
						MethodAttributes.HideBySig | MethodAttributes.Public)))
					{
						method.Emit(OpCodes.Ldarg_0);
						method.Emit(OpCodes.Ldstr, "Param");
						method.Emit(OpCodes.Ldstr, "VarArgParam");
						method.Emit(OpCodes.Ldc_I4_1);
						method.EmitCall(OpCodes.Call, varMethod,
							new Type[] { typeof(string), typeof(int) });
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
				".assembly MethodThatCallsVarArgsMethod",
				"{",
				"	.class public sealed ansi MethodThatCallsVarArgsMethod.SimpleClass extends object",
				"	{",
				"		.method public hidebysig specialname instance void .ctor() cil managed",
				"		{",
				"			.locals init ()",
				"			IL_0000:  ldarg.0",
				"			IL_0001:  call instance void object::.ctor()",
				"			IL_0006:  ret",
				"		}",
				"		",
				"		.method public hidebysig instance vararg void VarArgsMethod() cil managed",
				"		{",
				"			.locals init ()",
				"			IL_0000:  ret",
				"		}",
				"		",
				"		.method public hidebysig instance void CallVarArgsMethod() cil managed",
				"		{",
				"			.locals init ()",
				"			IL_0000:  ldarg.0",
				"			IL_0001:  ldstr \"Param\"",
				"			IL_0007:  ldstr \"VarArgParam\"",
				"			IL_0013:  ldc.i4.1",
				"			IL_0014:  call instance vararg void MethodThatCallsVarArgsMethod.SimpleClass::VarArgsMethod()",
				"			IL_0019:  ret",
				"		}",
				"	}",
				"}"
			};
		}
	}
}

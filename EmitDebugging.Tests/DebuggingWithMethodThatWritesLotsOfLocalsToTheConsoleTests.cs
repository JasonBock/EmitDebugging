using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using System.Xml.XPath;
using Xunit;

namespace EmitDebugging.Tests
{
	public sealed class DebuggingWithMethodThatWritesLotsOfLocalsToTheConsoleTests
		: DebuggingTests
	{
		[SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
		protected override void AssertSequencePoints(XPathNavigator navigator)
		{
			var entries = navigator.Select(
				"./symbols/methods/method[@name=\"MethodThatWritesLotsOfLocalsToTheConsole.SimpleClass.WriteToConsole\"]/sequencepoints/entry");
			Assert.Equal(6, entries.Count);

			int visitedCount = 0;

			foreach (XPathNavigator entry in entries)
			{
				if (entry.GetAttribute("il_offset", string.Empty) == "0x0")
				{
					Assert.Equal("16", entry.GetAttribute("start_row", string.Empty));
					Assert.Equal("18", entry.GetAttribute("end_row", string.Empty));
					Assert.Equal("4", entry.GetAttribute("start_column", string.Empty));
					Assert.Equal("76", entry.GetAttribute("end_column", string.Empty));
					visitedCount++;
				}
				else if (entry.GetAttribute("il_offset", string.Empty) == "0x38")
				{
					Assert.Equal("31", entry.GetAttribute("start_row", string.Empty));
					Assert.Equal("31", entry.GetAttribute("end_row", string.Empty));
					Assert.Equal("4", entry.GetAttribute("start_column", string.Empty));
					Assert.Equal("17", entry.GetAttribute("end_column", string.Empty));
					visitedCount++;
				}
			}

			Assert.Equal(2, visitedCount);
		}

		protected override AssemblyDebugging CreateAssembly()
		{
			using (var assembly = DebuggingTests.CreateDebuggingAssembly("MethodThatWritesLotsOfLocalsToTheConsole"))
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
						var objectCtor = typeof(object).GetConstructor(Type.EmptyTypes);
						ctor.Emit(OpCodes.Ldarg_0);
						ctor.Emit(OpCodes.Call, objectCtor);
						ctor.Emit(OpCodes.Ret);
					}

					using (var method = type.GetMethodDebugging(
						type.Builder.DefineMethod("WriteToConsole",
						MethodAttributes.HideBySig | MethodAttributes.Public)))
					{
						for (var i = 0; i < 5; i++)
						{
							method.EmitWriteLine(method.DeclareLocal(typeof(int)));
						}
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
				".assembly MethodThatWritesLotsOfLocalsToTheConsole",
				"{",
				"	.class public sealed ansi MethodThatWritesLotsOfLocalsToTheConsole.SimpleClass extends object",
				"	{",
				"		.method public hidebysig specialname instance void .ctor() cil managed",
				"		{",
				"			.locals init ()",
				"			IL_0000:  ldarg.0",
				"			IL_0001:  call instance void object::.ctor()",
				"			IL_0006:  ret",
				"		}",
				"		",
				"		.method public hidebysig instance void WriteToConsole() cil managed",
				"		{",
				"			.locals init ([0] int32 V_0, [1] int32 V_1, [2] int32 V_2, [3] int32 V_3, [4] int32 V_4)",
				"			IL_0000:  call class System.IO.TextWriter System.Console::get_Out()",
				"			IL_0005:  ldloc.0",
				"			IL_0006:  callvirt instance void System.IO.TextWriter::WriteLine(int32)",
				"			IL_000B:  call class System.IO.TextWriter System.Console::get_Out()",
				"			IL_0010:  ldloc.1",
				"			IL_0011:  callvirt instance void System.IO.TextWriter::WriteLine(int32)",
				"			IL_0016:  call class System.IO.TextWriter System.Console::get_Out()",
				"			IL_001B:  ldloc.2",
				"			IL_001C:  callvirt instance void System.IO.TextWriter::WriteLine(int32)",
				"			IL_0021:  call class System.IO.TextWriter System.Console::get_Out()",
				"			IL_0026:  ldloc.3",
				"			IL_0027:  callvirt instance void System.IO.TextWriter::WriteLine(int32)",
				"			IL_002C:  call class System.IO.TextWriter System.Console::get_Out()",
				"			IL_0031:  ldloc 4",
				"			IL_0033:  callvirt instance void System.IO.TextWriter::WriteLine(int32)",
				"			IL_0038:  ret",
				"		}",
				"	}",
				"}",
			};
		}
	}
}

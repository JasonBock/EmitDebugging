using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using System.Xml.XPath;
using Xunit;

namespace EmitDebugging.Tests
{
	public sealed class DebuggingWithMethodThatWritesToTheConsole 
		: DebuggingTests
	{
		[SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
		protected override void AssertSequencePoints(XPathNavigator navigator)
		{
			var entries = navigator.Select(
				"./symbols/methods/method[@name=\"MethodThatWritesToTheConsole.SimpleClass.WriteToConsole\"]/sequencepoints/entry");
			Assert.Equal(8, entries.Count);

			int visitedCount = 0;

			foreach (XPathNavigator entry in entries)
			{
				if (entry.GetAttribute("il_offset", string.Empty) == "0x0")
				{
					Assert.Equal("24", entry.GetAttribute("start_row", string.Empty));
					Assert.Equal("26", entry.GetAttribute("end_row", string.Empty));
					Assert.Equal("4", entry.GetAttribute("start_column", string.Empty));
					Assert.Equal("77", entry.GetAttribute("end_column", string.Empty));
					visitedCount++;
				}
				else if (entry.GetAttribute("il_offset", string.Empty) == "0x60")
				{
					Assert.Equal("46", entry.GetAttribute("start_row", string.Empty));
					Assert.Equal("46", entry.GetAttribute("end_row", string.Empty));
					Assert.Equal("4", entry.GetAttribute("start_column", string.Empty));
					Assert.Equal("17", entry.GetAttribute("end_column", string.Empty));
					visitedCount++;
				}
			}

			Assert.Equal(2, visitedCount);
		}

		protected override AssemblyDebugging CreateAssembly()
		{
			using (var assembly = DebuggingTests.CreateDebuggingAssembly("MethodThatWritesToTheConsole"))
			{
				using (var type = DebuggingTests.CreateDebuggingType(
					assembly, assembly.Builder.GetDynamicModule(assembly.Builder.GetName().Name),
					"SimpleClass"))
				{
					var intField = type.Builder.DefineField("guidFieldToWrite",
						typeof(int),
						FieldAttributes.Private | FieldAttributes.Static);
					var stringField = type.Builder.DefineField("stringFieldToWrite",
						typeof(string),
						FieldAttributes.Private);
					var randomField = type.Builder.DefineField("randomFieldToWrite",
						typeof(Random),
						FieldAttributes.Private);

					using (var ctor = type.GetMethodDebugging(
						type.Builder.DefineConstructor(
							MethodAttributes.Public | MethodAttributes.SpecialName |
							MethodAttributes.RTSpecialName | MethodAttributes.HideBySig,
							CallingConventions.Standard, Type.EmptyTypes)))
					{
						var objectCtor = typeof(object).GetConstructor(Type.EmptyTypes);
						ctor.Emit(OpCodes.Ldarg_0);
						ctor.Emit(OpCodes.Call, objectCtor);

						ctor.Emit(OpCodes.Ldarg_0);
						ctor.Emit(OpCodes.Ldstr, "data");
						ctor.Emit(OpCodes.Stfld, stringField);

						ctor.Emit(OpCodes.Ldc_I4_3);
						ctor.Emit(OpCodes.Stsfld, intField);

						ctor.Emit(OpCodes.Ldarg_0);
						var randomCtor = typeof(Random).GetConstructor(Type.EmptyTypes);
						ctor.Emit(OpCodes.Newobj, randomCtor);
						ctor.Emit(OpCodes.Stfld, randomField);
						ctor.Emit(OpCodes.Ret);
					}

					using (var method = type.GetMethodDebugging(
						type.Builder.DefineMethod("WriteToConsole",
						MethodAttributes.HideBySig | MethodAttributes.Public)))
					{
						var stringLocal = method.DeclareLocal(typeof(string));
						var intLocal = method.DeclareLocal(typeof(int));
						var randomLocal = method.DeclareLocal(typeof(Random));
						method.EmitWriteLine(stringLocal);
						method.EmitWriteLine(intLocal);
						method.EmitWriteLine(randomLocal);
						method.EmitWriteLine(stringField);
						method.EmitWriteLine(intField);
						method.EmitWriteLine(randomField);
						method.EmitWriteLine("some data");
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
				".assembly MethodThatWritesToTheConsole",
				"{",
				"	.class public sealed ansi MethodThatWritesToTheConsole.SimpleClass extends object",
				"	{",
				"		.method public hidebysig specialname instance void .ctor() cil managed",
				"		{",
				"			.locals init ()",
				"			IL_0000:  ldarg.0",
				"			IL_0001:  call instance void object::.ctor()",
				"			IL_0006:  ldarg.0",
				"			IL_0007:  ldstr \"data\"",
				"			IL_000C:  stfld string MethodThatWritesToTheConsole.SimpleClass::stringFieldToWrite",
				"			IL_0011:  ldc.i4.3",
				"			IL_0012:  stsfld int32 MethodThatWritesToTheConsole.SimpleClass::guidFieldToWrite",
				"			IL_0017:  ldarg.0",
				"			IL_0018:  newobj instance void System.Random::.ctor()",
				"			IL_001D:  stfld System.Random MethodThatWritesToTheConsole.SimpleClass::randomFieldToWrite",
				"			IL_0022:  ret",
				"		}",
				"		",
				"		.method public hidebysig instance void WriteToConsole() cil managed",
				"		{",
				"			.locals init ([0] string V_0, [1] int32 V_1, [2] class [mscorlib]System.Random V_2)",
				"			IL_0000:  call class System.IO.TextWriter System.Console::get_Out()",
				"			IL_0005:  ldloc.0",
				"			IL_0006:  callvirt instance void System.IO.TextWriter::WriteLine(string)",
				"			IL_000B:  call class System.IO.TextWriter System.Console::get_Out()",
				"			IL_0010:  ldloc.1",
				"			IL_0011:  callvirt instance void System.IO.TextWriter::WriteLine(int32)",
				"			IL_0016:  call class System.IO.TextWriter System.Console::get_Out()",
				"			IL_001B:  ldloc.2",
				"			IL_001C:  callvirt instance void System.IO.TextWriter::WriteLine(object)",
				"			IL_0021:  call class System.IO.TextWriter System.Console::get_Out()",
				"			IL_0026:  ldarg.0",
				"			IL_0027:  ldfld string MethodThatWritesToTheConsole.SimpleClass::stringFieldToWrite",
				"			IL_002C:  callvirt instance void System.IO.TextWriter::WriteLine(string)",
				"			IL_0031:  call class System.IO.TextWriter System.Console::get_Out()",
				"			IL_0036:  ldsfld int32 MethodThatWritesToTheConsole.SimpleClass::guidFieldToWrite",
				"			IL_003B:  callvirt instance void System.IO.TextWriter::WriteLine(int32)",
				"			IL_0040:  call class System.IO.TextWriter System.Console::get_Out()",
				"			IL_0045:  ldarg.0",
				"			IL_0046:  ldfld System.Random MethodThatWritesToTheConsole.SimpleClass::randomFieldToWrite",
				"			IL_004B:  callvirt instance void System.IO.TextWriter::WriteLine(object)",
				"			IL_0050:  ldstr \"some data\"",
				"			IL_005A:  callvirt instance void System.IO.TextWriter::WriteLine(string)",
				"			IL_005F:  ret",
				"		}",
				"	}",
				"}",
			};
		}
	}
}

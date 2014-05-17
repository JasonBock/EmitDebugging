using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using System.Xml.XPath;
using Xunit;

namespace EmitDebugging.Tests
{
	public sealed class DebuggingWithMethodThatCreatesSwitchTests
		: DebuggingTests
	{
		[SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
		protected override void AssertSequencePoints(XPathNavigator navigator)
		{
			var entries = navigator.Select(
				"./symbols/methods/method[@name=\"MethodThatCreatesSwitch.SimpleClass.GenerateSwitch\"]/sequencepoints/entry");
			Assert.Equal(8, entries.Count);

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
				else if (entry.GetAttribute("il_offset", string.Empty) == "0x23")
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
			using (var assembly = DebuggingTests.CreateDebuggingAssembly("MethodThatCreatesSwitch"))
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
						type.Builder.DefineMethod("GenerateSwitch",
						MethodAttributes.HideBySig | MethodAttributes.Public,
						typeof(string), new Type[] { typeof(int) })))
					{
						var defaultCase = method.DefineLabel();
						var endOfMethod = method.DefineLabel();

						method.Emit(OpCodes.Ldarg_1);
						var jumpTable = new Label[] { method.DefineLabel(), method.DefineLabel() };
						method.Emit(OpCodes.Switch, jumpTable);

						method.Emit(OpCodes.Br_S, defaultCase);

						method.MarkLabel(jumpTable[0]);
						method.Emit(OpCodes.Ldstr, "It's zero.");
						method.Emit(OpCodes.Br_S, endOfMethod);

						method.MarkLabel(jumpTable[1]);
						method.Emit(OpCodes.Ldstr, "It's one.");
						method.Emit(OpCodes.Br_S, endOfMethod);

						method.MarkLabel(defaultCase);
						method.Emit(OpCodes.Ldstr, "It's something else.");

						method.MarkLabel(endOfMethod);
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
			this.RunTest(false);
		}

		protected override string[] GetExpectedFileLines()
		{
			return new string[] 
			{
				".assembly MethodThatCreatesSwitch",
				"{",
				"	.class public sealed ansi MethodThatCreatesSwitch.SimpleClass extends object",
				"	{",
				"		.method public hidebysig specialname instance void .ctor() cil managed",
				"		{",
				"			.locals init ()",
				"			IL_0000:  ldarg.0",
				"			IL_0001:  call instance void object::.ctor()",
				"			IL_0006:  ret",
				"		}",
				"		",
				"		.method public hidebysig instance string GenerateSwitch() cil managed",
				"		{",
				"			.locals init ()",
				"			IL_0000:  ldarg.1",
				"			IL_0001:  switch (IL_0005, IL_0012)",
				"			IL_0003:  br.s IL_001E",
				"			IL_0005:  ldstr \"It's zero.\"",
				"			IL_0010:  br.s IL_0033",
				"			IL_0012:  ldstr \"It's one.\"",
				"			IL_001C:  br.s IL_0033",
				"			IL_001E:  ldstr \"It's something else.\"",
				"			IL_0033:  ret",
				"		}",
				"	}",
				"}"
			};
		}
	}
}

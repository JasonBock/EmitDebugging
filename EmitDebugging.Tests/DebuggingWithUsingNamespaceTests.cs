using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using System.Xml.XPath;
using Xunit;

namespace EmitDebugging.Tests
{
	public sealed class DebuggingWithUsingNamespaceTests
		: DebuggingTests
	{
		[SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
		protected override void AssertSequencePoints(XPathNavigator navigator)
		{
			var entries = navigator.Select(
				"./symbols/methods/method[@name=\"MethodUsingNamespace.SimpleClass.UsingNamespace\"]/sequencepoints/entry");
			Assert.Equal(6, entries.Count);

			int visitedCount = 0;

			foreach (XPathNavigator entry in entries)
			{
				if (entry.GetAttribute("il_offset", string.Empty) == "0x0")
				{
					Assert.Equal("16", entry.GetAttribute("start_row", string.Empty));
					Assert.Equal("16", entry.GetAttribute("end_row", string.Empty));
					Assert.Equal("4", entry.GetAttribute("start_column", string.Empty));
					Assert.Equal("22", entry.GetAttribute("end_column", string.Empty));
					visitedCount++;
				}
				else if (entry.GetAttribute("il_offset", string.Empty) == "0x5")
				{
					Assert.Equal("23", entry.GetAttribute("start_row", string.Empty));
					Assert.Equal("23", entry.GetAttribute("end_row", string.Empty));
					Assert.Equal("4", entry.GetAttribute("start_column", string.Empty));
					Assert.Equal("17", entry.GetAttribute("end_column", string.Empty));
					visitedCount++;
				}
			}

			Assert.Equal(2, visitedCount);
		}

		protected override AssemblyDebugging CreateAssembly()
		{
			using (var assembly = DebuggingTests.CreateDebuggingAssembly("MethodUsingNamespace"))
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
						type.Builder.DefineMethod("UsingNamespace",
						MethodAttributes.HideBySig | MethodAttributes.Public)))
					{
						var outerLocal = method.DeclareLocal(typeof(int));
						method.Emit(OpCodes.Ldc_I4_3);
						method.Emit(OpCodes.Stloc, outerLocal);
						method.BeginScope();
						method.UsingNamespace("quux");
						var innerLocal = method.DeclareLocal(typeof(long));
						method.Emit(OpCodes.Ldloc, outerLocal);
						method.Emit(OpCodes.Conv_I8);
						method.Emit(OpCodes.Stloc, innerLocal);
						method.EndScope();
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
				".assembly MethodUsingNamespace",
				"{",
				"	.class public sealed ansi MethodUsingNamespace.SimpleClass extends object",
				"	{",
				"		.method public hidebysig specialname instance void .ctor() cil managed",
				"		{",
				"			.locals init ()",
				"			IL_0000:  ldarg.0",
				"			IL_0001:  call instance void object::.ctor()",
				"			IL_0006:  ret",
				"		}",
				"		",
				"		.method public hidebysig instance void UsingNamespace() cil managed",
				"		{",
				"			.locals init ([0] int32 V_0, [1] int64 V_1)",
				"			IL_0000:  ldc.i4.3",
				"			IL_0001:  stloc V_0",
				"			{",
				"				IL_0004:  ldloc V_0",
				"				IL_0007:  conv.i8",
				"				IL_0008:  stloc V_1",
				"			}",
				"			IL_000B:  ret",
				"		}",
				"	}",
				"}"
			};
		}
	}
}

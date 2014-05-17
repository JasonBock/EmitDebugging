using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using System.Xml.XPath;
using Xunit;

namespace EmitDebugging.Tests
{
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Calli")]
	public sealed class DebuggingWithMethodThatUsesManagedCalliTests
		: DebuggingTests
	{
		[SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
		protected override void AssertSequencePoints(XPathNavigator navigator)
		{
			var entries = navigator.Select(
				"./symbols/methods/method[@name=\"MethodThatUsesManagedCalli.SimpleClass.Calli\"]/sequencepoints/entry");
			Assert.Equal(3, entries.Count);

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
				else if (entry.GetAttribute("il_offset", string.Empty) == "0x6")
				{
					Assert.Equal("18", entry.GetAttribute("start_row", string.Empty));
					Assert.Equal("18", entry.GetAttribute("end_row", string.Empty));
					Assert.Equal("4", entry.GetAttribute("start_column", string.Empty));
					Assert.Equal("17", entry.GetAttribute("end_column", string.Empty));
					visitedCount++;
				}
			}

			Assert.Equal(2, visitedCount);
		}

		protected override AssemblyDebugging CreateAssembly()
		{
			using (var assembly = DebuggingTests.CreateDebuggingAssembly("MethodThatUsesManagedCalli"))
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
						type.Builder.DefineMethod("Calli",
						MethodAttributes.HideBySig | MethodAttributes.Public, null,
						new Type[] { typeof(IntPtr) })))
					{
						method.Emit(OpCodes.Ldarg_1);
						method.EmitCalli(OpCodes.Calli, CallingConventions.Standard | CallingConventions.HasThis,
							typeof(int), new Type[] { typeof(long), typeof(string) }, null);
						method.Emit(OpCodes.Ret);
					}

					using (var method = type.GetMethodDebugging(
						type.Builder.DefineMethod("CalliVarArg",
						MethodAttributes.HideBySig | MethodAttributes.Public, null,
						new Type[] { typeof(IntPtr) })))
					{
						method.Emit(OpCodes.Ldarg_1);
						method.EmitCalli(OpCodes.Calli, CallingConventions.VarArgs,
							typeof(int), new Type[] { typeof(long), typeof(string) },
							new Type[] { typeof(long), typeof(string) });
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
			// Note: This assembly cannot be verified:
			// http://blogs.msdn.com/shawnfa/archive/2004/06/14/155478.aspx
			this.RunTest(false);
		}

		protected override string[] GetExpectedFileLines()
		{
			return new string[] 
			{
				".assembly MethodThatUsesManagedCalli",
				"{",
				"	.class public sealed ansi MethodThatUsesManagedCalli.SimpleClass extends object",
				"	{",
				"		.method public hidebysig specialname instance void .ctor() cil managed",
				"		{",
				"			.locals init ()",
				"			IL_0000:  ldarg.0",
				"			IL_0001:  call instance void object::.ctor()",
				"			IL_0006:  ret",
				"		}",
				"		",
				"		.method public hidebysig instance void Calli() cil managed",
				"		{",
				"			.locals init ()",
				"			IL_0000:  ldarg.1",
				"			IL_0001:  calli standard hasthis int32(int64, string)",
				"			IL_0006:  ret",
				"		}",
				"		",
				"		.method public hidebysig instance void CalliVarArg() cil managed",
				"		{",
				"			.locals init ()",
				"			IL_0000:  ldarg.1",
				"			IL_0001:  calli varargs int32(int64, string, ..., int64, string)",
				"			IL_0006:  ret",
				"		}",
				"	}",
				"}"
			};
		}
	}
}

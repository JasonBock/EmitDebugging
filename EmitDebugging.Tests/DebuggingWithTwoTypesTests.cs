using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Xml.XPath;
using Xunit;

namespace EmitDebugging.Tests
{
	public sealed class DebuggingWithTwoTypesTests 
		: DebuggingTests
	{
		protected override void AssertSequencePoints(XPathNavigator navigator)
		{
			DebuggingWithTwoTypesTests.AssertSequencePointsInFirstType(navigator);
			DebuggingWithTwoTypesTests.AssertSequencePointsInSecondType(navigator);
		}

		private static void AssertSequencePointsInFirstType(XPathNavigator navigator)
		{
			var entries = navigator.Select(
				"./symbols/methods/method[@name=\"AssemblyWithTwoTypes.FirstClass.ReflectArgument\"]/sequencepoints/entry");
			Assert.Equal(2, entries.Count);

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
				else if (entry.GetAttribute("il_offset", string.Empty) == "0x1")
				{
					Assert.Equal("17", entry.GetAttribute("start_row", string.Empty));
					Assert.Equal("17", entry.GetAttribute("end_row", string.Empty));
					Assert.Equal("4", entry.GetAttribute("start_column", string.Empty));
					Assert.Equal("17", entry.GetAttribute("end_column", string.Empty));
					visitedCount++;
				}
			}

			Assert.Equal(2, visitedCount);
		}

		private static void AssertSequencePointsInSecondType(XPathNavigator navigator)
		{
			var entries = navigator.Select(
				"./symbols/methods/method[@name=\"AssemblyWithTwoTypes.SecondClass.ReflectArgument\"]/sequencepoints/entry");
			Assert.Equal(2, entries.Count);

			int visitedCount = 0;

			foreach (XPathNavigator entry in entries)
			{
				if (entry.GetAttribute("il_offset", string.Empty) == "0x0")
				{
					Assert.Equal("34", entry.GetAttribute("start_row", string.Empty));
					Assert.Equal("34", entry.GetAttribute("end_row", string.Empty));
					Assert.Equal("4", entry.GetAttribute("start_column", string.Empty));
					Assert.Equal("21", entry.GetAttribute("end_column", string.Empty));
					visitedCount++;
				}
				else if (entry.GetAttribute("il_offset", string.Empty) == "0x1")
				{
					Assert.Equal("35", entry.GetAttribute("start_row", string.Empty));
					Assert.Equal("35", entry.GetAttribute("end_row", string.Empty));
					Assert.Equal("4", entry.GetAttribute("start_column", string.Empty));
					Assert.Equal("17", entry.GetAttribute("end_column", string.Empty));
					visitedCount++;
				}
			}

			Assert.Equal(2, visitedCount);
		}

		private static void BuildFirstType(AssemblyDebugging assembly)
		{
			using (var type = DebuggingTests.CreateDebuggingType(
				assembly, assembly.Builder.GetDynamicModule(assembly.Builder.GetName().Name),
				"FirstClass"))
			{
				DebuggingWithTwoTypesTests.BuildMethodsInType(type);
			}
		}

		private static void BuildMethodsInType(TypeDebugging type)
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
				type.Builder.DefineMethod("ReflectArgument",
				MethodAttributes.HideBySig | MethodAttributes.Static | MethodAttributes.Public,
				typeof(int), new Type[] { typeof(int) })))
			{
				method.Emit(OpCodes.Ldarg_0);
				method.Emit(OpCodes.Ret);
			}

			type.Builder.CreateType();
		}

		private static void BuildSecondType(AssemblyDebugging assembly)
		{
			using (var type = DebuggingTests.CreateDebuggingType(
				assembly, assembly.Builder.GetDynamicModule(assembly.Builder.GetName().Name),
				"SecondClass"))
			{
				DebuggingWithTwoTypesTests.BuildMethodsInType(type);
			}
		}

		protected override AssemblyDebugging CreateAssembly()
		{
			using (var assembly = DebuggingTests.CreateDebuggingAssembly("AssemblyWithTwoTypes"))
			{
				DebuggingWithTwoTypesTests.BuildFirstType(assembly);
				DebuggingWithTwoTypesTests.BuildSecondType(assembly);

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
				".assembly AssemblyWithTwoTypes",
				"{",
				"	.class public sealed ansi AssemblyWithTwoTypes.FirstClass extends object",
				"	{",
				"		.method public hidebysig specialname instance void .ctor() cil managed",
				"		{",
				"			.locals init ()",
				"			IL_0000:  ldarg.0",
				"			IL_0001:  call instance void object::.ctor()",
				"			IL_0006:  ret",
				"		}",
				"		",	
				"		.method public hidebysig static int32 ReflectArgument() cil managed",
				"		{",
				"			.locals init ()",
				"			IL_0000:  ldarg.0",
				"			IL_0001:  ret",
				"		}",
				"	}",
				"	",
				"	.class public sealed ansi AssemblyWithTwoTypes.SecondClass extends object",
				"	{",
				"		.method public hidebysig specialname instance void .ctor() cil managed",
				"		{",
				"			.locals init ()",
				"			IL_0000:  ldarg.0",
				"			IL_0001:  call instance void object::.ctor()",
				"			IL_0006:  ret",
				"		}",
				"		",
				"		.method public hidebysig static int32 ReflectArgument() cil managed",
				"		{",
				"			.locals init ()",
				"			IL_0000:  ldarg.0",
				"			IL_0001:  ret",
				"		}",
				"	}",
				"}"			
			};
		}
	}
}

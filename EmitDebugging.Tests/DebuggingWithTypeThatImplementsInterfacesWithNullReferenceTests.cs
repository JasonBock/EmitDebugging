using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using System.Xml.XPath;
using Xunit;

namespace EmitDebugging.Tests
{
	public sealed class DebuggingWithTypeThatImplementsInterfacesWithNullReferenceTests
		: DebuggingTests
	{
		[SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
		protected override void AssertSequencePoints(XPathNavigator navigator)
		{
			var entries = navigator.Select(
				"./symbols/methods/method[@name=\"AssemblyWithOneTypeThatImplementsInterfacesWithNullReference.SimpleClass.JustReturn\"]/sequencepoints/entry");
			Assert.Equal(1, entries.Count);

			int visitedCount = 0;

			foreach (XPathNavigator entry in entries)
			{
				if (entry.GetAttribute("il_offset", string.Empty) == "0x0")
				{
					Assert.Equal("29", entry.GetAttribute("start_row", string.Empty));
					Assert.Equal("29", entry.GetAttribute("end_row", string.Empty));
					Assert.Equal("4", entry.GetAttribute("start_column", string.Empty));
					Assert.Equal("17", entry.GetAttribute("end_column", string.Empty));
					visitedCount++;
				}
			}

			Assert.Equal(1, visitedCount);
		}

		protected override AssemblyDebugging CreateAssembly()
		{
			using (var assembly = DebuggingTests.CreateDebuggingAssembly("AssemblyWithOneTypeThatImplementsInterfacesWithNullReference"))
			{
				using (var type = DebuggingTests.CreateDebuggingType(assembly,
					assembly.Builder.GetDynamicModule(assembly.Builder.GetName().Name),
					"SimpleClass", new HashSet<Type>() { typeof(IComparable), null, typeof(IDisposable) }))
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

					using (var compareToMethod = type.GetMethodDebugging(
						type.Builder.DefineMethod("CompareTo",
						MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Public,
						typeof(int), new Type[] { typeof(object) })))
					{
						compareToMethod.Emit(OpCodes.Ldc_I4_0);
						compareToMethod.Emit(OpCodes.Ret);
						var iCompareToMethod = typeof(IComparable).GetMethod("CompareTo");
						type.Builder.DefineMethodOverride(compareToMethod.Builder, iCompareToMethod);
					}

					using (var disposeMethod = type.GetMethodDebugging(
						type.Builder.DefineMethod("Dispose",
						MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Public)))
					{
						disposeMethod.Emit(OpCodes.Ret);
						var iDisposeMethod = typeof(IDisposable).GetMethod("Dispose");
						type.Builder.DefineMethodOverride(disposeMethod.Builder, iDisposeMethod);
					}

					using (var method = type.GetMethodDebugging(
						type.Builder.DefineMethod("JustReturn",
						MethodAttributes.HideBySig | MethodAttributes.Public)))
					{
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
				".assembly AssemblyWithOneTypeThatImplementsInterfacesWithNullReference",
				"{",
				"	.class public sealed ansi AssemblyWithOneTypeThatImplementsInterfacesWithNullReference.SimpleClass extends object implements System.IComparable implements System.IDisposable",
				"	{",
				"		.method public hidebysig specialname instance void .ctor() cil managed",
				"		{",
				"			.locals init ()",
				"			IL_0000:  ldarg.0",
				"			IL_0001:  call instance void object::.ctor()",
				"			IL_0006:  ret",
				"		}",
				"		",
				"		.method public virtual hidebysig instance int32 CompareTo() cil managed",
				"		{",
				"			.locals init ()",
				"			IL_0000:  ldc.i4.0",
				"			IL_0001:  ret",
				"		}",
				"		",
				"		.method public virtual hidebysig instance void Dispose() cil managed",
				"		{",
				"			.locals init ()",
				"			IL_0000:  ret",
				"		}",
				"		",
				"		.method public hidebysig instance void JustReturn() cil managed",
				"		{",
				"			.locals init ()",
				"			IL_0000:  ret",
				"		}",
				"	}",
				"}"
			};
		}
	}
}

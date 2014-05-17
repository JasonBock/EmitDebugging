using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using Xunit;

namespace EmitDebugging.Tests
{
	public sealed class TypeDebuggingTests
		: AssemblyCreationTests
	{
		[Fact]
		[SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
		public static void CallDisposeAfterDisposing()
		{
			TypeDebugging type = null;

			using (var assembly = AssemblyCreationTests.CreateDebuggingAssembly("Me"))
			{
				using (type = AssemblyCreationTests.CreateDebuggingType(assembly,
					assembly.Builder.GetDynamicModule("Me"), "Type")) { }

				type.Dispose();
			}
		}

		[Fact]
		public static void CallGetMethodDebuggingWithConstructorBuilderAfterDisposing()
		{
			TypeDebugging type = null;

			using (var assembly = AssemblyCreationTests.CreateDebuggingAssembly("Me"))
			{
				using (type = AssemblyCreationTests.CreateDebuggingType(assembly,
					assembly.Builder.GetDynamicModule("Me"), "Type")) { }

				Assert.Throws<ObjectDisposedException>(() => type.GetMethodDebugging(null as ConstructorBuilder));
			}
		}

		[Fact]
		public static void CallGetMethodDebuggingWithMethodBuilderAfterDisposing()
		{
			TypeDebugging type = null;

			using (var assembly = AssemblyCreationTests.CreateDebuggingAssembly("Me"))
			{
				using (type = AssemblyCreationTests.CreateDebuggingType(assembly,
					assembly.Builder.GetDynamicModule("Me"), "Type")) { }

				Assert.Throws<ObjectDisposedException>(() => type.GetMethodDebugging(null as MethodBuilder));
			}
		}
	}
}

using System;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace EmitDebugging.Tests
{
	public sealed class AssemblyDebuggingTests
		: AssemblyCreationTests
	{
		[Fact]
		[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public static void CreateWithNullArgument()
		{
			Assert.Throws<ArgumentNullException>(() => new AssemblyDebugging(null, null, null));
		}

		[Fact]
		[SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
		public static void CallDisposeAfterDisposing()
		{
			AssemblyDebugging assembly = null;

			using (assembly = AssemblyCreationTests.CreateDebuggingAssembly("Me")) { }

			assembly.Dispose();
		}

		[Fact]
		public static void CallEqualsAfterDisposing()
		{
			AssemblyDebugging assembly = null;

			using (assembly = AssemblyCreationTests.CreateDebuggingAssembly("Me")) { }

			Assert.Throws<ObjectDisposedException>(() => assembly.GetTypeDebugging(null));
		}
	}
}

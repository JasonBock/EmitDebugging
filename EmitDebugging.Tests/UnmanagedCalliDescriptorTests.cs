using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Xunit;

namespace EmitDebugging.Tests
{
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Calli")]
	public static class UnmanagedCalliDescriptorTests
	{
		[Fact]
		public static void GetDescription()
		{
			Assert.Equal("unmanaged stdcall int32(uint8, valuetype System.Guid)",
				new UnmanagedCalliDescriptor(CallingConvention.StdCall, typeof(int),
					new Type[] { typeof(byte), typeof(Guid) }).Value);
		}

		[Fact]
		public static void GetDescriptionWithNoArguments()
		{
			Assert.Equal("unmanaged stdcall int32()",
				new UnmanagedCalliDescriptor(CallingConvention.StdCall, typeof(int),
					Type.EmptyTypes).Value);
		}

		[Fact]
		public static void GetDescriptionWithNullArguments()
		{
			Assert.Equal("unmanaged stdcall int32()",
				new UnmanagedCalliDescriptor(CallingConvention.StdCall, typeof(int),
					null).Value);
		}

		[Fact]
		public static void GetDescriptionWithNullReturn()
		{
			Assert.Equal("unmanaged stdcall void(uint8, valuetype System.Guid)",
				new UnmanagedCalliDescriptor(CallingConvention.StdCall, null,
					new Type[] { typeof(byte), typeof(Guid) }).Value);
		}

		[Fact]
		public static void GetDescriptionWithVoidReturn()
		{
			Assert.Equal("unmanaged stdcall void(uint8, valuetype System.Guid)",
				new UnmanagedCalliDescriptor(CallingConvention.StdCall, typeof(void),
					new Type[] { typeof(byte), typeof(Guid) }).Value);
		}
	}
}

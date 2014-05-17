using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Xunit;

namespace EmitDebugging.Tests
{
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Calli")]
	public static class ManagedCalliDescriptorTests
	{
		[Fact]
		public static void GetDescription()
		{
			Assert.Equal("standard hasthis int32(uint8, valuetype System.Guid)",
				new ManagedCalliDescriptor(CallingConventions.Standard | CallingConventions.HasThis,
					typeof(int), new Type[] { typeof(byte), typeof(Guid) }, null).Value);
		}

		[Fact]
		public static void GetDescriptionWithVarArgArgumentsButConventionIsNotVarArg()
		{
			Assert.Equal("standard hasthis int32(uint8, valuetype System.Guid)",
				new ManagedCalliDescriptor(CallingConventions.Standard | CallingConventions.HasThis,
					typeof(int), new Type[] { typeof(byte), typeof(Guid) },
					new Type[] { typeof(byte), typeof(Guid) }).Value);
		}

		[Fact]
		public static void GetDescriptionWithVarArgConvention()
		{
			Assert.Equal("varargs int32(uint8, valuetype System.Guid, ..., uint8, valuetype System.Guid)",
				new ManagedCalliDescriptor(CallingConventions.VarArgs,
					typeof(int), new Type[] { typeof(byte), typeof(Guid) },
					new Type[] { typeof(byte), typeof(Guid) }).Value);
		}

		[Fact]
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "NoVar")]
		public static void GetDescriptionWithVarArgConventionAndNoVarArgArgumentsGiven()
		{
			Assert.Equal("varargs int32(uint8, valuetype System.Guid, ...)",
				new ManagedCalliDescriptor(CallingConventions.VarArgs,
					typeof(int), new Type[] { typeof(byte), typeof(Guid) },
					Type.EmptyTypes).Value);
		}
	}
}

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Xunit;

namespace EmitDebugging.Tests
{
	public sealed class FieldInfoDescriptorTests
	{
#pragma warning disable 169
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
		private Guid someField;
#pragma warning restore 169

		[Fact]
		public void GetDescription()
		{
			Assert.Equal("System.Guid EmitDebugging.Tests.FieldInfoDescriptorTests::someField",
				new FieldInfoDescriptor(this.GetType().GetField(
					"someField", BindingFlags.NonPublic | BindingFlags.Instance)).Value);
		}
	}
}

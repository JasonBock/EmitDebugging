using System.Diagnostics.CodeAnalysis;
using EmitDebugging.Extensions;
using Xunit;

namespace EmitDebugging.Tests.Extensions
{
	public sealed class MethodBaseExtensionsGetGenericParameterDeclarationNamesTests
	{
		[Fact]
		public void GetNamesFromMethodWithManyGenerics()
		{
			var names = this.GetType().GetMethod(
				"ManyGenerics").GetGenericParameterDeclarationNames();
			Assert.Equal(5, names.Count);
			Assert.True(names.Contains("T"));
			Assert.True(names.Contains("U"));
			Assert.True(names.Contains("V"));
			Assert.True(names.Contains("W"));
			Assert.True(names.Contains("X"));
		}

		[Fact]
		public void GetNamesFromMethodWithNoGenerics()
		{
			Assert.Equal(0, this.GetType().GetMethod(
				"NoGenerics").GetGenericParameterDeclarationNames().Count);
		}

		[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "U")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "W")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "X")]
		[SuppressMessage("Microsoft.Naming", "CA1715:IdentifiersShouldHaveCorrectPrefix", MessageId = "T")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "V")]
		[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
		public void ManyGenerics<T, U, V, W, X>() { }

		[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
		public void NoGenerics() { }
	}
}

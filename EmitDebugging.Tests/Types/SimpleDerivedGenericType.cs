using System;
using System.Diagnostics.CodeAnalysis;

namespace EmitDebugging.Tests.Types
{
	[SuppressMessage("Microsoft.Naming", "CA1715:IdentifiersShouldHaveCorrectPrefix", MessageId = "T")]
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "U")]
	public class SimpleDerivedGenericType<U>
		: SimpleGenericType<string, U>
	{
		[SuppressMessage("Microsoft.Naming", "CA1715:IdentifiersShouldHaveCorrectPrefix", MessageId = "T")]
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "a")]
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "b")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "a")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "V")]
		public void DerivedMethodOne<V>(U a, V b) { }

		public void DerivedCallMethod()
		{
			var simple = new SimpleDerivedGenericType<Uri>();
			simple.DerivedMethodOne<string>(new Uri("http://www.somesite.com"), "s");
		}
	}
}

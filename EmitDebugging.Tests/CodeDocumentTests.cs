using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.SymbolStore;
using System.Reflection.Emit;
using Xunit;

namespace EmitDebugging.Tests
{
	public static class CodeDocumentTests
	{
		[Fact]
		[SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
		public static void CallDisposeAfterDisposing()
		{
			CodeDocument document = null;

			using (document = new CodeDocument(0, 0)) { }

			document.Dispose();
		}

		[Fact]
		public static void CallWriteTextWithCodeLineAfterDisposing()
		{
			CodeDocument document = null;

			using (document = new CodeDocument(0, 0)) { }

			Assert.Throws<ObjectDisposedException>(() => document.WriteText(null as CodeLine));
		}

		[Fact]
		public static void CallWriteTextWithListOfCodeLineILGeneratorISymbolDocumentWriterAfterDisposing()
		{
			CodeDocument document = null;

			using (document = new CodeDocument(0, 0)) { }

			Assert.Throws<ObjectDisposedException>(() => document.WriteText(null as List<CodeLine>,
				null as ILGenerator, null as ISymbolDocumentWriter));
		}

		[Fact]
		public static void CallWriteTextWithIEnumerableOfCodeLineILGeneratorISymbolDocumentWriterAfterDisposing()
		{
			CodeDocument document = null;

			using (document = new CodeDocument(0, 0)) { }

			Assert.Throws<ObjectDisposedException>(() => document.WriteText(null as IEnumerable<CodeLine>,
				null as ILGenerator, null as ISymbolDocumentWriter));
		}

		[Fact]
		public static void CallWriteTextWithCodeLineILGeneratorISymbolDocumentWriterAfterDisposing()
		{
			CodeDocument document = null;

			using (document = new CodeDocument(0, 0)) { }

			Assert.Throws<ObjectDisposedException>(() => document.WriteText(null as CodeLine,
				null as ILGenerator, null as ISymbolDocumentWriter));
		}

		[Fact]
		public static void WriteMultipleLinesWithDifferentDebuggableStates()
		{
			using (var document = new CodeDocument(0, 0))
			{
				document.WriteText(new List<CodeLine>()
				{
					new CodeLine("x", true),
					new CodeLine("x", false),
					new CodeLine("x", true)
				}, null, null);
			}
		}
	}
}

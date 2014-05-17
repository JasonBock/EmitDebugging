using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.SymbolStore;
using System.IO;
using System.Reflection.Emit;

namespace EmitDebugging
{
	public sealed class CodeDocument
		: IDisposable
	{
		private CodeDocument()
			: base()
		{
			this.Lines = new List<CodeLine>();
		}

		internal CodeDocument(string ilFile)
			: this()
		{
			this.Writer = new StreamWriter(ilFile, false);
		}

		[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		internal CodeDocument(int linesOfCode, int indentLevel)
			: this()
		{
			this.LinesOfCode = linesOfCode;
			this.IndentLevel = indentLevel;
			this.Writer = new StreamWriter(new MemoryStream());
		}

		~CodeDocument()
		{
			this.Dispose(false);
		}

		private void CheckForDisposed()
		{
			if (this.Disposed)
			{
				throw new ObjectDisposedException("CodeDocument");
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (!this.Disposed && disposing)
			{
				this.Writer.Dispose();
			}

			this.Disposed = true;
		}

		internal void WriteText(CodeLine line)
		{
			this.CheckForDisposed();
			this.WriteText(line, null, null);
		}

		internal void WriteText(IEnumerable<CodeLine> lines, ILGenerator generator, ISymbolDocumentWriter symbolDocument)
		{
			this.CheckForDisposed();

			foreach (var line in lines)
			{
				this.WriteText(line, generator, symbolDocument);
			}
		}

		internal void WriteText(CodeLine line, ILGenerator generator, ISymbolDocumentWriter symbolDocument)
		{
			this.CheckForDisposed();

			this.IndentLevel += (int)line.Indent;

			var code = new string('\t', this.IndentLevel) + line.Code;

			this.Writer.WriteLine(code);
			this.Lines.Add(line);
			this.LinesOfCode++;

			if (line.IsDebuggable && generator != null && symbolDocument != null)
			{
				generator.MarkSequencePoint(symbolDocument,
					this.LinesOfCode, this.IndentLevel + 1,
					this.LinesOfCode, this.IndentLevel + 1 + line.Code.Length);
			}
		}

		internal void WriteText(List<CodeLine> lines, ILGenerator generator, ISymbolDocumentWriter symbolDocument)
		{
			this.CheckForDisposed();

			var startColumn = this.IndentLevel;
			var endColumn = this.IndentLevel;
			var areAllLinesDebuggable = true;

			foreach (var line in lines)
			{
				areAllLinesDebuggable &= line.IsDebuggable;

				this.IndentLevel += (int)line.Indent;

				var currentEndColumn = this.IndentLevel + 1 + line.Code.Length;
				endColumn = currentEndColumn > endColumn ? currentEndColumn : endColumn;

				var code = new string('\t', this.IndentLevel) + line.Code;

				this.Writer.WriteLine(code);
				this.Lines.Add(line);
				this.LinesOfCode++;
			}

			if (areAllLinesDebuggable && generator != null && symbolDocument != null)
			{
				generator.MarkSequencePoint(symbolDocument,
					this.LinesOfCode - lines.Count + 1, startColumn + 1,
					this.LinesOfCode, endColumn + 1);
			}
		}

		private bool Disposed
		{
			get;
			set;
		}

		internal int IndentLevel
		{
			get;
			private set;
		}

		internal List<CodeLine> Lines
		{
			get;
			private set;
		}

		internal int LinesOfCode
		{
			get;
			private set;
		}

		private StreamWriter Writer
		{
			get;
			set;
		}
	}
}

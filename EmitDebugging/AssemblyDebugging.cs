using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.SymbolStore;
using System.Reflection.Emit;

namespace EmitDebugging
{
	[SuppressMessage("Microsoft.Interoperability", "CA1405:ComVisibleTypeBaseTypesShouldBeComVisible")]
	public sealed class AssemblyDebugging
		: Debugging<AssemblyBuilder>, IDisposable
	{
		public AssemblyDebugging(string codeFile, AssemblyBuilder assembly, ISymbolDocumentWriter symbolDocument)
			: base()
		{
			if (assembly == null)
			{
				throw new ArgumentNullException("assembly");
			}

			this.Builder = assembly;

			if (!string.IsNullOrEmpty(codeFile))
			{
				this.Document = new CodeDocument(codeFile);
			}

			this.SymbolDocument = symbolDocument;
			this.BeginAssembly();
		}

		~AssemblyDebugging()
		{
			this.Dispose(false);
		}

		private void CheckForDisposed()
		{
			if (this.Disposed)
			{
				throw new ObjectDisposedException("AssemblyDebugging");
			}
		}

		public override void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (!this.Disposed && disposing)
			{
				var decrease = Indention.Decrease;

				if (!this.CreatedType)
				{
					decrease = Indention.KeepCurrent;
				}

				this.Document.WriteText(new CodeLine("}", decrease));
				this.Document.Dispose();
			}

			this.Disposed = true;
		}

		public TypeDebugging GetTypeDebugging(TypeBuilder type)
		{
			return this.GetTypeDebugging(type, null);
		}

		public TypeDebugging GetTypeDebugging(TypeBuilder type, HashSet<Type> interfacesToImplement)
		{
			this.CheckForDisposed();

			var increase = Indention.Increase;

			if (this.CreatedType)
			{
				increase = Indention.KeepCurrent;

				if (this.Document != null)
				{
					this.Document.WriteText(new CodeLine(string.Empty, increase));
				}
			}
			else
			{
				this.CreatedType = true;
			}

			return new TypeDebugging(type, interfacesToImplement, this.Document, this.SymbolDocument, increase);
		}

		private void BeginAssembly()
		{
			this.Document.WriteText(new CodeLine(".assembly " + this.Builder.GetName().Name));
			this.Document.WriteText(new CodeLine("{"));
		}

		private bool CreatedType
		{
			get;
			set;
		}

		private bool Disposed
		{
			get;
			set;
		}

		private CodeDocument Document
		{
			get;
			set;
		}

		private ISymbolDocumentWriter SymbolDocument
		{
			get;
			set;
		}
	}
}

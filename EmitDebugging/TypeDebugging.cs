using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.SymbolStore;
using System.Reflection.Emit;
using EmitDebugging.Extensions;

namespace EmitDebugging
{
	[SuppressMessage("Microsoft.Interoperability", "CA1405:ComVisibleTypeBaseTypesShouldBeComVisible")]
	public sealed class TypeDebugging
		: Debugging<TypeBuilder>, IDisposable
	{
		internal TypeDebugging(TypeBuilder type, HashSet<Type> interfacesToImplement, CodeDocument document,
			ISymbolDocumentWriter symbolDocument, Indention firstIndent)
			: base()
		{
			this.Builder = type;
			this.Document = document;
			this.SymbolDocument = symbolDocument;
			this.InterfacesToImplement = interfacesToImplement;
			this.AddInterfacesToImplement();
			this.BeginType(firstIndent);
		}

		~TypeDebugging()
		{
			this.Dispose(false);
		}

		private void AddInterfacesToImplement()
		{
			if (this.InterfacesToImplement != null && this.InterfacesToImplement.Count > 0)
			{
				foreach (var interfaceToImplement in this.InterfacesToImplement)
				{
					if (interfaceToImplement != null && interfaceToImplement.IsInterface)
					{
						this.Builder.AddInterfaceImplementation(interfaceToImplement);
					}
				}
			}
		}

		private void CheckForDisposed()
		{
			if (this.Disposed)
			{
				throw new ObjectDisposedException("TypeDebugging");
			}
		}

		public override void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if ((!this.Disposed && disposing) && (this.Document != null))
			{
				Indention decrease = Indention.Decrease;
				if (!this.CreatedMethod)
				{
					decrease = Indention.KeepCurrent;
				}

				this.Document.WriteText(new CodeLine("}", decrease));
			}

			this.Disposed = true;
		}

		public ConstructorMethodDebugging GetMethodDebugging(ConstructorBuilder method)
		{
			this.CheckForDisposed();
			return new ConstructorMethodDebugging(
				method, this.Document, this.SymbolDocument, this.HandleFirstMethod());
		}

		public MethodDebugging GetMethodDebugging(MethodBuilder method)
		{
			this.CheckForDisposed();
			return new MethodDebugging(
				method, this.Document, this.SymbolDocument, this.HandleFirstMethod());
		}

		private Indention HandleFirstMethod()
		{
			this.CheckForDisposed();

			var increase = Indention.Increase;

			if (this.CreatedMethod)
			{
				increase = Indention.KeepCurrent;

				if (this.Document != null)
				{
					this.Document.WriteText(new CodeLine(string.Empty, increase));
				}

				return increase;
			}

			this.CreatedMethod = true;
			return increase;
		}

		private void BeginType(Indention firstIndent)
		{
			if (this.Document != null)
			{
				var definitionParts = new List<string>(){ 
					".class", this.Builder.GetAttributes(), this.Builder.FullName,
					"extends", new TypeDescriptor(this.Builder.BaseType,
						this.Builder.Assembly).Value };

				if (this.InterfacesToImplement != null && this.InterfacesToImplement.Count > 0)
				{
					foreach (var interfaceToImplement in this.InterfacesToImplement)
					{
						if (interfaceToImplement != null && interfaceToImplement.IsInterface)
						{
							definitionParts.Add("implements");
							definitionParts.Add(interfaceToImplement.FullName);
						}
					}
				}

				this.Document.WriteText(new CodeLine(
					string.Join(" ", definitionParts.ToArray()), firstIndent));
				this.Document.WriteText(new CodeLine("{"));
			}
		}

		private bool CreatedMethod
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

		private HashSet<Type> InterfacesToImplement
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

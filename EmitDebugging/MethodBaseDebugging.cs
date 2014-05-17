using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.SymbolStore;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace EmitDebugging
{
	public abstract class MethodBaseDebugging<T>
		: Debugging<T>, IDisposable
		where T : MethodBase
	{
		private MethodBaseDebugging()
		{
			this.LabelFixups = new Dictionary<Label, string>();
			this.Locals = new List<LocalBuilder>();
		}

		protected internal MethodBaseDebugging(T method, ILGenerator generator, CodeDocument document, ISymbolDocumentWriter symbolDocument, Indention firstIndent)
			: this()
		{
			this.Builder = method;
			this.Generator = generator;
			this.Document = document;
			this.SymbolDocument = symbolDocument;
			this.CurrentIndent = firstIndent;

			if (this.Document != null)
			{
				this.BeginMethod(method);
			}
		}

		~MethodBaseDebugging()
		{
			this.Dispose(false);
		}

		public void BeginCatchBlock(Type exceptionType)
		{
			this.CheckForDisposed();

			if (this.Document != null)
			{
				this.CurrentIndent = Indention.Decrease;
				var endBlock = new CodeLine("}", this.CurrentIndent);
				this.CurrentIndent = Indention.KeepCurrent;
				var catchLine = exceptionType == null ? "catch" :
					"catch " + new TypeDescriptor(exceptionType, this.Builder.DeclaringType.Assembly).Value;
				var catchException = new CodeLine(catchLine, this.CurrentIndent);
				var beginBlock = new CodeLine("{", this.CurrentIndent);
				this.CurrentIndent = Indention.Increase;
				this.TempDocument.WriteText(
					new CodeLine[] { endBlock, catchException, beginBlock },
					this.Generator, this.SymbolDocument);
			}

			this.Generator.BeginCatchBlock(exceptionType);
		}

		public void BeginExceptFilterBlock()
		{
			this.CheckForDisposed();
			this.BeginHandlerBlock("filter");
			this.Generator.BeginExceptFilterBlock();
		}

		public Label BeginExceptionBlock()
		{
			this.CheckForDisposed();

			if (this.Document != null)
			{
				var tryStatement = new CodeLine(".try", this.CurrentIndent);
				this.CurrentIndent = Indention.KeepCurrent;
				var beginBlock = new CodeLine("{", this.CurrentIndent);
				this.CurrentIndent = Indention.Increase;
				this.TempDocument.WriteText(
					new CodeLine[] { tryStatement, beginBlock },
					this.Generator, this.SymbolDocument);
				this.TryBlockCount++;
			}

			var label = this.Generator.BeginExceptionBlock();

			if (this.Document != null)
			{
				this.LabelFixups.Add(label, string.Empty);
			}

			return label;
		}

		private void BeginHandlerBlock(string name)
		{
			if (this.Document != null)
			{
				this.CurrentIndent = Indention.Decrease;
				var endBlock = new CodeLine("}", this.CurrentIndent);
				this.CurrentIndent = Indention.KeepCurrent;
				var handlerStatement = new CodeLine(name, this.CurrentIndent);
				this.CurrentIndent = Indention.KeepCurrent;
				var beginBlock = new CodeLine("{", this.CurrentIndent);
				this.CurrentIndent = Indention.Increase;
				this.TempDocument.WriteText(
					new CodeLine[] { endBlock, handlerStatement, beginBlock },
					this.Generator, this.SymbolDocument);
			}
		}

		public void BeginFaultBlock()
		{
			this.CheckForDisposed();
			this.BeginHandlerBlock("fault");
			this.Generator.BeginFaultBlock();
		}

		public void BeginFinallyBlock()
		{
			this.CheckForDisposed();
			this.BeginHandlerBlock("finally");
			this.Generator.BeginFinallyBlock();
		}

		public void BeginScope()
		{
			this.CheckForDisposed();

			if (this.Document != null)
			{
				this.CurrentIndent = Indention.KeepCurrent;
				this.TempDocument.WriteText(
					new CodeLine("{", this.CurrentIndent));
				this.CurrentIndent = Indention.Increase;
			}

			this.Generator.BeginScope();
		}

		private void CheckForDisposed()
		{
			if (this.Disposed)
			{
				throw new ObjectDisposedException("MethodDebugging");
			}
		}

		private string GetLocals()
		{
			this.CheckForDisposed();

			string locals = string.Empty;

			if (this.Locals.Count > 0)
			{
				var localDeclarations = new List<string>();

				foreach (var local in this.Locals)
				{
					var localIndex = local.LocalIndex;
					var name = "V_" + local.LocalIndex.ToString(CultureInfo.CurrentCulture);

					localDeclarations.Add(string.Format(CultureInfo.CurrentCulture, "[{0}] {1} {2}",
						new object[] { localIndex, new TypeDescriptor(
							local.LocalType, this.Builder.DeclaringType.Assembly).Value, 
							name }));
				}

				locals = string.Join(", ", localDeclarations.ToArray());
			}

			return locals;
		}

		private string Convert(Label target)
		{
			return this.LabelFixups[target];
		}

		public LocalBuilder DeclareLocal(Type localType)
		{
			this.CheckForDisposed();
			return this.DeclareLocal(localType, false);
		}

		public LocalBuilder DeclareLocal(Type localType, bool pinned)
		{
			this.CheckForDisposed();

			var item = this.Generator.DeclareLocal(localType, pinned);

			if (this.Document != null)
			{
				this.Locals.Add(item);

				if (this.SymbolDocument != null)
				{
					item.SetLocalSymInfo(
						"V_" + item.LocalIndex.ToString(CultureInfo.CurrentCulture));
				}
			}

			return item;
		}

		public Label DefineLabel()
		{
			this.CheckForDisposed();

			var label = this.Generator.DefineLabel();

			if (this.Document != null)
			{
				this.LabelFixups.Add(label, string.Empty);
			}

			return label;
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
				if (this.Document != null)
				{
					this.EndMethod();
				}

				if (this.TempDocument != null)
				{
					this.TempDocument.Dispose();
				}
			}

			this.Disposed = true;
		}

		public void Emit(OpCode opCode)
		{
			this.CheckForDisposed();

			if (this.Document != null)
			{
				this.TempDocument.WriteText(
					new CodeLine(this.GetLabelPrefix() + opCode.ToString(),
					this.CurrentIndent, true), this.Generator, this.SymbolDocument);
				this.CurrentIndent = Indention.KeepCurrent;
				this.LabelIndex += opCode.Size;
			}

			this.Generator.Emit(opCode);
		}

		public void Emit(OpCode opCode, byte arg)
		{
			this.CheckForDisposed();

			if (this.Document != null)
			{
				this.TempDocument.WriteText(
					new CodeLine(this.GetLabelPrefix() + opCode.ToString() +
					" " + arg.ToString(CultureInfo.CurrentCulture), this.CurrentIndent, true),
					this.Generator, this.SymbolDocument);
				this.CurrentIndent = Indention.KeepCurrent;
				this.LabelIndex += opCode.Size + sizeof(byte);
			}

			this.Generator.Emit(opCode, arg);
		}

		public void Emit(OpCode opCode, double arg)
		{
			this.CheckForDisposed();

			if (this.Document != null)
			{
				this.TempDocument.WriteText(
					new CodeLine(this.GetLabelPrefix() + opCode.ToString() +
					" " + arg.ToString(CultureInfo.CurrentCulture), this.CurrentIndent, true),
					this.Generator, this.SymbolDocument);
				this.CurrentIndent = Indention.KeepCurrent;
				this.LabelIndex += opCode.Size + sizeof(double);
			}

			this.Generator.Emit(opCode, arg);
		}

		public void Emit(OpCode opCode, short arg)
		{
			this.CheckForDisposed();

			if (this.Document != null)
			{
				this.TempDocument.WriteText(
					new CodeLine(this.GetLabelPrefix() + opCode.ToString() +
						" " + arg.ToString(CultureInfo.CurrentCulture), this.CurrentIndent, true),
					this.Generator, this.SymbolDocument);
				this.CurrentIndent = Indention.KeepCurrent;
				this.LabelIndex += opCode.Size + sizeof(short);
			}

			this.Generator.Emit(opCode, arg);
		}

		public void Emit(OpCode opCode, int arg)
		{
			this.CheckForDisposed();

			if (this.Document != null)
			{
				this.TempDocument.WriteText(
					new CodeLine(this.GetLabelPrefix() + opCode.ToString() +
					" " + arg.ToString(CultureInfo.CurrentCulture), this.CurrentIndent, true),
					this.Generator, this.SymbolDocument);
				this.CurrentIndent = Indention.KeepCurrent;
				this.LabelIndex += opCode.Size + sizeof(int);
			}

			this.Generator.Emit(opCode, arg);
		}

		public void Emit(OpCode opCode, long arg)
		{
			this.CheckForDisposed();

			if (this.Document != null)
			{
				this.TempDocument.WriteText(
					new CodeLine(this.GetLabelPrefix() + opCode.ToString() +
						" " + arg.ToString(CultureInfo.CurrentCulture), this.CurrentIndent, true),
					this.Generator, this.SymbolDocument);
				this.CurrentIndent = Indention.KeepCurrent;
				this.LabelIndex += opCode.Size + sizeof(long);
			}

			this.Generator.Emit(opCode, arg);
		}

		public void Emit(OpCode opCode, ConstructorInfo con)
		{
			this.CheckForDisposed();

			if (this.Document != null)
			{
				this.TempDocument.WriteText(
					new CodeLine(this.GetLabelPrefix() + opCode.ToString() +
						" " + MethodDescriptorFactory.Create(con).Value, this.CurrentIndent, true),
					this.Generator, this.SymbolDocument);
				this.LabelIndex += opCode.Size + 4;
				this.CurrentIndent = Indention.KeepCurrent;
			}

			this.Generator.Emit(opCode, con);
		}

		public void Emit(OpCode opCode, Label label)
		{
			this.CheckForDisposed();

			if (this.Document != null)
			{
				var labels = new Label[] { label };
				this.TempDocument.WriteText(
					new CodeLine(this.GetLabelPrefix() + opCode.ToString() +
						" IL_{0}", this.CurrentIndent, true, labels), this.Generator, this.SymbolDocument);
				this.CurrentIndent = Indention.KeepCurrent;
				this.LabelIndex += opCode.Size + 1;
			}

			this.Generator.Emit(opCode, label);
		}

		public void Emit(OpCode opCode, SignatureHelper signature)
		{
			this.CheckForDisposed();

			if (this.Document != null)
			{
				var token = (this.Builder.Module as ModuleBuilder).GetSignatureToken(signature);

				this.TempDocument.WriteText(
					new CodeLine(this.GetLabelPrefix() + opCode.ToString() +
						" 0x" + token.Token.ToString("X4", CultureInfo.CurrentCulture),
						this.CurrentIndent, true),
					this.Generator, this.SymbolDocument);
				this.CurrentIndent = Indention.KeepCurrent;
				this.LabelIndex += opCode.Size + 4;
			}

			this.Generator.Emit(opCode, signature);
		}

		public void Emit(OpCode opCode, Label[] labels)
		{
			if (labels == null)
			{
				throw new ArgumentNullException("labels");
			}

			this.CheckForDisposed();

			if (this.Document != null)
			{
				var labelPositions = new List<string>();

				for (var i = 0; i <= labels.Length - 1; i++)
				{
					labelPositions.Add("IL_{" + i + "}");
				}

				var code = this.GetLabelPrefix() + opCode.ToString() + " (" +
					string.Join(", ", labelPositions.ToArray()) + ")";
				this.TempDocument.WriteText(
					new CodeLine(code, this.CurrentIndent, labels),
					this.Generator, this.SymbolDocument);
				this.CurrentIndent = Indention.KeepCurrent;
				this.LabelIndex += opCode.Size + 1;
			}

			this.Generator.Emit(opCode, labels);
		}

		public void Emit(OpCode opCode, LocalBuilder local)
		{
			this.CheckForDisposed();

			if (local == null)
			{
				throw new ArgumentNullException("local");
			}

			if (this.Document != null)
			{
				this.TempDocument.WriteText(
					new CodeLine(this.GetLabelPrefix() + opCode.ToString() +
						" V_" + local.LocalIndex.ToString(CultureInfo.CurrentCulture),
					this.CurrentIndent, true), this.Generator, this.SymbolDocument);
				this.CurrentIndent = Indention.KeepCurrent;
				this.LabelIndex += opCode.Size + 1;
			}

			this.Generator.Emit(opCode, local);
		}

		public void Emit(OpCode opCode, FieldInfo field)
		{
			this.CheckForDisposed();

			if (this.Document != null)
			{
				this.TempDocument.WriteText(
					new CodeLine(this.GetLabelPrefix() + opCode.ToString() +
						" " + new FieldInfoDescriptor(field).Value,
						this.CurrentIndent, true),
					this.Generator, this.SymbolDocument);
				this.CurrentIndent = Indention.KeepCurrent;
				this.LabelIndex += opCode.Size + 4;
			}

			this.Generator.Emit(opCode, field);
		}

		public void Emit(OpCode opCode, MethodInfo method)
		{
			this.CheckForDisposed();

			if (this.Document != null)
			{
				this.TempDocument.WriteText(
					new CodeLine(this.GetLabelPrefix() + opCode.ToString() +
						" " + MethodDescriptorFactory.Create(method).Value, this.CurrentIndent, true),
					this.Generator, this.SymbolDocument);
				this.CurrentIndent = Indention.KeepCurrent;
				this.LabelIndex += opCode.Size + 4;
			}

			this.Generator.Emit(opCode, method);
		}

		public void Emit(OpCode opCode, float arg)
		{
			this.CheckForDisposed();

			if (this.Document != null)
			{
				this.TempDocument.WriteText(
					new CodeLine(this.GetLabelPrefix() + opCode.ToString() +
						" " + arg.ToString(CultureInfo.CurrentCulture), this.CurrentIndent, true),
					this.Generator, this.SymbolDocument);
				this.CurrentIndent = Indention.KeepCurrent;
				this.LabelIndex += opCode.Size + sizeof(float);
			}

			this.Generator.Emit(opCode, arg);
		}

		public void Emit(OpCode opCode, string str)
		{
			this.CheckForDisposed();

			if (str == null)
			{
				throw new ArgumentNullException("str");
			}

			if (this.Document != null)
			{
				this.TempDocument.WriteText(
					new CodeLine(this.GetLabelPrefix() + opCode.ToString() +
						" \"" + str + "\"", this.CurrentIndent, true), this.Generator, this.SymbolDocument);
				this.CurrentIndent = Indention.KeepCurrent;
				this.LabelIndex += opCode.Size + str.Length;
			}

			this.Generator.Emit(opCode, str);
		}

		public void Emit(OpCode opCode, Type cls)
		{
			this.CheckForDisposed();

			if (this.Document != null)
			{
				this.TempDocument.WriteText(
					new CodeLine(this.GetLabelPrefix() + opCode.ToString() + " " +
						new TypeDescriptor(cls, this.Builder.DeclaringType.Assembly).Value,
							this.CurrentIndent, true), this.Generator, this.SymbolDocument);
				this.CurrentIndent = Indention.KeepCurrent;
				this.LabelIndex += opCode.Size + 4;
			}

			this.Generator.Emit(opCode, cls);
		}

		public void EmitCall(OpCode opCode, MethodInfo methodInfo, Type[] optionalParameterTypes)
		{
			this.CheckForDisposed();
			this.TempDocument.WriteText(
				new CodeLine(this.GetLabelPrefix() + opCode.ToString() + " " +
					MethodDescriptorFactory.Create(methodInfo).Value,
						this.CurrentIndent, true), this.Generator, this.SymbolDocument);
			this.LabelIndex += opCode.Size + 4;
			this.Generator.EmitCall(opCode, methodInfo, optionalParameterTypes);
		}

		private string EmitCallConsoleWriteLine(Type type)
		{
			var writer = typeof(TextWriter);

			var writeLine = writer.GetMethod("WriteLine", BindingFlags.Instance | BindingFlags.Public,
					null, new Type[] { type }, null);

			return this.GetLabelPrefix() + "callvirt " + MethodDescriptorFactory.Create(writeLine).Value;
		}

		private string EmitCallConsoleGetOutMethod()
		{
			return this.GetLabelPrefix() + "call " + MethodDescriptorFactory.Create(
				typeof(Console).GetProperty("Out", BindingFlags.Static | BindingFlags.Public).GetGetMethod()).Value;
		}

		public void EmitCalli(OpCode opCode, CallingConvention unmanagedCallConv, Type returnType, Type[] parameterTypes)
		{
			this.CheckForDisposed();
			this.TempDocument.WriteText(
				new CodeLine(this.GetLabelPrefix() + opCode.ToString() + " " +
					new UnmanagedCalliDescriptor(unmanagedCallConv, returnType, parameterTypes).Value,
						this.CurrentIndent, true), this.Generator, this.SymbolDocument);
			this.LabelIndex += opCode.Size + 4;
			this.Generator.EmitCalli(opCode, unmanagedCallConv, returnType, parameterTypes);
		}

		public void EmitCalli(OpCode opCode, CallingConventions callingConvention, Type returnType, Type[] parameterTypes, Type[] optionalParameterTypes)
		{
			this.CheckForDisposed();
			this.TempDocument.WriteText(
				new CodeLine(this.GetLabelPrefix() + opCode.ToString() + " " +
					new ManagedCalliDescriptor(callingConvention, returnType, parameterTypes, optionalParameterTypes).Value,
						this.CurrentIndent, true), this.Generator, this.SymbolDocument);
			this.LabelIndex += opCode.Size + 4;
			this.Generator.EmitCalli(opCode, callingConvention, returnType, parameterTypes,
				optionalParameterTypes);
		}

		public void EmitWriteLine(LocalBuilder localBuilder)
		{
			this.CheckForDisposed();

			if (localBuilder == null)
			{
				throw new ArgumentNullException("localBuilder");
			}

			if (this.Document != null)
			{
				this.CurrentIndent = Indention.KeepCurrent;
				var codeLines = new List<CodeLine>();

				codeLines.Add(new CodeLine(
					this.EmitCallConsoleGetOutMethod(), this.CurrentIndent, true));
				this.LabelIndex += 5;

				string loadValueCall = null;
				int instructionSize = 0;

				if (localBuilder.LocalIndex < 4)
				{
					loadValueCall = "ldloc." + localBuilder.LocalIndex;
					instructionSize = 1;
				}
				else
				{
					loadValueCall = "ldloc " + localBuilder.LocalIndex;
					instructionSize = 2;
				}

				codeLines.Add(new CodeLine(
					this.GetLabelPrefix() + loadValueCall, this.CurrentIndent, true));
				this.LabelIndex += instructionSize;

				codeLines.Add(new CodeLine(
					this.EmitCallConsoleWriteLine(localBuilder.LocalType), this.CurrentIndent, true));
				this.LabelIndex += 5;

				this.TempDocument.WriteText(
					codeLines, this.Generator, this.SymbolDocument);
			}

			this.Generator.EmitWriteLine(localBuilder);
		}

		public void EmitWriteLine(FieldInfo field)
		{
			this.CheckForDisposed();

			if (field == null)
			{
				throw new ArgumentNullException("field");
			}

			if (this.Document != null)
			{
				this.CurrentIndent = Indention.KeepCurrent;
				var codeLines = new List<CodeLine>();

				codeLines.Add(new CodeLine(
					this.EmitCallConsoleGetOutMethod(), this.CurrentIndent, true));
				this.LabelIndex += 5;

				string loadFieldOpCode = null;

				if (!field.IsStatic)
				{
					loadFieldOpCode = "ldfld ";
					codeLines.Add(new CodeLine(
						this.GetLabelPrefix() + "ldarg.0", this.CurrentIndent, true));
					this.LabelIndex += OpCodes.Ldarg_0.Size;
				}
				else
				{
					loadFieldOpCode = "ldsfld ";
				}

				codeLines.Add(new CodeLine(
					this.GetLabelPrefix() + loadFieldOpCode + new FieldInfoDescriptor(field).Value,
					this.CurrentIndent, true));
				this.LabelIndex += 5;

				codeLines.Add(new CodeLine(
					this.EmitCallConsoleWriteLine(field.FieldType),
					this.CurrentIndent, true));
				this.LabelIndex += 5;

				this.TempDocument.WriteText(
					codeLines, this.Generator, this.SymbolDocument);
			}

			this.Generator.EmitWriteLine(field);
		}

		public void EmitWriteLine(string value)
		{
			this.CheckForDisposed();

			if (value == null)
			{
				throw new ArgumentNullException("value");
			}

			if (this.Document != null)
			{
				this.CurrentIndent = Indention.KeepCurrent;
				var codeLines = new List<CodeLine>();

				var loadValueCall = "ldstr \"" + value + "\"";
				codeLines.Add(new CodeLine(
					this.GetLabelPrefix() + loadValueCall, this.CurrentIndent, true));
				this.LabelIndex += OpCodes.Ldstr.Size + value.Length;

				codeLines.Add(new CodeLine(
					this.EmitCallConsoleWriteLine(typeof(string)), this.CurrentIndent, true));
				this.LabelIndex += 5;

				this.TempDocument.WriteText(
					codeLines, this.Generator, this.SymbolDocument);
			}

			this.Generator.EmitWriteLine(value);
		}

		public void EndExceptionBlock()
		{
			this.CheckForDisposed();

			if (this.Document != null)
			{
				this.CurrentIndent = Indention.Decrease;
				this.TempDocument.WriteText(
					new CodeLine("}", this.CurrentIndent), this.Generator, this.SymbolDocument);
				this.TryBlockCount--;

				if (this.TryBlockCount <= 0)
				{
					this.TryBlockCount = 0;
					this.CurrentIndent = Indention.KeepCurrent;
				}
			}

			this.Generator.EndExceptionBlock();
		}

		private void EndMethod()
		{
			this.CheckForDisposed();

			if (this.Document != null)
			{
				var locals = this.GetLocals();

				this.TempDocument.Lines[2].Code =
					string.Format(CultureInfo.CurrentCulture,
						this.TempDocument.Lines[2].Code, locals);

				this.CurrentIndent = Indention.Decrease;
				this.TempDocument.WriteText(
					new CodeLine("}", this.CurrentIndent), this.Generator,
					this.SymbolDocument);
				this.CurrentIndent = Indention.KeepCurrent;

				foreach (var line in this.TempDocument.Lines)
				{
					if (line.Labels.Length > 0)
					{
						line.Code = string.Format(CultureInfo.CurrentCulture, line.Code,
							Array.ConvertAll<Label, string>(line.Labels, target => this.Convert(target)));
					}

					this.Document.WriteText(line, this.Generator, this.SymbolDocument);
				}
			}
		}

		public void EndScope()
		{
			this.CheckForDisposed();

			if (this.Document != null)
			{
				this.CurrentIndent = Indention.Decrease;
				this.TempDocument.WriteText(new CodeLine("}", this.CurrentIndent),
					this.Generator, this.SymbolDocument);
				this.CurrentIndent = Indention.KeepCurrent;
			}

			this.Generator.EndScope();
		}

		private string GetLabelPrefix()
		{
			this.CheckForDisposed();
			return ("IL_" + this.LabelIndex.ToString("X4", CultureInfo.CurrentCulture) + ":  ");
		}

		public void MarkLabel(Label label)
		{
			this.CheckForDisposed();

			this.Generator.MarkLabel(label);

			if (this.Document != null)
			{
				this.LabelFixups[label] = this.LabelIndex.ToString("X4", CultureInfo.CurrentCulture);
			}
		}

		[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		private void BeginMethod(MethodBase method)
		{
			if (this.Document != null)
			{
				this.TempDocument = new CodeDocument(
					this.Document.LinesOfCode, this.Document.IndentLevel);
				this.TempDocument.WriteText(new CodeLine(
					MethodDescriptorFactory.Create(method, true).Value, this.CurrentIndent));
				this.TempDocument.WriteText(new CodeLine("{"));
				this.CurrentIndent = Indention.Increase;
				this.TempDocument.WriteText(
					new CodeLine(".locals init ({0})", this.CurrentIndent));
				this.CurrentIndent = Indention.KeepCurrent;
			}
		}

		public void ThrowException(Type exceptionType)
		{
			this.CheckForDisposed();

			if (exceptionType == null)
			{
				throw new ArgumentNullException("exceptionType");
			}

			if (this.Document != null)
			{
				this.CurrentIndent = Indention.KeepCurrent;
				var codeLines = new List<CodeLine>();

				var ctor = exceptionType.GetConstructor(Type.EmptyTypes);
				codeLines.Add(new CodeLine(this.GetLabelPrefix() + "newobj " +
					MethodDescriptorFactory.Create(ctor).Value,
					this.CurrentIndent, true));
				this.LabelIndex += 5;

				codeLines.Add(new CodeLine(this.GetLabelPrefix() + "throw " + new TypeDescriptor(
					exceptionType, this.Builder.DeclaringType.Assembly).Value,
					this.CurrentIndent, true));
				this.LabelIndex += OpCodes.Throw.Size;

				this.TempDocument.WriteText(
					codeLines, this.Generator, this.SymbolDocument);
				this.CurrentIndent = Indention.KeepCurrent;
			}

			this.Generator.ThrowException(exceptionType);
		}

		public void UsingNamespace(string usingNamespace)
		{
			this.CheckForDisposed();
			this.Generator.UsingNamespace(usingNamespace);
		}

		private Indention CurrentIndent
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

		private ILGenerator Generator
		{
			get;
			set;
		}

		private Dictionary<Label, string> LabelFixups
		{
			get;
			set;
		}

		private int LabelIndex
		{
			get;
			set;
		}

		private List<LocalBuilder> Locals
		{
			get;
			set;
		}

		private ISymbolDocumentWriter SymbolDocument
		{
			get;
			set;
		}

		private CodeDocument TempDocument
		{
			get;
			set;
		}

		private int TryBlockCount
		{
			get;
			set;
		}
	}
}

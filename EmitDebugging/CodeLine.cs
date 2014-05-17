using System;
using System.Reflection.Emit;

namespace EmitDebugging
{
	internal sealed class CodeLine
	{
		private string code;

		private CodeLine()
			: base()
		{
			this.Labels = new Label[] { };
		}

		internal CodeLine(string code)
			: this()
		{
			if (code == null)
			{
				throw new ArgumentNullException("code");
			}

			this.code = code;
		}

		internal CodeLine(string code, Indention indent)
			: this(code, indent, false) { }

		internal CodeLine(string code, bool isDebuggable)
			: this(code)
		{
			this.IsDebuggable = isDebuggable;
		}

		internal CodeLine(string code, Indention indent, bool isDebuggable)
			: this(code, isDebuggable)
		{
			this.Indent = indent;
		}

		internal CodeLine(string code, Indention indent, params Label[] labels)
			: this(code, indent, false, labels) { }

		internal CodeLine(string code, Indention indent, bool isDebuggable, params Label[] labels)
			: this(code, indent, isDebuggable)
		{
			this.Labels = labels;
		}

		internal string Code
		{
			get
			{
				return this.code;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}

				this.code = value;
			}
		}

		internal Indention Indent
		{
			get;
			private set;
		}

		internal bool IsDebuggable
		{
			get;
			private set;
		}

		internal Label[] Labels
		{
			get;
			private set;
		}
	}
}


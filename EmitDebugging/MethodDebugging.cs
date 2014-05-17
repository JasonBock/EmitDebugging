using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.SymbolStore;
using System.Reflection.Emit;

namespace EmitDebugging
{
	[SuppressMessage("Microsoft.Interoperability", "CA1405:ComVisibleTypeBaseTypesShouldBeComVisible")]
	public sealed class MethodDebugging
		: MethodBaseDebugging<MethodBuilder>
	{
		internal MethodDebugging(MethodBuilder method, CodeDocument document,
			ISymbolDocumentWriter symbolDocument, Indention firstIndent)
			: base(method, method.GetILGenerator(), document, symbolDocument, firstIndent) { }
	}
}

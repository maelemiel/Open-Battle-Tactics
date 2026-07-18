using System.Runtime.InteropServices;

namespace System.Diagnostics.SymbolStore
{
	[ComVisible(true)]
	public interface ISymbolScope
	{
		int EndOffset { get; }

		ISymbolMethod Method { get; }

		ISymbolScope Parent { get; }

		int StartOffset { get; }

		ISymbolScope[] GetChildren();

		ISymbolVariable[] GetLocals();

		ISymbolNamespace[] GetNamespaces();
	}
}

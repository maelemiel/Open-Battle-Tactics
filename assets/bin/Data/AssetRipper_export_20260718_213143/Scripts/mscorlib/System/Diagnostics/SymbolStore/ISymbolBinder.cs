using System.Runtime.InteropServices;

namespace System.Diagnostics.SymbolStore
{
	[ComVisible(true)]
	public interface ISymbolBinder
	{
		[Obsolete("This interface is not 64-bit clean.  Use ISymbolBinder1 instead", false)]
		ISymbolReader GetReader(int importer, string filename, string searchPath);
	}
}

using System.Runtime.InteropServices;

namespace System.Diagnostics.SymbolStore
{
	[ComVisible(true)]
	public interface ISymbolDocumentWriter
	{
		void SetCheckSum(Guid algorithmId, byte[] checkSum);

		void SetSource(byte[] source);
	}
}

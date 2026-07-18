using System.Runtime.InteropServices;

namespace System.Collections
{
	[ComVisible(true)]
	public interface IDictionaryEnumerator : IEnumerator
	{
		DictionaryEntry Entry { get; }

		object Key { get; }

		object Value { get; }
	}
}

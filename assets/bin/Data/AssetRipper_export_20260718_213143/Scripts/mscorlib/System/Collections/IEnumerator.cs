using System.Runtime.InteropServices;

namespace System.Collections
{
	[ComVisible(true)]
	[Guid("496B0ABF-CDEE-11D3-88E8-00902754C43A")]
	public interface IEnumerator
	{
		object Current { get; }

		bool MoveNext();

		void Reset();
	}
}

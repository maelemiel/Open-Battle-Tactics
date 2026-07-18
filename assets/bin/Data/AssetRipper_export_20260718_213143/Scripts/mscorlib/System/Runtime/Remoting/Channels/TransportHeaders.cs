using System.Collections;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Channels
{
	[Serializable]
	[ComVisible(true)]
	[MonoTODO("Serialization format not compatible with .NET")]
	public class TransportHeaders : ITransportHeaders
	{
		private Hashtable hash_table;

		public object this[object key]
		{
			get
			{
				return hash_table[key];
			}
			set
			{
				hash_table[key] = value;
			}
		}

		public TransportHeaders()
		{
			hash_table = new Hashtable(CaseInsensitiveHashCodeProvider.DefaultInvariant, CaseInsensitiveComparer.DefaultInvariant);
		}

		public IEnumerator GetEnumerator()
		{
			return hash_table.GetEnumerator();
		}
	}
}

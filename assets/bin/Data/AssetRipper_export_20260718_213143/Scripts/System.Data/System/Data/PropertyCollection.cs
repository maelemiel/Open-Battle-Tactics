using System.Collections;
using System.Runtime.Serialization;

namespace System.Data
{
	[Serializable]
	public class PropertyCollection : Hashtable
	{
		public PropertyCollection()
		{
		}

		protected PropertyCollection(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}

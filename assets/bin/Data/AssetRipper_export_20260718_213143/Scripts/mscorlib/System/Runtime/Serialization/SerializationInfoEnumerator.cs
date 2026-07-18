using System.Collections;
using System.Runtime.InteropServices;

namespace System.Runtime.Serialization
{
	[ComVisible(true)]
	public sealed class SerializationInfoEnumerator : IEnumerator
	{
		private IEnumerator enumerator;

		object IEnumerator.Current
		{
			get
			{
				return enumerator.Current;
			}
		}

		public SerializationEntry Current
		{
			get
			{
				return (SerializationEntry)enumerator.Current;
			}
		}

		public string Name
		{
			get
			{
				return Current.Name;
			}
		}

		public Type ObjectType
		{
			get
			{
				return Current.ObjectType;
			}
		}

		public object Value
		{
			get
			{
				return Current.Value;
			}
		}

		internal SerializationInfoEnumerator(ArrayList list)
		{
			enumerator = list.GetEnumerator();
		}

		public bool MoveNext()
		{
			return enumerator.MoveNext();
		}

		public void Reset()
		{
			enumerator.Reset();
		}
	}
}

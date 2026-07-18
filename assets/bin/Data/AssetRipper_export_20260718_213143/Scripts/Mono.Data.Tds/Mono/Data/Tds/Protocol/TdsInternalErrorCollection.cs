using System.Collections;

namespace Mono.Data.Tds.Protocol
{
	public sealed class TdsInternalErrorCollection : IEnumerable
	{
		private ArrayList list;

		public int Count
		{
			get
			{
				return list.Count;
			}
		}

		public TdsInternalError this[int index]
		{
			get
			{
				return (TdsInternalError)list[index];
			}
			set
			{
				list[index] = value;
			}
		}

		public TdsInternalErrorCollection()
		{
			list = new ArrayList();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return list.GetEnumerator();
		}

		public int Add(TdsInternalError error)
		{
			return list.Add(error);
		}

		public void Clear()
		{
			list.Clear();
		}
	}
}

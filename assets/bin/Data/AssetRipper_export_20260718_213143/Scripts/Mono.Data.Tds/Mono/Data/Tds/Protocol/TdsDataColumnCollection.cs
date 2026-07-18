using System.Collections;

namespace Mono.Data.Tds.Protocol
{
	public class TdsDataColumnCollection : IEnumerable
	{
		private ArrayList list;

		public TdsDataColumn this[int index]
		{
			get
			{
				return (TdsDataColumn)list[index];
			}
			set
			{
				list[index] = value;
			}
		}

		public int Count
		{
			get
			{
				return list.Count;
			}
		}

		public TdsDataColumnCollection()
		{
			list = new ArrayList();
		}

		public int Add(TdsDataColumn schema)
		{
			int num = list.Add(schema);
			schema.ColumnOrdinal = num;
			return num;
		}

		public void Add(TdsDataColumnCollection columns)
		{
			foreach (TdsDataColumn column in columns)
			{
				Add(column);
			}
		}

		public IEnumerator GetEnumerator()
		{
			return list.GetEnumerator();
		}

		public void Clear()
		{
			list.Clear();
		}
	}
}

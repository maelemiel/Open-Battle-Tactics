using System.Runtime.InteropServices;

namespace System.Collections
{
	[ComVisible(true)]
	public interface IList : IEnumerable, ICollection
	{
		bool IsFixedSize { get; }

		bool IsReadOnly { get; }

		object this[int index] { get; set; }

		int Add(object value);

		void Clear();

		bool Contains(object value);

		int IndexOf(object value);

		void Insert(int index, object value);

		void Remove(object value);

		void RemoveAt(int index);
	}
}

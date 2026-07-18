using System.Runtime.InteropServices;

namespace System.Collections
{
	[ComVisible(true)]
	public interface IDictionary : IEnumerable, ICollection
	{
		bool IsFixedSize { get; }

		bool IsReadOnly { get; }

		object this[object key] { get; set; }

		ICollection Keys { get; }

		ICollection Values { get; }

		void Add(object key, object value);

		void Clear();

		bool Contains(object key);

		new IDictionaryEnumerator GetEnumerator();

		void Remove(object key);
	}
}

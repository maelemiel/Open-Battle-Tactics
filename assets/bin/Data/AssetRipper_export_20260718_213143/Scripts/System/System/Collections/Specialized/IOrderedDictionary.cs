namespace System.Collections.Specialized
{
	public interface IOrderedDictionary : ICollection, IDictionary, IEnumerable
	{
		object this[int idx] { get; set; }

		new IDictionaryEnumerator GetEnumerator();

		void Insert(int idx, object key, object value);

		void RemoveAt(int idx);
	}
}

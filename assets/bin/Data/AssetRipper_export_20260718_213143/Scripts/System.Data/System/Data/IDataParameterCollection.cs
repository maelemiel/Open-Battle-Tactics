using System.Collections;

namespace System.Data
{
	public interface IDataParameterCollection : IList, ICollection, IEnumerable
	{
		object this[string parameterName] { get; set; }

		void RemoveAt(string parameterName);

		int IndexOf(string parameterName);

		bool Contains(string parameterName);
	}
}

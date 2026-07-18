using System.Collections;
using System.Collections.Generic;

namespace System.Linq
{
	public interface IGrouping<TKey, TElement> : IEnumerable, IEnumerable<TElement>
	{
		TKey Key { get; }
	}
}

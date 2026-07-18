using System.Collections.Generic;

namespace System.Linq
{
	internal class SortSequenceContext<TElement, TKey> : SortContext<TElement>
	{
		private Func<TElement, TKey> selector;

		private IComparer<TKey> comparer;

		private TKey[] keys;

		public SortSequenceContext(Func<TElement, TKey> selector, IComparer<TKey> comparer, SortDirection direction, SortContext<TElement> child_context)
			: base(direction, child_context)
		{
			this.selector = selector;
			this.comparer = comparer;
		}

		public override void Initialize(TElement[] elements)
		{
			if (child_context != null)
			{
				child_context.Initialize(elements);
			}
			keys = new TKey[elements.Length];
			for (int i = 0; i < keys.Length; i++)
			{
				keys[i] = selector(elements[i]);
			}
		}

		public override int Compare(int first_index, int second_index)
		{
			int num = comparer.Compare(keys[first_index], keys[second_index]);
			if (num == 0)
			{
				if (child_context != null)
				{
					return child_context.Compare(first_index, second_index);
				}
				num = ((direction != SortDirection.Descending) ? (first_index - second_index) : (second_index - first_index));
			}
			return (direction != SortDirection.Descending) ? num : (-num);
		}
	}
}

using System.Collections.Generic;

namespace System.Linq
{
	internal class QuickSort<TElement>
	{
		private TElement[] elements;

		private int[] indexes;

		private SortContext<TElement> context;

		private QuickSort(IEnumerable<TElement> source, SortContext<TElement> context)
		{
			elements = source.ToArray();
			indexes = CreateIndexes(elements.Length);
			this.context = context;
		}

		private static int[] CreateIndexes(int length)
		{
			int[] array = new int[length];
			for (int i = 0; i < length; i++)
			{
				array[i] = i;
			}
			return array;
		}

		private void PerformSort()
		{
			if (elements.Length > 1)
			{
				context.Initialize(elements);
				Sort(0, indexes.Length - 1);
			}
		}

		private int CompareItems(int first_index, int second_index)
		{
			return context.Compare(first_index, second_index);
		}

		private int MedianOfThree(int left, int right)
		{
			int num = (left + right) / 2;
			if (CompareItems(indexes[num], indexes[left]) < 0)
			{
				Swap(left, num);
			}
			if (CompareItems(indexes[right], indexes[left]) < 0)
			{
				Swap(left, right);
			}
			if (CompareItems(indexes[right], indexes[num]) < 0)
			{
				Swap(num, right);
			}
			Swap(num, right - 1);
			return indexes[right - 1];
		}

		private void Sort(int left, int right)
		{
			if (left + 3 <= right)
			{
				int num = left;
				int num2 = right - 1;
				int second_index = MedianOfThree(left, right);
				while (true)
				{
					if (CompareItems(indexes[++num], second_index) >= 0)
					{
						while (CompareItems(indexes[--num2], second_index) > 0)
						{
						}
						if (num >= num2)
						{
							break;
						}
						Swap(num, num2);
					}
				}
				Swap(num, right - 1);
				Sort(left, num - 1);
				Sort(num + 1, right);
			}
			else
			{
				InsertionSort(left, right);
			}
		}

		private void InsertionSort(int left, int right)
		{
			for (int i = left + 1; i <= right; i++)
			{
				int num = indexes[i];
				int num2 = i;
				while (num2 > left && CompareItems(num, indexes[num2 - 1]) < 0)
				{
					indexes[num2] = indexes[num2 - 1];
					num2--;
				}
				indexes[num2] = num;
			}
		}

		private void Swap(int left, int right)
		{
			int num = indexes[right];
			indexes[right] = indexes[left];
			indexes[left] = num;
		}

		public static IEnumerable<TElement> Sort(IEnumerable<TElement> source, SortContext<TElement> context)
		{
			QuickSort<TElement> sorter = new QuickSort<TElement>(source, context);
			sorter.PerformSort();
			for (int i = 0; i < sorter.indexes.Length; i++)
			{
				yield return sorter.elements[sorter.indexes[i]];
			}
		}
	}
}

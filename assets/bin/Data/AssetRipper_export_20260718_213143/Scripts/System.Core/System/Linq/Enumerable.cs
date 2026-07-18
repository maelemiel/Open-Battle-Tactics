using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace System.Linq
{
	public static class Enumerable
	{
		private enum Fallback
		{
			Default = 0,
			Throw = 1
		}

		private class PredicateOf<T>
		{
			public static readonly Func<T, bool> Always = (T t) => true;
		}

		private class Function<T>
		{
			public static readonly Func<T, T> Identity = (T t) => t;
		}

		private class ReadOnlyCollectionOf<T>
		{
			public static readonly ReadOnlyCollection<T> Empty = new ReadOnlyCollection<T>(new T[0]);
		}

		public static TSource Aggregate<TSource>(this IEnumerable<TSource> source, Func<TSource, TSource, TSource> func)
		{
			Check.SourceAndFunc(source, func);
			using (IEnumerator<TSource> enumerator = source.GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					throw new InvalidOperationException("No elements in source list");
				}
				TSource val = enumerator.Current;
				while (enumerator.MoveNext())
				{
					val = func(val, enumerator.Current);
				}
				return val;
			}
		}

		public static TAccumulate Aggregate<TSource, TAccumulate>(this IEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func)
		{
			Check.SourceAndFunc(source, func);
			TAccumulate val = seed;
			foreach (TSource item in source)
			{
				val = func(val, item);
			}
			return val;
		}

		public static TResult Aggregate<TSource, TAccumulate, TResult>(this IEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func, Func<TAccumulate, TResult> resultSelector)
		{
			Check.SourceAndFunc(source, func);
			if (resultSelector == null)
			{
				throw new ArgumentNullException("resultSelector");
			}
			TAccumulate arg = seed;
			foreach (TSource item in source)
			{
				arg = func(arg, item);
			}
			return resultSelector(arg);
		}

		public static bool All<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			Check.SourceAndPredicate(source, predicate);
			foreach (TSource item in source)
			{
				if (!predicate(item))
				{
					return false;
				}
			}
			return true;
		}

		public static bool Any<TSource>(this IEnumerable<TSource> source)
		{
			Check.Source(source);
			ICollection<TSource> collection = source as ICollection<TSource>;
			if (collection != null)
			{
				return collection.Count > 0;
			}
			using (IEnumerator<TSource> enumerator = source.GetEnumerator())
			{
				return enumerator.MoveNext();
			}
		}

		public static bool Any<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			Check.SourceAndPredicate(source, predicate);
			foreach (TSource item in source)
			{
				if (predicate(item))
				{
					return true;
				}
			}
			return false;
		}

		public static IEnumerable<TSource> AsEnumerable<TSource>(this IEnumerable<TSource> source)
		{
			return source;
		}

		public static double Average(this IEnumerable<int> source)
		{
			return source.Average((long a, int b) => a + b, (long a, long b) => (double)a / (double)b);
		}

		public static double Average(this IEnumerable<long> source)
		{
			return source.Average((long a, long b) => a + b, (long a, long b) => (double)a / (double)b);
		}

		public static double Average(this IEnumerable<double> source)
		{
			return source.Average((double a, double b) => a + b, (double a, long b) => a / (double)b);
		}

		public static float Average(this IEnumerable<float> source)
		{
			return source.Average((double a, float b) => a + (double)b, (double a, long b) => (float)a / (float)b);
		}

		public static decimal Average(this IEnumerable<decimal> source)
		{
			return source.Average((decimal a, decimal b) => a + b, (decimal a, long b) => a / (decimal)b);
		}

		private static TResult Average<TElement, TAggregate, TResult>(this IEnumerable<TElement> source, Func<TAggregate, TElement, TAggregate> func, Func<TAggregate, long, TResult> result) where TElement : struct where TAggregate : struct where TResult : struct
		{
			Check.Source(source);
			TAggregate arg = default(TAggregate);
			long num = 0L;
			foreach (TElement item in source)
			{
				arg = func(arg, item);
				num++;
			}
			if (num == 0L)
			{
				throw new InvalidOperationException();
			}
			return result(arg, num);
		}

		private static TResult? AverageNullable<TElement, TAggregate, TResult>(this IEnumerable<TElement?> source, Func<TAggregate, TElement, TAggregate> func, Func<TAggregate, long, TResult> result) where TElement : struct where TAggregate : struct where TResult : struct
		{
			Check.Source(source);
			TAggregate arg = default(TAggregate);
			long num = 0L;
			foreach (TElement? item in source)
			{
				if (item.HasValue)
				{
					arg = func(arg, item.Value);
					num++;
				}
			}
			if (num == 0L)
			{
				return null;
			}
			return result(arg, num);
		}

		public static double? Average(this IEnumerable<int?> source)
		{
			Check.Source(source);
			return source.AverageNullable((long a, int b) => a + b, (long a, long b) => (double)a / (double)b);
		}

		public static double? Average(this IEnumerable<long?> source)
		{
			Check.Source(source);
			return source.AverageNullable((long a, long b) => a + b, (long a, long b) => (double)a / (double)b);
		}

		public static double? Average(this IEnumerable<double?> source)
		{
			Check.Source(source);
			return source.AverageNullable((double a, double b) => a + b, (double a, long b) => a / (double)b);
		}

		public static decimal? Average(this IEnumerable<decimal?> source)
		{
			Check.Source(source);
			return source.AverageNullable((decimal a, decimal b) => a + b, (decimal a, long b) => a / (decimal)b);
		}

		public static float? Average(this IEnumerable<float?> source)
		{
			Check.Source(source);
			return source.AverageNullable((double a, float b) => a + (double)b, (double a, long b) => (float)a / (float)b);
		}

		public static double Average<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
		{
			Check.SourceAndSelector(source, selector);
			return source.Select(selector).Average((long a, int b) => a + b, (long a, long b) => (double)a / (double)b);
		}

		public static double? Average<TSource>(this IEnumerable<TSource> source, Func<TSource, int?> selector)
		{
			Check.SourceAndSelector(source, selector);
			return source.Select(selector).AverageNullable((long a, int b) => a + b, (long a, long b) => (double)a / (double)b);
		}

		public static double Average<TSource>(this IEnumerable<TSource> source, Func<TSource, long> selector)
		{
			Check.SourceAndSelector(source, selector);
			return source.Select(selector).Average((long a, long b) => a + b, (long a, long b) => (double)a / (double)b);
		}

		public static double? Average<TSource>(this IEnumerable<TSource> source, Func<TSource, long?> selector)
		{
			Check.SourceAndSelector(source, selector);
			return source.Select(selector).AverageNullable((long a, long b) => a + b, (long a, long b) => (double)a / (double)b);
		}

		public static double Average<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector)
		{
			Check.SourceAndSelector(source, selector);
			return source.Select(selector).Average((double a, double b) => a + b, (double a, long b) => a / (double)b);
		}

		public static double? Average<TSource>(this IEnumerable<TSource> source, Func<TSource, double?> selector)
		{
			Check.SourceAndSelector(source, selector);
			return source.Select(selector).AverageNullable((double a, double b) => a + b, (double a, long b) => a / (double)b);
		}

		public static float Average<TSource>(this IEnumerable<TSource> source, Func<TSource, float> selector)
		{
			Check.SourceAndSelector(source, selector);
			return source.Select(selector).Average((double a, float b) => a + (double)b, (double a, long b) => (float)a / (float)b);
		}

		public static float? Average<TSource>(this IEnumerable<TSource> source, Func<TSource, float?> selector)
		{
			Check.SourceAndSelector(source, selector);
			return source.Select(selector).AverageNullable((double a, float b) => a + (double)b, (double a, long b) => (float)a / (float)b);
		}

		public static decimal Average<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector)
		{
			Check.SourceAndSelector(source, selector);
			return source.Select(selector).Average((decimal a, decimal b) => a + b, (decimal a, long b) => a / (decimal)b);
		}

		public static decimal? Average<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal?> selector)
		{
			Check.SourceAndSelector(source, selector);
			return source.Select(selector).AverageNullable((decimal a, decimal b) => a + b, (decimal a, long b) => a / (decimal)b);
		}

		public static IEnumerable<TResult> Cast<TResult>(this IEnumerable source)
		{
			Check.Source(source);
			IEnumerable<TResult> enumerable = source as IEnumerable<TResult>;
			if (enumerable != null)
			{
				return enumerable;
			}
			return CreateCastIterator<TResult>(source);
		}

		private static IEnumerable<TResult> CreateCastIterator<TResult>(IEnumerable source)
		{
			foreach (TResult item in source)
			{
				yield return item;
			}
		}

		public static IEnumerable<TSource> Concat<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
		{
			Check.FirstAndSecond(first, second);
			return CreateConcatIterator(first, second);
		}

		private static IEnumerable<TSource> CreateConcatIterator<TSource>(IEnumerable<TSource> first, IEnumerable<TSource> second)
		{
			foreach (TSource item in first)
			{
				yield return item;
			}
			foreach (TSource item2 in second)
			{
				yield return item2;
			}
		}

		public static bool Contains<TSource>(this IEnumerable<TSource> source, TSource value)
		{
			ICollection<TSource> collection = source as ICollection<TSource>;
			if (collection != null)
			{
				return collection.Contains(value);
			}
			return source.Contains(value, null);
		}

		public static bool Contains<TSource>(this IEnumerable<TSource> source, TSource value, IEqualityComparer<TSource> comparer)
		{
			Check.Source(source);
			if (comparer == null)
			{
				comparer = EqualityComparer<TSource>.Default;
			}
			foreach (TSource item in source)
			{
				if (comparer.Equals(item, value))
				{
					return true;
				}
			}
			return false;
		}

		public static int Count<TSource>(this IEnumerable<TSource> source)
		{
			Check.Source(source);
			ICollection<TSource> collection = source as ICollection<TSource>;
			if (collection != null)
			{
				return collection.Count;
			}
			int num = 0;
			using (IEnumerator<TSource> enumerator = source.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					num++;
				}
				return num;
			}
		}

		public static int Count<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> selector)
		{
			Check.SourceAndSelector(source, selector);
			int num = 0;
			foreach (TSource item in source)
			{
				if (selector(item))
				{
					num++;
				}
			}
			return num;
		}

		public static IEnumerable<TSource> DefaultIfEmpty<TSource>(this IEnumerable<TSource> source)
		{
			return source.DefaultIfEmpty(default(TSource));
		}

		public static IEnumerable<TSource> DefaultIfEmpty<TSource>(this IEnumerable<TSource> source, TSource defaultValue)
		{
			Check.Source(source);
			return CreateDefaultIfEmptyIterator(source, defaultValue);
		}

		private static IEnumerable<TSource> CreateDefaultIfEmptyIterator<TSource>(IEnumerable<TSource> source, TSource defaultValue)
		{
			bool empty = true;
			foreach (TSource item in source)
			{
				empty = false;
				yield return item;
			}
			if (empty)
			{
				yield return defaultValue;
			}
		}

		public static IEnumerable<TSource> Distinct<TSource>(this IEnumerable<TSource> source)
		{
			return source.Distinct(null);
		}

		public static IEnumerable<TSource> Distinct<TSource>(this IEnumerable<TSource> source, IEqualityComparer<TSource> comparer)
		{
			Check.Source(source);
			if (comparer == null)
			{
				comparer = EqualityComparer<TSource>.Default;
			}
			return CreateDistinctIterator(source, comparer);
		}

		private static IEnumerable<TSource> CreateDistinctIterator<TSource>(IEnumerable<TSource> source, IEqualityComparer<TSource> comparer)
		{
			HashSet<TSource> items = new HashSet<TSource>(comparer);
			foreach (TSource element in source)
			{
				if (!items.Contains(element))
				{
					items.Add(element);
					yield return element;
				}
			}
		}

		private static TSource ElementAt<TSource>(this IEnumerable<TSource> source, int index, Fallback fallback)
		{
			long num = 0L;
			foreach (TSource item in source)
			{
				if (index == num++)
				{
					return item;
				}
			}
			if (fallback == Fallback.Throw)
			{
				throw new ArgumentOutOfRangeException();
			}
			return default(TSource);
		}

		public static TSource ElementAt<TSource>(this IEnumerable<TSource> source, int index)
		{
			Check.Source(source);
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			IList<TSource> list = source as IList<TSource>;
			if (list != null)
			{
				return list[index];
			}
			return source.ElementAt(index, Fallback.Throw);
		}

		public static TSource ElementAtOrDefault<TSource>(this IEnumerable<TSource> source, int index)
		{
			Check.Source(source);
			if (index < 0)
			{
				return default(TSource);
			}
			IList<TSource> list = source as IList<TSource>;
			if (list != null)
			{
				return (index >= list.Count) ? default(TSource) : list[index];
			}
			return source.ElementAt(index, Fallback.Default);
		}

		public static IEnumerable<TResult> Empty<TResult>()
		{
			return new TResult[0];
		}

		public static IEnumerable<TSource> Except<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
		{
			return first.Except(second, null);
		}

		public static IEnumerable<TSource> Except<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
		{
			Check.FirstAndSecond(first, second);
			if (comparer == null)
			{
				comparer = EqualityComparer<TSource>.Default;
			}
			return CreateExceptIterator(first, second, comparer);
		}

		private static IEnumerable<TSource> CreateExceptIterator<TSource>(IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
		{
			HashSet<TSource> items = new HashSet<TSource>(second, comparer);
			foreach (TSource element in first)
			{
				if (!items.Contains(element, comparer))
				{
					yield return element;
				}
			}
		}

		private static TSource First<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, Fallback fallback)
		{
			foreach (TSource item in source)
			{
				if (predicate(item))
				{
					return item;
				}
			}
			if (fallback == Fallback.Throw)
			{
				throw new InvalidOperationException();
			}
			return default(TSource);
		}

		public static TSource First<TSource>(this IEnumerable<TSource> source)
		{
			Check.Source(source);
			IList<TSource> list = source as IList<TSource>;
			if (list != null)
			{
				if (list.Count != 0)
				{
					return list[0];
				}
				throw new InvalidOperationException();
			}
			using (IEnumerator<TSource> enumerator = source.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					return enumerator.Current;
				}
			}
			throw new InvalidOperationException();
		}

		public static TSource First<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			Check.SourceAndPredicate(source, predicate);
			return source.First(predicate, Fallback.Throw);
		}

		public static TSource FirstOrDefault<TSource>(this IEnumerable<TSource> source)
		{
			Check.Source(source);
			return source.First(PredicateOf<TSource>.Always, Fallback.Default);
		}

		public static TSource FirstOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			Check.SourceAndPredicate(source, predicate);
			return source.First(predicate, Fallback.Default);
		}

		private static List<T> ContainsGroup<K, T>(Dictionary<K, List<T>> items, K key, IEqualityComparer<K> comparer)
		{
			IEqualityComparer<K> equalityComparer = comparer ?? EqualityComparer<K>.Default;
			foreach (KeyValuePair<K, List<T>> item in items)
			{
				if (equalityComparer.Equals(item.Key, key))
				{
					return item.Value;
				}
			}
			return null;
		}

		public static IEnumerable<IGrouping<TKey, TSource>> GroupBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			return source.GroupBy(keySelector, null);
		}

		public static IEnumerable<IGrouping<TKey, TSource>> GroupBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
		{
			Check.SourceAndKeySelector(source, keySelector);
			return source.CreateGroupByIterator(keySelector, comparer);
		}

		private static IEnumerable<IGrouping<TKey, TSource>> CreateGroupByIterator<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
		{
			Dictionary<TKey, List<TSource>> groups = new Dictionary<TKey, List<TSource>>();
			List<TSource> nullList = new List<TSource>();
			int counter = 0;
			int nullCounter = -1;
			foreach (TSource element in source)
			{
				TKey key = keySelector(element);
				if (key == null)
				{
					nullList.Add(element);
					if (nullCounter == -1)
					{
						nullCounter = counter;
						counter++;
					}
					continue;
				}
				List<TSource> group = ContainsGroup(groups, key, comparer);
				if (group == null)
				{
					group = new List<TSource>();
					groups.Add(key, group);
					counter++;
				}
				group.Add(element);
			}
			counter = 0;
			foreach (KeyValuePair<TKey, List<TSource>> group2 in groups)
			{
				if (counter == nullCounter)
				{
					yield return new Grouping<TKey, TSource>(default(TKey), nullList);
					counter++;
				}
				yield return new Grouping<TKey, TSource>(group2.Key, group2.Value);
				counter++;
			}
			if (counter == nullCounter)
			{
				yield return new Grouping<TKey, TSource>(default(TKey), nullList);
				counter++;
			}
		}

		public static IEnumerable<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
		{
			return source.GroupBy(keySelector, elementSelector, null);
		}

		public static IEnumerable<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
		{
			Check.SourceAndKeyElementSelectors(source, keySelector, elementSelector);
			return source.CreateGroupByIterator(keySelector, elementSelector, comparer);
		}

		private static IEnumerable<IGrouping<TKey, TElement>> CreateGroupByIterator<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
		{
			Dictionary<TKey, List<TElement>> groups = new Dictionary<TKey, List<TElement>>();
			List<TElement> nullList = new List<TElement>();
			int counter = 0;
			int nullCounter = -1;
			foreach (TSource item in source)
			{
				TKey key = keySelector(item);
				TElement element = elementSelector(item);
				if (key == null)
				{
					nullList.Add(element);
					if (nullCounter == -1)
					{
						nullCounter = counter;
						counter++;
					}
					continue;
				}
				List<TElement> group = ContainsGroup(groups, key, comparer);
				if (group == null)
				{
					group = new List<TElement>();
					groups.Add(key, group);
					counter++;
				}
				group.Add(element);
			}
			counter = 0;
			foreach (KeyValuePair<TKey, List<TElement>> group2 in groups)
			{
				if (counter == nullCounter)
				{
					yield return new Grouping<TKey, TElement>(default(TKey), nullList);
					counter++;
				}
				yield return new Grouping<TKey, TElement>(group2.Key, group2.Value);
				counter++;
			}
			if (counter == nullCounter)
			{
				yield return new Grouping<TKey, TElement>(default(TKey), nullList);
				counter++;
			}
		}

		public static IEnumerable<TResult> GroupBy<TSource, TKey, TElement, TResult>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector)
		{
			return source.GroupBy(keySelector, elementSelector, resultSelector, null);
		}

		public static IEnumerable<TResult> GroupBy<TSource, TKey, TElement, TResult>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
		{
			Check.GroupBySelectors(source, keySelector, elementSelector, resultSelector);
			return source.CreateGroupByIterator(keySelector, elementSelector, resultSelector, comparer);
		}

		private static IEnumerable<TResult> CreateGroupByIterator<TSource, TKey, TElement, TResult>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
		{
			IEnumerable<IGrouping<TKey, TElement>> groups = source.GroupBy(keySelector, elementSelector, comparer);
			foreach (IGrouping<TKey, TElement> group in groups)
			{
				yield return resultSelector(group.Key, group);
			}
		}

		public static IEnumerable<TResult> GroupBy<TSource, TKey, TResult>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, IEnumerable<TSource>, TResult> resultSelector)
		{
			return source.GroupBy(keySelector, resultSelector, null);
		}

		public static IEnumerable<TResult> GroupBy<TSource, TKey, TResult>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, IEnumerable<TSource>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
		{
			Check.SourceAndKeyResultSelectors(source, keySelector, resultSelector);
			return source.CreateGroupByIterator(keySelector, resultSelector, comparer);
		}

		private static IEnumerable<TResult> CreateGroupByIterator<TSource, TKey, TResult>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, IEnumerable<TSource>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
		{
			IEnumerable<IGrouping<TKey, TSource>> groups = source.GroupBy(keySelector, comparer);
			foreach (IGrouping<TKey, TSource> group in groups)
			{
				yield return resultSelector(group.Key, group);
			}
		}

		public static IEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
		{
			return outer.GroupJoin(inner, outerKeySelector, innerKeySelector, resultSelector, null);
		}

		public static IEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
		{
			Check.JoinSelectors(outer, inner, outerKeySelector, innerKeySelector, resultSelector);
			if (comparer == null)
			{
				comparer = EqualityComparer<TKey>.Default;
			}
			return outer.CreateGroupJoinIterator(inner, outerKeySelector, innerKeySelector, resultSelector, comparer);
		}

		private static IEnumerable<TResult> CreateGroupJoinIterator<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
		{
			ILookup<TKey, TInner> innerKeys = inner.ToLookup(innerKeySelector, comparer);
			foreach (TOuter element in outer)
			{
				TKey outerKey = outerKeySelector(element);
				if (innerKeys.Contains(outerKey))
				{
					yield return resultSelector(element, innerKeys[outerKey]);
				}
				else
				{
					yield return resultSelector(element, Empty<TInner>());
				}
			}
		}

		public static IEnumerable<TSource> Intersect<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
		{
			return first.Intersect(second, null);
		}

		public static IEnumerable<TSource> Intersect<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
		{
			Check.FirstAndSecond(first, second);
			if (comparer == null)
			{
				comparer = EqualityComparer<TSource>.Default;
			}
			return CreateIntersectIterator(first, second, comparer);
		}

		private static IEnumerable<TSource> CreateIntersectIterator<TSource>(IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
		{
			HashSet<TSource> items = new HashSet<TSource>(second, comparer);
			foreach (TSource element in first)
			{
				if (items.Remove(element))
				{
					yield return element;
				}
			}
		}

		public static IEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector, IEqualityComparer<TKey> comparer)
		{
			Check.JoinSelectors(outer, inner, outerKeySelector, innerKeySelector, resultSelector);
			if (comparer == null)
			{
				comparer = EqualityComparer<TKey>.Default;
			}
			return outer.CreateJoinIterator(inner, outerKeySelector, innerKeySelector, resultSelector, comparer);
		}

		private static IEnumerable<TResult> CreateJoinIterator<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector, IEqualityComparer<TKey> comparer)
		{
			ILookup<TKey, TInner> innerKeys = inner.ToLookup(innerKeySelector, comparer);
			foreach (TOuter element in outer)
			{
				TKey outerKey = outerKeySelector(element);
				if (!innerKeys.Contains(outerKey))
				{
					continue;
				}
				foreach (TInner innerElement in innerKeys[outerKey])
				{
					yield return resultSelector(element, innerElement);
				}
			}
		}

		public static IEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
		{
			return outer.Join(inner, outerKeySelector, innerKeySelector, resultSelector, null);
		}

		private static TSource Last<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, Fallback fallback)
		{
			bool flag = true;
			TSource result = default(TSource);
			foreach (TSource item in source)
			{
				if (predicate(item))
				{
					result = item;
					flag = false;
				}
			}
			if (!flag)
			{
				return result;
			}
			if (fallback == Fallback.Throw)
			{
				throw new InvalidOperationException();
			}
			return result;
		}

		public static TSource Last<TSource>(this IEnumerable<TSource> source)
		{
			Check.Source(source);
			ICollection<TSource> collection = source as ICollection<TSource>;
			if (collection != null && collection.Count == 0)
			{
				throw new InvalidOperationException();
			}
			IList<TSource> list = source as IList<TSource>;
			if (list != null)
			{
				return list[list.Count - 1];
			}
			return source.Last(PredicateOf<TSource>.Always, Fallback.Throw);
		}

		public static TSource Last<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			Check.SourceAndPredicate(source, predicate);
			return source.Last(predicate, Fallback.Throw);
		}

		public static TSource LastOrDefault<TSource>(this IEnumerable<TSource> source)
		{
			Check.Source(source);
			IList<TSource> list = source as IList<TSource>;
			if (list != null)
			{
				return (list.Count <= 0) ? default(TSource) : list[list.Count - 1];
			}
			return source.Last(PredicateOf<TSource>.Always, Fallback.Default);
		}

		public static TSource LastOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			Check.SourceAndPredicate(source, predicate);
			return source.Last(predicate, Fallback.Default);
		}

		public static long LongCount<TSource>(this IEnumerable<TSource> source)
		{
			Check.Source(source);
			long num = 0L;
			using (IEnumerator<TSource> enumerator = source.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					num++;
				}
				return num;
			}
		}

		public static long LongCount<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> selector)
		{
			Check.SourceAndSelector(source, selector);
			long num = 0L;
			foreach (TSource item in source)
			{
				if (selector(item))
				{
					num++;
				}
			}
			return num;
		}

		public static int Max(this IEnumerable<int> source)
		{
			Check.Source(source);
			return Iterate(source, int.MinValue, (int a, int b) => Math.Max(a, b));
		}

		public static long Max(this IEnumerable<long> source)
		{
			Check.Source(source);
			return Iterate(source, long.MinValue, (long a, long b) => Math.Max(a, b));
		}

		public static double Max(this IEnumerable<double> source)
		{
			Check.Source(source);
			return Iterate(source, double.MinValue, (double a, double b) => Math.Max(a, b));
		}

		public static float Max(this IEnumerable<float> source)
		{
			Check.Source(source);
			return Iterate(source, float.MinValue, (float a, float b) => Math.Max(a, b));
		}

		public static decimal Max(this IEnumerable<decimal> source)
		{
			Check.Source(source);
			return Iterate(source, decimal.MinValue, (decimal a, decimal b) => Math.Max(a, b));
		}

		public static int? Max(this IEnumerable<int?> source)
		{
			Check.Source(source);
			return IterateNullable(source, (int a, int b) => Math.Max(a, b));
		}

		public static long? Max(this IEnumerable<long?> source)
		{
			Check.Source(source);
			return IterateNullable(source, (long a, long b) => Math.Max(a, b));
		}

		public static double? Max(this IEnumerable<double?> source)
		{
			Check.Source(source);
			return IterateNullable(source, (double a, double b) => Math.Max(a, b));
		}

		public static float? Max(this IEnumerable<float?> source)
		{
			Check.Source(source);
			return IterateNullable(source, (float a, float b) => Math.Max(a, b));
		}

		public static decimal? Max(this IEnumerable<decimal?> source)
		{
			Check.Source(source);
			return IterateNullable(source, (decimal a, decimal b) => Math.Max(a, b));
		}

		private static T? IterateNullable<T>(IEnumerable<T?> source, Func<T, T, T> selector) where T : struct
		{
			bool flag = true;
			T? result = null;
			foreach (T? item in source)
			{
				if (item.HasValue)
				{
					result = (result.HasValue ? new T?(selector(item.Value, result.Value)) : new T?(item.Value));
					flag = false;
				}
			}
			if (flag)
			{
				return null;
			}
			return result;
		}

		private static TRet? IterateNullable<TSource, TRet>(IEnumerable<TSource> source, Func<TSource, TRet?> source_selector, Func<TRet?, TRet?, bool> selector) where TRet : struct
		{
			bool flag = true;
			TRet? val = null;
			foreach (TSource item in source)
			{
				TRet? val2 = source_selector(item);
				if (!val.HasValue)
				{
					val = val2;
				}
				else if (selector(val2, val))
				{
					val = val2;
				}
				flag = false;
			}
			if (flag)
			{
				return null;
			}
			return val;
		}

		private static TSource IterateNullable<TSource>(IEnumerable<TSource> source, Func<TSource, TSource, bool> selector)
		{
			TSource val = default(TSource);
			foreach (TSource item in source)
			{
				if (item != null && (val == null || selector(item, val)))
				{
					val = item;
				}
			}
			return val;
		}

		private static TSource IterateNonNullable<TSource>(IEnumerable<TSource> source, Func<TSource, TSource, bool> selector)
		{
			TSource val = default(TSource);
			bool flag = true;
			foreach (TSource item in source)
			{
				if (flag)
				{
					val = item;
					flag = false;
				}
				else if (selector(item, val))
				{
					val = item;
				}
			}
			if (flag)
			{
				throw new InvalidOperationException();
			}
			return val;
		}

		public static TSource Max<TSource>(this IEnumerable<TSource> source)
		{
			Check.Source(source);
			Comparer<TSource> comparer = Comparer<TSource>.Default;
			Func<TSource, TSource, bool> selector = (TSource a, TSource b) => comparer.Compare(a, b) > 0;
			if (default(TSource) == null)
			{
				return IterateNullable(source, selector);
			}
			return IterateNonNullable(source, selector);
		}

		public static int Max<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
		{
			Check.SourceAndSelector(source, selector);
			return Iterate(source, int.MinValue, (TSource a, int b) => Math.Max(selector(a), b));
		}

		public static long Max<TSource>(this IEnumerable<TSource> source, Func<TSource, long> selector)
		{
			Check.SourceAndSelector(source, selector);
			return Iterate(source, long.MinValue, (TSource a, long b) => Math.Max(selector(a), b));
		}

		public static double Max<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector)
		{
			Check.SourceAndSelector(source, selector);
			return Iterate(source, double.MinValue, (TSource a, double b) => Math.Max(selector(a), b));
		}

		public static float Max<TSource>(this IEnumerable<TSource> source, Func<TSource, float> selector)
		{
			Check.SourceAndSelector(source, selector);
			return Iterate(source, float.MinValue, (TSource a, float b) => Math.Max(selector(a), b));
		}

		public static decimal Max<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector)
		{
			Check.SourceAndSelector(source, selector);
			return Iterate(source, decimal.MinValue, (TSource a, decimal b) => Math.Max(selector(a), b));
		}

		private static U Iterate<T, U>(IEnumerable<T> source, U initValue, Func<T, U, U> selector)
		{
			bool flag = true;
			foreach (T item in source)
			{
				initValue = selector(item, initValue);
				flag = false;
			}
			if (flag)
			{
				throw new InvalidOperationException();
			}
			return initValue;
		}

		public static int? Max<TSource>(this IEnumerable<TSource> source, Func<TSource, int?> selector)
		{
			Check.SourceAndSelector(source, selector);
			return IterateNullable(source, selector, (int? a, int? b) => a.HasValue && b.HasValue && a.Value > b.Value);
		}

		public static long? Max<TSource>(this IEnumerable<TSource> source, Func<TSource, long?> selector)
		{
			Check.SourceAndSelector(source, selector);
			return IterateNullable(source, selector, (long? a, long? b) => a.HasValue && b.HasValue && a.Value > b.Value);
		}

		public static double? Max<TSource>(this IEnumerable<TSource> source, Func<TSource, double?> selector)
		{
			Check.SourceAndSelector(source, selector);
			return IterateNullable(source, selector, (double? a, double? b) => a.HasValue && b.HasValue && a.Value > b.Value);
		}

		public static float? Max<TSource>(this IEnumerable<TSource> source, Func<TSource, float?> selector)
		{
			Check.SourceAndSelector(source, selector);
			return IterateNullable(source, selector, (float? a, float? b) => a.HasValue && b.HasValue && a.Value > b.Value);
		}

		public static decimal? Max<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal?> selector)
		{
			Check.SourceAndSelector(source, selector);
			return IterateNullable(source, selector, (decimal? a, decimal? b) => a.HasValue && b.HasValue && a.Value > b.Value);
		}

		public static TResult Max<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
		{
			Check.SourceAndSelector(source, selector);
			return source.Select(selector).Max();
		}

		public static int Min(this IEnumerable<int> source)
		{
			Check.Source(source);
			return Iterate(source, int.MaxValue, (int a, int b) => Math.Min(a, b));
		}

		public static long Min(this IEnumerable<long> source)
		{
			Check.Source(source);
			return Iterate(source, long.MaxValue, (long a, long b) => Math.Min(a, b));
		}

		public static double Min(this IEnumerable<double> source)
		{
			Check.Source(source);
			return Iterate(source, double.MaxValue, (double a, double b) => Math.Min(a, b));
		}

		public static float Min(this IEnumerable<float> source)
		{
			Check.Source(source);
			return Iterate(source, float.MaxValue, (float a, float b) => Math.Min(a, b));
		}

		public static decimal Min(this IEnumerable<decimal> source)
		{
			Check.Source(source);
			return Iterate(source, decimal.MaxValue, (decimal a, decimal b) => Math.Min(a, b));
		}

		public static int? Min(this IEnumerable<int?> source)
		{
			Check.Source(source);
			return IterateNullable(source, (int a, int b) => Math.Min(a, b));
		}

		public static long? Min(this IEnumerable<long?> source)
		{
			Check.Source(source);
			return IterateNullable(source, (long a, long b) => Math.Min(a, b));
		}

		public static double? Min(this IEnumerable<double?> source)
		{
			Check.Source(source);
			return IterateNullable(source, (double a, double b) => Math.Min(a, b));
		}

		public static float? Min(this IEnumerable<float?> source)
		{
			Check.Source(source);
			return IterateNullable(source, (float a, float b) => Math.Min(a, b));
		}

		public static decimal? Min(this IEnumerable<decimal?> source)
		{
			Check.Source(source);
			return IterateNullable(source, (decimal a, decimal b) => Math.Min(a, b));
		}

		public static TSource Min<TSource>(this IEnumerable<TSource> source)
		{
			Check.Source(source);
			Comparer<TSource> comparer = Comparer<TSource>.Default;
			Func<TSource, TSource, bool> selector = (TSource a, TSource b) => comparer.Compare(a, b) < 0;
			if (default(TSource) == null)
			{
				return IterateNullable(source, selector);
			}
			return IterateNonNullable(source, selector);
		}

		public static int Min<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
		{
			Check.SourceAndSelector(source, selector);
			return Iterate(source, int.MaxValue, (TSource a, int b) => Math.Min(selector(a), b));
		}

		public static long Min<TSource>(this IEnumerable<TSource> source, Func<TSource, long> selector)
		{
			Check.SourceAndSelector(source, selector);
			return Iterate(source, long.MaxValue, (TSource a, long b) => Math.Min(selector(a), b));
		}

		public static double Min<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector)
		{
			Check.SourceAndSelector(source, selector);
			return Iterate(source, double.MaxValue, (TSource a, double b) => Math.Min(selector(a), b));
		}

		public static float Min<TSource>(this IEnumerable<TSource> source, Func<TSource, float> selector)
		{
			Check.SourceAndSelector(source, selector);
			return Iterate(source, float.MaxValue, (TSource a, float b) => Math.Min(selector(a), b));
		}

		public static decimal Min<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector)
		{
			Check.SourceAndSelector(source, selector);
			return Iterate(source, decimal.MaxValue, (TSource a, decimal b) => Math.Min(selector(a), b));
		}

		public static int? Min<TSource>(this IEnumerable<TSource> source, Func<TSource, int?> selector)
		{
			Check.SourceAndSelector(source, selector);
			return IterateNullable(source, selector, (int? a, int? b) => a.HasValue && b.HasValue && a.Value < b.Value);
		}

		public static long? Min<TSource>(this IEnumerable<TSource> source, Func<TSource, long?> selector)
		{
			Check.SourceAndSelector(source, selector);
			return IterateNullable(source, selector, (long? a, long? b) => a.HasValue && b.HasValue && a.Value < b.Value);
		}

		public static float? Min<TSource>(this IEnumerable<TSource> source, Func<TSource, float?> selector)
		{
			Check.SourceAndSelector(source, selector);
			return IterateNullable(source, selector, (float? a, float? b) => a.HasValue && b.HasValue && a.Value < b.Value);
		}

		public static double? Min<TSource>(this IEnumerable<TSource> source, Func<TSource, double?> selector)
		{
			Check.SourceAndSelector(source, selector);
			return IterateNullable(source, selector, (double? a, double? b) => a.HasValue && b.HasValue && a.Value < b.Value);
		}

		public static decimal? Min<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal?> selector)
		{
			Check.SourceAndSelector(source, selector);
			return IterateNullable(source, selector, (decimal? a, decimal? b) => a.HasValue && b.HasValue && a.Value < b.Value);
		}

		public static TResult Min<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
		{
			Check.SourceAndSelector(source, selector);
			return source.Select(selector).Min();
		}

		public static IEnumerable<TResult> OfType<TResult>(this IEnumerable source)
		{
			Check.Source(source);
			return CreateOfTypeIterator<TResult>(source);
		}

		private static IEnumerable<TResult> CreateOfTypeIterator<TResult>(IEnumerable source)
		{
			foreach (object element in source)
			{
				if (element is TResult)
				{
					yield return (TResult)element;
				}
			}
		}

		public static IOrderedEnumerable<TSource> OrderBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			return source.OrderBy(keySelector, null);
		}

		public static IOrderedEnumerable<TSource> OrderBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
		{
			Check.SourceAndKeySelector(source, keySelector);
			return new OrderedSequence<TSource, TKey>(source, keySelector, comparer, SortDirection.Ascending);
		}

		public static IOrderedEnumerable<TSource> OrderByDescending<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			return source.OrderByDescending(keySelector, null);
		}

		public static IOrderedEnumerable<TSource> OrderByDescending<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
		{
			Check.SourceAndKeySelector(source, keySelector);
			return new OrderedSequence<TSource, TKey>(source, keySelector, comparer, SortDirection.Descending);
		}

		public static IEnumerable<int> Range(int start, int count)
		{
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			long num = (long)start + (long)count - 1;
			if (num > int.MaxValue)
			{
				throw new ArgumentOutOfRangeException();
			}
			return CreateRangeIterator(start, (int)num);
		}

		private static IEnumerable<int> CreateRangeIterator(int start, int upto)
		{
			for (int i = start; i <= upto; i++)
			{
				yield return i;
			}
		}

		public static IEnumerable<TResult> Repeat<TResult>(TResult element, int count)
		{
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			return CreateRepeatIterator(element, count);
		}

		private static IEnumerable<TResult> CreateRepeatIterator<TResult>(TResult element, int count)
		{
			for (int i = 0; i < count; i++)
			{
				yield return element;
			}
		}

		public static IEnumerable<TSource> Reverse<TSource>(this IEnumerable<TSource> source)
		{
			Check.Source(source);
			return CreateReverseIterator(source);
		}

		private static IEnumerable<TSource> CreateReverseIterator<TSource>(IEnumerable<TSource> source)
		{
			IList<TSource> list = source as IList<TSource>;
			if (list == null)
			{
				list = new List<TSource>(source);
			}
			for (int i = list.Count - 1; i >= 0; i--)
			{
				yield return list[i];
			}
		}

		public static IEnumerable<TResult> Select<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
		{
			Check.SourceAndSelector(source, selector);
			return CreateSelectIterator(source, selector);
		}

		private static IEnumerable<TResult> CreateSelectIterator<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, TResult> selector)
		{
			foreach (TSource element in source)
			{
				yield return selector(element);
			}
		}

		public static IEnumerable<TResult> Select<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, int, TResult> selector)
		{
			Check.SourceAndSelector(source, selector);
			return CreateSelectIterator(source, selector);
		}

		private static IEnumerable<TResult> CreateSelectIterator<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, int, TResult> selector)
		{
			int counter = 0;
			foreach (TSource element in source)
			{
				yield return selector(element, counter);
				counter++;
			}
		}

		public static IEnumerable<TResult> SelectMany<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, IEnumerable<TResult>> selector)
		{
			Check.SourceAndSelector(source, selector);
			return CreateSelectManyIterator(source, selector);
		}

		private static IEnumerable<TResult> CreateSelectManyIterator<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, IEnumerable<TResult>> selector)
		{
			foreach (TSource element in source)
			{
				foreach (TResult item in selector(element))
				{
					yield return item;
				}
			}
		}

		public static IEnumerable<TResult> SelectMany<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, int, IEnumerable<TResult>> selector)
		{
			Check.SourceAndSelector(source, selector);
			return CreateSelectManyIterator(source, selector);
		}

		private static IEnumerable<TResult> CreateSelectManyIterator<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, int, IEnumerable<TResult>> selector)
		{
			int counter = 0;
			foreach (TSource element in source)
			{
				foreach (TResult item in selector(element, counter))
				{
					yield return item;
				}
				counter++;
			}
		}

		public static IEnumerable<TResult> SelectMany<TSource, TCollection, TResult>(this IEnumerable<TSource> source, Func<TSource, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> selector)
		{
			Check.SourceAndCollectionSelectors(source, collectionSelector, selector);
			return CreateSelectManyIterator(source, collectionSelector, selector);
		}

		private static IEnumerable<TResult> CreateSelectManyIterator<TSource, TCollection, TResult>(IEnumerable<TSource> source, Func<TSource, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> selector)
		{
			foreach (TSource element in source)
			{
				foreach (TCollection collection in collectionSelector(element))
				{
					yield return selector(element, collection);
				}
			}
		}

		public static IEnumerable<TResult> SelectMany<TSource, TCollection, TResult>(this IEnumerable<TSource> source, Func<TSource, int, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> selector)
		{
			Check.SourceAndCollectionSelectors(source, collectionSelector, selector);
			return CreateSelectManyIterator(source, collectionSelector, selector);
		}

		private static IEnumerable<TResult> CreateSelectManyIterator<TSource, TCollection, TResult>(IEnumerable<TSource> source, Func<TSource, int, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> selector)
		{
			int counter = 0;
			foreach (TSource element in source)
			{
				foreach (TCollection collection in collectionSelector(element, counter++))
				{
					yield return selector(element, collection);
				}
			}
		}

		private static TSource Single<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, Fallback fallback)
		{
			bool flag = false;
			TSource result = default(TSource);
			foreach (TSource item in source)
			{
				if (predicate(item))
				{
					if (flag)
					{
						throw new InvalidOperationException();
					}
					flag = true;
					result = item;
				}
			}
			if (!flag && fallback == Fallback.Throw)
			{
				throw new InvalidOperationException();
			}
			return result;
		}

		public static TSource Single<TSource>(this IEnumerable<TSource> source)
		{
			Check.Source(source);
			return source.Single(PredicateOf<TSource>.Always, Fallback.Throw);
		}

		public static TSource Single<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			Check.SourceAndPredicate(source, predicate);
			return source.Single(predicate, Fallback.Throw);
		}

		public static TSource SingleOrDefault<TSource>(this IEnumerable<TSource> source)
		{
			Check.Source(source);
			return source.Single(PredicateOf<TSource>.Always, Fallback.Default);
		}

		public static TSource SingleOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			Check.SourceAndPredicate(source, predicate);
			return source.Single(predicate, Fallback.Default);
		}

		public static IEnumerable<TSource> Skip<TSource>(this IEnumerable<TSource> source, int count)
		{
			Check.Source(source);
			return CreateSkipIterator(source, count);
		}

		private static IEnumerable<TSource> CreateSkipIterator<TSource>(IEnumerable<TSource> source, int count)
		{
			IEnumerator<TSource> enumerator = source.GetEnumerator();
			try
			{
				while (count-- > 0)
				{
					if (!enumerator.MoveNext())
					{
						yield break;
					}
				}
				while (enumerator.MoveNext())
				{
					yield return enumerator.Current;
				}
			}
			finally
			{
				enumerator.Dispose();
			}
		}

		public static IEnumerable<TSource> SkipWhile<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			Check.SourceAndPredicate(source, predicate);
			return CreateSkipWhileIterator(source, predicate);
		}

		private static IEnumerable<TSource> CreateSkipWhileIterator<TSource>(IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			bool yield = false;
			foreach (TSource element in source)
			{
				if (yield)
				{
					yield return element;
				}
				else if (!predicate(element))
				{
					yield return element;
					yield = true;
				}
			}
		}

		public static IEnumerable<TSource> SkipWhile<TSource>(this IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
		{
			Check.SourceAndPredicate(source, predicate);
			return CreateSkipWhileIterator(source, predicate);
		}

		private static IEnumerable<TSource> CreateSkipWhileIterator<TSource>(IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
		{
			int counter = 0;
			bool yield = false;
			foreach (TSource element in source)
			{
				if (yield)
				{
					yield return element;
				}
				else if (!predicate(element, counter))
				{
					yield return element;
					yield = true;
				}
				counter++;
			}
		}

		public static int Sum(this IEnumerable<int> source)
		{
			Check.Source(source);
			return source.Sum((int a, int b) => checked(a + b));
		}

		public static int? Sum(this IEnumerable<int?> source)
		{
			Check.Source(source);
			return source.SumNullable(0, (int? total, int? element) => (!element.HasValue) ? total : ((!total.HasValue || !element.HasValue) ? ((int?)null) : new int?(checked(total.Value + element.Value))));
		}

		public static int Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
		{
			Check.SourceAndSelector(source, selector);
			return source.Sum((int a, TSource b) => checked(a + selector(b)));
		}

		public static int? Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, int?> selector)
		{
			Check.SourceAndSelector(source, selector);
			return source.SumNullable(0, delegate(int? a, TSource b)
			{
				int? num = selector(b);
				return (!num.HasValue) ? a : ((!a.HasValue) ? ((int?)null) : new int?(checked(a.Value + num.Value)));
			});
		}

		public static long Sum(this IEnumerable<long> source)
		{
			Check.Source(source);
			return source.Sum((long a, long b) => checked(a + b));
		}

		public static long? Sum(this IEnumerable<long?> source)
		{
			Check.Source(source);
			return source.SumNullable(0L, (long? total, long? element) => (!element.HasValue) ? total : ((!total.HasValue || !element.HasValue) ? ((long?)null) : new long?(checked(total.Value + element.Value))));
		}

		public static long Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, long> selector)
		{
			Check.SourceAndSelector(source, selector);
			return source.Sum((long a, TSource b) => checked(a + selector(b)));
		}

		public static long? Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, long?> selector)
		{
			Check.SourceAndSelector(source, selector);
			return source.SumNullable(0L, delegate(long? a, TSource b)
			{
				long? num = selector(b);
				return (!num.HasValue) ? a : ((!a.HasValue) ? ((long?)null) : new long?(checked(a.Value + num.Value)));
			});
		}

		public static double Sum(this IEnumerable<double> source)
		{
			Check.Source(source);
			return source.Sum((double a, double b) => a + b);
		}

		public static double? Sum(this IEnumerable<double?> source)
		{
			Check.Source(source);
			return source.SumNullable(0.0, (double? total, double? element) => (!element.HasValue) ? total : ((!total.HasValue || !element.HasValue) ? ((double?)null) : new double?(total.Value + element.Value)));
		}

		public static double Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector)
		{
			Check.SourceAndSelector(source, selector);
			return source.Sum((double a, TSource b) => a + selector(b));
		}

		public static double? Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, double?> selector)
		{
			Check.SourceAndSelector(source, selector);
			return source.SumNullable(0.0, delegate(double? a, TSource b)
			{
				double? num = selector(b);
				return (!num.HasValue) ? a : ((!a.HasValue) ? ((double?)null) : new double?(a.Value + num.Value));
			});
		}

		public static float Sum(this IEnumerable<float> source)
		{
			Check.Source(source);
			return source.Sum((float a, float b) => a + b);
		}

		public static float? Sum(this IEnumerable<float?> source)
		{
			Check.Source(source);
			return source.SumNullable(0f, (float? total, float? element) => (!element.HasValue) ? total : ((!total.HasValue || !element.HasValue) ? ((float?)null) : new float?(total.Value + element.Value)));
		}

		public static float Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, float> selector)
		{
			Check.SourceAndSelector(source, selector);
			return source.Sum((float a, TSource b) => a + selector(b));
		}

		public static float? Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, float?> selector)
		{
			Check.SourceAndSelector(source, selector);
			return source.SumNullable(0f, delegate(float? a, TSource b)
			{
				float? num = selector(b);
				return (!num.HasValue) ? a : ((!a.HasValue) ? ((float?)null) : new float?(a.Value + num.Value));
			});
		}

		public static decimal Sum(this IEnumerable<decimal> source)
		{
			Check.Source(source);
			return source.Sum((decimal a, decimal b) => a + b);
		}

		public static decimal? Sum(this IEnumerable<decimal?> source)
		{
			Check.Source(source);
			return source.SumNullable(0m, (decimal? total, decimal? element) => (!element.HasValue) ? total : ((!total.HasValue || !element.HasValue) ? ((decimal?)null) : new decimal?(total.Value + element.Value)));
		}

		public static decimal Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector)
		{
			Check.SourceAndSelector(source, selector);
			return source.Sum((decimal a, TSource b) => a + selector(b));
		}

		public static decimal? Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal?> selector)
		{
			Check.SourceAndSelector(source, selector);
			return source.SumNullable(0m, delegate(decimal? a, TSource b)
			{
				decimal? num = selector(b);
				return (!num.HasValue) ? a : ((!a.HasValue) ? ((decimal?)null) : new decimal?(a.Value + num.Value));
			});
		}

		private static TR Sum<TA, TR>(this IEnumerable<TA> source, Func<TR, TA, TR> selector)
		{
			TR val = default(TR);
			long num = 0L;
			foreach (TA item in source)
			{
				val = selector(val, item);
				num++;
			}
			return val;
		}

		private static TR SumNullable<TA, TR>(this IEnumerable<TA> source, TR zero, Func<TR, TA, TR> selector)
		{
			TR val = zero;
			foreach (TA item in source)
			{
				val = selector(val, item);
			}
			return val;
		}

		public static IEnumerable<TSource> Take<TSource>(this IEnumerable<TSource> source, int count)
		{
			Check.Source(source);
			return CreateTakeIterator(source, count);
		}

		private static IEnumerable<TSource> CreateTakeIterator<TSource>(IEnumerable<TSource> source, int count)
		{
			if (count <= 0)
			{
				yield break;
			}
			int counter = 0;
			foreach (TSource item in source)
			{
				yield return item;
				int num;
				counter = (num = counter + 1);
				if (num == count)
				{
					break;
				}
			}
		}

		public static IEnumerable<TSource> TakeWhile<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			Check.SourceAndPredicate(source, predicate);
			return CreateTakeWhileIterator(source, predicate);
		}

		private static IEnumerable<TSource> CreateTakeWhileIterator<TSource>(IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			foreach (TSource element in source)
			{
				if (!predicate(element))
				{
					break;
				}
				yield return element;
			}
		}

		public static IEnumerable<TSource> TakeWhile<TSource>(this IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
		{
			Check.SourceAndPredicate(source, predicate);
			return CreateTakeWhileIterator(source, predicate);
		}

		private static IEnumerable<TSource> CreateTakeWhileIterator<TSource>(IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
		{
			int counter = 0;
			foreach (TSource element in source)
			{
				if (!predicate(element, counter))
				{
					break;
				}
				yield return element;
				counter++;
			}
		}

		public static IOrderedEnumerable<TSource> ThenBy<TSource, TKey>(this IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			return source.ThenBy(keySelector, null);
		}

		public static IOrderedEnumerable<TSource> ThenBy<TSource, TKey>(this IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
		{
			Check.SourceAndKeySelector(source, keySelector);
			return source.CreateOrderedEnumerable(keySelector, comparer, false);
		}

		public static IOrderedEnumerable<TSource> ThenByDescending<TSource, TKey>(this IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			return source.ThenByDescending(keySelector, null);
		}

		public static IOrderedEnumerable<TSource> ThenByDescending<TSource, TKey>(this IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
		{
			Check.SourceAndKeySelector(source, keySelector);
			return source.CreateOrderedEnumerable(keySelector, comparer, true);
		}

		public static TSource[] ToArray<TSource>(this IEnumerable<TSource> source)
		{
			Check.Source(source);
			ICollection<TSource> collection = source as ICollection<TSource>;
			if (collection != null)
			{
				TSource[] array = new TSource[collection.Count];
				collection.CopyTo(array, 0);
				return array;
			}
			return new List<TSource>(source).ToArray();
		}

		public static Dictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
		{
			return source.ToDictionary(keySelector, elementSelector, null);
		}

		public static Dictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
		{
			Check.SourceAndKeyElementSelectors(source, keySelector, elementSelector);
			if (comparer == null)
			{
				comparer = EqualityComparer<TKey>.Default;
			}
			Dictionary<TKey, TElement> dictionary = new Dictionary<TKey, TElement>(comparer);
			foreach (TSource item in source)
			{
				dictionary.Add(keySelector(item), elementSelector(item));
			}
			return dictionary;
		}

		public static Dictionary<TKey, TSource> ToDictionary<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			return source.ToDictionary(keySelector, null);
		}

		public static Dictionary<TKey, TSource> ToDictionary<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
		{
			return source.ToDictionary(keySelector, Function<TSource>.Identity, comparer);
		}

		public static List<TSource> ToList<TSource>(this IEnumerable<TSource> source)
		{
			Check.Source(source);
			return new List<TSource>(source);
		}

		public static ILookup<TKey, TSource> ToLookup<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			return source.ToLookup(keySelector, Function<TSource>.Identity, null);
		}

		public static ILookup<TKey, TSource> ToLookup<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
		{
			return source.ToLookup(keySelector, (TSource element) => element, comparer);
		}

		public static ILookup<TKey, TElement> ToLookup<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
		{
			return source.ToLookup(keySelector, elementSelector, null);
		}

		public static ILookup<TKey, TElement> ToLookup<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
		{
			Check.SourceAndKeyElementSelectors(source, keySelector, elementSelector);
			List<TElement> list = null;
			Dictionary<TKey, List<TElement>> dictionary = new Dictionary<TKey, List<TElement>>(comparer ?? EqualityComparer<TKey>.Default);
			foreach (TSource item in source)
			{
				TKey val = keySelector(item);
				List<TElement> value;
				if (val == null)
				{
					if (list == null)
					{
						list = new List<TElement>();
					}
					value = list;
				}
				else if (!dictionary.TryGetValue(val, out value))
				{
					value = new List<TElement>();
					dictionary.Add(val, value);
				}
				value.Add(elementSelector(item));
			}
			return new Lookup<TKey, TElement>(dictionary, list);
		}

		public static bool SequenceEqual<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
		{
			return first.SequenceEqual(second, null);
		}

		public static bool SequenceEqual<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
		{
			Check.FirstAndSecond(first, second);
			if (comparer == null)
			{
				comparer = EqualityComparer<TSource>.Default;
			}
			using (IEnumerator<TSource> enumerator = first.GetEnumerator())
			{
				using (IEnumerator<TSource> enumerator2 = second.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (!enumerator2.MoveNext())
						{
							return false;
						}
						if (!comparer.Equals(enumerator.Current, enumerator2.Current))
						{
							return false;
						}
					}
					return !enumerator2.MoveNext();
				}
			}
		}

		public static IEnumerable<TSource> Union<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
		{
			Check.FirstAndSecond(first, second);
			return first.Union(second, null);
		}

		public static IEnumerable<TSource> Union<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
		{
			Check.FirstAndSecond(first, second);
			if (comparer == null)
			{
				comparer = EqualityComparer<TSource>.Default;
			}
			return CreateUnionIterator(first, second, comparer);
		}

		private static IEnumerable<TSource> CreateUnionIterator<TSource>(IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
		{
			HashSet<TSource> items = new HashSet<TSource>(comparer);
			foreach (TSource element in first)
			{
				if (!items.Contains(element))
				{
					items.Add(element);
					yield return element;
				}
			}
			foreach (TSource element2 in second)
			{
				if (!items.Contains(element2, comparer))
				{
					items.Add(element2);
					yield return element2;
				}
			}
		}

		public static IEnumerable<TSource> Where<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			Check.SourceAndPredicate(source, predicate);
			return CreateWhereIterator(source, predicate);
		}

		private static IEnumerable<TSource> CreateWhereIterator<TSource>(IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			foreach (TSource element in source)
			{
				if (predicate(element))
				{
					yield return element;
				}
			}
		}

		public static IEnumerable<TSource> Where<TSource>(this IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
		{
			Check.SourceAndPredicate(source, predicate);
			return source.CreateWhereIterator(predicate);
		}

		private static IEnumerable<TSource> CreateWhereIterator<TSource>(this IEnumerable<TSource> source, Func<TSource, int, bool> predicate)
		{
			int counter = 0;
			foreach (TSource element in source)
			{
				if (predicate(element, counter))
				{
					yield return element;
				}
				counter++;
			}
		}

		internal static ReadOnlyCollection<TSource> ToReadOnlyCollection<TSource>(this IEnumerable<TSource> source)
		{
			if (source == null)
			{
				return ReadOnlyCollectionOf<TSource>.Empty;
			}
			ReadOnlyCollection<TSource> readOnlyCollection = source as ReadOnlyCollection<TSource>;
			if (readOnlyCollection != null)
			{
				return readOnlyCollection;
			}
			return new ReadOnlyCollection<TSource>(source.ToArray());
		}
	}
}

namespace System.Xml
{
	public class NameTable : XmlNameTable
	{
		private class Entry
		{
			public string str;

			public int hash;

			public int len;

			public Entry next;

			public Entry(string str, int hash, Entry next)
			{
				this.str = str;
				len = str.Length;
				this.hash = hash;
				this.next = next;
			}
		}

		private const int INITIAL_BUCKETS = 128;

		private int count = 128;

		private Entry[] buckets = new Entry[128];

		private int size;

		public override string Add(char[] key, int start, int len)
		{
			if ((0 > start && start >= key.Length) || (0 > len && len >= key.Length - len))
			{
				throw new IndexOutOfRangeException("The Index is out of range.");
			}
			if (len == 0)
			{
				return string.Empty;
			}
			int num = 0;
			int num2 = start + len;
			for (int i = start; i < num2; i++)
			{
				num = (num << 5) - num + key[i];
			}
			num &= 0x7FFFFFFF;
			for (Entry entry = buckets[num % count]; entry != null; entry = entry.next)
			{
				if (entry.hash == num && entry.len == len && StrEqArray(entry.str, key, start))
				{
					return entry.str;
				}
			}
			return AddEntry(new string(key, start, len), num);
		}

		public override string Add(string key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			int length = key.Length;
			if (length == 0)
			{
				return string.Empty;
			}
			int num = 0;
			for (int i = 0; i < length; i++)
			{
				num = (num << 5) - num + key[i];
			}
			num &= 0x7FFFFFFF;
			for (Entry entry = buckets[num % count]; entry != null; entry = entry.next)
			{
				if (entry.hash == num && entry.len == key.Length && entry.str == key)
				{
					return entry.str;
				}
			}
			return AddEntry(key, num);
		}

		public override string Get(char[] key, int start, int len)
		{
			if ((0 > start && start >= key.Length) || (0 > len && len >= key.Length - len))
			{
				throw new IndexOutOfRangeException("The Index is out of range.");
			}
			if (len == 0)
			{
				return string.Empty;
			}
			int num = 0;
			int num2 = start + len;
			for (int i = start; i < num2; i++)
			{
				num = (num << 5) - num + key[i];
			}
			num &= 0x7FFFFFFF;
			for (Entry entry = buckets[num % count]; entry != null; entry = entry.next)
			{
				if (entry.hash == num && entry.len == len && StrEqArray(entry.str, key, start))
				{
					return entry.str;
				}
			}
			return null;
		}

		public override string Get(string value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			int length = value.Length;
			if (length == 0)
			{
				return string.Empty;
			}
			int num = 0;
			for (int i = 0; i < length; i++)
			{
				num = (num << 5) - num + value[i];
			}
			num &= 0x7FFFFFFF;
			for (Entry entry = buckets[num % count]; entry != null; entry = entry.next)
			{
				if (entry.hash == num && entry.len == value.Length && entry.str == value)
				{
					return entry.str;
				}
			}
			return null;
		}

		private string AddEntry(string str, int hash)
		{
			int num = hash % count;
			buckets[num] = new Entry(str, hash, buckets[num]);
			if (size++ == count)
			{
				count <<= 1;
				int num2 = count - 1;
				Entry[] array = new Entry[count];
				for (int i = 0; i < buckets.Length; i++)
				{
					Entry entry = buckets[i];
					Entry entry2 = entry;
					while (entry2 != null)
					{
						int num3 = entry2.hash & num2;
						Entry next = entry2.next;
						entry2.next = array[num3];
						array[num3] = entry2;
						entry2 = next;
					}
				}
				buckets = array;
			}
			return str;
		}

		private static bool StrEqArray(string str, char[] str2, int start)
		{
			int length = str.Length;
			length--;
			start += length;
			do
			{
				if (str[length] != str2[start])
				{
					return false;
				}
				length--;
				start--;
			}
			while (length >= 0);
			return true;
		}
	}
}

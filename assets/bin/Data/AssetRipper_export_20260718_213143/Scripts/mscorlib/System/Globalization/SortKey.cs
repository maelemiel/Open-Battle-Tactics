using System.Runtime.InteropServices;

namespace System.Globalization
{
	[Serializable]
	[ComVisible(true)]
	public class SortKey
	{
		private readonly string source;

		private readonly CompareOptions options;

		private readonly byte[] key;

		private readonly int lcid;

		public virtual string OriginalString
		{
			get
			{
				return source;
			}
		}

		public virtual byte[] KeyData
		{
			get
			{
				return key;
			}
		}

		internal SortKey(int lcid, string source, CompareOptions opt)
		{
			this.lcid = lcid;
			this.source = source;
			options = opt;
		}

		internal SortKey(int lcid, string source, byte[] buffer, CompareOptions opt, int lv1Length, int lv2Length, int lv3Length, int kanaSmallLength, int markTypeLength, int katakanaLength, int kanaWidthLength, int identLength)
		{
			this.lcid = lcid;
			this.source = source;
			key = buffer;
			options = opt;
		}

		public static int Compare(SortKey sortkey1, SortKey sortkey2)
		{
			if (sortkey1 == null)
			{
				throw new ArgumentNullException("sortkey1");
			}
			if (sortkey2 == null)
			{
				throw new ArgumentNullException("sortkey2");
			}
			if (object.ReferenceEquals(sortkey1, sortkey2) || object.ReferenceEquals(sortkey1.OriginalString, sortkey2.OriginalString))
			{
				return 0;
			}
			byte[] keyData = sortkey1.KeyData;
			byte[] keyData2 = sortkey2.KeyData;
			int num = ((keyData.Length <= keyData2.Length) ? keyData.Length : keyData2.Length);
			for (int i = 0; i < num; i++)
			{
				if (keyData[i] != keyData2[i])
				{
					return (keyData[i] >= keyData2[i]) ? 1 : (-1);
				}
			}
			return (keyData.Length != keyData2.Length) ? ((keyData.Length >= keyData2.Length) ? 1 : (-1)) : 0;
		}

		public override bool Equals(object value)
		{
			SortKey sortKey = value as SortKey;
			if (sortKey != null && lcid == sortKey.lcid && options == sortKey.options && Compare(this, sortKey) == 0)
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			if (key.Length == 0)
			{
				return 0;
			}
			int num = key[0];
			for (int i = 1; i < key.Length; i++)
			{
				num ^= key[i] << (i & 3);
			}
			return num;
		}

		public override string ToString()
		{
			return string.Concat("SortKey - ", lcid, ", ", options, ", ", source);
		}
	}
}

using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Collections
{
	[Serializable]
	[ComVisible(true)]
	public sealed class Comparer : ISerializable, IComparer
	{
		public static readonly Comparer Default = new Comparer();

		public static readonly Comparer DefaultInvariant = new Comparer(CultureInfo.InvariantCulture);

		private CompareInfo m_compareInfo;

		private Comparer()
		{
		}

		public Comparer(CultureInfo culture)
		{
			if (culture == null)
			{
				throw new ArgumentNullException("culture");
			}
			m_compareInfo = culture.CompareInfo;
		}

		public int Compare(object a, object b)
		{
			if (a == b)
			{
				return 0;
			}
			if (a == null)
			{
				return -1;
			}
			if (b == null)
			{
				return 1;
			}
			if (m_compareInfo != null)
			{
				string text = a as string;
				string text2 = b as string;
				if (text != null && text2 != null)
				{
					return m_compareInfo.Compare(text, text2);
				}
			}
			if (a is IComparable)
			{
				return (a as IComparable).CompareTo(b);
			}
			if (b is IComparable)
			{
				return -(b as IComparable).CompareTo(a);
			}
			throw new ArgumentException(Locale.GetText("Neither 'a' nor 'b' implements IComparable."));
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			info.AddValue("CompareInfo", m_compareInfo, typeof(CompareInfo));
		}
	}
}

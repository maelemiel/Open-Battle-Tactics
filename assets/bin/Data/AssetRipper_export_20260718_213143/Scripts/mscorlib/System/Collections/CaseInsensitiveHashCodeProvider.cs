using System.Globalization;
using System.Runtime.InteropServices;

namespace System.Collections
{
	[Serializable]
	[ComVisible(true)]
	[Obsolete("Please use StringComparer instead.")]
	public class CaseInsensitiveHashCodeProvider : IHashCodeProvider
	{
		private static readonly CaseInsensitiveHashCodeProvider singletonInvariant = new CaseInsensitiveHashCodeProvider(CultureInfo.InvariantCulture);

		private static CaseInsensitiveHashCodeProvider singleton;

		private static readonly object sync = new object();

		private TextInfo m_text;

		public static CaseInsensitiveHashCodeProvider Default
		{
			get
			{
				lock (sync)
				{
					if (singleton == null)
					{
						singleton = new CaseInsensitiveHashCodeProvider();
					}
					else if (singleton.m_text == null)
					{
						if (!AreEqual(CultureInfo.CurrentCulture, CultureInfo.InvariantCulture))
						{
							singleton = new CaseInsensitiveHashCodeProvider();
						}
					}
					else if (!AreEqual(singleton.m_text, CultureInfo.CurrentCulture))
					{
						singleton = new CaseInsensitiveHashCodeProvider();
					}
					return singleton;
				}
			}
		}

		public static CaseInsensitiveHashCodeProvider DefaultInvariant
		{
			get
			{
				return singletonInvariant;
			}
		}

		public CaseInsensitiveHashCodeProvider()
		{
			CultureInfo currentCulture = CultureInfo.CurrentCulture;
			if (!AreEqual(currentCulture, CultureInfo.InvariantCulture))
			{
				m_text = CultureInfo.CurrentCulture.TextInfo;
			}
		}

		public CaseInsensitiveHashCodeProvider(CultureInfo culture)
		{
			if (culture == null)
			{
				throw new ArgumentNullException("culture");
			}
			if (!AreEqual(culture, CultureInfo.InvariantCulture))
			{
				m_text = culture.TextInfo;
			}
		}

		private static bool AreEqual(CultureInfo a, CultureInfo b)
		{
			return a.Name == b.Name;
		}

		private static bool AreEqual(TextInfo info, CultureInfo culture)
		{
			return info.CultureName == culture.Name;
		}

		public int GetHashCode(object obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}
			string text = obj as string;
			if (text == null)
			{
				return obj.GetHashCode();
			}
			int num = 0;
			if (m_text != null && !AreEqual(m_text, CultureInfo.InvariantCulture))
			{
				text = m_text.ToLower(text);
				foreach (char c in text)
				{
					num = num * 31 + c;
				}
			}
			else
			{
				for (int j = 0; j < text.Length; j++)
				{
					char c = char.ToLower(text[j], CultureInfo.InvariantCulture);
					num = num * 31 + c;
				}
			}
			return num;
		}
	}
}

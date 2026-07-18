using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;

namespace System.Globalization
{
	[Serializable]
	[ComVisible(true)]
	[MonoTODO("IDeserializationCallback isn't implemented.")]
	public class TextInfo : ICloneable, IDeserializationCallback
	{
		private struct Data
		{
			public int ansi;

			public int ebcdic;

			public int mac;

			public int oem;

			public byte list_sep;
		}

		private string m_listSeparator;

		private bool m_isReadOnly;

		private string customCultureName;

		[NonSerialized]
		private int m_nDataItem;

		private bool m_useUserOverride;

		private int m_win32LangID;

		[NonSerialized]
		private readonly CultureInfo ci;

		[NonSerialized]
		private readonly bool handleDotI;

		[NonSerialized]
		private readonly Data data;

		public virtual int ANSICodePage
		{
			get
			{
				Data data = this.data;
				return data.ansi;
			}
		}

		public virtual int EBCDICCodePage
		{
			get
			{
				Data data = this.data;
				return data.ebcdic;
			}
		}

		[ComVisible(false)]
		public int LCID
		{
			get
			{
				return m_win32LangID;
			}
		}

		public virtual string ListSeparator
		{
			get
			{
				if (m_listSeparator == null)
				{
					Data data = this.data;
					m_listSeparator = ((char)data.list_sep).ToString();
				}
				return m_listSeparator;
			}
			[ComVisible(false)]
			set
			{
				m_listSeparator = value;
			}
		}

		public virtual int MacCodePage
		{
			get
			{
				Data data = this.data;
				return data.mac;
			}
		}

		public virtual int OEMCodePage
		{
			get
			{
				Data data = this.data;
				return data.oem;
			}
		}

		[ComVisible(false)]
		public string CultureName
		{
			get
			{
				if (customCultureName == null)
				{
					customCultureName = ci.Name;
				}
				return customCultureName;
			}
		}

		[ComVisible(false)]
		public bool IsReadOnly
		{
			get
			{
				return m_isReadOnly;
			}
		}

		[ComVisible(false)]
		public bool IsRightToLeft
		{
			get
			{
				int win32LangID = m_win32LangID;
				if (win32LangID == 1 || win32LangID == 13 || win32LangID == 32 || win32LangID == 41 || win32LangID == 90 || win32LangID == 101 || win32LangID == 1025 || win32LangID == 1037 || win32LangID == 1056 || win32LangID == 1065 || win32LangID == 1114 || win32LangID == 1125 || win32LangID == 2049 || win32LangID == 3073 || win32LangID == 4097 || win32LangID == 5121 || win32LangID == 6145 || win32LangID == 7169 || win32LangID == 8193 || win32LangID == 9217 || win32LangID == 10241 || win32LangID == 11265 || win32LangID == 12289 || win32LangID == 13313 || win32LangID == 14337 || win32LangID == 15361 || win32LangID == 16385)
				{
					return true;
				}
				return false;
			}
		}

		internal unsafe TextInfo(CultureInfo ci, int lcid, void* data, bool read_only)
		{
			m_isReadOnly = read_only;
			m_win32LangID = lcid;
			this.ci = ci;
			if (data != null)
			{
				this.data = *(Data*)data;
			}
			else
			{
				this.data = default(Data);
				this.data.list_sep = 44;
			}
			CultureInfo cultureInfo = ci;
			while (cultureInfo.Parent != null && cultureInfo.Parent.LCID != 127 && cultureInfo.Parent != cultureInfo)
			{
				cultureInfo = cultureInfo.Parent;
			}
			if (cultureInfo != null)
			{
				int lCID = cultureInfo.LCID;
				if (lCID == 31 || lCID == 44)
				{
					handleDotI = true;
				}
			}
		}

		private TextInfo(TextInfo textInfo)
		{
			m_win32LangID = textInfo.m_win32LangID;
			m_nDataItem = textInfo.m_nDataItem;
			m_useUserOverride = textInfo.m_useUserOverride;
			m_listSeparator = textInfo.ListSeparator;
			customCultureName = textInfo.CultureName;
			ci = textInfo.ci;
			handleDotI = textInfo.handleDotI;
			data = textInfo.data;
		}

		[MonoTODO]
		void IDeserializationCallback.OnDeserialization(object sender)
		{
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			TextInfo textInfo = obj as TextInfo;
			if (textInfo == null)
			{
				return false;
			}
			if (textInfo.m_win32LangID != m_win32LangID)
			{
				return false;
			}
			if (textInfo.ci != ci)
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return m_win32LangID;
		}

		public override string ToString()
		{
			return "TextInfo - " + m_win32LangID;
		}

		public string ToTitleCase(string str)
		{
			if (str == null)
			{
				throw new ArgumentNullException("str");
			}
			StringBuilder stringBuilder = null;
			int num = 0;
			int num2 = 0;
			while (num < str.Length)
			{
				if (!char.IsLetter(str[num++]))
				{
					continue;
				}
				num--;
				char c = ToTitleCase(str[num]);
				bool flag = true;
				if (c == str[num])
				{
					flag = false;
					bool flag2 = true;
					int num3 = num;
					while (++num < str.Length && !char.IsWhiteSpace(str[num]))
					{
						c = ToTitleCase(str[num]);
						if (c != str[num])
						{
							flag2 = false;
							break;
						}
					}
					if (flag2)
					{
						continue;
					}
					num = num3;
					while (++num < str.Length && !char.IsWhiteSpace(str[num]))
					{
						if (ToLower(str[num]) != str[num])
						{
							flag = true;
							num = num3;
							break;
						}
					}
				}
				if (flag)
				{
					if (stringBuilder == null)
					{
						stringBuilder = new StringBuilder(str.Length);
					}
					stringBuilder.Append(str, num2, num - num2);
					stringBuilder.Append(ToTitleCase(str[num]));
					num2 = num + 1;
					while (++num < str.Length && !char.IsWhiteSpace(str[num]))
					{
						stringBuilder.Append(ToLower(str[num]));
					}
					num2 = num;
				}
			}
			if (stringBuilder != null)
			{
				stringBuilder.Append(str, num2, str.Length - num2);
			}
			return (stringBuilder == null) ? str : stringBuilder.ToString();
		}

		public virtual char ToLower(char c)
		{
			if (c < '@' || ('`' < c && c < '\u0080'))
			{
				return c;
			}
			if ('A' <= c && c <= 'Z' && (!handleDotI || c != 'I'))
			{
				return (char)(c + 32);
			}
			if (ci == null || ci.LCID == 127)
			{
				return char.ToLowerInvariant(c);
			}
			switch (c)
			{
			case 'I':
				if (handleDotI)
				{
					return 'ı';
				}
				break;
			case 'İ':
				return 'i';
			case 'ǅ':
				return 'ǆ';
			case 'ǈ':
				return 'ǉ';
			case 'ǋ':
				return 'ǌ';
			case 'ǲ':
				return 'ǳ';
			case 'ϒ':
				return 'υ';
			case 'ϓ':
				return 'ύ';
			case 'ϔ':
				return 'ϋ';
			}
			return char.ToLowerInvariant(c);
		}

		public virtual char ToUpper(char c)
		{
			if (c < '`')
			{
				return c;
			}
			if ('a' <= c && c <= 'z' && (!handleDotI || c != 'i'))
			{
				return (char)(c - 32);
			}
			if (ci == null || ci.LCID == 127)
			{
				return char.ToUpperInvariant(c);
			}
			switch (c)
			{
			case 'i':
				if (handleDotI)
				{
					return 'İ';
				}
				break;
			case 'ı':
				return 'I';
			case 'ǅ':
				return 'Ǆ';
			case 'ǈ':
				return 'Ǉ';
			case 'ǋ':
				return 'Ǌ';
			case 'ǲ':
				return 'Ǳ';
			case 'ΐ':
				return 'Ϊ';
			case 'ΰ':
				return 'Ϋ';
			case 'ϐ':
				return 'Β';
			case 'ϑ':
				return 'Θ';
			case 'ϕ':
				return 'Φ';
			case 'ϖ':
				return 'Π';
			case 'ϰ':
				return 'Κ';
			case 'ϱ':
				return 'Ρ';
			}
			return char.ToUpperInvariant(c);
		}

		private char ToTitleCase(char c)
		{
			switch (c)
			{
			case 'Ǆ':
			case 'ǅ':
			case 'ǆ':
				return 'ǅ';
			case 'Ǉ':
			case 'ǈ':
			case 'ǉ':
				return 'ǈ';
			case 'Ǌ':
			case 'ǋ':
			case 'ǌ':
				return 'ǋ';
			case 'Ǳ':
			case 'ǲ':
			case 'ǳ':
				return 'ǲ';
			default:
				if (('ⅰ' <= c && c <= 'ⅿ') || ('ⓐ' <= c && c <= 'ⓩ'))
				{
					return c;
				}
				return ToUpper(c);
			}
		}

		public unsafe virtual string ToLower(string str)
		{
			if (str == null)
			{
				throw new ArgumentNullException("str");
			}
			if (str.Length == 0)
			{
				return string.Empty;
			}
			string text = string.InternalAllocateStr(str.Length);
			fixed (char* ptr = str)
			{
				fixed (char* ptr2 = text)
				{
					char* ptr3 = ptr2;
					char* ptr4 = ptr;
					for (int i = 0; i < str.Length; i++)
					{
						*ptr3 = ToLower(*ptr4);
						ptr4++;
						ptr3++;
					}
				}
			}
			return text;
		}

		public unsafe virtual string ToUpper(string str)
		{
			if (str == null)
			{
				throw new ArgumentNullException("str");
			}
			if (str.Length == 0)
			{
				return string.Empty;
			}
			string text = string.InternalAllocateStr(str.Length);
			fixed (char* ptr = str)
			{
				fixed (char* ptr2 = text)
				{
					char* ptr3 = ptr2;
					char* ptr4 = ptr;
					for (int i = 0; i < str.Length; i++)
					{
						*ptr3 = ToUpper(*ptr4);
						ptr4++;
						ptr3++;
					}
				}
			}
			return text;
		}

		[ComVisible(false)]
		public static TextInfo ReadOnly(TextInfo textInfo)
		{
			if (textInfo == null)
			{
				throw new ArgumentNullException("textInfo");
			}
			TextInfo textInfo2 = new TextInfo(textInfo);
			textInfo2.m_isReadOnly = true;
			return textInfo2;
		}

		[ComVisible(false)]
		public virtual object Clone()
		{
			return new TextInfo(this);
		}
	}
}

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Text
{
	[Serializable]
	[ComVisible(true)]
	public abstract class Encoding : ICloneable
	{
		private sealed class ForwardingDecoder : Decoder
		{
			private Encoding encoding;

			public ForwardingDecoder(Encoding enc)
			{
				encoding = enc;
				DecoderFallback decoderFallback = encoding.DecoderFallback;
				if (decoderFallback != null)
				{
					base.Fallback = decoderFallback;
				}
			}

			public override int GetCharCount(byte[] bytes, int index, int count)
			{
				return encoding.GetCharCount(bytes, index, count);
			}

			public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
			{
				return encoding.GetChars(bytes, byteIndex, byteCount, chars, charIndex);
			}
		}

		private sealed class ForwardingEncoder : Encoder
		{
			private Encoding encoding;

			public ForwardingEncoder(Encoding enc)
			{
				encoding = enc;
				EncoderFallback encoderFallback = encoding.EncoderFallback;
				if (encoderFallback != null)
				{
					base.Fallback = encoderFallback;
				}
			}

			public override int GetByteCount(char[] chars, int index, int count, bool flush)
			{
				return encoding.GetByteCount(chars, index, count);
			}

			public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteCount, bool flush)
			{
				return encoding.GetBytes(chars, charIndex, charCount, bytes, byteCount);
			}
		}

		internal int codePage;

		internal int windows_code_page;

		private bool is_readonly = true;

		private DecoderFallback decoder_fallback;

		private EncoderFallback encoder_fallback;

		private static Assembly i18nAssembly;

		private static bool i18nDisabled;

		private static EncodingInfo[] encoding_infos;

		private static readonly object[] encodings = new object[43]
		{
			20127, "ascii", "us_ascii", "us", "ansi_x3.4_1968", "ansi_x3.4_1986", "cp367", "csascii", "ibm367", "iso_ir_6",
			"iso646_us", "iso_646.irv:1991", 65000, "utf_7", "csunicode11utf7", "unicode_1_1_utf_7", "unicode_2_0_utf_7", "x_unicode_1_1_utf_7", "x_unicode_2_0_utf_7", 65001,
			"utf_8", "unicode_1_1_utf_8", "unicode_2_0_utf_8", "x_unicode_1_1_utf_8", "x_unicode_2_0_utf_8", 1200, "utf_16", "UTF_16LE", "ucs_2", "unicode",
			"iso_10646_ucs2", 1201, "unicodefffe", "utf_16be", 12000, "utf_32", "UTF_32LE", "ucs_4", 12001, "UTF_32BE",
			28591, "iso_8859_1", "latin1"
		};

		internal string body_name;

		internal string encoding_name;

		internal string header_name;

		internal bool is_mail_news_display;

		internal bool is_mail_news_save;

		internal bool is_browser_save;

		internal bool is_browser_display;

		internal string web_name;

		private static volatile Encoding asciiEncoding;

		private static volatile Encoding bigEndianEncoding;

		private static volatile Encoding defaultEncoding;

		private static volatile Encoding utf7Encoding;

		private static volatile Encoding utf8EncodingWithMarkers;

		private static volatile Encoding utf8EncodingWithoutMarkers;

		private static volatile Encoding unicodeEncoding;

		private static volatile Encoding isoLatin1Encoding;

		private static volatile Encoding utf8EncodingUnsafe;

		private static volatile Encoding utf32Encoding;

		private static volatile Encoding bigEndianUTF32Encoding;

		private static readonly object lockobj = new object();

		[ComVisible(false)]
		public bool IsReadOnly
		{
			get
			{
				return is_readonly;
			}
		}

		[ComVisible(false)]
		public virtual bool IsSingleByte
		{
			get
			{
				return false;
			}
		}

		[ComVisible(false)]
		public DecoderFallback DecoderFallback
		{
			get
			{
				return decoder_fallback;
			}
			set
			{
				if (IsReadOnly)
				{
					throw new InvalidOperationException("This Encoding is readonly.");
				}
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				decoder_fallback = value;
			}
		}

		[ComVisible(false)]
		public EncoderFallback EncoderFallback
		{
			get
			{
				return encoder_fallback;
			}
			set
			{
				if (IsReadOnly)
				{
					throw new InvalidOperationException("This Encoding is readonly.");
				}
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				encoder_fallback = value;
			}
		}

		public virtual string BodyName
		{
			get
			{
				return body_name;
			}
		}

		public virtual int CodePage
		{
			get
			{
				return codePage;
			}
		}

		public virtual string EncodingName
		{
			get
			{
				return encoding_name;
			}
		}

		public virtual string HeaderName
		{
			get
			{
				return header_name;
			}
		}

		public virtual bool IsBrowserDisplay
		{
			get
			{
				return is_browser_display;
			}
		}

		public virtual bool IsBrowserSave
		{
			get
			{
				return is_browser_save;
			}
		}

		public virtual bool IsMailNewsDisplay
		{
			get
			{
				return is_mail_news_display;
			}
		}

		public virtual bool IsMailNewsSave
		{
			get
			{
				return is_mail_news_save;
			}
		}

		public virtual string WebName
		{
			get
			{
				return web_name;
			}
		}

		public virtual int WindowsCodePage
		{
			get
			{
				return windows_code_page;
			}
		}

		public static Encoding ASCII
		{
			get
			{
				if (asciiEncoding == null)
				{
					lock (lockobj)
					{
						if (asciiEncoding == null)
						{
							asciiEncoding = new ASCIIEncoding();
						}
					}
				}
				return asciiEncoding;
			}
		}

		public static Encoding BigEndianUnicode
		{
			get
			{
				if (bigEndianEncoding == null)
				{
					lock (lockobj)
					{
						if (bigEndianEncoding == null)
						{
							bigEndianEncoding = new UnicodeEncoding(true, true);
						}
					}
				}
				return bigEndianEncoding;
			}
		}

		public static Encoding Default
		{
			get
			{
				if (defaultEncoding == null)
				{
					lock (lockobj)
					{
						if (defaultEncoding == null)
						{
							int code_page = 1;
							string name = InternalCodePage(ref code_page);
							try
							{
								if (code_page == -1)
								{
									defaultEncoding = GetEncoding(name);
								}
								else
								{
									code_page &= 0xFFFFFFF;
									switch (code_page)
									{
									case 1:
										code_page = 20127;
										break;
									case 2:
										code_page = 65000;
										break;
									case 3:
										code_page = 65001;
										break;
									case 4:
										code_page = 1200;
										break;
									case 5:
										code_page = 1201;
										break;
									case 6:
										code_page = 28591;
										break;
									}
									defaultEncoding = GetEncoding(code_page);
								}
							}
							catch (NotSupportedException)
							{
								defaultEncoding = UTF8Unmarked;
							}
							catch (ArgumentException)
							{
								defaultEncoding = UTF8Unmarked;
							}
							defaultEncoding.is_readonly = true;
						}
					}
				}
				return defaultEncoding;
			}
		}

		private static Encoding ISOLatin1
		{
			get
			{
				if (isoLatin1Encoding == null)
				{
					lock (lockobj)
					{
						if (isoLatin1Encoding == null)
						{
							isoLatin1Encoding = new Latin1Encoding();
						}
					}
				}
				return isoLatin1Encoding;
			}
		}

		public static Encoding UTF7
		{
			get
			{
				if (utf7Encoding == null)
				{
					lock (lockobj)
					{
						if (utf7Encoding == null)
						{
							utf7Encoding = new UTF7Encoding();
						}
					}
				}
				return utf7Encoding;
			}
		}

		public static Encoding UTF8
		{
			get
			{
				if (utf8EncodingWithMarkers == null)
				{
					lock (lockobj)
					{
						if (utf8EncodingWithMarkers == null)
						{
							utf8EncodingWithMarkers = new UTF8Encoding(true);
						}
					}
				}
				return utf8EncodingWithMarkers;
			}
		}

		internal static Encoding UTF8Unmarked
		{
			get
			{
				if (utf8EncodingWithoutMarkers == null)
				{
					lock (lockobj)
					{
						if (utf8EncodingWithoutMarkers == null)
						{
							utf8EncodingWithoutMarkers = new UTF8Encoding(false, false);
						}
					}
				}
				return utf8EncodingWithoutMarkers;
			}
		}

		internal static Encoding UTF8UnmarkedUnsafe
		{
			get
			{
				if (utf8EncodingUnsafe == null)
				{
					lock (lockobj)
					{
						if (utf8EncodingUnsafe == null)
						{
							utf8EncodingUnsafe = new UTF8Encoding(false, false);
							utf8EncodingUnsafe.is_readonly = false;
							utf8EncodingUnsafe.DecoderFallback = new DecoderReplacementFallback(string.Empty);
							utf8EncodingUnsafe.is_readonly = true;
						}
					}
				}
				return utf8EncodingUnsafe;
			}
		}

		public static Encoding Unicode
		{
			get
			{
				if (unicodeEncoding == null)
				{
					lock (lockobj)
					{
						if (unicodeEncoding == null)
						{
							unicodeEncoding = new UnicodeEncoding(false, true);
						}
					}
				}
				return unicodeEncoding;
			}
		}

		public static Encoding UTF32
		{
			get
			{
				if (utf32Encoding == null)
				{
					lock (lockobj)
					{
						if (utf32Encoding == null)
						{
							utf32Encoding = new UTF32Encoding(false, true);
						}
					}
				}
				return utf32Encoding;
			}
		}

		internal static Encoding BigEndianUTF32
		{
			get
			{
				if (bigEndianUTF32Encoding == null)
				{
					lock (lockobj)
					{
						if (bigEndianUTF32Encoding == null)
						{
							bigEndianUTF32Encoding = new UTF32Encoding(true, true);
						}
					}
				}
				return bigEndianUTF32Encoding;
			}
		}

		protected Encoding()
		{
		}

		protected Encoding(int codePage)
		{
			this.codePage = (windows_code_page = codePage);
			switch (codePage)
			{
			default:
				decoder_fallback = DecoderFallback.ReplacementFallback;
				encoder_fallback = EncoderFallback.ReplacementFallback;
				break;
			case 20127:
			case 54936:
				decoder_fallback = DecoderFallback.ReplacementFallback;
				encoder_fallback = EncoderFallback.ReplacementFallback;
				break;
			case 1200:
			case 1201:
			case 12000:
			case 12001:
			case 65000:
			case 65001:
				decoder_fallback = DecoderFallback.StandardSafeFallback;
				encoder_fallback = EncoderFallback.StandardSafeFallback;
				break;
			}
		}

		internal static string _(string arg)
		{
			return arg;
		}

		internal void SetFallbackInternal(EncoderFallback e, DecoderFallback d)
		{
			if (e != null)
			{
				encoder_fallback = e;
			}
			if (d != null)
			{
				decoder_fallback = d;
			}
		}

		public static byte[] Convert(Encoding srcEncoding, Encoding dstEncoding, byte[] bytes)
		{
			if (srcEncoding == null)
			{
				throw new ArgumentNullException("srcEncoding");
			}
			if (dstEncoding == null)
			{
				throw new ArgumentNullException("dstEncoding");
			}
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			return dstEncoding.GetBytes(srcEncoding.GetChars(bytes, 0, bytes.Length));
		}

		public static byte[] Convert(Encoding srcEncoding, Encoding dstEncoding, byte[] bytes, int index, int count)
		{
			if (srcEncoding == null)
			{
				throw new ArgumentNullException("srcEncoding");
			}
			if (dstEncoding == null)
			{
				throw new ArgumentNullException("dstEncoding");
			}
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			if (index < 0 || index > bytes.Length)
			{
				throw new ArgumentOutOfRangeException("index", _("ArgRange_Array"));
			}
			if (count < 0 || bytes.Length - index < count)
			{
				throw new ArgumentOutOfRangeException("count", _("ArgRange_Array"));
			}
			return dstEncoding.GetBytes(srcEncoding.GetChars(bytes, index, count));
		}

		public override bool Equals(object value)
		{
			Encoding encoding = value as Encoding;
			if (encoding != null)
			{
				return codePage == encoding.codePage && DecoderFallback.Equals(encoding.DecoderFallback) && EncoderFallback.Equals(encoding.EncoderFallback);
			}
			return false;
		}

		public abstract int GetByteCount(char[] chars, int index, int count);

		public unsafe virtual int GetByteCount(string s)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			if (s.Length == 0)
			{
				return 0;
			}
			fixed (char* chars = s)
			{
				return GetByteCount(chars, s.Length);
			}
		}

		public virtual int GetByteCount(char[] chars)
		{
			if (chars != null)
			{
				return GetByteCount(chars, 0, chars.Length);
			}
			throw new ArgumentNullException("chars");
		}

		public abstract int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex);

		public unsafe virtual int GetBytes(string s, int charIndex, int charCount, byte[] bytes, int byteIndex)
		{
			//IL_00c0->IL00c8: Incompatible stack types: I vs Ref
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			if (charIndex < 0 || charIndex > s.Length)
			{
				throw new ArgumentOutOfRangeException("charIndex", _("ArgRange_Array"));
			}
			if (charCount < 0 || charIndex > s.Length - charCount)
			{
				throw new ArgumentOutOfRangeException("charCount", _("ArgRange_Array"));
			}
			if (byteIndex < 0 || byteIndex > bytes.Length)
			{
				throw new ArgumentOutOfRangeException("byteIndex", _("ArgRange_Array"));
			}
			if (charCount == 0 || bytes.Length == byteIndex)
			{
				return 0;
			}
			fixed (char* ptr = s)
			{
				fixed (byte* ptr2 = &(bytes != null && bytes.Length != 0 ? ref bytes[0] : ref *(byte*)null))
				{
					return GetBytes((char*)((byte*)ptr + charIndex * 2), charCount, ptr2 + byteIndex, bytes.Length - byteIndex);
				}
			}
		}

		public unsafe virtual byte[] GetBytes(string s)
		{
			//IL_0061->IL0068: Incompatible stack types: I vs Ref
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			if (s.Length == 0)
			{
				return new byte[0];
			}
			int byteCount = GetByteCount(s);
			if (byteCount == 0)
			{
				return new byte[0];
			}
			fixed (char* chars = s)
			{
				byte[] array = new byte[byteCount];
				fixed (byte* bytes = &(array != null && array.Length != 0 ? ref array[0] : ref *(byte*)null))
				{
					GetBytes(chars, s.Length, bytes, byteCount);
					return array;
				}
			}
		}

		public virtual byte[] GetBytes(char[] chars, int index, int count)
		{
			int byteCount = GetByteCount(chars, index, count);
			byte[] array = new byte[byteCount];
			GetBytes(chars, index, count, array, 0);
			return array;
		}

		public virtual byte[] GetBytes(char[] chars)
		{
			int byteCount = GetByteCount(chars, 0, chars.Length);
			byte[] array = new byte[byteCount];
			GetBytes(chars, 0, chars.Length, array, 0);
			return array;
		}

		public abstract int GetCharCount(byte[] bytes, int index, int count);

		public virtual int GetCharCount(byte[] bytes)
		{
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			return GetCharCount(bytes, 0, bytes.Length);
		}

		public abstract int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex);

		public virtual char[] GetChars(byte[] bytes, int index, int count)
		{
			int charCount = GetCharCount(bytes, index, count);
			char[] array = new char[charCount];
			GetChars(bytes, index, count, array, 0);
			return array;
		}

		public virtual char[] GetChars(byte[] bytes)
		{
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			int charCount = GetCharCount(bytes, 0, bytes.Length);
			char[] array = new char[charCount];
			GetChars(bytes, 0, bytes.Length, array, 0);
			return array;
		}

		public virtual Decoder GetDecoder()
		{
			return new ForwardingDecoder(this);
		}

		public virtual Encoder GetEncoder()
		{
			return new ForwardingEncoder(this);
		}

		private static object InvokeI18N(string name, params object[] args)
		{
			lock (lockobj)
			{
				if (i18nDisabled)
				{
					return null;
				}
				if (i18nAssembly == null)
				{
					try
					{
						try
						{
							i18nAssembly = Assembly.Load("I18N, Version=2.0.5.0, Culture=neutral, PublicKeyToken=0738eb9f132ed756");
						}
						catch (NotImplementedException)
						{
							i18nDisabled = true;
							return null;
						}
						if (i18nAssembly == null)
						{
							return null;
						}
					}
					catch (SystemException)
					{
						return null;
					}
				}
				Type type;
				try
				{
					type = i18nAssembly.GetType("I18N.Common.Manager");
				}
				catch (NotImplementedException)
				{
					i18nDisabled = true;
					return null;
				}
				if (type == null)
				{
					return null;
				}
				object obj;
				try
				{
					obj = type.InvokeMember("PrimaryManager", BindingFlags.Static | BindingFlags.Public | BindingFlags.GetProperty, null, null, null, null, null, null);
					if (obj == null)
					{
						return null;
					}
				}
				catch (MissingMethodException)
				{
					return null;
				}
				catch (SecurityException)
				{
					return null;
				}
				catch (NotImplementedException)
				{
					i18nDisabled = true;
					return null;
				}
				try
				{
					return type.InvokeMember(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod, null, obj, args, null, null, null);
				}
				catch (MissingMethodException)
				{
					return null;
				}
				catch (SecurityException)
				{
					return null;
				}
			}
		}

		public static Encoding GetEncoding(int codepage)
		{
			if (codepage < 0 || codepage > 65535)
			{
				throw new ArgumentOutOfRangeException("codepage", "Valid values are between 0 and 65535, inclusive.");
			}
			switch (codepage)
			{
			case 0:
				return Default;
			case 20127:
				return ASCII;
			case 65000:
				return UTF7;
			case 65001:
				return UTF8;
			case 12000:
				return UTF32;
			case 12001:
				return BigEndianUTF32;
			case 1200:
				return Unicode;
			case 1201:
				return BigEndianUnicode;
			case 28591:
				return ISOLatin1;
			default:
			{
				Encoding encoding = (Encoding)InvokeI18N("GetEncoding", codepage);
				if (encoding != null)
				{
					encoding.is_readonly = true;
					return encoding;
				}
				string text = "System.Text.CP" + codepage;
				Assembly executingAssembly = Assembly.GetExecutingAssembly();
				Type type = executingAssembly.GetType(text);
				if (type != null)
				{
					encoding = (Encoding)Activator.CreateInstance(type);
					encoding.is_readonly = true;
					return encoding;
				}
				type = Type.GetType(text);
				if (type != null)
				{
					encoding = (Encoding)Activator.CreateInstance(type);
					encoding.is_readonly = true;
					return encoding;
				}
				throw new NotSupportedException(string.Format("CodePage {0} not supported", codepage.ToString()));
			}
			}
		}

		[ComVisible(false)]
		public virtual object Clone()
		{
			Encoding encoding = (Encoding)MemberwiseClone();
			encoding.is_readonly = false;
			return encoding;
		}

		public static Encoding GetEncoding(int codepage, EncoderFallback encoderFallback, DecoderFallback decoderFallback)
		{
			if (encoderFallback == null)
			{
				throw new ArgumentNullException("encoderFallback");
			}
			if (decoderFallback == null)
			{
				throw new ArgumentNullException("decoderFallback");
			}
			Encoding encoding = GetEncoding(codepage).Clone() as Encoding;
			encoding.is_readonly = false;
			encoding.encoder_fallback = encoderFallback;
			encoding.decoder_fallback = decoderFallback;
			return encoding;
		}

		public static Encoding GetEncoding(string name, EncoderFallback encoderFallback, DecoderFallback decoderFallback)
		{
			if (encoderFallback == null)
			{
				throw new ArgumentNullException("encoderFallback");
			}
			if (decoderFallback == null)
			{
				throw new ArgumentNullException("decoderFallback");
			}
			Encoding encoding = GetEncoding(name).Clone() as Encoding;
			encoding.is_readonly = false;
			encoding.encoder_fallback = encoderFallback;
			encoding.decoder_fallback = decoderFallback;
			return encoding;
		}

		public static EncodingInfo[] GetEncodings()
		{
			if (encoding_infos == null)
			{
				int[] array = new int[95]
				{
					37, 437, 500, 708, 850, 852, 855, 857, 858, 860,
					861, 862, 863, 864, 865, 866, 869, 870, 874, 875,
					932, 936, 949, 950, 1026, 1047, 1140, 1141, 1142, 1143,
					1144, 1145, 1146, 1147, 1148, 1149, 1200, 1201, 1250, 1251,
					1252, 1253, 1254, 1255, 1256, 1257, 1258, 10000, 10079, 12000,
					12001, 20127, 20273, 20277, 20278, 20280, 20284, 20285, 20290, 20297,
					20420, 20424, 20866, 20871, 21025, 21866, 28591, 28592, 28593, 28594,
					28595, 28596, 28597, 28598, 28599, 28605, 38598, 50220, 50221, 50222,
					51932, 51949, 54936, 57002, 57003, 57004, 57005, 57006, 57007, 57008,
					57009, 57010, 57011, 65000, 65001
				};
				encoding_infos = new EncodingInfo[array.Length];
				for (int i = 0; i < array.Length; i++)
				{
					encoding_infos[i] = new EncodingInfo(array[i]);
				}
			}
			return encoding_infos;
		}

		[ComVisible(false)]
		public bool IsAlwaysNormalized()
		{
			return IsAlwaysNormalized(NormalizationForm.FormC);
		}

		[ComVisible(false)]
		public virtual bool IsAlwaysNormalized(NormalizationForm form)
		{
			return form == NormalizationForm.FormC && this is ASCIIEncoding;
		}

		public static Encoding GetEncoding(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			string text = name.ToLowerInvariant().Replace('-', '_');
			int codepage = 0;
			for (int i = 0; i < encodings.Length; i++)
			{
				object obj = encodings[i];
				if (obj is int)
				{
					codepage = (int)obj;
				}
				else if (text == (string)encodings[i])
				{
					return GetEncoding(codepage);
				}
			}
			Encoding encoding = (Encoding)InvokeI18N("GetEncoding", name);
			if (encoding != null)
			{
				return encoding;
			}
			string text2 = "System.Text.ENC" + text;
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			Type type = executingAssembly.GetType(text2);
			if (type != null)
			{
				return (Encoding)Activator.CreateInstance(type);
			}
			type = Type.GetType(text2);
			if (type != null)
			{
				return (Encoding)Activator.CreateInstance(type);
			}
			throw new ArgumentException(string.Format("Encoding name '{0}' not supported", name), "name");
		}

		public override int GetHashCode()
		{
			return DecoderFallback.GetHashCode() << 24 + EncoderFallback.GetHashCode() << 16 + codePage;
		}

		public abstract int GetMaxByteCount(int charCount);

		public abstract int GetMaxCharCount(int byteCount);

		public virtual byte[] GetPreamble()
		{
			return new byte[0];
		}

		public virtual string GetString(byte[] bytes, int index, int count)
		{
			return new string(GetChars(bytes, index, count));
		}

		public virtual string GetString(byte[] bytes)
		{
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			return GetString(bytes, 0, bytes.Length);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string InternalCodePage(ref int code_page);

		[CLSCompliant(false)]
		[ComVisible(false)]
		public unsafe virtual int GetByteCount(char* chars, int count)
		{
			if (chars == null)
			{
				throw new ArgumentNullException("chars");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			char[] array = new char[count];
			for (int i = 0; i < count; i++)
			{
				array[i] = *(char*)((byte*)chars + i * 2);
			}
			return GetByteCount(array);
		}

		[ComVisible(false)]
		[CLSCompliant(false)]
		public unsafe virtual int GetCharCount(byte* bytes, int count)
		{
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			byte[] array = new byte[count];
			for (int i = 0; i < count; i++)
			{
				array[i] = bytes[i];
			}
			return GetCharCount(array, 0, count);
		}

		[CLSCompliant(false)]
		[ComVisible(false)]
		public unsafe virtual int GetChars(byte* bytes, int byteCount, char* chars, int charCount)
		{
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			if (chars == null)
			{
				throw new ArgumentNullException("chars");
			}
			if (charCount < 0)
			{
				throw new ArgumentOutOfRangeException("charCount");
			}
			if (byteCount < 0)
			{
				throw new ArgumentOutOfRangeException("byteCount");
			}
			byte[] array = new byte[byteCount];
			for (int i = 0; i < byteCount; i++)
			{
				array[i] = bytes[i];
			}
			char[] chars2 = GetChars(array, 0, byteCount);
			int num = chars2.Length;
			if (num > charCount)
			{
				throw new ArgumentException("charCount is less than the number of characters produced", "charCount");
			}
			for (int j = 0; j < num; j++)
			{
				*(char*)((byte*)chars + j * 2) = chars2[j];
			}
			return num;
		}

		[CLSCompliant(false)]
		[ComVisible(false)]
		public unsafe virtual int GetBytes(char* chars, int charCount, byte* bytes, int byteCount)
		{
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			if (chars == null)
			{
				throw new ArgumentNullException("chars");
			}
			if (charCount < 0)
			{
				throw new ArgumentOutOfRangeException("charCount");
			}
			if (byteCount < 0)
			{
				throw new ArgumentOutOfRangeException("byteCount");
			}
			char[] array = new char[charCount];
			for (int i = 0; i < charCount; i++)
			{
				array[i] = *(char*)((byte*)chars + i * 2);
			}
			byte[] bytes2 = GetBytes(array, 0, charCount);
			int num = bytes2.Length;
			if (num > byteCount)
			{
				throw new ArgumentException("byteCount is less that the number of bytes produced", "byteCount");
			}
			for (int j = 0; j < num; j++)
			{
				bytes[j] = bytes2[j];
			}
			return bytes2.Length;
		}
	}
}

namespace System.Xml
{
	internal class XmlChar
	{
		public static readonly char[] WhitespaceChars = new char[4] { ' ', '\n', '\t', '\r' };

		private static readonly byte[] firstNamePages = new byte[256]
		{
			2, 3, 4, 5, 6, 7, 8, 0, 0, 9,
			10, 11, 12, 13, 14, 15, 16, 17, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			18, 19, 0, 20, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 21, 22,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 1, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 1, 23,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 1, 1, 1, 1, 1, 1, 1, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
			1, 1, 1, 1, 1, 24, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0
		};

		private static readonly byte[] namePages = new byte[256]
		{
			25, 3, 26, 27, 28, 29, 30, 0, 0, 31,
			32, 33, 34, 35, 36, 37, 16, 17, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			18, 19, 38, 20, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 39, 22,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 1, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 1, 23,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 1, 1, 1, 1, 1, 1, 1, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
			1, 1, 1, 1, 1, 24, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0
		};

		private static readonly uint[] nameBitmap = new uint[320]
		{
			0u, 0u, 0u, 0u, 0u, 0u, 0u, 0u, 4294967295u, 4294967295u,
			4294967295u, 4294967295u, 4294967295u, 4294967295u, 4294967295u, 4294967295u, 0u, 67108864u, 2281701374u, 134217726u,
			0u, 0u, 4286578687u, 4286578687u, 4294967295u, 2146697215u, 4294966782u, 2147483647u, 4294967295u, 4294967295u,
			4294959119u, 4231135231u, 16777215u, 0u, 4294901760u, 4294967295u, 4294967295u, 4160750079u, 3u, 0u,
			0u, 0u, 0u, 0u, 4294956864u, 4294967291u, 1417641983u, 1048573u, 4294959102u, 4294967295u,
			3758030847u, 4294967295u, 4294901763u, 4294967295u, 4294908319u, 54513663u, 0u, 4294836224u, 41943039u, 4294967294u,
			127u, 0u, 4294901760u, 460799u, 0u, 134217726u, 2046u, 4294836224u, 4294967295u, 2097151999u,
			3112959u, 96u, 4294967264u, 603979775u, 4278190080u, 3u, 4294549472u, 63307263u, 2952790016u, 196611u,
			4294543328u, 57540095u, 1577058304u, 1835008u, 4294684640u, 602799615u, 0u, 1u, 4294549472u, 600702463u,
			2952790016u, 3u, 3594373088u, 62899992u, 0u, 0u, 4294828000u, 66059775u, 0u, 3u,
			4294828000u, 66059775u, 1073741824u, 3u, 4294828000u, 67108351u, 0u, 3u, 0u, 0u,
			0u, 0u, 4294967294u, 884735u, 63u, 0u, 4277151126u, 537750702u, 31u, 0u,
			0u, 0u, 4294967039u, 1023u, 0u, 0u, 0u, 0u, 0u, 0u,
			0u, 0u, 0u, 4294967295u, 4294901823u, 8388607u, 514797u, 1342177280u, 2184269825u, 2908843u,
			1073741824u, 4118857984u, 7u, 33622016u, 4294967295u, 4294967295u, 4294967295u, 4294967295u, 268435455u, 4294967295u,
			4294967295u, 67108863u, 1061158911u, 4294967295u, 2868854591u, 1073741823u, 4294967295u, 1608515583u, 265232348u, 534519807u,
			0u, 19520u, 0u, 0u, 7u, 0u, 0u, 0u, 128u, 1022u,
			4294967294u, 4294967295u, 2097151u, 4294967294u, 4294967295u, 134217727u, 4294967264u, 8191u, 0u, 0u,
			0u, 0u, 0u, 0u, 4294967295u, 4294967295u, 4294967295u, 4294967295u, 4294967295u, 63u,
			0u, 0u, 4294967295u, 4294967295u, 4294967295u, 4294967295u, 4294967295u, 15u, 0u, 0u,
			0u, 134176768u, 2281701374u, 134217726u, 0u, 8388608u, 4286578687u, 4286578687u, 16777215u, 0u,
			4294901760u, 4294967295u, 4294967295u, 4160750079u, 196611u, 0u, 4294967295u, 4294967295u, 63u, 3u,
			4294956992u, 4294967291u, 1417641983u, 1048573u, 4294959102u, 4294967295u, 3758030847u, 4294967295u, 4294901883u, 4294967295u,
			4294908319u, 54513663u, 0u, 4294836224u, 41943039u, 4294967294u, 4294836351u, 3154116603u, 4294901782u, 460799u,
			0u, 134217726u, 524287u, 4294902783u, 4294967295u, 2097151999u, 4293885951u, 67059199u, 4294967278u, 4093640703u,
			4280172543u, 65487u, 4294549486u, 3552968191u, 2961193375u, 262095u, 4294543332u, 3547201023u, 1577073031u, 2097088u,
			4294684654u, 4092460543u, 15295u, 65473u, 4294549486u, 4090363391u, 2965387663u, 65475u, 3594373100u, 3284125464u,
			8404423u, 65408u, 4294828014u, 3287285247u, 6307295u, 65475u, 4294828012u, 3287285247u, 1080049119u, 65475u,
			4294828012u, 3288333823u, 8404431u, 65475u, 0u, 0u, 0u, 0u, 4294967294u, 134184959u,
			67076095u, 0u, 4277151126u, 1006595246u, 67059551u, 0u, 50331648u, 3265266687u, 4294967039u, 4294837247u,
			4273934303u, 50216959u, 0u, 0u, 0u, 0u, 0u, 0u, 0u, 0u,
			536805376u, 2u, 160u, 4128766u, 4294967294u, 4294967295u, 1713373183u, 4294967294u, 4294967295u, 2013265919u
		};

		public static bool IsWhitespace(int ch)
		{
			return ch == 32 || ch == 9 || ch == 13 || ch == 10;
		}

		public static bool IsWhitespace(string str)
		{
			for (int i = 0; i < str.Length; i++)
			{
				if (!IsWhitespace(str[i]))
				{
					return false;
				}
			}
			return true;
		}

		public static int IndexOfNonWhitespace(string str)
		{
			for (int i = 0; i < str.Length; i++)
			{
				if (!IsWhitespace(str[i]))
				{
					return i;
				}
			}
			return -1;
		}

		public static bool IsFirstNameChar(int ch)
		{
			if ((ch >= 97 && ch <= 122) || (ch >= 65 && ch <= 90))
			{
				return true;
			}
			if ((uint)ch <= 65535u)
			{
				return (nameBitmap[(firstNamePages[ch >> 8] << 3) + ((ch & 0xFF) >> 5)] & (1 << ch)) != 0;
			}
			return false;
		}

		public static bool IsValid(int ch)
		{
			return !IsInvalid(ch);
		}

		public static bool IsInvalid(int ch)
		{
			switch (ch)
			{
			case 9:
			case 10:
			case 13:
				return false;
			default:
				if (ch < 32)
				{
					return true;
				}
				if (ch < 55296)
				{
					return false;
				}
				if (ch < 57344)
				{
					return true;
				}
				if (ch < 65534)
				{
					return false;
				}
				if (ch < 65536)
				{
					return true;
				}
				if (ch < 1114112)
				{
					return false;
				}
				return true;
			}
		}

		public static int IndexOfInvalid(string s, bool allowSurrogate)
		{
			for (int i = 0; i < s.Length; i++)
			{
				if (IsInvalid(s[i]))
				{
					if (!allowSurrogate || i + 1 == s.Length || s[i] < '\ud800' || s[i] >= '\udc00' || s[i + 1] < '\udc00' || s[i + 1] >= '\ue000')
					{
						return i;
					}
					i++;
				}
			}
			return -1;
		}

		public static int IndexOfInvalid(char[] s, int start, int length, bool allowSurrogate)
		{
			int num = start + length;
			if (s.Length < num)
			{
				throw new ArgumentOutOfRangeException("length");
			}
			for (int i = start; i < num; i++)
			{
				if (IsInvalid(s[i]))
				{
					if (!allowSurrogate || i + 1 == num || s[i] < '\ud800' || s[i] >= '\udc00' || s[i + 1] < '\udc00' || s[i + 1] >= '\ue000')
					{
						return i;
					}
					i++;
				}
			}
			return -1;
		}

		public static bool IsNameChar(int ch)
		{
			if ((ch >= 97 && ch <= 122) || (ch >= 65 && ch <= 90))
			{
				return true;
			}
			if ((uint)ch <= 65535u)
			{
				return (nameBitmap[(namePages[ch >> 8] << 3) + ((ch & 0xFF) >> 5)] & (1 << ch)) != 0;
			}
			return false;
		}

		public static bool IsNCNameChar(int ch)
		{
			bool result = false;
			if (ch >= 0 && ch <= 65535 && ch != 58)
			{
				result = (nameBitmap[(namePages[ch >> 8] << 3) + ((ch & 0xFF) >> 5)] & (1 << (ch & 0x1F))) != 0;
			}
			return result;
		}

		public static bool IsName(string str)
		{
			if (str.Length == 0)
			{
				return false;
			}
			if (!IsFirstNameChar(str[0]))
			{
				return false;
			}
			for (int i = 1; i < str.Length; i++)
			{
				if (!IsNameChar(str[i]))
				{
					return false;
				}
			}
			return true;
		}

		public static bool IsNCName(string str)
		{
			if (str.Length == 0)
			{
				return false;
			}
			if (!IsFirstNameChar(str[0]))
			{
				return false;
			}
			for (int i = 0; i < str.Length; i++)
			{
				if (!IsNCNameChar(str[i]))
				{
					return false;
				}
			}
			return true;
		}

		public static bool IsNmToken(string str)
		{
			if (str.Length == 0)
			{
				return false;
			}
			for (int i = 0; i < str.Length; i++)
			{
				if (!IsNameChar(str[i]))
				{
					return false;
				}
			}
			return true;
		}

		public static bool IsPubidChar(int ch)
		{
			return ((IsWhitespace(ch) && ch != 9) || (97 <= ch && ch <= 122) || (65 <= ch && ch <= 90) || (48 <= ch && ch <= 57)) | ("-'()+,./:=?;!*#@$_%".IndexOf((char)ch) >= 0);
		}

		public static bool IsPubid(string str)
		{
			for (int i = 0; i < str.Length; i++)
			{
				if (!IsPubidChar(str[i]))
				{
					return false;
				}
			}
			return true;
		}

		public static bool IsValidIANAEncoding(string ianaEncoding)
		{
			if (ianaEncoding != null)
			{
				int length = ianaEncoding.Length;
				if (length > 0)
				{
					char c = ianaEncoding[0];
					if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'))
					{
						for (int i = 1; i < length; i++)
						{
							c = ianaEncoding[i];
							if ((c < 'A' || c > 'Z') && (c < 'a' || c > 'z') && (c < '0' || c > '9') && c != '.' && c != '_' && c != '-')
							{
								return false;
							}
						}
						return true;
					}
				}
			}
			return false;
		}

		public static int GetPredefinedEntity(string name)
		{
			switch (name)
			{
			case "amp":
				return 38;
			case "lt":
				return 60;
			case "gt":
				return 62;
			case "quot":
				return 34;
			case "apos":
				return 39;
			default:
				return -1;
			}
		}
	}
}

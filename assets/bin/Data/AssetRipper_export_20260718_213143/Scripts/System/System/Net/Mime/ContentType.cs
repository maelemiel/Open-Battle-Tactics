using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Text;

namespace System.Net.Mime
{
	public class ContentType
	{
		private static Encoding utf8unmarked;

		private string mediaType;

		private StringDictionary parameters = new StringDictionary();

		private static readonly char[] especials = new char[16]
		{
			'(', ')', '<', '>', '@', ',', ';', ':', '<', '>',
			'/', '[', ']', '?', '.', '='
		};

		private static Encoding UTF8Unmarked
		{
			get
			{
				if (utf8unmarked == null)
				{
					utf8unmarked = new UTF8Encoding(false);
				}
				return utf8unmarked;
			}
		}

		public string Boundary
		{
			get
			{
				return parameters["boundary"];
			}
			set
			{
				parameters["boundary"] = value;
			}
		}

		public string CharSet
		{
			get
			{
				return parameters["charset"];
			}
			set
			{
				parameters["charset"] = value;
			}
		}

		public string MediaType
		{
			get
			{
				return mediaType;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				if (value.Length < 1)
				{
					throw new ArgumentException();
				}
				if (value.IndexOf('/') < 1)
				{
					throw new FormatException();
				}
				if (value.IndexOf(';') != -1)
				{
					throw new FormatException();
				}
				mediaType = value;
			}
		}

		public string Name
		{
			get
			{
				return parameters["name"];
			}
			set
			{
				parameters["name"] = value;
			}
		}

		public StringDictionary Parameters
		{
			get
			{
				return parameters;
			}
		}

		public ContentType()
		{
			mediaType = "application/octet-stream";
		}

		public ContentType(string contentType)
		{
			if (contentType == null)
			{
				throw new ArgumentNullException("contentType");
			}
			if (contentType.Length < 1)
			{
				throw new ArgumentException("contentType");
			}
			int num = contentType.IndexOf(';');
			if (num > 0)
			{
				string[] array = contentType.Split(';');
				MediaType = array[0].Trim();
				for (int i = 1; i < array.Length; i++)
				{
					Parse(array[i]);
				}
			}
			else
			{
				MediaType = contentType.Trim();
			}
		}

		private void Parse(string pair)
		{
			if (pair != null && pair.Length >= 1)
			{
				string[] array = pair.Split('=');
				if (array.Length == 2)
				{
					parameters.Add(array[0].Trim(), array[1].Trim());
				}
			}
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as ContentType);
		}

		private bool Equals(ContentType other)
		{
			return other != null && ToString() == other.ToString();
		}

		public override int GetHashCode()
		{
			return ToString().GetHashCode();
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			Encoding enc = ((CharSet == null) ? Encoding.UTF8 : Encoding.GetEncoding(CharSet));
			stringBuilder.Append(MediaType);
			if (Parameters != null && Parameters.Count > 0)
			{
				foreach (DictionaryEntry parameter in parameters)
				{
					if (parameter.Value != null && parameter.Value.ToString().Length > 0)
					{
						stringBuilder.Append("; ");
						stringBuilder.Append(parameter.Key);
						stringBuilder.Append("=");
						stringBuilder.Append(WrapIfEspecialsExist(EncodeSubjectRFC2047(parameter.Value as string, enc)));
					}
				}
			}
			return stringBuilder.ToString();
		}

		private static string WrapIfEspecialsExist(string s)
		{
			s = s.Replace("\"", "\\\"");
			if (s.IndexOfAny(especials) >= 0)
			{
				return '"' + s + '"';
			}
			return s;
		}

		internal static Encoding GuessEncoding(string s)
		{
			for (int i = 0; i < s.Length; i++)
			{
				if (s[i] >= '\u0080')
				{
					return UTF8Unmarked;
				}
			}
			return null;
		}

		internal static TransferEncoding GuessTransferEncoding(Encoding enc)
		{
			if (Encoding.ASCII.Equals(enc))
			{
				return TransferEncoding.SevenBit;
			}
			if (Encoding.UTF8.CodePage == enc.CodePage || Encoding.Unicode.CodePage == enc.CodePage)
			{
				return TransferEncoding.Base64;
			}
			return TransferEncoding.QuotedPrintable;
		}

		internal static string To2047(byte[] bytes)
		{
			StringWriter stringWriter = new StringWriter();
			foreach (byte b in bytes)
			{
				if (b > 127 || b == 9)
				{
					stringWriter.Write("=");
					stringWriter.Write(Convert.ToString(b, 16).ToUpper());
				}
				else
				{
					stringWriter.Write(Convert.ToChar(b));
				}
			}
			return stringWriter.GetStringBuilder().ToString();
		}

		internal static string EncodeSubjectRFC2047(string s, Encoding enc)
		{
			if (s == null || Encoding.ASCII.Equals(enc))
			{
				return s;
			}
			for (int i = 0; i < s.Length; i++)
			{
				if (s[i] >= '\u0080')
				{
					string text = To2047(enc.GetBytes(s));
					return "=?" + enc.HeaderName + "?Q?" + text + "?=";
				}
			}
			return s;
		}
	}
}

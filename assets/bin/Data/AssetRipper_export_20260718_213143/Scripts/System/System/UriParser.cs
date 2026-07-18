using System.Collections;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace System
{
	public abstract class UriParser
	{
		private static object lock_object = new object();

		private static Hashtable table;

		internal string scheme_name;

		private int default_port;

		private static readonly Regex uri_regex = new Regex("^(([^:/?#]+):)?(//([^/?#]*))?([^?#]*)(\\?([^#]*))?(#(.*))?");

		private static readonly Regex auth_regex = new Regex("^(([^@]+)@)?(.*?)(:([0-9]+))?$");

		internal string SchemeName
		{
			get
			{
				return scheme_name;
			}
			set
			{
				scheme_name = value;
			}
		}

		internal int DefaultPort
		{
			get
			{
				return default_port;
			}
			set
			{
				default_port = value;
			}
		}

		private static Match ParseAuthority(Group g)
		{
			return auth_regex.Match(g.Value);
		}

		protected internal virtual string GetComponents(Uri uri, UriComponents components, UriFormat format)
		{
			if (format < UriFormat.UriEscaped || format > UriFormat.SafeUnescaped)
			{
				throw new ArgumentOutOfRangeException("format");
			}
			Match match = uri_regex.Match(uri.OriginalString);
			string value = scheme_name;
			int defaultPort = default_port;
			if (value == null || value == "*")
			{
				value = match.Groups[2].Value;
				defaultPort = Uri.GetDefaultPort(value);
			}
			else if (string.Compare(value, match.Groups[2].Value, true) != 0)
			{
				throw new SystemException("URI Parser: scheme mismatch: " + value + " vs. " + match.Groups[2].Value);
			}
			switch (components)
			{
			case UriComponents.Scheme:
				return value;
			case UriComponents.UserInfo:
				return ParseAuthority(match.Groups[4]).Groups[2].Value;
			case UriComponents.Host:
				return ParseAuthority(match.Groups[4]).Groups[3].Value;
			case UriComponents.Port:
			{
				string value2 = ParseAuthority(match.Groups[4]).Groups[5].Value;
				if (value2 != null && value2 != string.Empty && value2 != defaultPort.ToString())
				{
					return value2;
				}
				return string.Empty;
			}
			case UriComponents.Path:
				return Format(IgnoreFirstCharIf(match.Groups[5].Value, '/'), format);
			case UriComponents.Query:
				return Format(match.Groups[7].Value, format);
			case UriComponents.Fragment:
				return Format(match.Groups[9].Value, format);
			case UriComponents.StrongPort:
			{
				Group obj = ParseAuthority(match.Groups[4]).Groups[5];
				return (!obj.Success) ? defaultPort.ToString() : obj.Value;
			}
			case UriComponents.SerializationInfoString:
				components = UriComponents.AbsoluteUri;
				break;
			}
			Match match2 = ParseAuthority(match.Groups[4]);
			StringBuilder stringBuilder = new StringBuilder();
			if ((components & UriComponents.Scheme) != 0)
			{
				stringBuilder.Append(value);
				stringBuilder.Append(Uri.GetSchemeDelimiter(value));
			}
			if ((components & UriComponents.UserInfo) != 0)
			{
				stringBuilder.Append(match2.Groups[1].Value);
			}
			if ((components & UriComponents.Host) != 0)
			{
				stringBuilder.Append(match2.Groups[3].Value);
			}
			if ((components & UriComponents.StrongPort) != 0)
			{
				Group obj2 = match2.Groups[4];
				stringBuilder.Append((!obj2.Success) ? (":" + defaultPort) : obj2.Value);
			}
			if ((components & UriComponents.Port) != 0)
			{
				string value3 = match2.Groups[5].Value;
				if (value3 != null && value3 != string.Empty && value3 != defaultPort.ToString())
				{
					stringBuilder.Append(match2.Groups[4].Value);
				}
			}
			if ((components & UriComponents.Path) != 0)
			{
				stringBuilder.Append(match.Groups[5]);
			}
			if ((components & UriComponents.Query) != 0)
			{
				stringBuilder.Append(match.Groups[6]);
			}
			if ((components & UriComponents.Fragment) != 0)
			{
				stringBuilder.Append(match.Groups[8]);
			}
			return Format(stringBuilder.ToString(), format);
		}

		protected internal virtual void InitializeAndValidate(Uri uri, out UriFormatException parsingError)
		{
			if (uri.Scheme != scheme_name && scheme_name != "*")
			{
				parsingError = new UriFormatException("The argument Uri's scheme does not match");
			}
			else
			{
				parsingError = null;
			}
		}

		protected internal virtual bool IsBaseOf(Uri baseUri, Uri relativeUri)
		{
			if (Uri.Compare(baseUri, relativeUri, UriComponents.SchemeAndServer | UriComponents.UserInfo, UriFormat.Unescaped, StringComparison.InvariantCultureIgnoreCase) != 0)
			{
				return false;
			}
			string localPath = baseUri.LocalPath;
			int length = localPath.LastIndexOf('/') + 1;
			return string.Compare(localPath, 0, relativeUri.LocalPath, 0, length, StringComparison.InvariantCultureIgnoreCase) == 0;
		}

		protected internal virtual bool IsWellFormedOriginalString(Uri uri)
		{
			return uri.IsWellFormedOriginalString();
		}

		protected internal virtual UriParser OnNewUri()
		{
			return this;
		}

		[System.MonoTODO]
		protected virtual void OnRegister(string schemeName, int defaultPort)
		{
		}

		[System.MonoTODO]
		protected internal virtual string Resolve(Uri baseUri, Uri relativeUri, out UriFormatException parsingError)
		{
			throw new NotImplementedException();
		}

		private string IgnoreFirstCharIf(string s, char c)
		{
			if (s.Length == 0)
			{
				return string.Empty;
			}
			if (s[0] == c)
			{
				return s.Substring(1);
			}
			return s;
		}

		private string Format(string s, UriFormat format)
		{
			if (s.Length == 0)
			{
				return string.Empty;
			}
			switch (format)
			{
			case UriFormat.UriEscaped:
				return Uri.EscapeString(s, false, true, true);
			case UriFormat.SafeUnescaped:
				s = Uri.Unescape(s, false);
				return s;
			case UriFormat.Unescaped:
				return Uri.Unescape(s, false);
			default:
				throw new ArgumentOutOfRangeException("format");
			}
		}

		private static void CreateDefaults()
		{
			if (table != null)
			{
				return;
			}
			Hashtable hashtable = new Hashtable();
			InternalRegister(hashtable, new System.DefaultUriParser(), Uri.UriSchemeFile, -1);
			InternalRegister(hashtable, new System.DefaultUriParser(), Uri.UriSchemeFtp, 21);
			InternalRegister(hashtable, new System.DefaultUriParser(), Uri.UriSchemeGopher, 70);
			InternalRegister(hashtable, new System.DefaultUriParser(), Uri.UriSchemeHttp, 80);
			InternalRegister(hashtable, new System.DefaultUriParser(), Uri.UriSchemeHttps, 443);
			InternalRegister(hashtable, new System.DefaultUriParser(), Uri.UriSchemeMailto, 25);
			InternalRegister(hashtable, new System.DefaultUriParser(), Uri.UriSchemeNetPipe, -1);
			InternalRegister(hashtable, new System.DefaultUriParser(), Uri.UriSchemeNetTcp, -1);
			InternalRegister(hashtable, new System.DefaultUriParser(), Uri.UriSchemeNews, 119);
			InternalRegister(hashtable, new System.DefaultUriParser(), Uri.UriSchemeNntp, 119);
			InternalRegister(hashtable, new System.DefaultUriParser(), "ldap", 389);
			lock (lock_object)
			{
				if (table == null)
				{
					table = hashtable;
				}
				else
				{
					hashtable = null;
				}
			}
		}

		public static bool IsKnownScheme(string schemeName)
		{
			if (schemeName == null)
			{
				throw new ArgumentNullException("schemeName");
			}
			if (schemeName.Length == 0)
			{
				throw new ArgumentOutOfRangeException("schemeName");
			}
			CreateDefaults();
			string key = schemeName.ToLower(CultureInfo.InvariantCulture);
			return table[key] != null;
		}

		private static void InternalRegister(Hashtable table, UriParser uriParser, string schemeName, int defaultPort)
		{
			uriParser.SchemeName = schemeName;
			uriParser.DefaultPort = defaultPort;
			if (uriParser is GenericUriParser)
			{
				table.Add(schemeName, uriParser);
			}
			else
			{
				System.DefaultUriParser defaultUriParser = new System.DefaultUriParser();
				defaultUriParser.SchemeName = schemeName;
				defaultUriParser.DefaultPort = defaultPort;
				table.Add(schemeName, defaultUriParser);
			}
			uriParser.OnRegister(schemeName, defaultPort);
		}

		public static void Register(UriParser uriParser, string schemeName, int defaultPort)
		{
			if (uriParser == null)
			{
				throw new ArgumentNullException("uriParser");
			}
			if (schemeName == null)
			{
				throw new ArgumentNullException("schemeName");
			}
			if (defaultPort < -1 || defaultPort >= 65535)
			{
				throw new ArgumentOutOfRangeException("defaultPort");
			}
			CreateDefaults();
			string text = schemeName.ToLower(CultureInfo.InvariantCulture);
			if (table[text] != null)
			{
				string text2 = Locale.GetText("Scheme '{0}' is already registred.");
				throw new InvalidOperationException(text2);
			}
			InternalRegister(table, uriParser, text, defaultPort);
		}

		internal static UriParser GetParser(string schemeName)
		{
			if (schemeName == null)
			{
				return null;
			}
			CreateDefaults();
			string key = schemeName.ToLower(CultureInfo.InvariantCulture);
			return (UriParser)table[key];
		}
	}
}

using System.Collections;
using System.Collections.Specialized;
using System.Security;
using System.Text;

namespace System.Data.Common
{
	internal class DbConnectionOptions
	{
		internal NameValueCollection options;

		internal string normalizedConnectionString;

		[System.MonoTODO]
		public bool IsEmpty
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public string this[string keyword]
		{
			get
			{
				return options[keyword];
			}
		}

		public ICollection Keys
		{
			get
			{
				return options.Keys;
			}
		}

		internal DbConnectionOptions()
		{
		}

		protected internal DbConnectionOptions(DbConnectionOptions connectionOptions)
		{
			options = connectionOptions.options;
		}

		public DbConnectionOptions(string connectionString)
		{
			options = new NameValueCollection();
			ParseConnectionString(connectionString);
		}

		[System.MonoTODO]
		public DbConnectionOptions(string connectionString, Hashtable synonyms, bool useFirstKeyValuePair)
			: this(connectionString)
		{
		}

		[System.MonoTODO]
		protected void BuildConnectionString(StringBuilder builder, string[] withoutOptions, string insertValue)
		{
			throw new NotImplementedException();
		}

		public bool ContainsKey(string keyword)
		{
			return options.Get(keyword) != null;
		}

		public bool ConvertValueToBoolean(string keyname, bool defaultvalue)
		{
			if (ContainsKey(keyname))
			{
				return bool.Parse(this[keyname].Trim());
			}
			return defaultvalue;
		}

		public int ConvertValueToInt32(string keyname, int defaultvalue)
		{
			if (ContainsKey(keyname))
			{
				return int.Parse(this[keyname].Trim());
			}
			return defaultvalue;
		}

		[System.MonoTODO]
		public bool ConvertValueToIntegratedSecurity()
		{
			throw new NotImplementedException();
		}

		public string ConvertValueToString(string keyname, string defaultValue)
		{
			if (ContainsKey(keyname))
			{
				return this[keyname];
			}
			return defaultValue;
		}

		[System.MonoTODO]
		protected internal virtual PermissionSet CreatePermissionSet()
		{
			throw new NotImplementedException();
		}

		[System.MonoTODO]
		protected internal virtual string Expand()
		{
			throw new NotImplementedException();
		}

		[System.MonoTODO]
		public static string RemoveKeyValuePairs(string connectionString, string[] keynames)
		{
			throw new NotImplementedException();
		}

		[System.MonoTODO]
		public string UsersConnectionString(bool hisPasswordPwd)
		{
			throw new NotImplementedException();
		}

		internal void ParseConnectionString(string connectionString)
		{
			if (connectionString.Length == 0)
			{
				return;
			}
			connectionString += ";";
			bool flag = false;
			bool flag2 = false;
			bool flag3 = true;
			string text = string.Empty;
			string empty = string.Empty;
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < connectionString.Length; i++)
			{
				char c = connectionString[i];
				char c2 = ((i != connectionString.Length - 1) ? connectionString[i + 1] : '\0');
				switch (c)
				{
				case '\'':
					if (flag2)
					{
						stringBuilder.Append(c);
					}
					else if (c2.Equals(c))
					{
						stringBuilder.Append(c);
						i++;
					}
					else
					{
						flag = !flag;
					}
					break;
				case '"':
					if (flag)
					{
						stringBuilder.Append(c);
					}
					else if (c2.Equals(c))
					{
						stringBuilder.Append(c);
						i++;
					}
					else
					{
						flag2 = !flag2;
					}
					break;
				case ';':
					if (flag2 || flag)
					{
						stringBuilder.Append(c);
						break;
					}
					if (text != string.Empty && text != null)
					{
						empty = stringBuilder.ToString();
						options[text.Trim()] = empty;
					}
					flag3 = true;
					text = string.Empty;
					empty = string.Empty;
					stringBuilder = new StringBuilder();
					break;
				case '=':
					if (flag2 || flag || !flag3)
					{
						stringBuilder.Append(c);
					}
					else if (c2.Equals(c))
					{
						stringBuilder.Append(c);
						i++;
					}
					else
					{
						text = stringBuilder.ToString();
						stringBuilder = new StringBuilder();
						flag3 = false;
					}
					break;
				case ' ':
					if (flag || flag2)
					{
						stringBuilder.Append(c);
					}
					else if (stringBuilder.Length > 0 && !c2.Equals(';'))
					{
						stringBuilder.Append(c);
					}
					break;
				default:
					stringBuilder.Append(c);
					break;
				}
			}
			StringBuilder stringBuilder2 = new StringBuilder();
			ArrayList arrayList = new ArrayList();
			arrayList.AddRange(Keys);
			arrayList.Sort();
			foreach (string item in arrayList)
			{
				string value = string.Format("{0}=\"{1}\";", item, this[item].Replace("\"", "\"\""));
				stringBuilder2.Append(value);
			}
			normalizedConnectionString = stringBuilder2.ToString();
		}
	}
}

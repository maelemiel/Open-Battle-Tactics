using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Threading;

namespace System.Data.Common
{
	public class DbConnectionStringBuilder : ICollection, IEnumerable, IDictionary, ICustomTypeDescriptor
	{
		private readonly Dictionary<string, object> _dictionary;

		private readonly bool useOdbcRules;

		private static object _staticAttributeCollection;

		bool ICollection.IsSynchronized
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		object IDictionary.this[object keyword]
		{
			get
			{
				return this[(string)keyword];
			}
			set
			{
				this[(string)keyword] = value;
			}
		}

		[DesignOnly(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		public bool BrowsableConnectionString
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		public string ConnectionString
		{
			get
			{
				IDictionary<string, object> dictionary = _dictionary;
				StringBuilder stringBuilder = new StringBuilder();
				foreach (string key in Keys)
				{
					object value = null;
					if (dictionary.TryGetValue(key, out value))
					{
						string value2 = value.ToString();
						AppendKeyValuePair(stringBuilder, key, value2, useOdbcRules);
					}
				}
				return stringBuilder.ToString();
			}
			set
			{
				Clear();
				if (value != null && value.Trim().Length != 0)
				{
					ParseConnectionString(value);
				}
			}
		}

		[Browsable(false)]
		public virtual int Count
		{
			get
			{
				return _dictionary.Count;
			}
		}

		[Browsable(false)]
		public virtual bool IsFixedSize
		{
			get
			{
				return false;
			}
		}

		[Browsable(false)]
		public bool IsReadOnly
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		[Browsable(false)]
		public virtual object this[string keyword]
		{
			get
			{
				if (ContainsKey(keyword))
				{
					return _dictionary[keyword];
				}
				throw new ArgumentException(string.Format("Keyword '{0}' does not exist", keyword));
			}
			set
			{
				if (value == null)
				{
					Remove(keyword);
					return;
				}
				if (keyword == null)
				{
					throw new ArgumentNullException("keyword");
				}
				if (keyword.Length == 0)
				{
					throw CreateInvalidKeywordException(keyword);
				}
				for (int i = 0; i < keyword.Length; i++)
				{
					char c = keyword[i];
					if (i == 0 && (char.IsWhiteSpace(c) || c == ';'))
					{
						throw CreateInvalidKeywordException(keyword);
					}
					if (i == keyword.Length - 1 && char.IsWhiteSpace(c))
					{
						throw CreateInvalidKeywordException(keyword);
					}
					if (char.IsControl(c))
					{
						throw CreateInvalidKeywordException(keyword);
					}
				}
				if (ContainsKey(keyword))
				{
					_dictionary[keyword] = value;
				}
				else
				{
					_dictionary.Add(keyword, value);
				}
			}
		}

		[Browsable(false)]
		public virtual ICollection Keys
		{
			get
			{
				string[] array = new string[_dictionary.Keys.Count];
				((ICollection<string>)_dictionary.Keys).CopyTo(array, 0);
				return new ReadOnlyCollection<string>(array);
			}
		}

		[Browsable(false)]
		public virtual ICollection Values
		{
			get
			{
				object[] array = new object[_dictionary.Values.Count];
				((ICollection<object>)_dictionary.Values).CopyTo(array, 0);
				return new ReadOnlyCollection<object>(array);
			}
		}

		public DbConnectionStringBuilder()
			: this(false)
		{
		}

		public DbConnectionStringBuilder(bool useOdbcRules)
		{
			this.useOdbcRules = useOdbcRules;
			_dictionary = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
		}

		void ICollection.CopyTo(Array array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			KeyValuePair<string, object>[] array2 = array as KeyValuePair<string, object>[];
			if (array2 == null)
			{
				throw new ArgumentException("Target array type is not compatible with the type of items in the collection");
			}
			((ICollection<KeyValuePair<string, object>>)_dictionary).CopyTo(array2, index);
		}

		void IDictionary.Add(object keyword, object value)
		{
			Add((string)keyword, value);
		}

		bool IDictionary.Contains(object keyword)
		{
			return ContainsKey((string)keyword);
		}

		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			return _dictionary.GetEnumerator();
		}

		void IDictionary.Remove(object keyword)
		{
			Remove((string)keyword);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _dictionary.GetEnumerator();
		}

		AttributeCollection ICustomTypeDescriptor.GetAttributes()
		{
			object obj = _staticAttributeCollection;
			if (obj == null)
			{
				CLSCompliantAttribute cLSCompliantAttribute = new CLSCompliantAttribute(true);
				DefaultMemberAttribute defaultMemberAttribute = new DefaultMemberAttribute("Item");
				Attribute[] attributes = new Attribute[2] { cLSCompliantAttribute, defaultMemberAttribute };
				obj = new AttributeCollection(attributes);
			}
			Interlocked.CompareExchange(ref _staticAttributeCollection, obj, null);
			return _staticAttributeCollection as AttributeCollection;
		}

		string ICustomTypeDescriptor.GetClassName()
		{
			return GetType().ToString();
		}

		string ICustomTypeDescriptor.GetComponentName()
		{
			return null;
		}

		TypeConverter ICustomTypeDescriptor.GetConverter()
		{
			return new CollectionConverter();
		}

		EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
		{
			return null;
		}

		PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
		{
			return null;
		}

		object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
		{
			return null;
		}

		EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
		{
			return EventDescriptorCollection.Empty;
		}

		EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
		{
			return EventDescriptorCollection.Empty;
		}

		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
		{
			return PropertyDescriptorCollection.Empty;
		}

		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
		{
			return PropertyDescriptorCollection.Empty;
		}

		object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
		{
			throw new NotImplementedException();
		}

		public void Add(string keyword, object value)
		{
			this[keyword] = value;
		}

		public static void AppendKeyValuePair(StringBuilder builder, string keyword, string value, bool useOdbcRules)
		{
			if (builder == null)
			{
				throw new ArgumentNullException("builder");
			}
			if (keyword == null)
			{
				throw new ArgumentNullException("keyName");
			}
			if (keyword.Length == 0)
			{
				throw new ArgumentException("Empty keyword is not valid.");
			}
			if (builder.Length > 0)
			{
				builder.Append(';');
			}
			if (!useOdbcRules)
			{
				builder.Append(keyword.Replace("=", "=="));
			}
			else
			{
				builder.Append(keyword);
			}
			builder.Append('=');
			if (value == null || value.Length == 0)
			{
				return;
			}
			if (!useOdbcRules)
			{
				bool flag = value.IndexOf('"') > -1;
				bool flag2 = value.IndexOf('\'') > -1;
				if (flag && flag2)
				{
					builder.Append('"');
					builder.Append(value.Replace("\"", "\"\""));
					builder.Append('"');
				}
				else if (flag)
				{
					builder.Append('\'');
					builder.Append(value);
					builder.Append('\'');
				}
				else if (flag2 || value.IndexOf('=') > -1 || value.IndexOf(';') > -1)
				{
					builder.Append('"');
					builder.Append(value);
					builder.Append('"');
				}
				else if (ValueNeedsQuoting(value))
				{
					builder.Append('"');
					builder.Append(value);
					builder.Append('"');
				}
				else
				{
					builder.Append(value);
				}
				return;
			}
			int num = 0;
			bool flag3 = false;
			int length = value.Length;
			bool flag4 = false;
			int num2 = -1;
			for (int i = 0; i < length; i++)
			{
				int num3 = 0;
				num3 = ((i != length - 1) ? value[i + 1] : (-1));
				char c = value[i];
				switch (c)
				{
				case '{':
					num++;
					break;
				case '}':
					if (num3.Equals(c))
					{
						i++;
						continue;
					}
					num--;
					if (num3 != -1)
					{
						flag4 = true;
					}
					break;
				case ';':
					flag3 = true;
					break;
				}
				num2 = c;
			}
			if (value[0] == '{' && (num2 != 125 || (num == 0 && flag4)))
			{
				builder.Append('{');
				builder.Append(value.Replace("}", "}}"));
				builder.Append('}');
			}
			else if (string.Compare(keyword, "Driver", StringComparison.InvariantCultureIgnoreCase) == 0)
			{
				if (value[0] == '{' && num2 == 125 && !flag4)
				{
					builder.Append(value);
					return;
				}
				builder.Append('{');
				builder.Append(value.Replace("}", "}}"));
				builder.Append('}');
			}
			else if (value[0] == '{' && (num != 0 || num2 != 125) && flag4)
			{
				builder.Append('{');
				builder.Append(value.Replace("}", "}}"));
				builder.Append('}');
			}
			else if (value[0] != '{' && flag3)
			{
				builder.Append('{');
				builder.Append(value.Replace("}", "}}"));
				builder.Append('}');
			}
			else
			{
				builder.Append(value);
			}
		}

		public static void AppendKeyValuePair(StringBuilder builder, string keyword, string value)
		{
			AppendKeyValuePair(builder, keyword, value, false);
		}

		public virtual void Clear()
		{
			_dictionary.Clear();
		}

		public virtual bool ContainsKey(string keyword)
		{
			if (keyword == null)
			{
				throw new ArgumentNullException("keyword");
			}
			return _dictionary.ContainsKey(keyword);
		}

		public virtual bool EquivalentTo(DbConnectionStringBuilder connectionStringBuilder)
		{
			bool result = true;
			try
			{
				if (Count != connectionStringBuilder.Count)
				{
					result = false;
				}
				else
				{
					foreach (string key in Keys)
					{
						if (!this[key].Equals(connectionStringBuilder[key]))
						{
							result = false;
							break;
						}
					}
				}
			}
			catch (ArgumentException)
			{
				result = false;
			}
			return result;
		}

		[System.MonoTODO]
		protected virtual void GetProperties(Hashtable propertyDescriptors)
		{
			throw new NotImplementedException();
		}

		[System.MonoTODO]
		protected internal void ClearPropertyDescriptors()
		{
			throw new NotImplementedException();
		}

		public virtual bool Remove(string keyword)
		{
			if (keyword == null)
			{
				throw new ArgumentNullException("keyword");
			}
			return _dictionary.Remove(keyword);
		}

		public virtual bool ShouldSerialize(string keyword)
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return ConnectionString;
		}

		public virtual bool TryGetValue(string keyword, out object value)
		{
			bool flag = ContainsKey(keyword);
			if (flag)
			{
				value = this[keyword];
			}
			else
			{
				value = null;
			}
			return flag;
		}

		private static ArgumentException CreateInvalidKeywordException(string keyword)
		{
			return new ArgumentException("A keyword cannot contain control characters, leading semicolons or leading or trailing whitespace.", keyword);
		}

		private static ArgumentException CreateConnectionStringInvalidException(int index)
		{
			return new ArgumentException("Format of initialization string does not conform to specifications at index " + index + ".");
		}

		private static bool ValueNeedsQuoting(string value)
		{
			foreach (char c in value)
			{
				if (char.IsWhiteSpace(c))
				{
					return true;
				}
			}
			return false;
		}

		private void ParseConnectionString(string connectionString)
		{
			if (useOdbcRules)
			{
				ParseConnectionStringOdbc(connectionString);
			}
			else
			{
				ParseConnectionStringNonOdbc(connectionString);
			}
		}

		private void ParseConnectionStringOdbc(string connectionString)
		{
			bool flag = false;
			bool flag2 = false;
			bool flag3 = true;
			bool flag4 = false;
			string text = string.Empty;
			string empty = string.Empty;
			StringBuilder stringBuilder = new StringBuilder();
			int length = connectionString.Length;
			for (int i = 0; i < length; i++)
			{
				char c = connectionString[i];
				int num = ((i != length - 1) ? connectionString[i + 1] : (-1));
				switch (c)
				{
				case '{':
					if (flag3)
					{
						stringBuilder.Append(c);
						continue;
					}
					if (stringBuilder.Length == 0)
					{
						flag4 = true;
					}
					stringBuilder.Append(c);
					continue;
				case '}':
				{
					if (flag3 || !flag4)
					{
						stringBuilder.Append(c);
						continue;
					}
					if (num == -1)
					{
						stringBuilder.Append(c);
						flag4 = false;
						continue;
					}
					if (num.Equals(c))
					{
						stringBuilder.Append(c);
						stringBuilder.Append(c);
						i++;
						continue;
					}
					int num2 = NextNonWhitespaceChar(connectionString, i);
					if (num2 != -1 && (ushort)num2 != 59)
					{
						throw CreateConnectionStringInvalidException(num2);
					}
					stringBuilder.Append(c);
					flag4 = false;
					continue;
				}
				case ';':
					if (flag3 || flag4)
					{
						stringBuilder.Append(c);
						continue;
					}
					if (text.Length > 0 && stringBuilder.Length > 0)
					{
						empty = stringBuilder.ToString();
						text = text.ToLower().TrimEnd();
						this[text] = empty;
					}
					else if (stringBuilder.Length > 0)
					{
						throw CreateConnectionStringInvalidException(c);
					}
					flag3 = true;
					text = string.Empty;
					stringBuilder.Length = 0;
					continue;
				case '=':
					if (flag4 || !flag3)
					{
						stringBuilder.Append(c);
						continue;
					}
					text = stringBuilder.ToString();
					if (text.Length == 0)
					{
						throw CreateConnectionStringInvalidException(c);
					}
					stringBuilder.Length = 0;
					flag3 = false;
					continue;
				}
				if (flag2 || flag || flag4)
				{
					stringBuilder.Append(c);
				}
				else if (char.IsWhiteSpace(c))
				{
					if (stringBuilder.Length > 0)
					{
						int num3 = SkipTrailingWhitespace(connectionString, i);
						if (num3 == -1)
						{
							stringBuilder.Append(c);
						}
						else
						{
							i = num3;
						}
					}
				}
				else
				{
					stringBuilder.Append(c);
				}
			}
			if ((flag3 && stringBuilder.Length > 0) || flag2 || flag || flag4)
			{
				throw CreateConnectionStringInvalidException(length - 1);
			}
			if (text.Length > 0 && stringBuilder.Length > 0)
			{
				empty = stringBuilder.ToString();
				text = text.ToLower().TrimEnd();
				this[text] = empty;
			}
		}

		private void ParseConnectionStringNonOdbc(string connectionString)
		{
			bool flag = false;
			bool flag2 = false;
			bool flag3 = true;
			string text = string.Empty;
			string empty = string.Empty;
			StringBuilder stringBuilder = new StringBuilder();
			int length = connectionString.Length;
			for (int i = 0; i < length; i++)
			{
				char c = connectionString[i];
				int num = ((i != length - 1) ? connectionString[i + 1] : (-1));
				switch (c)
				{
				case '\'':
					if (flag3)
					{
						stringBuilder.Append(c);
					}
					else if (flag2)
					{
						stringBuilder.Append(c);
					}
					else if (flag)
					{
						if (num == -1)
						{
							flag = false;
						}
						else if (num.Equals(c))
						{
							stringBuilder.Append(c);
							i++;
						}
						else
						{
							int num3 = NextNonWhitespaceChar(connectionString, i);
							if (num3 != -1 && (ushort)num3 != 59)
							{
								throw CreateConnectionStringInvalidException(num3);
							}
							flag = false;
						}
						if (!flag)
						{
							empty = stringBuilder.ToString();
							text = text.ToLower().TrimEnd();
							this[text] = empty;
							flag3 = true;
							text = string.Empty;
							stringBuilder.Length = 0;
						}
					}
					else if (stringBuilder.Length == 0)
					{
						flag = true;
					}
					else
					{
						stringBuilder.Append(c);
					}
					continue;
				case '"':
					if (flag3)
					{
						stringBuilder.Append(c);
					}
					else if (flag)
					{
						stringBuilder.Append(c);
					}
					else if (flag2)
					{
						if (num == -1)
						{
							flag2 = false;
							continue;
						}
						if (num.Equals(c))
						{
							stringBuilder.Append(c);
							i++;
							continue;
						}
						int num2 = NextNonWhitespaceChar(connectionString, i);
						if (num2 != -1 && (ushort)num2 != 59)
						{
							throw CreateConnectionStringInvalidException(num2);
						}
						flag2 = false;
					}
					else if (stringBuilder.Length == 0)
					{
						flag2 = true;
					}
					else
					{
						stringBuilder.Append(c);
					}
					continue;
				case ';':
					if (flag3)
					{
						stringBuilder.Append(c);
						continue;
					}
					if (flag2 || flag)
					{
						stringBuilder.Append(c);
						continue;
					}
					if (text.Length > 0 && stringBuilder.Length > 0)
					{
						empty = stringBuilder.ToString();
						text = text.ToLower().TrimEnd();
						this[text] = empty;
					}
					else if (stringBuilder.Length > 0)
					{
						throw CreateConnectionStringInvalidException(c);
					}
					flag3 = true;
					text = string.Empty;
					stringBuilder.Length = 0;
					continue;
				case '=':
					if (flag2 || flag || !flag3)
					{
						stringBuilder.Append(c);
						continue;
					}
					if (num != -1 && num.Equals(c))
					{
						stringBuilder.Append(c);
						i++;
						continue;
					}
					text = stringBuilder.ToString();
					if (text.Length == 0)
					{
						throw CreateConnectionStringInvalidException(c);
					}
					stringBuilder.Length = 0;
					flag3 = false;
					continue;
				}
				if (flag2 || flag)
				{
					stringBuilder.Append(c);
				}
				else if (char.IsWhiteSpace(c))
				{
					if (stringBuilder.Length > 0)
					{
						int num4 = SkipTrailingWhitespace(connectionString, i);
						if (num4 == -1)
						{
							stringBuilder.Append(c);
						}
						else
						{
							i = num4;
						}
					}
				}
				else
				{
					stringBuilder.Append(c);
				}
			}
			if ((flag3 && stringBuilder.Length > 0) || flag2 || flag)
			{
				throw CreateConnectionStringInvalidException(length - 1);
			}
			if (text.Length > 0 && stringBuilder.Length > 0)
			{
				empty = stringBuilder.ToString();
				text = text.ToLower().TrimEnd();
				this[text] = empty;
			}
		}

		private static int SkipTrailingWhitespace(string value, int index)
		{
			int length = value.Length;
			for (int i = index + 1; i < length; i++)
			{
				char c = value[i];
				if (c == ';')
				{
					return i - 1;
				}
				if (!char.IsWhiteSpace(c))
				{
					return -1;
				}
			}
			return length - 1;
		}

		private static int NextNonWhitespaceChar(string value, int index)
		{
			int length = value.Length;
			for (int i = index + 1; i < length; i++)
			{
				char c = value[i];
				if (!char.IsWhiteSpace(c))
				{
					return c;
				}
			}
			return -1;
		}
	}
}

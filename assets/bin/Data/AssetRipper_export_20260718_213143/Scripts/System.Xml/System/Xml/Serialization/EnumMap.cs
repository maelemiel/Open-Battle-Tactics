using System.Globalization;
using System.Text;

namespace System.Xml.Serialization
{
	internal class EnumMap : ObjectMap
	{
		public class EnumMapMember
		{
			private readonly string _xmlName;

			private readonly string _enumName;

			private readonly long _value;

			private string _documentation;

			public string XmlName
			{
				get
				{
					return _xmlName;
				}
			}

			public string EnumName
			{
				get
				{
					return _enumName;
				}
			}

			public long Value
			{
				get
				{
					return _value;
				}
			}

			public string Documentation
			{
				get
				{
					return _documentation;
				}
				set
				{
					_documentation = value;
				}
			}

			public EnumMapMember(string xmlName, string enumName)
				: this(xmlName, enumName, 0L)
			{
			}

			public EnumMapMember(string xmlName, string enumName, long value)
			{
				_xmlName = xmlName;
				_enumName = enumName;
				_value = value;
			}
		}

		private readonly EnumMapMember[] _members;

		private readonly bool _isFlags;

		private readonly string[] _enumNames;

		private readonly string[] _xmlNames;

		private readonly long[] _values;

		public bool IsFlags
		{
			get
			{
				return _isFlags;
			}
		}

		public EnumMapMember[] Members
		{
			get
			{
				return _members;
			}
		}

		public string[] EnumNames
		{
			get
			{
				return _enumNames;
			}
		}

		public string[] XmlNames
		{
			get
			{
				return _xmlNames;
			}
		}

		public long[] Values
		{
			get
			{
				return _values;
			}
		}

		public EnumMap(EnumMapMember[] members, bool isFlags)
		{
			_members = members;
			_isFlags = isFlags;
			_enumNames = new string[_members.Length];
			_xmlNames = new string[_members.Length];
			_values = new long[_members.Length];
			for (int i = 0; i < _members.Length; i++)
			{
				EnumMapMember enumMapMember = _members[i];
				_enumNames[i] = enumMapMember.EnumName;
				_xmlNames[i] = enumMapMember.XmlName;
				_values[i] = enumMapMember.Value;
			}
		}

		public string GetXmlName(string typeName, object enumValue)
		{
			if (enumValue is string)
			{
				throw new InvalidCastException();
			}
			long num = 0L;
			try
			{
				num = ((IConvertible)enumValue).ToInt64(CultureInfo.CurrentCulture);
			}
			catch (FormatException)
			{
				throw new InvalidCastException();
			}
			for (int i = 0; i < Values.Length; i++)
			{
				if (Values[i] == num)
				{
					return XmlNames[i];
				}
			}
			if (IsFlags && num == 0L)
			{
				return string.Empty;
			}
			string text = string.Empty;
			if (IsFlags)
			{
				text = XmlCustomFormatter.FromEnum(num, XmlNames, Values, typeName);
			}
			if (text.Length == 0)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "'{0}' is not a valid value for {1}.", num, typeName));
			}
			return text;
		}

		public string GetEnumName(string typeName, string xmlName)
		{
			if (_isFlags)
			{
				xmlName = xmlName.Trim();
				if (xmlName.Length == 0)
				{
					return "0";
				}
				StringBuilder stringBuilder = new StringBuilder();
				string[] array = xmlName.Split(null);
				string[] array2 = array;
				foreach (string text in array2)
				{
					if (text == string.Empty)
					{
						continue;
					}
					string text2 = null;
					for (int j = 0; j < XmlNames.Length; j++)
					{
						if (XmlNames[j] == text)
						{
							text2 = EnumNames[j];
							break;
						}
					}
					if (text2 != null)
					{
						if (stringBuilder.Length > 0)
						{
							stringBuilder.Append(',');
						}
						stringBuilder.Append(text2);
						continue;
					}
					throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "'{0}' is not a valid value for {1}.", text, typeName));
				}
				return stringBuilder.ToString();
			}
			EnumMapMember[] members = _members;
			foreach (EnumMapMember enumMapMember in members)
			{
				if (enumMapMember.XmlName == xmlName)
				{
					return enumMapMember.EnumName;
				}
			}
			return null;
		}
	}
}

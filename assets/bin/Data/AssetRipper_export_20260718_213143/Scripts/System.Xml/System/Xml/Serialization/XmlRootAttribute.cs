using System.Text;

namespace System.Xml.Serialization
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface | AttributeTargets.ReturnValue)]
	public class XmlRootAttribute : Attribute
	{
		private string dataType;

		private string elementName;

		private bool isNullable = true;

		private bool isNullableSpecified;

		private string ns;

		public string DataType
		{
			get
			{
				if (dataType == null)
				{
					return string.Empty;
				}
				return dataType;
			}
			set
			{
				dataType = value;
			}
		}

		public string ElementName
		{
			get
			{
				if (elementName == null)
				{
					return string.Empty;
				}
				return elementName;
			}
			set
			{
				elementName = value;
			}
		}

		public bool IsNullable
		{
			get
			{
				return isNullable;
			}
			set
			{
				isNullableSpecified = true;
				isNullable = value;
			}
		}

		public bool IsNullableSpecified
		{
			get
			{
				return isNullableSpecified;
			}
		}

		public string Namespace
		{
			get
			{
				return ns;
			}
			set
			{
				ns = value;
			}
		}

		internal string Key
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				AddKeyHash(stringBuilder);
				return stringBuilder.ToString();
			}
		}

		public XmlRootAttribute()
		{
		}

		public XmlRootAttribute(string elementName)
		{
			this.elementName = elementName;
		}

		internal void AddKeyHash(StringBuilder sb)
		{
			sb.Append("XRA ");
			KeyHelper.AddField(sb, 1, ns);
			KeyHelper.AddField(sb, 2, elementName);
			KeyHelper.AddField(sb, 3, dataType);
			KeyHelper.AddField(sb, 4, isNullable);
			sb.Append('|');
		}
	}
}

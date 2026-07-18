using System.Text;
using System.Xml.Schema;

namespace System.Xml.Serialization
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
	public class XmlAttributeAttribute : Attribute
	{
		private string attributeName;

		private string dataType;

		private Type type;

		private XmlSchemaForm form;

		private string ns;

		public string AttributeName
		{
			get
			{
				if (attributeName == null)
				{
					return string.Empty;
				}
				return attributeName;
			}
			set
			{
				attributeName = value;
			}
		}

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

		public XmlSchemaForm Form
		{
			get
			{
				return form;
			}
			set
			{
				form = value;
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

		public Type Type
		{
			get
			{
				return type;
			}
			set
			{
				type = value;
			}
		}

		public XmlAttributeAttribute()
		{
		}

		public XmlAttributeAttribute(string attributeName)
		{
			this.attributeName = attributeName;
		}

		public XmlAttributeAttribute(Type type)
		{
			this.type = type;
		}

		public XmlAttributeAttribute(string attributeName, Type type)
		{
			this.attributeName = attributeName;
			this.type = type;
		}

		internal void AddKeyHash(StringBuilder sb)
		{
			sb.Append("XAA ");
			KeyHelper.AddField(sb, 1, ns);
			KeyHelper.AddField(sb, 2, attributeName);
			KeyHelper.AddField(sb, 3, form.ToString(), XmlSchemaForm.None.ToString());
			KeyHelper.AddField(sb, 4, dataType);
			KeyHelper.AddField(sb, 5, type);
			sb.Append('|');
		}
	}
}

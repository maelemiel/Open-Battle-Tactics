using System.Text;
using System.Xml.Schema;

namespace System.Xml.Serialization
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = true)]
	public class XmlElementAttribute : Attribute
	{
		private string dataType;

		private string elementName;

		private XmlSchemaForm form;

		private string ns;

		private bool isNullable;

		private bool isNullableSpecified;

		private Type type;

		private int order = -1;

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

		internal bool IsNullableSpecified
		{
			get
			{
				return isNullableSpecified;
			}
		}

		[System.MonoTODO]
		public int Order
		{
			get
			{
				return order;
			}
			set
			{
				order = value;
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

		public XmlElementAttribute()
		{
		}

		public XmlElementAttribute(string elementName)
		{
			this.elementName = elementName;
		}

		public XmlElementAttribute(Type type)
		{
			this.type = type;
		}

		public XmlElementAttribute(string elementName, Type type)
		{
			this.elementName = elementName;
			this.type = type;
		}

		internal void AddKeyHash(StringBuilder sb)
		{
			sb.Append("XEA ");
			KeyHelper.AddField(sb, 1, ns);
			KeyHelper.AddField(sb, 2, elementName);
			KeyHelper.AddField(sb, 3, form.ToString(), XmlSchemaForm.None.ToString());
			KeyHelper.AddField(sb, 4, dataType);
			KeyHelper.AddField(sb, 5, type);
			KeyHelper.AddField(sb, 6, isNullable);
			sb.Append('|');
		}
	}
}

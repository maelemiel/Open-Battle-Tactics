using System.Text;
using System.Xml.Schema;

namespace System.Xml.Serialization
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
	public class XmlArrayAttribute : Attribute
	{
		private string elementName;

		private XmlSchemaForm form;

		private bool isNullable;

		private string ns;

		private int order = -1;

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

		public bool IsNullable
		{
			get
			{
				return isNullable;
			}
			set
			{
				isNullable = value;
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

		public XmlArrayAttribute()
		{
		}

		public XmlArrayAttribute(string elementName)
		{
			this.elementName = elementName;
		}

		internal void AddKeyHash(StringBuilder sb)
		{
			sb.Append("XAAT ");
			KeyHelper.AddField(sb, 1, ns);
			KeyHelper.AddField(sb, 2, elementName);
			KeyHelper.AddField(sb, 3, form.ToString(), XmlSchemaForm.None.ToString());
			KeyHelper.AddField(sb, 4, isNullable);
			sb.Append('|');
		}
	}
}

using System.Text;

namespace System.Xml.Serialization
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = true)]
	public class XmlAnyElementAttribute : Attribute
	{
		private string elementName;

		private string ns;

		private bool isNamespaceSpecified;

		private int order = -1;

		public string Name
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

		public string Namespace
		{
			get
			{
				return ns;
			}
			set
			{
				isNamespaceSpecified = true;
				ns = value;
			}
		}

		internal bool NamespaceSpecified
		{
			get
			{
				return isNamespaceSpecified;
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

		public XmlAnyElementAttribute()
		{
		}

		public XmlAnyElementAttribute(string name)
		{
			elementName = name;
		}

		public XmlAnyElementAttribute(string name, string ns)
		{
			elementName = name;
			this.ns = ns;
		}

		internal void AddKeyHash(StringBuilder sb)
		{
			sb.Append("XAEA ");
			KeyHelper.AddField(sb, 1, ns);
			KeyHelper.AddField(sb, 2, elementName);
			sb.Append('|');
		}
	}
}

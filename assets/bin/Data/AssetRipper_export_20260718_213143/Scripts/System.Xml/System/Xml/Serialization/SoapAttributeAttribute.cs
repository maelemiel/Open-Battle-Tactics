using System.Text;

namespace System.Xml.Serialization
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
	public class SoapAttributeAttribute : Attribute
	{
		private string attrName;

		private string dataType;

		private string ns;

		public string AttributeName
		{
			get
			{
				if (attrName == null)
				{
					return string.Empty;
				}
				return attrName;
			}
			set
			{
				attrName = value;
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

		public SoapAttributeAttribute()
		{
		}

		public SoapAttributeAttribute(string attrName)
		{
			this.attrName = attrName;
		}

		internal void AddKeyHash(StringBuilder sb)
		{
			sb.Append("SAA ");
			KeyHelper.AddField(sb, 1, attrName);
			KeyHelper.AddField(sb, 2, dataType);
			KeyHelper.AddField(sb, 3, ns);
			sb.Append("|");
		}
	}
}

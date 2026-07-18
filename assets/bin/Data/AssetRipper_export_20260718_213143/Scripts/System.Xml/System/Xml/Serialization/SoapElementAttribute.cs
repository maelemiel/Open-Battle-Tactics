using System.Text;

namespace System.Xml.Serialization
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
	public class SoapElementAttribute : Attribute
	{
		private string dataType;

		private string elementName;

		private bool isNullable;

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
				isNullable = value;
			}
		}

		public SoapElementAttribute()
		{
		}

		public SoapElementAttribute(string elementName)
		{
			this.elementName = elementName;
		}

		internal void AddKeyHash(StringBuilder sb)
		{
			sb.Append("SEA ");
			KeyHelper.AddField(sb, 1, elementName);
			KeyHelper.AddField(sb, 2, dataType);
			KeyHelper.AddField(sb, 3, isNullable);
			sb.Append('|');
		}
	}
}

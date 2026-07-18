using System.Text;

namespace System.Xml.Serialization
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
	public class XmlTextAttribute : Attribute
	{
		private string dataType;

		private Type type;

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

		public XmlTextAttribute()
		{
		}

		public XmlTextAttribute(Type type)
		{
			this.type = type;
		}

		internal void AddKeyHash(StringBuilder sb)
		{
			sb.Append("XTXA ");
			KeyHelper.AddField(sb, 1, type);
			KeyHelper.AddField(sb, 2, dataType);
			sb.Append('|');
		}
	}
}

using System.Text;

namespace System.Xml.Serialization
{
	[AttributeUsage(AttributeTargets.Field)]
	public class XmlEnumAttribute : Attribute
	{
		private string name;

		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
			}
		}

		public XmlEnumAttribute()
		{
		}

		public XmlEnumAttribute(string name)
		{
			this.name = name;
		}

		internal void AddKeyHash(StringBuilder sb)
		{
			sb.Append("XENA ");
			KeyHelper.AddField(sb, 1, name);
			sb.Append('|');
		}
	}
}

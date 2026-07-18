using System.Text;

namespace System.Xml.Serialization
{
	[AttributeUsage(AttributeTargets.Field)]
	public class SoapEnumAttribute : Attribute
	{
		private string name;

		public string Name
		{
			get
			{
				if (name == null)
				{
					return string.Empty;
				}
				return name;
			}
			set
			{
				name = value;
			}
		}

		public SoapEnumAttribute()
		{
		}

		public SoapEnumAttribute(string name)
		{
			this.name = name;
		}

		internal void AddKeyHash(StringBuilder sb)
		{
			sb.Append("SENA ");
			KeyHelper.AddField(sb, 1, name);
			sb.Append('|');
		}
	}
}

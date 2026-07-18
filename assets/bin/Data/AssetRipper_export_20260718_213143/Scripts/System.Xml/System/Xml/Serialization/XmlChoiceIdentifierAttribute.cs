using System.Reflection;
using System.Text;

namespace System.Xml.Serialization
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
	public class XmlChoiceIdentifierAttribute : Attribute
	{
		private string memberName;

		public string MemberName
		{
			get
			{
				if (memberName == null)
				{
					return string.Empty;
				}
				return memberName;
			}
			set
			{
				memberName = value;
			}
		}

		internal MemberInfo MemberInfo { get; set; }

		public XmlChoiceIdentifierAttribute()
		{
		}

		public XmlChoiceIdentifierAttribute(string name)
		{
			memberName = name;
		}

		internal void AddKeyHash(StringBuilder sb)
		{
			sb.Append("XCA ");
			KeyHelper.AddField(sb, 1, memberName);
			sb.Append('|');
		}
	}
}

using System.Text;

namespace System.Xml.Serialization
{
	public class XmlReflectionMember
	{
		private bool isReturnValue;

		private string memberName;

		private Type memberType;

		private bool overrideIsNullable;

		private SoapAttributes soapAttributes;

		private XmlAttributes xmlAttributes;

		private Type declaringType;

		public bool IsReturnValue
		{
			get
			{
				return isReturnValue;
			}
			set
			{
				isReturnValue = value;
			}
		}

		public string MemberName
		{
			get
			{
				return memberName;
			}
			set
			{
				memberName = value;
			}
		}

		public Type MemberType
		{
			get
			{
				return memberType;
			}
			set
			{
				memberType = value;
			}
		}

		public bool OverrideIsNullable
		{
			get
			{
				return overrideIsNullable;
			}
			set
			{
				overrideIsNullable = value;
			}
		}

		public SoapAttributes SoapAttributes
		{
			get
			{
				if (soapAttributes == null)
				{
					soapAttributes = new SoapAttributes();
				}
				return soapAttributes;
			}
			set
			{
				soapAttributes = value;
			}
		}

		public XmlAttributes XmlAttributes
		{
			get
			{
				if (xmlAttributes == null)
				{
					xmlAttributes = new XmlAttributes();
				}
				return xmlAttributes;
			}
			set
			{
				xmlAttributes = value;
			}
		}

		internal Type DeclaringType
		{
			get
			{
				return declaringType;
			}
			set
			{
				declaringType = value;
			}
		}

		public XmlReflectionMember()
		{
		}

		internal XmlReflectionMember(string name, Type type, XmlAttributes attributes)
		{
			memberName = name;
			memberType = type;
			xmlAttributes = attributes;
		}

		internal XmlReflectionMember(string name, Type type, SoapAttributes attributes)
		{
			memberName = name;
			memberType = type;
			soapAttributes = attributes;
		}

		internal void AddKeyHash(StringBuilder sb)
		{
			sb.Append("XRM ");
			KeyHelper.AddField(sb, 1, isReturnValue);
			KeyHelper.AddField(sb, 1, memberName);
			KeyHelper.AddField(sb, 1, memberType);
			KeyHelper.AddField(sb, 1, overrideIsNullable);
			if (soapAttributes != null)
			{
				soapAttributes.AddKeyHash(sb);
			}
			if (xmlAttributes != null)
			{
				xmlAttributes.AddKeyHash(sb);
			}
			sb.Append('|');
		}
	}
}

using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace System.Xml.Serialization
{
	public class SoapAttributes
	{
		private SoapAttributeAttribute soapAttribute;

		private object soapDefaultValue = DBNull.Value;

		private SoapElementAttribute soapElement;

		private SoapEnumAttribute soapEnum;

		private bool soapIgnore;

		private SoapTypeAttribute soapType;

		public SoapAttributeAttribute SoapAttribute
		{
			get
			{
				return soapAttribute;
			}
			set
			{
				soapAttribute = value;
			}
		}

		public object SoapDefaultValue
		{
			get
			{
				return soapDefaultValue;
			}
			set
			{
				soapDefaultValue = value;
			}
		}

		public SoapElementAttribute SoapElement
		{
			get
			{
				return soapElement;
			}
			set
			{
				soapElement = value;
			}
		}

		public SoapEnumAttribute SoapEnum
		{
			get
			{
				return soapEnum;
			}
			set
			{
				soapEnum = value;
			}
		}

		public bool SoapIgnore
		{
			get
			{
				return soapIgnore;
			}
			set
			{
				soapIgnore = value;
			}
		}

		public SoapTypeAttribute SoapType
		{
			get
			{
				return soapType;
			}
			set
			{
				soapType = value;
			}
		}

		public SoapAttributes()
		{
		}

		public SoapAttributes(ICustomAttributeProvider provider)
		{
			object[] customAttributes = provider.GetCustomAttributes(false);
			object[] array = customAttributes;
			foreach (object obj in array)
			{
				if (obj is SoapAttributeAttribute)
				{
					soapAttribute = (SoapAttributeAttribute)obj;
				}
				else if (obj is DefaultValueAttribute)
				{
					soapDefaultValue = ((DefaultValueAttribute)obj).Value;
				}
				else if (obj is SoapElementAttribute)
				{
					soapElement = (SoapElementAttribute)obj;
				}
				else if (obj is SoapEnumAttribute)
				{
					soapEnum = (SoapEnumAttribute)obj;
				}
				else if (obj is SoapIgnoreAttribute)
				{
					soapIgnore = true;
				}
				else if (obj is SoapTypeAttribute)
				{
					soapType = (SoapTypeAttribute)obj;
				}
			}
		}

		internal void AddKeyHash(StringBuilder sb)
		{
			sb.Append("SA ");
			if (soapIgnore)
			{
				sb.Append('i');
			}
			if (soapAttribute != null)
			{
				soapAttribute.AddKeyHash(sb);
			}
			if (soapElement != null)
			{
				soapElement.AddKeyHash(sb);
			}
			if (soapEnum != null)
			{
				soapEnum.AddKeyHash(sb);
			}
			if (soapType != null)
			{
				soapType.AddKeyHash(sb);
			}
			if (soapDefaultValue == null)
			{
				sb.Append("n");
			}
			else if (!(soapDefaultValue is DBNull))
			{
				string text = XmlCustomFormatter.ToXmlString(TypeTranslator.GetTypeData(soapDefaultValue.GetType()), soapDefaultValue);
				sb.Append("v" + text);
			}
			sb.Append("|");
		}
	}
}

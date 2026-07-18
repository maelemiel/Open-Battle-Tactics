using System.Collections;
using System.Text;

namespace System.Xml.Serialization
{
	public class SoapAttributeOverrides
	{
		private Hashtable overrides;

		public SoapAttributes this[Type type]
		{
			get
			{
				return this[type, string.Empty];
			}
		}

		public SoapAttributes this[Type type, string member]
		{
			get
			{
				return (SoapAttributes)overrides[GetKey(type, member)];
			}
		}

		public SoapAttributeOverrides()
		{
			overrides = new Hashtable();
		}

		public void Add(Type type, SoapAttributes attributes)
		{
			Add(type, string.Empty, attributes);
		}

		public void Add(Type type, string member, SoapAttributes attributes)
		{
			if (overrides[GetKey(type, member)] != null)
			{
				throw new Exception("The attributes for the given type and Member already exist in the collection");
			}
			overrides.Add(GetKey(type, member), attributes);
		}

		private TypeMember GetKey(Type type, string member)
		{
			return new TypeMember(type, member);
		}

		internal void AddKeyHash(StringBuilder sb)
		{
			sb.Append("SAO ");
			foreach (DictionaryEntry @override in overrides)
			{
				SoapAttributes soapAttributes = (SoapAttributes)overrides[@override.Key];
				sb.Append(@override.Key.ToString()).Append(' ');
				soapAttributes.AddKeyHash(sb);
			}
			sb.Append("|");
		}
	}
}

using System.Text;

namespace System.Xml.Serialization
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface)]
	public class XmlTypeAttribute : Attribute
	{
		private bool includeInSchema = true;

		private string ns;

		private string typeName;

		private bool anonymousType;

		public bool AnonymousType
		{
			get
			{
				return anonymousType;
			}
			set
			{
				anonymousType = value;
			}
		}

		public bool IncludeInSchema
		{
			get
			{
				return includeInSchema;
			}
			set
			{
				includeInSchema = value;
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

		public string TypeName
		{
			get
			{
				if (typeName == null)
				{
					return string.Empty;
				}
				return typeName;
			}
			set
			{
				typeName = value;
			}
		}

		public XmlTypeAttribute()
		{
		}

		public XmlTypeAttribute(string typeName)
		{
			this.typeName = typeName;
		}

		internal void AddKeyHash(StringBuilder sb)
		{
			sb.Append("XTA ");
			KeyHelper.AddField(sb, 1, ns);
			KeyHelper.AddField(sb, 2, typeName);
			KeyHelper.AddField(sb, 4, includeInSchema);
			sb.Append('|');
		}
	}
}

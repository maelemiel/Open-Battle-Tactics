using System.Text;

namespace System.Xml.Serialization
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface)]
	public class SoapTypeAttribute : Attribute
	{
		private string ns;

		private string typeName;

		private bool includeInSchema = true;

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

		public SoapTypeAttribute()
		{
		}

		public SoapTypeAttribute(string typeName)
		{
			this.typeName = typeName;
		}

		public SoapTypeAttribute(string typeName, string ns)
		{
			this.typeName = typeName;
			this.ns = ns;
		}

		internal void AddKeyHash(StringBuilder sb)
		{
			sb.Append("STA ");
			KeyHelper.AddField(sb, 1, ns);
			KeyHelper.AddField(sb, 2, typeName);
			KeyHelper.AddField(sb, 3, includeInSchema);
			sb.Append('|');
		}
	}
}

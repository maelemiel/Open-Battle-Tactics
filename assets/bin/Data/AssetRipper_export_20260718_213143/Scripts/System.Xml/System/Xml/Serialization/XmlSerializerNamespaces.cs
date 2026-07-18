using System.Collections.Specialized;

namespace System.Xml.Serialization
{
	public class XmlSerializerNamespaces
	{
		private ListDictionary namespaces;

		public int Count
		{
			get
			{
				return namespaces.Count;
			}
		}

		internal ListDictionary Namespaces
		{
			get
			{
				return namespaces;
			}
		}

		public XmlSerializerNamespaces()
		{
			namespaces = new ListDictionary();
		}

		public XmlSerializerNamespaces(XmlQualifiedName[] namespaces)
			: this()
		{
			foreach (XmlQualifiedName xmlQualifiedName in namespaces)
			{
				this.namespaces[xmlQualifiedName.Name] = xmlQualifiedName;
			}
		}

		public XmlSerializerNamespaces(XmlSerializerNamespaces namespaces)
			: this(namespaces.ToArray())
		{
		}

		public void Add(string prefix, string ns)
		{
			XmlQualifiedName xmlQualifiedName = new XmlQualifiedName(prefix, ns);
			namespaces[xmlQualifiedName.Name] = xmlQualifiedName;
		}

		public XmlQualifiedName[] ToArray()
		{
			XmlQualifiedName[] array = new XmlQualifiedName[namespaces.Count];
			namespaces.Values.CopyTo(array, 0);
			return array;
		}

		internal string GetPrefix(string Ns)
		{
			foreach (string key in namespaces.Keys)
			{
				if (Ns == ((XmlQualifiedName)namespaces[key]).Namespace)
				{
					return key;
				}
			}
			return null;
		}
	}
}

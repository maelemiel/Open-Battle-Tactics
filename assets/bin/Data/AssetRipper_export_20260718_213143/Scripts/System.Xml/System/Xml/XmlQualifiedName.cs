namespace System.Xml
{
	[Serializable]
	public class XmlQualifiedName
	{
		public static readonly XmlQualifiedName Empty = new XmlQualifiedName();

		private readonly string name;

		private readonly string ns;

		private readonly int hash;

		public bool IsEmpty
		{
			get
			{
				return name.Length == 0 && ns.Length == 0;
			}
		}

		public string Name
		{
			get
			{
				return name;
			}
		}

		public string Namespace
		{
			get
			{
				return ns;
			}
		}

		public XmlQualifiedName()
			: this(string.Empty, string.Empty)
		{
		}

		public XmlQualifiedName(string name)
			: this(name, string.Empty)
		{
		}

		public XmlQualifiedName(string name, string ns)
		{
			this.name = ((name != null) ? name : string.Empty);
			this.ns = ((ns != null) ? ns : string.Empty);
			hash = this.name.GetHashCode() ^ this.ns.GetHashCode();
		}

		public override bool Equals(object other)
		{
			return this == other as XmlQualifiedName;
		}

		public override int GetHashCode()
		{
			return hash;
		}

		public override string ToString()
		{
			if (ns == string.Empty)
			{
				return name;
			}
			return ns + ":" + name;
		}

		public static string ToString(string name, string ns)
		{
			if (ns == string.Empty)
			{
				return name;
			}
			return ns + ":" + name;
		}

		internal static XmlQualifiedName Parse(string name, IXmlNamespaceResolver resolver)
		{
			return Parse(name, resolver, false);
		}

		internal static XmlQualifiedName Parse(string name, IXmlNamespaceResolver resolver, bool considerDefaultNamespace)
		{
			int num = name.IndexOf(':');
			if (num < 0 && !considerDefaultNamespace)
			{
				return new XmlQualifiedName(name);
			}
			string prefix = ((num >= 0) ? name.Substring(0, num) : string.Empty);
			string text = ((num >= 0) ? name.Substring(num + 1) : name);
			string text2 = resolver.LookupNamespace(prefix);
			if (text2 == null)
			{
				throw new ArgumentException("Invalid qualified name.");
			}
			return new XmlQualifiedName(text, text2);
		}

		internal static XmlQualifiedName Parse(string name, XmlReader reader)
		{
			int num = name.IndexOf(':');
			if (num < 0)
			{
				return new XmlQualifiedName(name);
			}
			string text = reader.LookupNamespace(name.Substring(0, num));
			if (text == null)
			{
				throw new ArgumentException("Invalid qualified name.");
			}
			return new XmlQualifiedName(name.Substring(num + 1), text);
		}

		public static bool operator ==(XmlQualifiedName a, XmlQualifiedName b)
		{
			if ((object)a == b)
			{
				return true;
			}
			if ((object)a == null || (object)b == null)
			{
				return false;
			}
			return a.hash == b.hash && a.name == b.name && a.ns == b.ns;
		}

		public static bool operator !=(XmlQualifiedName a, XmlQualifiedName b)
		{
			return !(a == b);
		}
	}
}

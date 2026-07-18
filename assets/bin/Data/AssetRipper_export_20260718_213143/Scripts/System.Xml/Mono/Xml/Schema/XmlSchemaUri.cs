using System;

namespace Mono.Xml.Schema
{
	internal class XmlSchemaUri : Uri
	{
		public string value;

		public XmlSchemaUri(string src)
			: this(src, HasValidScheme(src))
		{
		}

		private XmlSchemaUri(string src, bool formal)
			: base((!formal) ? ("anyuri:" + src) : src, !formal)
		{
			value = src;
		}

		private static bool HasValidScheme(string src)
		{
			int num = src.IndexOf(':');
			if (num < 0)
			{
				return false;
			}
			for (int i = 0; i < num; i++)
			{
				switch (src[i])
				{
				case '+':
				case '-':
				case '.':
					continue;
				}
				if (char.IsLetterOrDigit(src[i]))
				{
					continue;
				}
				return false;
			}
			return true;
		}

		public override bool Equals(object obj)
		{
			if (obj is XmlSchemaUri)
			{
				return (XmlSchemaUri)obj == this;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return value.GetHashCode();
		}

		public override string ToString()
		{
			return value;
		}

		public static bool operator ==(XmlSchemaUri v1, XmlSchemaUri v2)
		{
			return v1.value == v2.value;
		}

		public static bool operator !=(XmlSchemaUri v1, XmlSchemaUri v2)
		{
			return v1.value != v2.value;
		}
	}
}

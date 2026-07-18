using System.Collections.Specialized;
using System.Xml.Schema;

namespace Mono.Xml.Schema
{
	internal class XsdWildcard
	{
		private XmlSchemaObject xsobj;

		public XmlSchemaContentProcessing ResolvedProcessing;

		public string TargetNamespace;

		public bool SkipCompile;

		public bool HasValueAny;

		public bool HasValueLocal;

		public bool HasValueOther;

		public bool HasValueTargetNamespace;

		public StringCollection ResolvedNamespaces;

		public XsdWildcard(XmlSchemaObject wildcard)
		{
			xsobj = wildcard;
		}

		private void Reset()
		{
			HasValueAny = false;
			HasValueLocal = false;
			HasValueOther = false;
			HasValueTargetNamespace = false;
			ResolvedNamespaces = new StringCollection();
		}

		public void Compile(string nss, ValidationEventHandler h, XmlSchema schema)
		{
			if (SkipCompile)
			{
				return;
			}
			Reset();
			int num = 0;
			string list = ((nss != null) ? nss : "##any");
			string[] array = XmlSchemaUtil.SplitList(list);
			foreach (string text in array)
			{
				switch (text)
				{
				case "##any":
					if (HasValueAny)
					{
						xsobj.error(h, "Multiple specification of ##any was found.");
					}
					num |= 1;
					HasValueAny = true;
					continue;
				case "##other":
					if (HasValueOther)
					{
						xsobj.error(h, "Multiple specification of ##other was found.");
					}
					num |= 2;
					HasValueOther = true;
					continue;
				case "##targetNamespace":
					if (HasValueTargetNamespace)
					{
						xsobj.error(h, "Multiple specification of ##targetNamespace was found.");
					}
					num |= 4;
					HasValueTargetNamespace = true;
					continue;
				case "##local":
					if (HasValueLocal)
					{
						xsobj.error(h, "Multiple specification of ##local was found.");
					}
					num |= 8;
					HasValueLocal = true;
					continue;
				}
				if (!XmlSchemaUtil.CheckAnyUri(text))
				{
					xsobj.error(h, "the namespace is not a valid anyURI");
					continue;
				}
				if (ResolvedNamespaces.Contains(text))
				{
					xsobj.error(h, "Multiple specification of '" + text + "' was found.");
					continue;
				}
				num |= 0x10;
				ResolvedNamespaces.Add(text);
			}
			if ((num & 1) == 1 && num != 1)
			{
				xsobj.error(h, "##any if present must be the only namespace attribute");
			}
			if ((num & 2) == 2 && num != 2)
			{
				xsobj.error(h, "##other if present must be the only namespace attribute");
			}
		}

		public bool ExamineAttributeWildcardIntersection(XmlSchemaAny other, ValidationEventHandler h, XmlSchema schema)
		{
			if (HasValueAny == other.HasValueAny && HasValueLocal == other.HasValueLocal && HasValueOther == other.HasValueOther && HasValueTargetNamespace == other.HasValueTargetNamespace && ResolvedProcessing == other.ResolvedProcessContents)
			{
				bool flag = false;
				for (int i = 0; i < ResolvedNamespaces.Count; i++)
				{
					if (!other.ResolvedNamespaces.Contains(ResolvedNamespaces[i]))
					{
						flag = true;
					}
				}
				if (!flag)
				{
					return false;
				}
			}
			if (HasValueAny)
			{
				return !other.HasValueAny && !other.HasValueLocal && !other.HasValueOther && !other.HasValueTargetNamespace && other.ResolvedNamespaces.Count == 0;
			}
			if (other.HasValueAny)
			{
				return !HasValueAny && !HasValueLocal && !HasValueOther && !HasValueTargetNamespace && ResolvedNamespaces.Count == 0;
			}
			if (HasValueOther && other.HasValueOther && TargetNamespace != other.TargetNamespace)
			{
				return false;
			}
			if (HasValueOther)
			{
				if (other.HasValueLocal && TargetNamespace != string.Empty)
				{
					return false;
				}
				if (other.HasValueTargetNamespace && TargetNamespace != other.TargetNamespace)
				{
					return false;
				}
				return other.ValidateWildcardAllowsNamespaceName(TargetNamespace, h, schema, false);
			}
			if (other.HasValueOther)
			{
				if (HasValueLocal && other.TargetNamespace != string.Empty)
				{
					return false;
				}
				if (HasValueTargetNamespace && other.TargetNamespace != TargetNamespace)
				{
					return false;
				}
				return ValidateWildcardAllowsNamespaceName(other.TargetNamespace, h, schema, false);
			}
			if (ResolvedNamespaces.Count > 0)
			{
				for (int j = 0; j < ResolvedNamespaces.Count; j++)
				{
					if (other.ResolvedNamespaces.Contains(ResolvedNamespaces[j]))
					{
						return false;
					}
				}
			}
			return true;
		}

		public bool ValidateWildcardAllowsNamespaceName(string ns, ValidationEventHandler h, XmlSchema schema, bool raiseError)
		{
			if (HasValueAny)
			{
				return true;
			}
			if (HasValueOther && ns != TargetNamespace)
			{
				return true;
			}
			if (HasValueTargetNamespace && ns == TargetNamespace)
			{
				return true;
			}
			if (HasValueLocal && ns == string.Empty)
			{
				return true;
			}
			for (int i = 0; i < ResolvedNamespaces.Count; i++)
			{
				if (ns == ResolvedNamespaces[i])
				{
					return true;
				}
			}
			if (raiseError)
			{
				xsobj.error(h, "This wildcard does not allow the namespace: " + ns);
			}
			return false;
		}

		internal void ValidateWildcardSubset(XsdWildcard other, ValidationEventHandler h, XmlSchema schema)
		{
			ValidateWildcardSubset(other, h, schema, true);
		}

		internal bool ValidateWildcardSubset(XsdWildcard other, ValidationEventHandler h, XmlSchema schema, bool raiseError)
		{
			if (other.HasValueAny)
			{
				return true;
			}
			if (HasValueOther && other.HasValueOther && (TargetNamespace == other.TargetNamespace || other.TargetNamespace == null || other.TargetNamespace == string.Empty))
			{
				return true;
			}
			if (HasValueAny)
			{
				if (raiseError)
				{
					xsobj.error(h, "Invalid wildcard subset was found.");
				}
				return false;
			}
			if (other.HasValueOther)
			{
				if ((HasValueTargetNamespace && other.TargetNamespace == TargetNamespace) || (HasValueLocal && (other.TargetNamespace == null || other.TargetNamespace.Length == 0)))
				{
					if (raiseError)
					{
						xsobj.error(h, "Invalid wildcard subset was found.");
					}
					return false;
				}
				for (int i = 0; i < ResolvedNamespaces.Count; i++)
				{
					if (ResolvedNamespaces[i] == other.TargetNamespace)
					{
						if (raiseError)
						{
							xsobj.error(h, "Invalid wildcard subset was found.");
						}
						return false;
					}
				}
			}
			else
			{
				if ((HasValueLocal && !other.HasValueLocal) || (HasValueTargetNamespace && !other.HasValueTargetNamespace))
				{
					if (raiseError)
					{
						xsobj.error(h, "Invalid wildcard subset was found.");
					}
					return false;
				}
				if (HasValueOther)
				{
					if (raiseError)
					{
						xsobj.error(h, "Invalid wildcard subset was found.");
					}
					return false;
				}
				for (int j = 0; j < ResolvedNamespaces.Count; j++)
				{
					if (!other.ResolvedNamespaces.Contains(ResolvedNamespaces[j]))
					{
						if (raiseError)
						{
							xsobj.error(h, "Invalid wildcard subset was found.");
						}
						return false;
					}
				}
			}
			return true;
		}
	}
}

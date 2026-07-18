using System.Collections;
using System.Collections.Generic;

namespace System.Xml
{
	public class XmlNamespaceManager : IEnumerable, IXmlNamespaceResolver
	{
		private struct NsDecl
		{
			public string Prefix;

			public string Uri;
		}

		private struct NsScope
		{
			public int DeclCount;

			public string DefaultNamespace;
		}

		internal const string XmlnsXml = "http://www.w3.org/XML/1998/namespace";

		internal const string XmlnsXmlns = "http://www.w3.org/2000/xmlns/";

		internal const string PrefixXml = "xml";

		internal const string PrefixXmlns = "xmlns";

		private NsDecl[] decls;

		private int declPos = -1;

		private NsScope[] scopes;

		private int scopePos = -1;

		private string defaultNamespace;

		private int count;

		private XmlNameTable nameTable;

		internal bool internalAtomizedNames;

		public virtual string DefaultNamespace
		{
			get
			{
				return (defaultNamespace != null) ? defaultNamespace : string.Empty;
			}
		}

		public virtual XmlNameTable NameTable
		{
			get
			{
				return nameTable;
			}
		}

		public XmlNamespaceManager(XmlNameTable nameTable)
		{
			if (nameTable == null)
			{
				throw new ArgumentNullException("nameTable");
			}
			this.nameTable = nameTable;
			nameTable.Add("xmlns");
			nameTable.Add("xml");
			nameTable.Add(string.Empty);
			nameTable.Add("http://www.w3.org/2000/xmlns/");
			nameTable.Add("http://www.w3.org/XML/1998/namespace");
			InitData();
		}

		private void InitData()
		{
			decls = new NsDecl[10];
			scopes = new NsScope[40];
		}

		private void GrowDecls()
		{
			NsDecl[] sourceArray = decls;
			decls = new NsDecl[declPos * 2 + 1];
			if (declPos > 0)
			{
				Array.Copy(sourceArray, 0, decls, 0, declPos);
			}
		}

		private void GrowScopes()
		{
			NsScope[] sourceArray = scopes;
			scopes = new NsScope[scopePos * 2 + 1];
			if (scopePos > 0)
			{
				Array.Copy(sourceArray, 0, scopes, 0, scopePos);
			}
		}

		public virtual void AddNamespace(string prefix, string uri)
		{
			AddNamespace(prefix, uri, false);
		}

		private void AddNamespace(string prefix, string uri, bool atomizedNames)
		{
			if (prefix == null)
			{
				throw new ArgumentNullException("prefix", "Value cannot be null.");
			}
			if (uri == null)
			{
				throw new ArgumentNullException("uri", "Value cannot be null.");
			}
			if (!atomizedNames)
			{
				prefix = nameTable.Add(prefix);
				uri = nameTable.Add(uri);
			}
			if (prefix == "xml" && uri == "http://www.w3.org/XML/1998/namespace")
			{
				return;
			}
			IsValidDeclaration(prefix, uri, true);
			if (prefix.Length == 0)
			{
				defaultNamespace = uri;
			}
			for (int num = declPos; num > declPos - count; num--)
			{
				if (object.ReferenceEquals(decls[num].Prefix, prefix))
				{
					decls[num].Uri = uri;
					return;
				}
			}
			declPos++;
			count++;
			if (declPos == decls.Length)
			{
				GrowDecls();
			}
			decls[declPos].Prefix = prefix;
			decls[declPos].Uri = uri;
		}

		private static string IsValidDeclaration(string prefix, string uri, bool throwException)
		{
			string text = null;
			if (prefix == "xml" && uri != "http://www.w3.org/XML/1998/namespace")
			{
				text = string.Format("Prefix \"xml\" can only be bound to the fixed namespace URI \"{0}\". \"{1}\" is invalid.", "http://www.w3.org/XML/1998/namespace", uri);
			}
			else if (text == null && prefix == "xmlns")
			{
				text = "Declaring prefix named \"xmlns\" is not allowed to any namespace.";
			}
			else if (text == null && uri == "http://www.w3.org/2000/xmlns/")
			{
				text = string.Format("Namespace URI \"{0}\" cannot be declared with any namespace.", "http://www.w3.org/2000/xmlns/");
			}
			if (text != null && throwException)
			{
				throw new ArgumentException(text);
			}
			return text;
		}

		public virtual IEnumerator GetEnumerator()
		{
			Hashtable hashtable = new Hashtable();
			for (int i = 0; i <= declPos; i++)
			{
				if (decls[i].Prefix != string.Empty && decls[i].Uri != null)
				{
					hashtable[decls[i].Prefix] = decls[i].Uri;
				}
			}
			hashtable[string.Empty] = DefaultNamespace;
			hashtable["xml"] = "http://www.w3.org/XML/1998/namespace";
			hashtable["xmlns"] = "http://www.w3.org/2000/xmlns/";
			return hashtable.Keys.GetEnumerator();
		}

		public virtual IDictionary<string, string> GetNamespacesInScope(XmlNamespaceScope scope)
		{
			IDictionary namespacesInScopeImpl = GetNamespacesInScopeImpl(scope);
			IDictionary<string, string> dictionary = new Dictionary<string, string>(namespacesInScopeImpl.Count);
			foreach (DictionaryEntry item in namespacesInScopeImpl)
			{
				dictionary[(string)item.Key] = (string)item.Value;
			}
			return dictionary;
		}

		internal virtual IDictionary GetNamespacesInScopeImpl(XmlNamespaceScope scope)
		{
			Hashtable hashtable = new Hashtable();
			if (scope == XmlNamespaceScope.Local)
			{
				for (int i = 0; i < count; i++)
				{
					if (decls[declPos - i].Prefix == string.Empty && decls[declPos - i].Uri == string.Empty)
					{
						if (hashtable.Contains(string.Empty))
						{
							hashtable.Remove(string.Empty);
						}
					}
					else if (decls[declPos - i].Uri != null)
					{
						hashtable.Add(decls[declPos - i].Prefix, decls[declPos - i].Uri);
					}
				}
				return hashtable;
			}
			for (int j = 0; j <= declPos; j++)
			{
				if (decls[j].Prefix == string.Empty && decls[j].Uri == string.Empty)
				{
					if (hashtable.Contains(string.Empty))
					{
						hashtable.Remove(string.Empty);
					}
				}
				else if (decls[j].Uri != null)
				{
					hashtable[decls[j].Prefix] = decls[j].Uri;
				}
			}
			if (scope == XmlNamespaceScope.All)
			{
				hashtable.Add("xml", "http://www.w3.org/XML/1998/namespace");
			}
			return hashtable;
		}

		public virtual bool HasNamespace(string prefix)
		{
			return HasNamespace(prefix, false);
		}

		private bool HasNamespace(string prefix, bool atomizedNames)
		{
			if (prefix == null || count == 0)
			{
				return false;
			}
			for (int num = declPos; num > declPos - count; num--)
			{
				if (decls[num].Prefix == prefix)
				{
					return true;
				}
			}
			return false;
		}

		public virtual string LookupNamespace(string prefix)
		{
			switch (prefix)
			{
			case "xmlns":
				return nameTable.Get("http://www.w3.org/2000/xmlns/");
			case "xml":
				return nameTable.Get("http://www.w3.org/XML/1998/namespace");
			case "":
				return DefaultNamespace;
			case null:
				return null;
			default:
			{
				for (int num = declPos; num >= 0; num--)
				{
					if (CompareString(decls[num].Prefix, prefix, internalAtomizedNames) && decls[num].Uri != null)
					{
						return decls[num].Uri;
					}
				}
				return null;
			}
			}
		}

		internal string LookupNamespace(string prefix, bool atomizedNames)
		{
			internalAtomizedNames = atomizedNames;
			string result = LookupNamespace(prefix);
			internalAtomizedNames = false;
			return result;
		}

		public virtual string LookupPrefix(string uri)
		{
			return LookupPrefix(uri, false);
		}

		private bool CompareString(string s1, string s2, bool atomizedNames)
		{
			if (atomizedNames)
			{
				return object.ReferenceEquals(s1, s2);
			}
			return s1 == s2;
		}

		internal string LookupPrefix(string uri, bool atomizedName)
		{
			return LookupPrefixCore(uri, atomizedName, false);
		}

		internal string LookupPrefixExclusive(string uri, bool atomizedName)
		{
			return LookupPrefixCore(uri, atomizedName, true);
		}

		private string LookupPrefixCore(string uri, bool atomizedName, bool excludeOverriden)
		{
			if (uri == null)
			{
				return null;
			}
			if (CompareString(uri, DefaultNamespace, atomizedName))
			{
				return string.Empty;
			}
			if (CompareString(uri, "http://www.w3.org/XML/1998/namespace", atomizedName))
			{
				return "xml";
			}
			if (CompareString(uri, "http://www.w3.org/2000/xmlns/", atomizedName))
			{
				return "xmlns";
			}
			for (int num = declPos; num >= 0; num--)
			{
				if (CompareString(decls[num].Uri, uri, atomizedName) && decls[num].Prefix.Length > 0 && (!excludeOverriden || !IsOverriden(num)))
				{
					return decls[num].Prefix;
				}
			}
			return null;
		}

		private bool IsOverriden(int idx)
		{
			if (idx == declPos)
			{
				return false;
			}
			string prefix = decls[idx + 1].Prefix;
			for (int i = idx + 1; i <= declPos; i++)
			{
				if ((object)decls[idx].Prefix == prefix)
				{
					return true;
				}
			}
			return false;
		}

		public virtual bool PopScope()
		{
			if (scopePos == -1)
			{
				return false;
			}
			declPos -= count;
			defaultNamespace = scopes[scopePos].DefaultNamespace;
			count = scopes[scopePos].DeclCount;
			scopePos--;
			return true;
		}

		public virtual void PushScope()
		{
			scopePos++;
			if (scopePos == scopes.Length)
			{
				GrowScopes();
			}
			scopes[scopePos].DefaultNamespace = defaultNamespace;
			scopes[scopePos].DeclCount = count;
			count = 0;
		}

		public virtual void RemoveNamespace(string prefix, string uri)
		{
			RemoveNamespace(prefix, uri, false);
		}

		private void RemoveNamespace(string prefix, string uri, bool atomizedNames)
		{
			if (prefix == null)
			{
				throw new ArgumentNullException("prefix");
			}
			if (uri == null)
			{
				throw new ArgumentNullException("uri");
			}
			if (count == 0)
			{
				return;
			}
			for (int num = declPos; num > declPos - count; num--)
			{
				if (CompareString(decls[num].Prefix, prefix, atomizedNames) && CompareString(decls[num].Uri, uri, atomizedNames))
				{
					decls[num].Uri = null;
				}
			}
		}
	}
}

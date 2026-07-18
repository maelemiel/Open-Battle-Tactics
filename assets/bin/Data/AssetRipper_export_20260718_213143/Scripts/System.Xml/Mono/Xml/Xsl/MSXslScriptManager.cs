using System;
using System.Collections;
using System.Globalization;
using System.Security.Policy;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace Mono.Xml.Xsl
{
	internal class MSXslScriptManager
	{
		private enum ScriptingLanguage
		{
			JScript = 0,
			VisualBasic = 1,
			CSharp = 2
		}

		private class MSXslScript
		{
			private ScriptingLanguage language;

			private string implementsPrefix;

			private string code;

			private Evidence evidence;

			public ScriptingLanguage Language
			{
				get
				{
					return language;
				}
			}

			public string ImplementsPrefix
			{
				get
				{
					return implementsPrefix;
				}
			}

			public string Code
			{
				get
				{
					return code;
				}
			}

			public MSXslScript(XPathNavigator nav, Evidence evidence)
			{
				this.evidence = evidence;
				code = nav.Value;
				if (nav.MoveToFirstAttribute())
				{
					do
					{
						switch (nav.LocalName)
						{
						case "language":
							switch (nav.Value.ToLower(CultureInfo.InvariantCulture))
							{
							case "jscript":
							case "javascript":
								language = ScriptingLanguage.JScript;
								break;
							case "vb":
							case "visualbasic":
								language = ScriptingLanguage.VisualBasic;
								break;
							case "c#":
							case "csharp":
								language = ScriptingLanguage.CSharp;
								break;
							default:
								throw new XsltException("Invalid scripting language!", null);
							}
							break;
						case "implements-prefix":
							implementsPrefix = nav.Value;
							break;
						}
					}
					while (nav.MoveToNextAttribute());
					nav.MoveToParent();
				}
				if (implementsPrefix == null)
				{
					throw new XsltException("need implements-prefix attr", null);
				}
			}

			public object Compile(XPathNavigator node)
			{
				throw new NotImplementedException();
			}
		}

		private Hashtable scripts = new Hashtable();

		public void AddScript(Compiler c)
		{
			MSXslScript mSXslScript = new MSXslScript(c.Input, c.Evidence);
			string text = c.Input.GetNamespace(mSXslScript.ImplementsPrefix);
			if (text == null)
			{
				throw new XsltCompileException("Specified prefix for msxsl:script was not found: " + mSXslScript.ImplementsPrefix, null, c.Input);
			}
			scripts.Add(text, mSXslScript.Compile(c.Input));
		}

		public object GetExtensionObject(string ns)
		{
			if (!scripts.ContainsKey(ns))
			{
				return null;
			}
			return Activator.CreateInstance((Type)scripts[ns]);
		}
	}
}

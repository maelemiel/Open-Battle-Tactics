using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security;
using System.Text;
using Mono.Security.Cryptography;
using Mono.Xml;

namespace Mono.Security
{
	internal class StrongNameManager
	{
		private class Element
		{
			internal Hashtable assemblies;

			public Element()
			{
				assemblies = new Hashtable();
			}

			public Element(string assembly, string users)
				: this()
			{
				assemblies.Add(assembly, users);
			}

			public string GetUsers(string assembly)
			{
				return (string)assemblies[assembly];
			}
		}

		private static Hashtable mappings;

		private static Hashtable tokens;

		static StrongNameManager()
		{
		}

		public static void LoadConfig(string filename)
		{
			if (!File.Exists(filename))
			{
				return;
			}
			SecurityParser securityParser = new SecurityParser();
			using (StreamReader streamReader = new StreamReader(filename))
			{
				string xml = streamReader.ReadToEnd();
				securityParser.LoadXml(xml);
			}
			SecurityElement securityElement = securityParser.ToXml();
			if (securityElement == null || !(securityElement.Tag == "configuration"))
			{
				return;
			}
			SecurityElement securityElement2 = securityElement.SearchForChildByTag("strongNames");
			if (securityElement2 != null && securityElement2.Children.Count > 0)
			{
				SecurityElement securityElement3 = securityElement2.SearchForChildByTag("pubTokenMapping");
				if (securityElement3 != null && securityElement3.Children.Count > 0)
				{
					LoadMapping(securityElement3);
				}
				SecurityElement securityElement4 = securityElement2.SearchForChildByTag("verificationSettings");
				if (securityElement4 != null && securityElement4.Children.Count > 0)
				{
					LoadVerificationSettings(securityElement4);
				}
			}
		}

		private static void LoadMapping(SecurityElement mapping)
		{
			if (mappings == null)
			{
				mappings = new Hashtable();
			}
			lock (mappings.SyncRoot)
			{
				foreach (SecurityElement child in mapping.Children)
				{
					if (child.Tag != "map")
					{
						continue;
					}
					string text = child.Attribute("Token");
					if (text == null || text.Length != 16)
					{
						continue;
					}
					text = text.ToUpper(CultureInfo.InvariantCulture);
					string text2 = child.Attribute("PublicKey");
					if (text2 != null)
					{
						if (mappings[text] == null)
						{
							mappings.Add(text, text2);
						}
						else
						{
							mappings[text] = text2;
						}
					}
				}
			}
		}

		private static void LoadVerificationSettings(SecurityElement settings)
		{
			if (tokens == null)
			{
				tokens = new Hashtable();
			}
			lock (tokens.SyncRoot)
			{
				foreach (SecurityElement child in settings.Children)
				{
					if (child.Tag != "skip")
					{
						continue;
					}
					string text = child.Attribute("Token");
					if (text == null)
					{
						continue;
					}
					text = text.ToUpper(CultureInfo.InvariantCulture);
					string text2 = child.Attribute("Assembly");
					if (text2 == null)
					{
						text2 = "*";
					}
					string text3 = child.Attribute("Users");
					if (text3 == null)
					{
						text3 = "*";
					}
					Element element = (Element)tokens[text];
					if (element == null)
					{
						element = new Element(text2, text3);
						tokens.Add(text, element);
						continue;
					}
					string text4 = (string)element.assemblies[text2];
					if (text4 == null)
					{
						element.assemblies.Add(text2, text3);
						continue;
					}
					if (text3 == "*")
					{
						element.assemblies[text2] = "*";
						continue;
					}
					string text5 = (string)element.assemblies[text2];
					string value = text5 + "," + text3;
					element.assemblies[text2] = value;
				}
			}
		}

		public static byte[] GetMappedPublicKey(byte[] token)
		{
			if (mappings == null || token == null)
			{
				return null;
			}
			string key = CryptoConvert.ToHex(token);
			string text = (string)mappings[key];
			if (text == null)
			{
				return null;
			}
			return CryptoConvert.FromHex(text);
		}

		public static bool MustVerify(AssemblyName an)
		{
			if (an == null || tokens == null)
			{
				return true;
			}
			string key = CryptoConvert.ToHex(an.GetPublicKeyToken());
			Element element = (Element)tokens[key];
			if (element != null)
			{
				string users = element.GetUsers(an.Name);
				if (users == null)
				{
					users = element.GetUsers("*");
				}
				if (users != null)
				{
					if (users == "*")
					{
						return false;
					}
					return users.IndexOf(Environment.UserName) < 0;
				}
			}
			return true;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("Public Key Token\tAssemblies\t\tUsers");
			stringBuilder.Append(Environment.NewLine);
			foreach (DictionaryEntry token in tokens)
			{
				stringBuilder.Append((string)token.Key);
				Element element = (Element)token.Value;
				bool flag = true;
				foreach (DictionaryEntry assembly in element.assemblies)
				{
					if (flag)
					{
						stringBuilder.Append("\t");
						flag = false;
					}
					else
					{
						stringBuilder.Append("\t\t\t");
					}
					stringBuilder.Append((string)assembly.Key);
					stringBuilder.Append("\t");
					string text = (string)assembly.Value;
					if (text == "*")
					{
						text = "All users";
					}
					stringBuilder.Append(text);
					stringBuilder.Append(Environment.NewLine);
				}
			}
			return stringBuilder.ToString();
		}
	}
}

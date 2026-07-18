using System;
using System.Collections;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace Mono.Xml.Xsl
{
	internal class XslOutput
	{
		private string uri;

		private XmlQualifiedName customMethod;

		private OutputMethod method = OutputMethod.Unknown;

		private string version;

		private Encoding encoding = Encoding.UTF8;

		private bool omitXmlDeclaration;

		private StandaloneType standalone;

		private string doctypePublic;

		private string doctypeSystem;

		private XmlQualifiedName[] cdataSectionElements;

		private string indent;

		private string mediaType;

		private string stylesheetVersion;

		private ArrayList cdSectsList = new ArrayList();

		public OutputMethod Method
		{
			get
			{
				return method;
			}
		}

		public XmlQualifiedName CustomMethod
		{
			get
			{
				return customMethod;
			}
		}

		public string Version
		{
			get
			{
				return version;
			}
		}

		public Encoding Encoding
		{
			get
			{
				return encoding;
			}
		}

		public string Uri
		{
			get
			{
				return uri;
			}
		}

		public bool OmitXmlDeclaration
		{
			get
			{
				return omitXmlDeclaration;
			}
		}

		public StandaloneType Standalone
		{
			get
			{
				return standalone;
			}
		}

		public string DoctypePublic
		{
			get
			{
				return doctypePublic;
			}
		}

		public string DoctypeSystem
		{
			get
			{
				return doctypeSystem;
			}
		}

		public XmlQualifiedName[] CDataSectionElements
		{
			get
			{
				if (cdataSectionElements == null)
				{
					cdataSectionElements = cdSectsList.ToArray(typeof(XmlQualifiedName)) as XmlQualifiedName[];
				}
				return cdataSectionElements;
			}
		}

		public string Indent
		{
			get
			{
				return indent;
			}
		}

		public string MediaType
		{
			get
			{
				return mediaType;
			}
		}

		public XslOutput(string uri, string stylesheetVersion)
		{
			this.uri = uri;
			this.stylesheetVersion = stylesheetVersion;
		}

		public void Fill(XPathNavigator nav)
		{
			if (nav.MoveToFirstAttribute())
			{
				ProcessAttribute(nav);
				while (nav.MoveToNextAttribute())
				{
					ProcessAttribute(nav);
				}
				nav.MoveToParent();
			}
		}

		private void ProcessAttribute(XPathNavigator nav)
		{
			if (nav.NamespaceURI != string.Empty)
			{
				return;
			}
			string value = nav.Value;
			switch (nav.LocalName)
			{
			case "cdata-section-elements":
				if (value.Length > 0)
				{
					cdSectsList.AddRange(XslNameUtil.FromListString(value, nav));
				}
				break;
			case "method":
				if (value.Length == 0)
				{
					break;
				}
				switch (value)
				{
				case "xml":
					method = OutputMethod.XML;
					break;
				case "html":
					omitXmlDeclaration = true;
					method = OutputMethod.HTML;
					break;
				case "text":
					omitXmlDeclaration = true;
					method = OutputMethod.Text;
					break;
				default:
					method = OutputMethod.Custom;
					customMethod = XslNameUtil.FromString(value, nav);
					if (customMethod.Namespace == string.Empty)
					{
						IXmlLineInfo xmlLineInfo2 = nav as IXmlLineInfo;
						throw new XsltCompileException(new ArgumentException("Invalid output method value: '" + value + "'. It must be either 'xml' or 'html' or 'text' or QName."), nav.BaseURI, (xmlLineInfo2 != null) ? xmlLineInfo2.LineNumber : 0, (xmlLineInfo2 != null) ? xmlLineInfo2.LinePosition : 0);
					}
					break;
				}
				break;
			case "version":
				if (value.Length > 0)
				{
					version = value;
				}
				break;
			case "encoding":
				if (value.Length > 0)
				{
					try
					{
						encoding = Encoding.GetEncoding(value);
						break;
					}
					catch (ArgumentException)
					{
						break;
					}
					catch (NotSupportedException)
					{
						break;
					}
				}
				break;
			case "standalone":
				switch (value)
				{
				case "yes":
					standalone = StandaloneType.YES;
					break;
				case "no":
					standalone = StandaloneType.NO;
					break;
				default:
				{
					if (stylesheetVersion != "1.0")
					{
						break;
					}
					IXmlLineInfo xmlLineInfo3 = nav as IXmlLineInfo;
					throw new XsltCompileException(new XsltException("'" + value + "' is an invalid value for 'standalone' attribute.", null), nav.BaseURI, (xmlLineInfo3 != null) ? xmlLineInfo3.LineNumber : 0, (xmlLineInfo3 != null) ? xmlLineInfo3.LinePosition : 0);
				}
				}
				break;
			case "doctype-public":
				doctypePublic = value;
				break;
			case "doctype-system":
				doctypeSystem = value;
				break;
			case "media-type":
				if (value.Length > 0)
				{
					mediaType = value;
				}
				break;
			case "omit-xml-declaration":
				switch (value)
				{
				case "yes":
					omitXmlDeclaration = true;
					break;
				case "no":
					omitXmlDeclaration = false;
					break;
				default:
				{
					if (stylesheetVersion != "1.0")
					{
						break;
					}
					IXmlLineInfo xmlLineInfo4 = nav as IXmlLineInfo;
					throw new XsltCompileException(new XsltException("'" + value + "' is an invalid value for 'omit-xml-declaration' attribute.", null), nav.BaseURI, (xmlLineInfo4 != null) ? xmlLineInfo4.LineNumber : 0, (xmlLineInfo4 != null) ? xmlLineInfo4.LinePosition : 0);
				}
				}
				break;
			case "indent":
				indent = value;
				if (stylesheetVersion != "1.0")
				{
					break;
				}
				switch (value)
				{
				case "yes":
				case "no":
					break;
				default:
				{
					OutputMethod outputMethod = method;
					if (outputMethod == OutputMethod.Custom)
					{
						break;
					}
					throw new XsltCompileException(string.Format("Unexpected 'indent' attribute value in 'output' element: '{0}'", value), null, nav);
				}
				}
				break;
			default:
			{
				if (stylesheetVersion != "1.0")
				{
					break;
				}
				IXmlLineInfo xmlLineInfo = nav as IXmlLineInfo;
				throw new XsltCompileException(new XsltException("'" + nav.LocalName + "' is an invalid attribute for 'output' element.", null), nav.BaseURI, (xmlLineInfo != null) ? xmlLineInfo.LineNumber : 0, (xmlLineInfo != null) ? xmlLineInfo.LinePosition : 0);
			}
			}
		}
	}
}

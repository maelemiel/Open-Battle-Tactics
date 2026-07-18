using System.IO;
using System.Xml.XPath;
using Mono.Xml.Xsl;

namespace System.Xml.Xsl
{
	[System.MonoTODO]
	public sealed class XslCompiledTransform
	{
		private bool enable_debug;

		private object debugger;

		private CompiledStylesheet s;

		private XmlWriterSettings output_settings = new XmlWriterSettings();

		[System.MonoTODO]
		public XmlWriterSettings OutputSettings
		{
			get
			{
				return output_settings;
			}
		}

		public XslCompiledTransform()
			: this(false)
		{
		}

		public XslCompiledTransform(bool enableDebug)
		{
			enable_debug = enableDebug;
			if (enable_debug)
			{
				debugger = new NoOperationDebugger();
			}
			output_settings.ConformanceLevel = ConformanceLevel.Fragment;
		}

		public void Transform(string inputfile, string outputfile)
		{
			using (Stream output = File.Create(outputfile))
			{
				Transform(new XPathDocument(inputfile, XmlSpace.Preserve), null, output);
			}
		}

		public void Transform(string inputfile, XmlWriter output)
		{
			Transform(inputfile, null, output);
		}

		public void Transform(string inputfile, XsltArgumentList args, Stream output)
		{
			Transform(new XPathDocument(inputfile, XmlSpace.Preserve), args, output);
		}

		public void Transform(string inputfile, XsltArgumentList args, TextWriter output)
		{
			Transform(new XPathDocument(inputfile, XmlSpace.Preserve), args, output);
		}

		public void Transform(string inputfile, XsltArgumentList args, XmlWriter output)
		{
			Transform(new XPathDocument(inputfile, XmlSpace.Preserve), args, output);
		}

		public void Transform(XmlReader reader, XmlWriter output)
		{
			Transform(reader, null, output);
		}

		public void Transform(XmlReader reader, XsltArgumentList args, Stream output)
		{
			Transform(new XPathDocument(reader, XmlSpace.Preserve), args, output);
		}

		public void Transform(XmlReader reader, XsltArgumentList args, TextWriter output)
		{
			Transform(new XPathDocument(reader, XmlSpace.Preserve), args, output);
		}

		public void Transform(XmlReader reader, XsltArgumentList args, XmlWriter output)
		{
			Transform(reader, args, output, null);
		}

		public void Transform(IXPathNavigable input, XsltArgumentList args, TextWriter output)
		{
			Transform(input.CreateNavigator(), args, output);
		}

		public void Transform(IXPathNavigable input, XsltArgumentList args, Stream output)
		{
			Transform(input.CreateNavigator(), args, output);
		}

		public void Transform(IXPathNavigable input, XmlWriter output)
		{
			Transform(input, null, output);
		}

		public void Transform(IXPathNavigable input, XsltArgumentList args, XmlWriter output)
		{
			Transform(input.CreateNavigator(), args, output, null);
		}

		public void Transform(XmlReader input, XsltArgumentList args, XmlWriter output, XmlResolver resolver)
		{
			Transform(new XPathDocument(input, XmlSpace.Preserve).CreateNavigator(), args, output, resolver);
		}

		private void Transform(XPathNavigator input, XsltArgumentList args, XmlWriter output, XmlResolver resolver)
		{
			if (s == null)
			{
				throw new XsltException("No stylesheet was loaded.", null);
			}
			Outputter outputtter = new GenericOutputter(output, s.Outputs, null);
			new XslTransformProcessor(s, debugger).Process(input, outputtter, args, resolver);
			output.Flush();
		}

		private void Transform(XPathNavigator input, XsltArgumentList args, Stream output)
		{
			XslOutput xslOutput = (XslOutput)s.Outputs[string.Empty];
			Transform(input, args, new StreamWriter(output, xslOutput.Encoding));
		}

		private void Transform(XPathNavigator input, XsltArgumentList args, TextWriter output)
		{
			if (s == null)
			{
				throw new XsltException("No stylesheet was loaded.", null);
			}
			Outputter outputter = new GenericOutputter(output, s.Outputs, output.Encoding);
			new XslTransformProcessor(s, debugger).Process(input, outputter, args, null);
			outputter.Done();
			output.Flush();
		}

		private XmlReader GetXmlReader(string url)
		{
			XmlResolver xmlResolver = new XmlUrlResolver();
			Uri uri = xmlResolver.ResolveUri(null, url);
			Stream input = xmlResolver.GetEntity(uri, null, typeof(Stream)) as Stream;
			XmlTextReader xmlTextReader = new XmlTextReader(uri.ToString(), input);
			xmlTextReader.XmlResolver = xmlResolver;
			XmlValidatingReader xmlValidatingReader = new XmlValidatingReader(xmlTextReader);
			xmlValidatingReader.XmlResolver = xmlResolver;
			xmlValidatingReader.ValidationType = ValidationType.None;
			return xmlValidatingReader;
		}

		public void Load(string url)
		{
			using (XmlReader stylesheet = GetXmlReader(url))
			{
				Load(stylesheet);
			}
		}

		public void Load(XmlReader stylesheet)
		{
			Load(stylesheet, null, null);
		}

		public void Load(IXPathNavigable stylesheet)
		{
			Load(stylesheet.CreateNavigator(), null, null);
		}

		public void Load(IXPathNavigable stylesheet, XsltSettings settings, XmlResolver resolver)
		{
			Load(stylesheet.CreateNavigator(), settings, resolver);
		}

		public void Load(XmlReader stylesheet, XsltSettings settings, XmlResolver resolver)
		{
			Load(new XPathDocument(stylesheet, XmlSpace.Preserve).CreateNavigator(), settings, resolver);
		}

		public void Load(string stylesheet, XsltSettings settings, XmlResolver resolver)
		{
			Load(new XPathDocument(stylesheet, XmlSpace.Preserve).CreateNavigator(), settings, resolver);
		}

		private void Load(XPathNavigator stylesheet, XsltSettings settings, XmlResolver resolver)
		{
			s = new Compiler(debugger).Compile(stylesheet, resolver, null);
		}
	}
}

using System.IO;
using System.Security.Policy;
using System.Xml.XPath;
using Mono.Xml.Xsl;

namespace System.Xml.Xsl
{
	public sealed class XslTransform
	{
		internal static readonly bool TemplateStackFrameError;

		internal static readonly TextWriter TemplateStackFrameOutput;

		private object debugger;

		private CompiledStylesheet s;

		private XmlResolver xmlResolver = new XmlUrlResolver();

		[System.MonoTODO]
		public XmlResolver XmlResolver
		{
			set
			{
				xmlResolver = value;
			}
		}

		public XslTransform()
			: this(GetDefaultDebugger())
		{
		}

		internal XslTransform(object debugger)
		{
			this.debugger = debugger;
		}

		static XslTransform()
		{
			switch (Environment.GetEnvironmentVariable("MONO_XSLT_STACK_FRAME"))
			{
			case "stdout":
				TemplateStackFrameOutput = Console.Out;
				break;
			case "stderr":
				TemplateStackFrameOutput = Console.Error;
				break;
			case "error":
				TemplateStackFrameError = true;
				break;
			}
		}

		private static object GetDefaultDebugger()
		{
			string text = null;
			try
			{
				text = Environment.GetEnvironmentVariable("MONO_XSLT_DEBUGGER");
			}
			catch (Exception)
			{
			}
			if (text == null)
			{
				return null;
			}
			if (text == "simple")
			{
				return new SimpleXsltDebugger();
			}
			return Activator.CreateInstance(Type.GetType(text));
		}

		public XmlReader Transform(IXPathNavigable input, XsltArgumentList args)
		{
			return Transform(input.CreateNavigator(), args, xmlResolver);
		}

		public XmlReader Transform(IXPathNavigable input, XsltArgumentList args, XmlResolver resolver)
		{
			return Transform(input.CreateNavigator(), args, resolver);
		}

		public XmlReader Transform(XPathNavigator input, XsltArgumentList args)
		{
			return Transform(input, args, xmlResolver);
		}

		public XmlReader Transform(XPathNavigator input, XsltArgumentList args, XmlResolver resolver)
		{
			MemoryStream memoryStream = new MemoryStream();
			Transform(input, args, new XmlTextWriter(memoryStream, null), resolver);
			memoryStream.Position = 0L;
			return new XmlTextReader(memoryStream, XmlNodeType.Element, null);
		}

		public void Transform(IXPathNavigable input, XsltArgumentList args, TextWriter output)
		{
			Transform(input.CreateNavigator(), args, output, xmlResolver);
		}

		public void Transform(IXPathNavigable input, XsltArgumentList args, TextWriter output, XmlResolver resolver)
		{
			Transform(input.CreateNavigator(), args, output, resolver);
		}

		public void Transform(IXPathNavigable input, XsltArgumentList args, Stream output)
		{
			Transform(input.CreateNavigator(), args, output, xmlResolver);
		}

		public void Transform(IXPathNavigable input, XsltArgumentList args, Stream output, XmlResolver resolver)
		{
			Transform(input.CreateNavigator(), args, output, resolver);
		}

		public void Transform(IXPathNavigable input, XsltArgumentList args, XmlWriter output)
		{
			Transform(input.CreateNavigator(), args, output, xmlResolver);
		}

		public void Transform(IXPathNavigable input, XsltArgumentList args, XmlWriter output, XmlResolver resolver)
		{
			Transform(input.CreateNavigator(), args, output, resolver);
		}

		public void Transform(XPathNavigator input, XsltArgumentList args, XmlWriter output)
		{
			Transform(input, args, output, xmlResolver);
		}

		public void Transform(XPathNavigator input, XsltArgumentList args, XmlWriter output, XmlResolver resolver)
		{
			if (s == null)
			{
				throw new XsltException("No stylesheet was loaded.", null);
			}
			Outputter outputtter = new GenericOutputter(output, s.Outputs, null);
			new XslTransformProcessor(s, debugger).Process(input, outputtter, args, resolver);
			output.Flush();
		}

		public void Transform(XPathNavigator input, XsltArgumentList args, Stream output)
		{
			Transform(input, args, output, xmlResolver);
		}

		public void Transform(XPathNavigator input, XsltArgumentList args, Stream output, XmlResolver resolver)
		{
			XslOutput xslOutput = (XslOutput)s.Outputs[string.Empty];
			Transform(input, args, new StreamWriter(output, xslOutput.Encoding), resolver);
		}

		public void Transform(XPathNavigator input, XsltArgumentList args, TextWriter output)
		{
			Transform(input, args, output, xmlResolver);
		}

		public void Transform(XPathNavigator input, XsltArgumentList args, TextWriter output, XmlResolver resolver)
		{
			if (s == null)
			{
				throw new XsltException("No stylesheet was loaded.", null);
			}
			Outputter outputter = new GenericOutputter(output, s.Outputs, output.Encoding);
			new XslTransformProcessor(s, debugger).Process(input, outputter, args, resolver);
			outputter.Done();
			output.Flush();
		}

		public void Transform(string inputfile, string outputfile)
		{
			Transform(inputfile, outputfile, xmlResolver);
		}

		public void Transform(string inputfile, string outputfile, XmlResolver resolver)
		{
			using (Stream output = new FileStream(outputfile, FileMode.Create, FileAccess.ReadWrite))
			{
				Transform(new XPathDocument(inputfile).CreateNavigator(), null, output, resolver);
			}
		}

		public void Load(string url)
		{
			Load(url, null);
		}

		public void Load(string url, XmlResolver resolver)
		{
			XmlResolver xmlResolver = resolver;
			if (xmlResolver == null)
			{
				xmlResolver = new XmlUrlResolver();
			}
			Uri uri = xmlResolver.ResolveUri(null, url);
			using (Stream input = xmlResolver.GetEntity(uri, null, typeof(Stream)) as Stream)
			{
				XmlTextReader xmlTextReader = new XmlTextReader(uri.ToString(), input);
				xmlTextReader.XmlResolver = xmlResolver;
				XmlValidatingReader xmlValidatingReader = new XmlValidatingReader(xmlTextReader);
				xmlValidatingReader.XmlResolver = xmlResolver;
				xmlValidatingReader.ValidationType = ValidationType.None;
				Load(new XPathDocument(xmlValidatingReader, XmlSpace.Preserve).CreateNavigator(), resolver, null);
			}
		}

		public void Load(XmlReader stylesheet)
		{
			Load(stylesheet, null, null);
		}

		public void Load(XmlReader stylesheet, XmlResolver resolver)
		{
			Load(stylesheet, resolver, null);
		}

		public void Load(XPathNavigator stylesheet)
		{
			Load(stylesheet, null, null);
		}

		public void Load(XPathNavigator stylesheet, XmlResolver resolver)
		{
			Load(stylesheet, resolver, null);
		}

		public void Load(IXPathNavigable stylesheet)
		{
			Load(stylesheet.CreateNavigator(), null);
		}

		public void Load(IXPathNavigable stylesheet, XmlResolver resolver)
		{
			Load(stylesheet.CreateNavigator(), resolver);
		}

		public void Load(IXPathNavigable stylesheet, XmlResolver resolver, Evidence evidence)
		{
			Load(stylesheet.CreateNavigator(), resolver, evidence);
		}

		public void Load(XPathNavigator stylesheet, XmlResolver resolver, Evidence evidence)
		{
			s = new Compiler(debugger).Compile(stylesheet, resolver, evidence);
		}

		public void Load(XmlReader stylesheet, XmlResolver resolver, Evidence evidence)
		{
			Load(new XPathDocument(stylesheet, XmlSpace.Preserve).CreateNavigator(), resolver, evidence);
		}
	}
}

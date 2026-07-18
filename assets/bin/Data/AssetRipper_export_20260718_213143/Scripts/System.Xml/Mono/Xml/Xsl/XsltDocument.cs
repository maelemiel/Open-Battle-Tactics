using System;
using System.Collections;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;

namespace Mono.Xml.Xsl
{
	internal class XsltDocument : XPathFunction
	{
		private Expression arg0;

		private Expression arg1;

		private XPathNavigator doc;

		private static string VoidBaseUriFlag = "&^)(*&%*^$&$VOID!BASE!URI!";

		public override XPathResultType ReturnType
		{
			get
			{
				return XPathResultType.NodeSet;
			}
		}

		internal override bool Peer
		{
			get
			{
				return arg0.Peer && (arg1 == null || arg1.Peer);
			}
		}

		public XsltDocument(FunctionArguments args, Compiler c)
			: base(args)
		{
			if (args == null || (args.Tail != null && args.Tail.Tail != null))
			{
				throw new XPathException("document takes one or two args");
			}
			arg0 = args.Arg;
			if (args.Tail != null)
			{
				arg1 = args.Tail.Arg;
			}
			doc = c.Input.Clone();
		}

		public override object Evaluate(BaseIterator iter)
		{
			string baseUri = null;
			if (arg1 != null)
			{
				XPathNodeIterator xPathNodeIterator = arg1.EvaluateNodeSet(iter);
				baseUri = ((!xPathNodeIterator.MoveNext()) ? VoidBaseUriFlag : xPathNodeIterator.Current.BaseURI);
			}
			object obj = arg0.Evaluate(iter);
			if (obj is XPathNodeIterator)
			{
				return GetDocument(iter.NamespaceManager as XsltCompiledContext, (XPathNodeIterator)obj, baseUri);
			}
			return GetDocument(iter.NamespaceManager as XsltCompiledContext, (!(obj is IFormattable)) ? obj.ToString() : ((IFormattable)obj).ToString(null, CultureInfo.InvariantCulture), baseUri);
		}

		private Uri Resolve(string thisUri, string baseUri, XslTransformProcessor p)
		{
			XmlResolver resolver = p.Resolver;
			if (resolver == null)
			{
				return null;
			}
			Uri baseUri2 = null;
			if (!object.ReferenceEquals(baseUri, VoidBaseUriFlag) && baseUri != string.Empty)
			{
				baseUri2 = resolver.ResolveUri(null, baseUri);
			}
			return resolver.ResolveUri(baseUri2, thisUri);
		}

		private XPathNodeIterator GetDocument(XsltCompiledContext xsltContext, XPathNodeIterator itr, string baseUri)
		{
			ArrayList arrayList = new ArrayList();
			try
			{
				Hashtable hashtable = new Hashtable();
				while (itr.MoveNext())
				{
					Uri uri = Resolve(itr.Current.Value, (baseUri == null) ? doc.BaseURI : baseUri, xsltContext.Processor);
					if (!hashtable.ContainsKey(uri))
					{
						hashtable.Add(uri, null);
						if (uri != null && uri.ToString() == string.Empty)
						{
							XPathNavigator xPathNavigator = doc.Clone();
							xPathNavigator.MoveToRoot();
							arrayList.Add(xPathNavigator);
						}
						else
						{
							arrayList.Add(xsltContext.Processor.GetDocument(uri));
						}
					}
				}
			}
			catch (Exception)
			{
				arrayList.Clear();
			}
			return new ListIterator(arrayList, xsltContext);
		}

		private XPathNodeIterator GetDocument(XsltCompiledContext xsltContext, string arg0, string baseUri)
		{
			try
			{
				Uri uri = Resolve(arg0, (baseUri == null) ? doc.BaseURI : baseUri, xsltContext.Processor);
				XPathNavigator xPathNavigator;
				if (uri != null && uri.ToString() == string.Empty)
				{
					xPathNavigator = doc.Clone();
					xPathNavigator.MoveToRoot();
				}
				else
				{
					xPathNavigator = xsltContext.Processor.GetDocument(uri);
				}
				return new SelfIterator(xPathNavigator, xsltContext);
			}
			catch (Exception)
			{
				return new ListIterator(new ArrayList(), xsltContext);
			}
		}

		public override string ToString()
		{
			return "document(" + arg0.ToString() + ((arg1 == null) ? string.Empty : ",") + ((arg1 == null) ? string.Empty : arg1.ToString()) + ")";
		}
	}
}

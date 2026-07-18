using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Schema;
using System.Xml.Xsl;
using Mono.Xml.XPath;

namespace System.Xml.XPath
{
	public abstract class XPathNavigator : XPathItem, ICloneable, IXPathNavigable, IXmlNamespaceResolver
	{
		private class EnumerableIterator : XPathNodeIterator
		{
			private IEnumerable source;

			private IEnumerator e;

			private int pos;

			public override int CurrentPosition
			{
				get
				{
					return pos;
				}
			}

			public override XPathNavigator Current
			{
				get
				{
					return (pos != 0) ? ((XPathNavigator)e.Current) : null;
				}
			}

			public EnumerableIterator(IEnumerable source, int pos)
			{
				this.source = source;
				for (int i = 0; i < pos; i++)
				{
					MoveNext();
				}
			}

			public override XPathNodeIterator Clone()
			{
				return new EnumerableIterator(source, pos);
			}

			public override bool MoveNext()
			{
				if (e == null)
				{
					e = source.GetEnumerator();
				}
				if (!e.MoveNext())
				{
					return false;
				}
				pos++;
				return true;
			}
		}

		private static readonly char[] escape_text_chars = new char[3] { '&', '<', '>' };

		private static readonly char[] escape_attr_chars = new char[6] { '"', '&', '<', '>', '\r', '\n' };

		public static IEqualityComparer NavigatorComparer
		{
			get
			{
				return XPathNavigatorComparer.Instance;
			}
		}

		public abstract string BaseURI { get; }

		public virtual bool CanEdit
		{
			get
			{
				return false;
			}
		}

		public virtual bool HasAttributes
		{
			get
			{
				if (!MoveToFirstAttribute())
				{
					return false;
				}
				MoveToParent();
				return true;
			}
		}

		public virtual bool HasChildren
		{
			get
			{
				if (!MoveToFirstChild())
				{
					return false;
				}
				MoveToParent();
				return true;
			}
		}

		public abstract bool IsEmptyElement { get; }

		public abstract string LocalName { get; }

		public abstract string Name { get; }

		public abstract string NamespaceURI { get; }

		public abstract XmlNameTable NameTable { get; }

		public abstract XPathNodeType NodeType { get; }

		public abstract string Prefix { get; }

		public virtual string XmlLang
		{
			get
			{
				XPathNavigator xPathNavigator = Clone();
				XPathNodeType nodeType = xPathNavigator.NodeType;
				if (nodeType == XPathNodeType.Attribute || nodeType == XPathNodeType.Namespace)
				{
					xPathNavigator.MoveToParent();
				}
				do
				{
					if (xPathNavigator.MoveToAttribute("lang", "http://www.w3.org/XML/1998/namespace"))
					{
						return xPathNavigator.Value;
					}
				}
				while (xPathNavigator.MoveToParent());
				return string.Empty;
			}
		}

		public virtual string InnerXml
		{
			get
			{
				switch (NodeType)
				{
				case XPathNodeType.Attribute:
				case XPathNodeType.Namespace:
					return EscapeString(Value, true);
				case XPathNodeType.Text:
				case XPathNodeType.SignificantWhitespace:
				case XPathNodeType.Whitespace:
					return string.Empty;
				case XPathNodeType.ProcessingInstruction:
				case XPathNodeType.Comment:
					return Value;
				default:
				{
					XmlReader xmlReader = ReadSubtree();
					xmlReader.Read();
					int num = xmlReader.Depth;
					if (NodeType != XPathNodeType.Root)
					{
						xmlReader.Read();
					}
					else
					{
						num = -1;
					}
					StringWriter stringWriter = new StringWriter();
					XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
					xmlWriterSettings.Indent = true;
					xmlWriterSettings.ConformanceLevel = ConformanceLevel.Fragment;
					xmlWriterSettings.OmitXmlDeclaration = true;
					XmlWriter xmlWriter = XmlWriter.Create(stringWriter, xmlWriterSettings);
					while (!xmlReader.EOF && xmlReader.Depth > num)
					{
						xmlWriter.WriteNode(xmlReader, false);
					}
					return stringWriter.ToString();
				}
				}
			}
			set
			{
				DeleteChildren();
				if (NodeType == XPathNodeType.Attribute)
				{
					SetValue(value);
				}
				else
				{
					AppendChild(value);
				}
			}
		}

		public sealed override bool IsNode
		{
			get
			{
				return true;
			}
		}

		public virtual string OuterXml
		{
			get
			{
				switch (NodeType)
				{
				case XPathNodeType.Attribute:
					return Prefix + ((Prefix.Length <= 0) ? string.Empty : ":") + LocalName + "=\"" + EscapeString(Value, true) + "\"";
				case XPathNodeType.Namespace:
					return "xmlns" + ((LocalName.Length <= 0) ? string.Empty : ":") + LocalName + "=\"" + EscapeString(Value, true) + "\"";
				case XPathNodeType.Text:
					return EscapeString(Value, false);
				case XPathNodeType.SignificantWhitespace:
				case XPathNodeType.Whitespace:
					return Value;
				default:
				{
					XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
					xmlWriterSettings.Indent = true;
					xmlWriterSettings.OmitXmlDeclaration = true;
					xmlWriterSettings.ConformanceLevel = ConformanceLevel.Fragment;
					StringBuilder stringBuilder = new StringBuilder();
					using (XmlWriter writer = XmlWriter.Create(stringBuilder, xmlWriterSettings))
					{
						WriteSubtree(writer);
					}
					return stringBuilder.ToString();
				}
				}
			}
			set
			{
				switch (NodeType)
				{
				case XPathNodeType.Root:
				case XPathNodeType.Attribute:
				case XPathNodeType.Namespace:
					throw new XmlException("Setting OuterXml Root, Attribute and Namespace is not supported.");
				}
				DeleteSelf();
				AppendChild(value);
				MoveToFirstChild();
			}
		}

		public virtual IXmlSchemaInfo SchemaInfo
		{
			get
			{
				return null;
			}
		}

		public override object TypedValue
		{
			get
			{
				XPathNodeType nodeType = NodeType;
				if ((nodeType == XPathNodeType.Element || nodeType == XPathNodeType.Attribute) && XmlType != null)
				{
					XmlSchemaDatatype datatype = XmlType.Datatype;
					if (datatype != null)
					{
						return datatype.ParseValue(Value, NameTable, this);
					}
				}
				return Value;
			}
		}

		public virtual object UnderlyingObject
		{
			get
			{
				return null;
			}
		}

		public override bool ValueAsBoolean
		{
			get
			{
				return XQueryConvert.StringToBoolean(Value);
			}
		}

		public override DateTime ValueAsDateTime
		{
			get
			{
				return XmlConvert.ToDateTime(Value);
			}
		}

		public override double ValueAsDouble
		{
			get
			{
				return XQueryConvert.StringToDouble(Value);
			}
		}

		public override int ValueAsInt
		{
			get
			{
				return XQueryConvert.StringToInt(Value);
			}
		}

		public override long ValueAsLong
		{
			get
			{
				return XQueryConvert.StringToInteger(Value);
			}
		}

		public override Type ValueType
		{
			get
			{
				return (SchemaInfo == null || SchemaInfo.SchemaType == null || SchemaInfo.SchemaType.Datatype == null) ? null : SchemaInfo.SchemaType.Datatype.ValueType;
			}
		}

		public override XmlSchemaType XmlType
		{
			get
			{
				if (SchemaInfo != null)
				{
					return SchemaInfo.SchemaType;
				}
				return null;
			}
		}

		object ICloneable.Clone()
		{
			return Clone();
		}

		public abstract XPathNavigator Clone();

		public virtual XmlNodeOrder ComparePosition(XPathNavigator nav)
		{
			if (IsSamePosition(nav))
			{
				return XmlNodeOrder.Same;
			}
			if (IsDescendant(nav))
			{
				return XmlNodeOrder.Before;
			}
			if (nav.IsDescendant(this))
			{
				return XmlNodeOrder.After;
			}
			XPathNavigator xPathNavigator = Clone();
			XPathNavigator xPathNavigator2 = nav.Clone();
			xPathNavigator.MoveToRoot();
			xPathNavigator2.MoveToRoot();
			if (!xPathNavigator.IsSamePosition(xPathNavigator2))
			{
				return XmlNodeOrder.Unknown;
			}
			xPathNavigator.MoveTo(this);
			xPathNavigator2.MoveTo(nav);
			int num = 0;
			while (xPathNavigator.MoveToParent())
			{
				num++;
			}
			xPathNavigator.MoveTo(this);
			int num2 = 0;
			while (xPathNavigator2.MoveToParent())
			{
				num2++;
			}
			xPathNavigator2.MoveTo(nav);
			int num3;
			for (num3 = num; num3 > num2; num3--)
			{
				xPathNavigator.MoveToParent();
			}
			for (int num4 = num2; num4 > num3; num4--)
			{
				xPathNavigator2.MoveToParent();
			}
			while (!xPathNavigator.IsSamePosition(xPathNavigator2))
			{
				xPathNavigator.MoveToParent();
				xPathNavigator2.MoveToParent();
				num3--;
			}
			xPathNavigator.MoveTo(this);
			for (int num5 = num; num5 > num3 + 1; num5--)
			{
				xPathNavigator.MoveToParent();
			}
			xPathNavigator2.MoveTo(nav);
			for (int num6 = num2; num6 > num3 + 1; num6--)
			{
				xPathNavigator2.MoveToParent();
			}
			if (xPathNavigator.NodeType == XPathNodeType.Namespace)
			{
				if (xPathNavigator2.NodeType != XPathNodeType.Namespace)
				{
					return XmlNodeOrder.Before;
				}
				while (xPathNavigator.MoveToNextNamespace())
				{
					if (xPathNavigator.IsSamePosition(xPathNavigator2))
					{
						return XmlNodeOrder.Before;
					}
				}
				return XmlNodeOrder.After;
			}
			if (xPathNavigator2.NodeType == XPathNodeType.Namespace)
			{
				return XmlNodeOrder.After;
			}
			if (xPathNavigator.NodeType == XPathNodeType.Attribute)
			{
				if (xPathNavigator2.NodeType != XPathNodeType.Attribute)
				{
					return XmlNodeOrder.Before;
				}
				while (xPathNavigator.MoveToNextAttribute())
				{
					if (xPathNavigator.IsSamePosition(xPathNavigator2))
					{
						return XmlNodeOrder.Before;
					}
				}
				return XmlNodeOrder.After;
			}
			while (xPathNavigator.MoveToNext())
			{
				if (xPathNavigator.IsSamePosition(xPathNavigator2))
				{
					return XmlNodeOrder.Before;
				}
			}
			return XmlNodeOrder.After;
		}

		public virtual XPathExpression Compile(string xpath)
		{
			return XPathExpression.Compile(xpath);
		}

		internal virtual XPathExpression Compile(string xpath, IStaticXsltContext ctx)
		{
			return XPathExpression.Compile(xpath, null, ctx);
		}

		public virtual object Evaluate(string xpath)
		{
			return Evaluate(Compile(xpath));
		}

		public virtual object Evaluate(XPathExpression expr)
		{
			return Evaluate(expr, null);
		}

		public virtual object Evaluate(XPathExpression expr, XPathNodeIterator context)
		{
			return Evaluate(expr, context, null);
		}

		private BaseIterator ToBaseIterator(XPathNodeIterator iter, IXmlNamespaceResolver ctx)
		{
			BaseIterator baseIterator = iter as BaseIterator;
			if (baseIterator == null)
			{
				baseIterator = new WrapperIterator(iter, ctx);
			}
			return baseIterator;
		}

		private object Evaluate(XPathExpression expr, XPathNodeIterator context, IXmlNamespaceResolver ctx)
		{
			CompiledExpression compiledExpression = (CompiledExpression)expr;
			if (ctx == null)
			{
				ctx = compiledExpression.NamespaceManager;
			}
			if (context == null)
			{
				context = new NullIterator(this, ctx);
			}
			BaseIterator baseIterator = ToBaseIterator(context, ctx);
			baseIterator.NamespaceManager = ctx;
			return compiledExpression.Evaluate(baseIterator);
		}

		internal XPathNodeIterator EvaluateNodeSet(XPathExpression expr, XPathNodeIterator context, IXmlNamespaceResolver ctx)
		{
			CompiledExpression compiledExpression = (CompiledExpression)expr;
			if (ctx == null)
			{
				ctx = compiledExpression.NamespaceManager;
			}
			if (context == null)
			{
				context = new NullIterator(this, compiledExpression.NamespaceManager);
			}
			BaseIterator baseIterator = ToBaseIterator(context, ctx);
			baseIterator.NamespaceManager = ctx;
			return compiledExpression.EvaluateNodeSet(baseIterator);
		}

		internal string EvaluateString(XPathExpression expr, XPathNodeIterator context, IXmlNamespaceResolver ctx)
		{
			CompiledExpression compiledExpression = (CompiledExpression)expr;
			if (ctx == null)
			{
				ctx = compiledExpression.NamespaceManager;
			}
			if (context == null)
			{
				context = new NullIterator(this, compiledExpression.NamespaceManager);
			}
			BaseIterator iter = ToBaseIterator(context, ctx);
			return compiledExpression.EvaluateString(iter);
		}

		internal double EvaluateNumber(XPathExpression expr, XPathNodeIterator context, IXmlNamespaceResolver ctx)
		{
			CompiledExpression compiledExpression = (CompiledExpression)expr;
			if (ctx == null)
			{
				ctx = compiledExpression.NamespaceManager;
			}
			if (context == null)
			{
				context = new NullIterator(this, compiledExpression.NamespaceManager);
			}
			BaseIterator baseIterator = ToBaseIterator(context, ctx);
			baseIterator.NamespaceManager = ctx;
			return compiledExpression.EvaluateNumber(baseIterator);
		}

		internal bool EvaluateBoolean(XPathExpression expr, XPathNodeIterator context, IXmlNamespaceResolver ctx)
		{
			CompiledExpression compiledExpression = (CompiledExpression)expr;
			if (ctx == null)
			{
				ctx = compiledExpression.NamespaceManager;
			}
			if (context == null)
			{
				context = new NullIterator(this, compiledExpression.NamespaceManager);
			}
			BaseIterator baseIterator = ToBaseIterator(context, ctx);
			baseIterator.NamespaceManager = ctx;
			return compiledExpression.EvaluateBoolean(baseIterator);
		}

		public virtual string GetAttribute(string localName, string namespaceURI)
		{
			if (!MoveToAttribute(localName, namespaceURI))
			{
				return string.Empty;
			}
			string value = Value;
			MoveToParent();
			return value;
		}

		public virtual string GetNamespace(string name)
		{
			if (!MoveToNamespace(name))
			{
				return string.Empty;
			}
			string value = Value;
			MoveToParent();
			return value;
		}

		public virtual bool IsDescendant(XPathNavigator nav)
		{
			if (nav != null)
			{
				nav = nav.Clone();
				while (nav.MoveToParent())
				{
					if (IsSamePosition(nav))
					{
						return true;
					}
				}
			}
			return false;
		}

		public abstract bool IsSamePosition(XPathNavigator other);

		public virtual bool Matches(string xpath)
		{
			return Matches(Compile(xpath));
		}

		public virtual bool Matches(XPathExpression expr)
		{
			Expression expression = ((CompiledExpression)expr).ExpressionNode;
			if (expression is ExprRoot)
			{
				return NodeType == XPathNodeType.Root;
			}
			NodeTest nodeTest = expression as NodeTest;
			if (nodeTest != null)
			{
				Axes axis = nodeTest.Axis.Axis;
				if (axis != Axes.Attribute && axis != Axes.Child)
				{
					throw new XPathException("Only child and attribute pattern are allowed for a pattern.");
				}
				return nodeTest.Match(((CompiledExpression)expr).NamespaceManager, this);
			}
			if (expression is ExprFilter)
			{
				do
				{
					expression = ((ExprFilter)expression).LeftHandSide;
				}
				while (expression is ExprFilter);
				if (expression is NodeTest && !((NodeTest)expression).Match(((CompiledExpression)expr).NamespaceManager, this))
				{
					return false;
				}
			}
			switch (expression.ReturnType)
			{
			default:
				return false;
			case XPathResultType.NodeSet:
			case XPathResultType.Any:
			{
				XPathNodeType evaluatedNodeType = expression.EvaluatedNodeType;
				if ((evaluatedNodeType == XPathNodeType.Attribute || evaluatedNodeType == XPathNodeType.Namespace) && NodeType != expression.EvaluatedNodeType)
				{
					return false;
				}
				XPathNodeIterator xPathNodeIterator = Select(expr);
				while (xPathNodeIterator.MoveNext())
				{
					if (IsSamePosition(xPathNodeIterator.Current))
					{
						return true;
					}
				}
				XPathNavigator xPathNavigator = Clone();
				while (xPathNavigator.MoveToParent())
				{
					xPathNodeIterator = xPathNavigator.Select(expr);
					while (xPathNodeIterator.MoveNext())
					{
						if (IsSamePosition(xPathNodeIterator.Current))
						{
							return true;
						}
					}
				}
				return false;
			}
			}
		}

		public abstract bool MoveTo(XPathNavigator other);

		public virtual bool MoveToAttribute(string localName, string namespaceURI)
		{
			if (MoveToFirstAttribute())
			{
				do
				{
					if (LocalName == localName && NamespaceURI == namespaceURI)
					{
						return true;
					}
				}
				while (MoveToNextAttribute());
				MoveToParent();
			}
			return false;
		}

		public virtual bool MoveToNamespace(string name)
		{
			if (MoveToFirstNamespace())
			{
				do
				{
					if (LocalName == name)
					{
						return true;
					}
				}
				while (MoveToNextNamespace());
				MoveToParent();
			}
			return false;
		}

		public virtual bool MoveToFirst()
		{
			return MoveToFirstImpl();
		}

		public virtual void MoveToRoot()
		{
			while (MoveToParent())
			{
			}
		}

		internal bool MoveToFirstImpl()
		{
			XPathNodeType nodeType = NodeType;
			if (nodeType == XPathNodeType.Attribute || nodeType == XPathNodeType.Namespace)
			{
				return false;
			}
			if (!MoveToParent())
			{
				return false;
			}
			MoveToFirstChild();
			return true;
		}

		public abstract bool MoveToFirstAttribute();

		public abstract bool MoveToFirstChild();

		public bool MoveToFirstNamespace()
		{
			return MoveToFirstNamespace(XPathNamespaceScope.All);
		}

		public abstract bool MoveToFirstNamespace(XPathNamespaceScope namespaceScope);

		public abstract bool MoveToId(string id);

		public abstract bool MoveToNext();

		public abstract bool MoveToNextAttribute();

		public bool MoveToNextNamespace()
		{
			return MoveToNextNamespace(XPathNamespaceScope.All);
		}

		public abstract bool MoveToNextNamespace(XPathNamespaceScope namespaceScope);

		public abstract bool MoveToParent();

		public abstract bool MoveToPrevious();

		public virtual XPathNodeIterator Select(string xpath)
		{
			return Select(Compile(xpath));
		}

		public virtual XPathNodeIterator Select(XPathExpression expr)
		{
			return Select(expr, null);
		}

		internal XPathNodeIterator Select(XPathExpression expr, IXmlNamespaceResolver ctx)
		{
			CompiledExpression compiledExpression = (CompiledExpression)expr;
			if (ctx == null)
			{
				ctx = compiledExpression.NamespaceManager;
			}
			BaseIterator iter = new NullIterator(this, ctx);
			return compiledExpression.EvaluateNodeSet(iter);
		}

		public virtual XPathNodeIterator SelectAncestors(XPathNodeType type, bool matchSelf)
		{
			Axes axis = (matchSelf ? Axes.AncestorOrSelf : Axes.Ancestor);
			return SelectTest(new NodeTypeTest(axis, type));
		}

		public virtual XPathNodeIterator SelectAncestors(string name, string namespaceURI, bool matchSelf)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (namespaceURI == null)
			{
				throw new ArgumentNullException("namespaceURI");
			}
			Axes axis = (matchSelf ? Axes.AncestorOrSelf : Axes.Ancestor);
			XmlQualifiedName name2 = new XmlQualifiedName(name, namespaceURI);
			return SelectTest(new NodeNameTest(axis, name2, true));
		}

		private static IEnumerable EnumerateChildren(XPathNavigator n, XPathNodeType type)
		{
			if (!n.MoveToFirstChild())
			{
				yield break;
			}
			n.MoveToParent();
			XPathNavigator nav = n.Clone();
			nav.MoveToFirstChild();
			XPathNavigator nav2 = null;
			do
			{
				if (type == XPathNodeType.All || nav.NodeType == type)
				{
					if (nav2 == null)
					{
						nav2 = nav.Clone();
					}
					else
					{
						nav2.MoveTo(nav);
					}
					yield return nav2;
				}
			}
			while (nav.MoveToNext());
		}

		public virtual XPathNodeIterator SelectChildren(XPathNodeType type)
		{
			return new WrapperIterator(new EnumerableIterator(EnumerateChildren(this, type), 0), null);
		}

		private static IEnumerable EnumerateChildren(XPathNavigator n, string name, string ns)
		{
			if (!n.MoveToFirstChild())
			{
				yield break;
			}
			n.MoveToParent();
			XPathNavigator nav = n.Clone();
			nav.MoveToFirstChild();
			XPathNavigator nav2 = nav.Clone();
			do
			{
				if ((name == string.Empty || nav.LocalName == name) && (ns == string.Empty || nav.NamespaceURI == ns))
				{
					nav2.MoveTo(nav);
					yield return nav2;
				}
			}
			while (nav.MoveToNext());
		}

		public virtual XPathNodeIterator SelectChildren(string name, string namespaceURI)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (namespaceURI == null)
			{
				throw new ArgumentNullException("namespaceURI");
			}
			return new WrapperIterator(new EnumerableIterator(EnumerateChildren(this, name, namespaceURI), 0), null);
		}

		public virtual XPathNodeIterator SelectDescendants(XPathNodeType type, bool matchSelf)
		{
			Axes axis = ((!matchSelf) ? Axes.Descendant : Axes.DescendantOrSelf);
			return SelectTest(new NodeTypeTest(axis, type));
		}

		public virtual XPathNodeIterator SelectDescendants(string name, string namespaceURI, bool matchSelf)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (namespaceURI == null)
			{
				throw new ArgumentNullException("namespaceURI");
			}
			Axes axis = ((!matchSelf) ? Axes.Descendant : Axes.DescendantOrSelf);
			XmlQualifiedName name2 = new XmlQualifiedName(name, namespaceURI);
			return SelectTest(new NodeNameTest(axis, name2, true));
		}

		internal XPathNodeIterator SelectTest(NodeTest test)
		{
			return test.EvaluateNodeSet(new NullIterator(this));
		}

		public override string ToString()
		{
			return Value;
		}

		public virtual bool CheckValidity(XmlSchemaSet schemas, ValidationEventHandler handler)
		{
			XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
			xmlReaderSettings.NameTable = NameTable;
			xmlReaderSettings.SetSchemas(schemas);
			xmlReaderSettings.ValidationEventHandler += handler;
			xmlReaderSettings.ValidationType = ValidationType.Schema;
			try
			{
				XmlReader xmlReader = XmlReader.Create(ReadSubtree(), xmlReaderSettings);
				while (!xmlReader.EOF)
				{
					xmlReader.Read();
				}
			}
			catch (XmlSchemaValidationException)
			{
				return false;
			}
			return true;
		}

		public virtual XPathNavigator CreateNavigator()
		{
			return Clone();
		}

		public virtual object Evaluate(string xpath, IXmlNamespaceResolver nsResolver)
		{
			return Evaluate(Compile(xpath), null, nsResolver);
		}

		public virtual IDictionary<string, string> GetNamespacesInScope(XmlNamespaceScope scope)
		{
			IDictionary<string, string> dictionary = new Dictionary<string, string>();
			int num;
			switch (scope)
			{
			case XmlNamespaceScope.Local:
				num = 2;
				break;
			case XmlNamespaceScope.ExcludeXml:
				num = 1;
				break;
			default:
				num = 0;
				break;
			}
			XPathNamespaceScope namespaceScope = (XPathNamespaceScope)num;
			XPathNavigator xPathNavigator = Clone();
			if (xPathNavigator.NodeType != XPathNodeType.Element)
			{
				xPathNavigator.MoveToParent();
			}
			if (!xPathNavigator.MoveToFirstNamespace(namespaceScope))
			{
				return dictionary;
			}
			do
			{
				dictionary.Add(xPathNavigator.Name, xPathNavigator.Value);
			}
			while (xPathNavigator.MoveToNextNamespace(namespaceScope));
			return dictionary;
		}

		public virtual string LookupNamespace(string prefix)
		{
			XPathNavigator xPathNavigator = Clone();
			if (xPathNavigator.NodeType != XPathNodeType.Element)
			{
				xPathNavigator.MoveToParent();
			}
			if (xPathNavigator.MoveToNamespace(prefix))
			{
				return xPathNavigator.Value;
			}
			return null;
		}

		public virtual string LookupPrefix(string namespaceUri)
		{
			XPathNavigator xPathNavigator = Clone();
			if (xPathNavigator.NodeType != XPathNodeType.Element)
			{
				xPathNavigator.MoveToParent();
			}
			if (!xPathNavigator.MoveToFirstNamespace())
			{
				return null;
			}
			do
			{
				if (xPathNavigator.Value == namespaceUri)
				{
					return xPathNavigator.Name;
				}
			}
			while (xPathNavigator.MoveToNextNamespace());
			return null;
		}

		private bool MoveTo(XPathNodeIterator iter)
		{
			if (iter.MoveNext())
			{
				MoveTo(iter.Current);
				return true;
			}
			return false;
		}

		public virtual bool MoveToChild(XPathNodeType type)
		{
			return MoveTo(SelectChildren(type));
		}

		public virtual bool MoveToChild(string localName, string namespaceURI)
		{
			return MoveTo(SelectChildren(localName, namespaceURI));
		}

		public virtual bool MoveToNext(string localName, string namespaceURI)
		{
			XPathNavigator xPathNavigator = Clone();
			while (xPathNavigator.MoveToNext())
			{
				if (xPathNavigator.LocalName == localName && xPathNavigator.NamespaceURI == namespaceURI)
				{
					MoveTo(xPathNavigator);
					return true;
				}
			}
			return false;
		}

		public virtual bool MoveToNext(XPathNodeType type)
		{
			XPathNavigator xPathNavigator = Clone();
			while (xPathNavigator.MoveToNext())
			{
				if (type == XPathNodeType.All || xPathNavigator.NodeType == type)
				{
					MoveTo(xPathNavigator);
					return true;
				}
			}
			return false;
		}

		public virtual bool MoveToFollowing(string localName, string namespaceURI)
		{
			return MoveToFollowing(localName, namespaceURI, null);
		}

		public virtual bool MoveToFollowing(string localName, string namespaceURI, XPathNavigator end)
		{
			if (localName == null)
			{
				throw new ArgumentNullException("localName");
			}
			if (namespaceURI == null)
			{
				throw new ArgumentNullException("namespaceURI");
			}
			localName = NameTable.Get(localName);
			if (localName == null)
			{
				return false;
			}
			namespaceURI = NameTable.Get(namespaceURI);
			if (namespaceURI == null)
			{
				return false;
			}
			XPathNavigator xPathNavigator = Clone();
			XPathNodeType nodeType = xPathNavigator.NodeType;
			if (nodeType == XPathNodeType.Attribute || nodeType == XPathNodeType.Namespace)
			{
				xPathNavigator.MoveToParent();
			}
			do
			{
				if (!xPathNavigator.MoveToFirstChild())
				{
					while (!xPathNavigator.MoveToNext())
					{
						if (!xPathNavigator.MoveToParent())
						{
							return false;
						}
					}
				}
				if (end != null && end.IsSamePosition(xPathNavigator))
				{
					return false;
				}
			}
			while (!object.ReferenceEquals(localName, xPathNavigator.LocalName) || !object.ReferenceEquals(namespaceURI, xPathNavigator.NamespaceURI));
			MoveTo(xPathNavigator);
			return true;
		}

		public virtual bool MoveToFollowing(XPathNodeType type)
		{
			return MoveToFollowing(type, null);
		}

		public virtual bool MoveToFollowing(XPathNodeType type, XPathNavigator end)
		{
			if (type == XPathNodeType.Root)
			{
				return false;
			}
			XPathNavigator xPathNavigator = Clone();
			XPathNodeType nodeType = xPathNavigator.NodeType;
			if (nodeType == XPathNodeType.Attribute || nodeType == XPathNodeType.Namespace)
			{
				xPathNavigator.MoveToParent();
			}
			do
			{
				if (!xPathNavigator.MoveToFirstChild())
				{
					while (!xPathNavigator.MoveToNext())
					{
						if (!xPathNavigator.MoveToParent())
						{
							return false;
						}
					}
				}
				if (end != null && end.IsSamePosition(xPathNavigator))
				{
					return false;
				}
			}
			while (type != XPathNodeType.All && xPathNavigator.NodeType != type);
			MoveTo(xPathNavigator);
			return true;
		}

		public virtual XmlReader ReadSubtree()
		{
			XPathNodeType nodeType = NodeType;
			if (nodeType == XPathNodeType.Root || nodeType == XPathNodeType.Element)
			{
				return new XPathNavigatorReader(this);
			}
			throw new InvalidOperationException(string.Format("NodeType {0} is not supported to read as a subtree of an XPathNavigator.", NodeType));
		}

		public virtual XPathNodeIterator Select(string xpath, IXmlNamespaceResolver nsResolver)
		{
			return Select(Compile(xpath), nsResolver);
		}

		public virtual XPathNavigator SelectSingleNode(string xpath)
		{
			return SelectSingleNode(xpath, null);
		}

		public virtual XPathNavigator SelectSingleNode(string xpath, IXmlNamespaceResolver nsResolver)
		{
			XPathExpression xPathExpression = Compile(xpath);
			xPathExpression.SetContext(nsResolver);
			return SelectSingleNode(xPathExpression);
		}

		public virtual XPathNavigator SelectSingleNode(XPathExpression expression)
		{
			XPathNodeIterator xPathNodeIterator = Select(expression);
			if (xPathNodeIterator.MoveNext())
			{
				return xPathNodeIterator.Current;
			}
			return null;
		}

		public override object ValueAs(Type type, IXmlNamespaceResolver nsResolver)
		{
			return new XmlAtomicValue(Value, XmlSchemaSimpleType.XsString).ValueAs(type, nsResolver);
		}

		public virtual void WriteSubtree(XmlWriter writer)
		{
			writer.WriteNode(this, false);
		}

		private static string EscapeString(string value, bool attr)
		{
			StringBuilder stringBuilder = null;
			char[] anyOf = ((!attr) ? escape_text_chars : escape_attr_chars);
			if (value.IndexOfAny(anyOf) < 0)
			{
				return value;
			}
			stringBuilder = new StringBuilder(value, value.Length + 10);
			if (attr)
			{
				stringBuilder.Replace("\"", "&quot;");
			}
			stringBuilder.Replace("<", "&lt;");
			stringBuilder.Replace(">", "&gt;");
			if (attr)
			{
				stringBuilder.Replace("\r\n", "&#10;");
				stringBuilder.Replace("\r", "&#10;");
				stringBuilder.Replace("\n", "&#10;");
			}
			return stringBuilder.ToString();
		}

		private XmlReader CreateFragmentReader(string fragment)
		{
			XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
			xmlReaderSettings.ConformanceLevel = ConformanceLevel.Fragment;
			XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(NameTable);
			foreach (KeyValuePair<string, string> item in GetNamespacesInScope(XmlNamespaceScope.All))
			{
				xmlNamespaceManager.AddNamespace(item.Key, item.Value);
			}
			return XmlReader.Create(new StringReader(fragment), xmlReaderSettings, new XmlParserContext(NameTable, xmlNamespaceManager, null, XmlSpace.None));
		}

		public virtual XmlWriter AppendChild()
		{
			throw new NotSupportedException();
		}

		public virtual void AppendChild(string xmlFragments)
		{
			AppendChild(CreateFragmentReader(xmlFragments));
		}

		public virtual void AppendChild(XmlReader reader)
		{
			XmlWriter xmlWriter = AppendChild();
			while (!reader.EOF)
			{
				xmlWriter.WriteNode(reader, false);
			}
			xmlWriter.Close();
		}

		public virtual void AppendChild(XPathNavigator nav)
		{
			AppendChild(new XPathNavigatorReader(nav));
		}

		public virtual void AppendChildElement(string prefix, string name, string ns, string value)
		{
			XmlWriter xmlWriter = AppendChild();
			xmlWriter.WriteStartElement(prefix, name, ns);
			xmlWriter.WriteString(value);
			xmlWriter.WriteEndElement();
			xmlWriter.Close();
		}

		public virtual void CreateAttribute(string prefix, string localName, string namespaceURI, string value)
		{
			using (XmlWriter xmlWriter = CreateAttributes())
			{
				xmlWriter.WriteAttributeString(prefix, localName, namespaceURI, value);
			}
		}

		public virtual XmlWriter CreateAttributes()
		{
			throw new NotSupportedException();
		}

		public virtual void DeleteSelf()
		{
			throw new NotSupportedException();
		}

		public virtual void DeleteRange(XPathNavigator nav)
		{
			throw new NotSupportedException();
		}

		public virtual XmlWriter ReplaceRange(XPathNavigator nav)
		{
			throw new NotSupportedException();
		}

		public virtual XmlWriter InsertAfter()
		{
			switch (NodeType)
			{
			case XPathNodeType.Root:
			case XPathNodeType.Attribute:
			case XPathNodeType.Namespace:
				throw new InvalidOperationException(string.Format("Insertion after {0} is not allowed.", NodeType));
			default:
			{
				XPathNavigator xPathNavigator = Clone();
				if (xPathNavigator.MoveToNext())
				{
					return xPathNavigator.InsertBefore();
				}
				if (xPathNavigator.MoveToParent())
				{
					return xPathNavigator.AppendChild();
				}
				throw new InvalidOperationException("Could not move to parent to insert sibling node");
			}
			}
		}

		public virtual void InsertAfter(string xmlFragments)
		{
			InsertAfter(CreateFragmentReader(xmlFragments));
		}

		public virtual void InsertAfter(XmlReader reader)
		{
			using (XmlWriter xmlWriter = InsertAfter())
			{
				xmlWriter.WriteNode(reader, false);
			}
		}

		public virtual void InsertAfter(XPathNavigator nav)
		{
			InsertAfter(new XPathNavigatorReader(nav));
		}

		public virtual XmlWriter InsertBefore()
		{
			throw new NotSupportedException();
		}

		public virtual void InsertBefore(string xmlFragments)
		{
			InsertBefore(CreateFragmentReader(xmlFragments));
		}

		public virtual void InsertBefore(XmlReader reader)
		{
			using (XmlWriter xmlWriter = InsertBefore())
			{
				xmlWriter.WriteNode(reader, false);
			}
		}

		public virtual void InsertBefore(XPathNavigator nav)
		{
			InsertBefore(new XPathNavigatorReader(nav));
		}

		public virtual void InsertElementAfter(string prefix, string localName, string namespaceURI, string value)
		{
			using (XmlWriter xmlWriter = InsertAfter())
			{
				xmlWriter.WriteElementString(prefix, localName, namespaceURI, value);
			}
		}

		public virtual void InsertElementBefore(string prefix, string localName, string namespaceURI, string value)
		{
			using (XmlWriter xmlWriter = InsertBefore())
			{
				xmlWriter.WriteElementString(prefix, localName, namespaceURI, value);
			}
		}

		public virtual XmlWriter PrependChild()
		{
			XPathNavigator xPathNavigator = Clone();
			if (xPathNavigator.MoveToFirstChild())
			{
				return xPathNavigator.InsertBefore();
			}
			return AppendChild();
		}

		public virtual void PrependChild(string xmlFragments)
		{
			PrependChild(CreateFragmentReader(xmlFragments));
		}

		public virtual void PrependChild(XmlReader reader)
		{
			using (XmlWriter xmlWriter = PrependChild())
			{
				xmlWriter.WriteNode(reader, false);
			}
		}

		public virtual void PrependChild(XPathNavigator nav)
		{
			PrependChild(new XPathNavigatorReader(nav));
		}

		public virtual void PrependChildElement(string prefix, string localName, string namespaceURI, string value)
		{
			using (XmlWriter xmlWriter = PrependChild())
			{
				xmlWriter.WriteElementString(prefix, localName, namespaceURI, value);
			}
		}

		public virtual void ReplaceSelf(string xmlFragment)
		{
			ReplaceSelf(CreateFragmentReader(xmlFragment));
		}

		public virtual void ReplaceSelf(XmlReader reader)
		{
			throw new NotSupportedException();
		}

		public virtual void ReplaceSelf(XPathNavigator navigator)
		{
			ReplaceSelf(new XPathNavigatorReader(navigator));
		}

		[System.MonoTODO]
		public virtual void SetTypedValue(object value)
		{
			throw new NotSupportedException();
		}

		public virtual void SetValue(string value)
		{
			throw new NotSupportedException();
		}

		private void DeleteChildren()
		{
			switch (NodeType)
			{
			case XPathNodeType.Namespace:
				throw new InvalidOperationException("Removing namespace node content is not supported.");
			case XPathNodeType.Attribute:
				return;
			case XPathNodeType.Text:
			case XPathNodeType.SignificantWhitespace:
			case XPathNodeType.Whitespace:
			case XPathNodeType.ProcessingInstruction:
			case XPathNodeType.Comment:
				DeleteSelf();
				return;
			}
			if (HasChildren)
			{
				XPathNavigator xPathNavigator = Clone();
				xPathNavigator.MoveToFirstChild();
				while (!xPathNavigator.IsSamePosition(this))
				{
					xPathNavigator.DeleteSelf();
				}
			}
		}
	}
}

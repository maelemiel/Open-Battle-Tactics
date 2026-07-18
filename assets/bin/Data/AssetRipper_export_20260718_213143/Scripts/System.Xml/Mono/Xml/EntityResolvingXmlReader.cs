using System;
using System.Collections.Generic;
using System.Xml;

namespace Mono.Xml
{
	internal class EntityResolvingXmlReader : XmlReader, IHasXmlParserContext, IXmlLineInfo, IXmlNamespaceResolver
	{
		private EntityResolvingXmlReader entity;

		private XmlReader source;

		private XmlParserContext context;

		private XmlResolver resolver;

		private EntityHandling entity_handling;

		private bool entity_inside_attr;

		private bool inside_attr;

		private bool do_resolve;

		XmlParserContext IHasXmlParserContext.ParserContext
		{
			get
			{
				return context;
			}
		}

		private XmlReader Current
		{
			get
			{
				return (entity == null || entity.ReadState == ReadState.Initial) ? source : entity;
			}
		}

		public override int AttributeCount
		{
			get
			{
				return Current.AttributeCount;
			}
		}

		public override string BaseURI
		{
			get
			{
				return Current.BaseURI;
			}
		}

		public override bool CanResolveEntity
		{
			get
			{
				return true;
			}
		}

		public override int Depth
		{
			get
			{
				if (entity != null && entity.ReadState == ReadState.Interactive)
				{
					return source.Depth + entity.Depth + 1;
				}
				return source.Depth;
			}
		}

		public override bool EOF
		{
			get
			{
				return source.EOF;
			}
		}

		public override bool HasValue
		{
			get
			{
				return Current.HasValue;
			}
		}

		public override bool IsDefault
		{
			get
			{
				return Current.IsDefault;
			}
		}

		public override bool IsEmptyElement
		{
			get
			{
				return Current.IsEmptyElement;
			}
		}

		public override string LocalName
		{
			get
			{
				return Current.LocalName;
			}
		}

		public override string Name
		{
			get
			{
				return Current.Name;
			}
		}

		public override string NamespaceURI
		{
			get
			{
				return Current.NamespaceURI;
			}
		}

		public override XmlNameTable NameTable
		{
			get
			{
				return Current.NameTable;
			}
		}

		public override XmlNodeType NodeType
		{
			get
			{
				if (entity != null)
				{
					if (entity.ReadState == ReadState.Initial)
					{
						return source.NodeType;
					}
					return (!entity.EOF) ? entity.NodeType : XmlNodeType.EndEntity;
				}
				return source.NodeType;
			}
		}

		internal XmlParserContext ParserContext
		{
			get
			{
				return context;
			}
		}

		public override string Prefix
		{
			get
			{
				return Current.Prefix;
			}
		}

		public override char QuoteChar
		{
			get
			{
				return Current.QuoteChar;
			}
		}

		public override ReadState ReadState
		{
			get
			{
				return (entity != null) ? ReadState.Interactive : source.ReadState;
			}
		}

		public override string Value
		{
			get
			{
				return Current.Value;
			}
		}

		public override string XmlLang
		{
			get
			{
				return Current.XmlLang;
			}
		}

		public override XmlSpace XmlSpace
		{
			get
			{
				return Current.XmlSpace;
			}
		}

		public EntityHandling EntityHandling
		{
			get
			{
				return entity_handling;
			}
			set
			{
				if (entity != null)
				{
					entity.EntityHandling = value;
				}
				entity_handling = value;
			}
		}

		public int LineNumber
		{
			get
			{
				IXmlLineInfo xmlLineInfo = Current as IXmlLineInfo;
				return (xmlLineInfo != null) ? xmlLineInfo.LineNumber : 0;
			}
		}

		public int LinePosition
		{
			get
			{
				IXmlLineInfo xmlLineInfo = Current as IXmlLineInfo;
				return (xmlLineInfo != null) ? xmlLineInfo.LinePosition : 0;
			}
		}

		public XmlResolver XmlResolver
		{
			set
			{
				if (entity != null)
				{
					entity.XmlResolver = value;
				}
				resolver = value;
			}
		}

		public EntityResolvingXmlReader(XmlReader source)
		{
			this.source = source;
			IHasXmlParserContext hasXmlParserContext = source as IHasXmlParserContext;
			if (hasXmlParserContext != null)
			{
				context = hasXmlParserContext.ParserContext;
			}
			else
			{
				context = new XmlParserContext(source.NameTable, new XmlNamespaceManager(source.NameTable), null, XmlSpace.None);
			}
		}

		private EntityResolvingXmlReader(XmlReader entityContainer, bool inside_attr)
		{
			source = entityContainer;
			entity_inside_attr = inside_attr;
		}

		IDictionary<string, string> IXmlNamespaceResolver.GetNamespacesInScope(XmlNamespaceScope scope)
		{
			return GetNamespacesInScope(scope);
		}

		string IXmlNamespaceResolver.LookupPrefix(string ns)
		{
			return ((IXmlNamespaceResolver)Current).LookupPrefix(ns);
		}

		private void CopyProperties(EntityResolvingXmlReader other)
		{
			context = other.context;
			resolver = other.resolver;
			entity_handling = other.entity_handling;
		}

		public override void Close()
		{
			if (entity != null)
			{
				entity.Close();
			}
			source.Close();
		}

		public override string GetAttribute(int i)
		{
			return Current.GetAttribute(i);
		}

		public override string GetAttribute(string name)
		{
			return Current.GetAttribute(name);
		}

		public override string GetAttribute(string localName, string namespaceURI)
		{
			return Current.GetAttribute(localName, namespaceURI);
		}

		public IDictionary<string, string> GetNamespacesInScope(XmlNamespaceScope scope)
		{
			return ((IXmlNamespaceResolver)Current).GetNamespacesInScope(scope);
		}

		public override string LookupNamespace(string prefix)
		{
			return Current.LookupNamespace(prefix);
		}

		public override void MoveToAttribute(int i)
		{
			if (entity != null && entity_inside_attr)
			{
				entity.Close();
				entity = null;
			}
			Current.MoveToAttribute(i);
			inside_attr = true;
		}

		public override bool MoveToAttribute(string name)
		{
			if (entity != null && !entity_inside_attr)
			{
				return entity.MoveToAttribute(name);
			}
			if (!source.MoveToAttribute(name))
			{
				return false;
			}
			if (entity != null && entity_inside_attr)
			{
				entity.Close();
				entity = null;
			}
			inside_attr = true;
			return true;
		}

		public override bool MoveToAttribute(string localName, string namespaceName)
		{
			if (entity != null && !entity_inside_attr)
			{
				return entity.MoveToAttribute(localName, namespaceName);
			}
			if (!source.MoveToAttribute(localName, namespaceName))
			{
				return false;
			}
			if (entity != null && entity_inside_attr)
			{
				entity.Close();
				entity = null;
			}
			inside_attr = true;
			return true;
		}

		public override bool MoveToElement()
		{
			if (entity != null && entity_inside_attr)
			{
				entity.Close();
				entity = null;
			}
			if (!Current.MoveToElement())
			{
				return false;
			}
			inside_attr = false;
			return true;
		}

		public override bool MoveToFirstAttribute()
		{
			if (entity != null && !entity_inside_attr)
			{
				return entity.MoveToFirstAttribute();
			}
			if (!source.MoveToFirstAttribute())
			{
				return false;
			}
			if (entity != null && entity_inside_attr)
			{
				entity.Close();
				entity = null;
			}
			inside_attr = true;
			return true;
		}

		public override bool MoveToNextAttribute()
		{
			if (entity != null && !entity_inside_attr)
			{
				return entity.MoveToNextAttribute();
			}
			if (!source.MoveToNextAttribute())
			{
				return false;
			}
			if (entity != null && entity_inside_attr)
			{
				entity.Close();
				entity = null;
			}
			inside_attr = true;
			return true;
		}

		public override bool Read()
		{
			if (do_resolve)
			{
				DoResolveEntity();
				do_resolve = false;
			}
			inside_attr = false;
			if (entity != null && (entity_inside_attr || entity.EOF))
			{
				entity.Close();
				entity = null;
			}
			if (entity != null)
			{
				if (entity.Read())
				{
					return true;
				}
				if (EntityHandling == EntityHandling.ExpandEntities)
				{
					entity.Close();
					entity = null;
					return Read();
				}
				return true;
			}
			if (!source.Read())
			{
				return false;
			}
			if (EntityHandling == EntityHandling.ExpandEntities && source.NodeType == XmlNodeType.EntityReference)
			{
				ResolveEntity();
				return Read();
			}
			return true;
		}

		public override bool ReadAttributeValue()
		{
			if (entity != null && entity_inside_attr)
			{
				if (!entity.EOF)
				{
					entity.Read();
					return true;
				}
				entity.Close();
				entity = null;
			}
			return Current.ReadAttributeValue();
		}

		public override string ReadString()
		{
			return base.ReadString();
		}

		public override void ResolveEntity()
		{
			DoResolveEntity();
		}

		private void DoResolveEntity()
		{
			if (entity != null)
			{
				entity.ResolveEntity();
				return;
			}
			if (source.NodeType != XmlNodeType.EntityReference)
			{
				throw new InvalidOperationException("The current node is not an Entity Reference");
			}
			if (ParserContext.Dtd == null)
			{
				throw new XmlException(this, BaseURI, string.Format("Cannot resolve entity without DTD: '{0}'", source.Name));
			}
			XmlReader xmlReader = ParserContext.Dtd.GenerateEntityContentReader(source.Name, ParserContext);
			if (xmlReader == null)
			{
				throw new XmlException(this, BaseURI, string.Format("Reference to undeclared entity '{0}'.", source.Name));
			}
			entity = new EntityResolvingXmlReader(xmlReader, inside_attr);
			entity.CopyProperties(this);
		}

		public override void Skip()
		{
			base.Skip();
		}

		public bool HasLineInfo()
		{
			IXmlLineInfo xmlLineInfo = Current as IXmlLineInfo;
			return xmlLineInfo != null && xmlLineInfo.HasLineInfo();
		}
	}
}

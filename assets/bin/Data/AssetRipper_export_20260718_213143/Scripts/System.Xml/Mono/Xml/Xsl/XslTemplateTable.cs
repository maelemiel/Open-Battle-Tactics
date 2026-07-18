using System;
using System.Collections;
using System.Xml;
using System.Xml.XPath;

namespace Mono.Xml.Xsl
{
	internal class XslTemplateTable
	{
		private Hashtable templateTables = new Hashtable();

		private Hashtable namedTemplates = new Hashtable();

		private XslStylesheet parent;

		public Hashtable TemplateTables
		{
			get
			{
				return templateTables;
			}
		}

		public XslModedTemplateTable this[XmlQualifiedName mode]
		{
			get
			{
				return templateTables[mode] as XslModedTemplateTable;
			}
		}

		public XslTemplateTable(XslStylesheet parent)
		{
			this.parent = parent;
		}

		public void Add(XslTemplate template)
		{
			if (template.Name != XmlQualifiedName.Empty)
			{
				if (namedTemplates[template.Name] != null)
				{
					throw new InvalidOperationException(string.Concat("Named template ", template.Name, " is already registered."));
				}
				namedTemplates[template.Name] = template;
			}
			if (template.Match != null)
			{
				XslModedTemplateTable xslModedTemplateTable = this[template.Mode];
				if (xslModedTemplateTable == null)
				{
					xslModedTemplateTable = new XslModedTemplateTable(template.Mode);
					Add(xslModedTemplateTable);
				}
				xslModedTemplateTable.Add(template);
			}
		}

		public void Add(XslModedTemplateTable table)
		{
			if (this[table.Mode] != null)
			{
				throw new InvalidOperationException(string.Concat("Mode ", table.Mode, " is already registered."));
			}
			templateTables.Add(table.Mode, table);
		}

		public XslTemplate FindMatch(XPathNavigator node, XmlQualifiedName mode, XslTransformProcessor p)
		{
			if (this[mode] != null)
			{
				XslTemplate xslTemplate = this[mode].FindMatch(node, p);
				if (xslTemplate != null)
				{
					return xslTemplate;
				}
			}
			for (int num = parent.Imports.Count - 1; num >= 0; num--)
			{
				XslStylesheet xslStylesheet = (XslStylesheet)parent.Imports[num];
				XslTemplate xslTemplate = xslStylesheet.Templates.FindMatch(node, mode, p);
				if (xslTemplate != null)
				{
					return xslTemplate;
				}
			}
			return null;
		}

		public XslTemplate FindTemplate(XmlQualifiedName name)
		{
			XslTemplate xslTemplate = (XslTemplate)namedTemplates[name];
			if (xslTemplate != null)
			{
				return xslTemplate;
			}
			for (int num = parent.Imports.Count - 1; num >= 0; num--)
			{
				XslStylesheet xslStylesheet = (XslStylesheet)parent.Imports[num];
				xslTemplate = xslStylesheet.Templates.FindTemplate(name);
				if (xslTemplate != null)
				{
					return xslTemplate;
				}
			}
			return null;
		}
	}
}

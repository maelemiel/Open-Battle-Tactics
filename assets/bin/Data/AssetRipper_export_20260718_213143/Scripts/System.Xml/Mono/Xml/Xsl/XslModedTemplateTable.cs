using System;
using System.Collections;
using System.Xml;
using System.Xml.XPath;
using Mono.Xml.XPath;

namespace Mono.Xml.Xsl
{
	internal class XslModedTemplateTable
	{
		private class TemplateWithPriority : IComparable
		{
			public readonly double Priority;

			public readonly XslTemplate Template;

			public readonly Pattern Pattern;

			public readonly int TemplateID;

			public TemplateWithPriority(XslTemplate t, Pattern p)
			{
				Template = t;
				Pattern = p;
				Priority = p.DefaultPriority;
				TemplateID = t.Id;
			}

			public TemplateWithPriority(XslTemplate t, double p)
			{
				Template = t;
				Pattern = t.Match;
				Priority = p;
				TemplateID = t.Id;
			}

			public int CompareTo(object o)
			{
				TemplateWithPriority templateWithPriority = (TemplateWithPriority)o;
				int num = Priority.CompareTo(templateWithPriority.Priority);
				if (num != 0)
				{
					return num;
				}
				return TemplateID.CompareTo(templateWithPriority.TemplateID);
			}

			public bool Matches(XPathNavigator n, XslTransformProcessor p)
			{
				return p.Matches(Pattern, n);
			}
		}

		private ArrayList unnamedTemplates = new ArrayList();

		private XmlQualifiedName mode;

		private bool sorted;

		public XmlQualifiedName Mode
		{
			get
			{
				return mode;
			}
		}

		public XslModedTemplateTable(XmlQualifiedName mode)
		{
			if (mode == null)
			{
				throw new InvalidOperationException();
			}
			this.mode = mode;
		}

		public void Add(XslTemplate t)
		{
			if (!double.IsNaN(t.Priority))
			{
				unnamedTemplates.Add(new TemplateWithPriority(t, t.Priority));
			}
			else
			{
				Add(t, t.Match);
			}
		}

		public void Add(XslTemplate t, Pattern p)
		{
			if (p is UnionPattern)
			{
				Add(t, ((UnionPattern)p).p0);
				Add(t, ((UnionPattern)p).p1);
			}
			else
			{
				unnamedTemplates.Add(new TemplateWithPriority(t, p));
			}
		}

		public XslTemplate FindMatch(XPathNavigator node, XslTransformProcessor p)
		{
			if (!sorted)
			{
				unnamedTemplates.Sort();
				unnamedTemplates.Reverse();
				sorted = true;
			}
			for (int i = 0; i < unnamedTemplates.Count; i++)
			{
				TemplateWithPriority templateWithPriority = (TemplateWithPriority)unnamedTemplates[i];
				if (templateWithPriority.Matches(node, p))
				{
					return templateWithPriority.Template;
				}
			}
			return null;
		}
	}
}

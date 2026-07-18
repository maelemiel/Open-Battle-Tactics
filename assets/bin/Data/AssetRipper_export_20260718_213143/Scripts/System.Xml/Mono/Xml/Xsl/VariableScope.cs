using System;
using System.Collections;
using System.Xml;
using Mono.Xml.Xsl.Operations;

namespace Mono.Xml.Xsl
{
	internal class VariableScope
	{
		private ArrayList variableNames;

		private Hashtable variables;

		private VariableScope parent;

		private int nextSlot;

		private int highTide;

		public int VariableHighTide
		{
			get
			{
				return System.Math.Max(highTide, nextSlot);
			}
		}

		public VariableScope Parent
		{
			get
			{
				return parent;
			}
		}

		public VariableScope(VariableScope parent)
		{
			this.parent = parent;
			if (parent != null)
			{
				nextSlot = parent.nextSlot;
			}
		}

		internal void giveHighTideToParent()
		{
			if (parent != null)
			{
				parent.highTide = System.Math.Max(VariableHighTide, parent.VariableHighTide);
			}
		}

		public int AddVariable(XslLocalVariable v)
		{
			if (variables == null)
			{
				variableNames = new ArrayList();
				variables = new Hashtable();
			}
			variables[v.Name] = v;
			int num = variableNames.IndexOf(v.Name);
			if (num >= 0)
			{
				return num;
			}
			variableNames.Add(v.Name);
			return nextSlot++;
		}

		public XslLocalVariable ResolveStatic(XmlQualifiedName name)
		{
			for (VariableScope variableScope = this; variableScope != null; variableScope = variableScope.Parent)
			{
				if (variableScope.variables != null)
				{
					XslLocalVariable xslLocalVariable = variableScope.variables[name] as XslLocalVariable;
					if (xslLocalVariable != null)
					{
						return xslLocalVariable;
					}
				}
			}
			return null;
		}

		public XslLocalVariable Resolve(XslTransformProcessor p, XmlQualifiedName name)
		{
			for (VariableScope variableScope = this; variableScope != null; variableScope = variableScope.Parent)
			{
				if (variableScope.variables != null)
				{
					XslLocalVariable xslLocalVariable = variableScope.variables[name] as XslLocalVariable;
					if (xslLocalVariable != null && xslLocalVariable.IsEvaluated(p))
					{
						return xslLocalVariable;
					}
				}
			}
			return null;
		}
	}
}

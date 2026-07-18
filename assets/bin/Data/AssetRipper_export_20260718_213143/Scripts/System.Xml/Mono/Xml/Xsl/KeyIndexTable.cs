using System.Collections;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace Mono.Xml.Xsl
{
	internal class KeyIndexTable
	{
		private XsltCompiledContext ctx;

		private ArrayList keys;

		private Hashtable mappedDocuments;

		public ArrayList Keys
		{
			get
			{
				return keys;
			}
		}

		public KeyIndexTable(XsltCompiledContext ctx, ArrayList keys)
		{
			this.ctx = ctx;
			this.keys = keys;
		}

		private void CollectTable(XPathNavigator doc, XsltContext ctx, Hashtable map)
		{
			for (int i = 0; i < keys.Count; i++)
			{
				CollectTable(doc, ctx, map, (XslKey)keys[i]);
			}
		}

		private void CollectTable(XPathNavigator doc, XsltContext ctx, Hashtable map, XslKey key)
		{
			XPathNavigator xPathNavigator = doc.Clone();
			xPathNavigator.MoveToRoot();
			XPathNavigator xPathNavigator2 = doc.Clone();
			bool matchesAttributes = false;
			XPathNodeType evaluatedNodeType = key.Match.EvaluatedNodeType;
			if (evaluatedNodeType == XPathNodeType.Attribute || evaluatedNodeType == XPathNodeType.All)
			{
				matchesAttributes = true;
			}
			do
			{
				if (key.Match.Matches(xPathNavigator, ctx))
				{
					xPathNavigator2.MoveTo(xPathNavigator);
					CollectIndex(xPathNavigator, xPathNavigator2, map);
				}
			}
			while (MoveNavigatorToNext(xPathNavigator, matchesAttributes));
			if (map == null)
			{
				return;
			}
			foreach (ArrayList value in map.Values)
			{
				value.Sort(XPathNavigatorComparer.Instance);
			}
		}

		private bool MoveNavigatorToNext(XPathNavigator nav, bool matchesAttributes)
		{
			if (matchesAttributes)
			{
				if (nav.NodeType != XPathNodeType.Attribute && nav.MoveToFirstAttribute())
				{
					return true;
				}
				if (nav.NodeType == XPathNodeType.Attribute)
				{
					if (nav.MoveToNextAttribute())
					{
						return true;
					}
					nav.MoveToParent();
				}
			}
			if (nav.MoveToFirstChild())
			{
				return true;
			}
			do
			{
				if (nav.MoveToNext())
				{
					return true;
				}
			}
			while (nav.MoveToParent());
			return false;
		}

		private void CollectIndex(XPathNavigator nav, XPathNavigator target, Hashtable map)
		{
			for (int i = 0; i < keys.Count; i++)
			{
				CollectIndex(nav, target, map, (XslKey)keys[i]);
			}
		}

		private void CollectIndex(XPathNavigator nav, XPathNavigator target, Hashtable map, XslKey key)
		{
			switch (key.Use.ReturnType)
			{
			case XPathResultType.NodeSet:
			{
				XPathNodeIterator xPathNodeIterator = nav.Select(key.Use);
				while (xPathNodeIterator.MoveNext())
				{
					AddIndex(xPathNodeIterator.Current.Value, target, map);
				}
				break;
			}
			case XPathResultType.Any:
			{
				object obj = nav.Evaluate(key.Use);
				XPathNodeIterator xPathNodeIterator = obj as XPathNodeIterator;
				if (xPathNodeIterator != null)
				{
					while (xPathNodeIterator.MoveNext())
					{
						AddIndex(xPathNodeIterator.Current.Value, target, map);
					}
				}
				else
				{
					AddIndex(XPathFunctions.ToString(obj), target, map);
				}
				break;
			}
			default:
			{
				string key2 = nav.EvaluateString(key.Use, null, null);
				AddIndex(key2, target, map);
				break;
			}
			}
		}

		private void AddIndex(string key, XPathNavigator target, Hashtable map)
		{
			ArrayList arrayList = map[key] as ArrayList;
			if (arrayList == null)
			{
				arrayList = (ArrayList)(map[key] = new ArrayList());
			}
			for (int i = 0; i < arrayList.Count; i++)
			{
				if (((XPathNavigator)arrayList[i]).IsSamePosition(target))
				{
					return;
				}
			}
			arrayList.Add(target.Clone());
		}

		private ArrayList GetNodesByValue(XPathNavigator nav, string value, XsltContext ctx)
		{
			if (mappedDocuments == null)
			{
				mappedDocuments = new Hashtable();
			}
			Hashtable hashtable = (Hashtable)mappedDocuments[nav.BaseURI];
			if (hashtable == null)
			{
				hashtable = new Hashtable();
				mappedDocuments.Add(nav.BaseURI, hashtable);
				CollectTable(nav, ctx, hashtable);
			}
			return hashtable[value] as ArrayList;
		}

		public bool Matches(XPathNavigator nav, string value, XsltContext ctx)
		{
			ArrayList nodesByValue = GetNodesByValue(nav, value, ctx);
			if (nodesByValue == null)
			{
				return false;
			}
			for (int i = 0; i < nodesByValue.Count; i++)
			{
				if (((XPathNavigator)nodesByValue[i]).IsSamePosition(nav))
				{
					return true;
				}
			}
			return false;
		}

		public BaseIterator Evaluate(BaseIterator iter, Expression valueExpr)
		{
			XPathNodeIterator xPathNodeIterator = iter;
			if (iter.CurrentPosition == 0)
			{
				xPathNodeIterator = iter.Clone();
				xPathNodeIterator.MoveNext();
			}
			XPathNavigator current = xPathNodeIterator.Current;
			object obj = valueExpr.Evaluate(iter);
			XPathNodeIterator xPathNodeIterator2 = obj as XPathNodeIterator;
			XsltContext nsm = iter.NamespaceManager as XsltContext;
			BaseIterator baseIterator = null;
			if (xPathNodeIterator2 != null)
			{
				while (xPathNodeIterator2.MoveNext())
				{
					ArrayList nodesByValue = GetNodesByValue(current, xPathNodeIterator2.Current.Value, nsm);
					if (nodesByValue != null)
					{
						ListIterator listIterator = new ListIterator(nodesByValue, nsm);
						baseIterator = ((baseIterator != null) ? ((BaseIterator)new UnionIterator(iter, baseIterator, listIterator)) : ((BaseIterator)listIterator));
					}
				}
			}
			else if (current != null)
			{
				ArrayList nodesByValue2 = GetNodesByValue(current, XPathFunctions.ToString(obj), nsm);
				if (nodesByValue2 != null)
				{
					baseIterator = new ListIterator(nodesByValue2, nsm);
				}
			}
			return (baseIterator == null) ? new NullIterator(iter) : baseIterator;
		}
	}
}

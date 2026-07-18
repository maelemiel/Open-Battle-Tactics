using System.Collections;
using System.Xml.XPath;

namespace Mono.Xml.Xsl
{
	internal class XslSortEvaluator
	{
		private XPathExpression select;

		private Sort[] sorterTemplates;

		private XPathSorter[] sorters;

		private XPathSorters sortRunner;

		private bool isSorterContextDependent;

		public XslSortEvaluator(XPathExpression select, Sort[] sorterTemplates)
		{
			this.select = select;
			this.sorterTemplates = sorterTemplates;
			PopulateConstantSorters();
			sortRunner = new XPathSorters();
		}

		private void PopulateConstantSorters()
		{
			sorters = new XPathSorter[sorterTemplates.Length];
			for (int i = 0; i < sorterTemplates.Length; i++)
			{
				Sort sort = sorterTemplates[i];
				if (sort.IsContextDependent)
				{
					isSorterContextDependent = true;
				}
				else
				{
					sorters[i] = sort.ToXPathSorter(null);
				}
			}
		}

		public BaseIterator SortedSelect(XslTransformProcessor p)
		{
			if (isSorterContextDependent)
			{
				for (int i = 0; i < sorters.Length; i++)
				{
					if (sorterTemplates[i].IsContextDependent)
					{
						sorters[i] = sorterTemplates[i].ToXPathSorter(p);
					}
				}
			}
			BaseIterator baseIterator = (BaseIterator)p.Select(select);
			p.PushNodeset(baseIterator);
			p.PushForEachContext();
			ArrayList arrayList = new ArrayList(baseIterator.Count);
			while (baseIterator.MoveNext())
			{
				XPathSortElement xPathSortElement = new XPathSortElement();
				xPathSortElement.Navigator = baseIterator.Current.Clone();
				xPathSortElement.Values = new object[sorters.Length];
				for (int j = 0; j < sorters.Length; j++)
				{
					xPathSortElement.Values[j] = sorters[j].Evaluate(baseIterator);
				}
				arrayList.Add(xPathSortElement);
			}
			p.PopForEachContext();
			p.PopNodeset();
			sortRunner.CopyFrom(sorters);
			return sortRunner.Sort(arrayList, baseIterator.NamespaceManager);
		}
	}
}

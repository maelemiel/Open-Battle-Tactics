using System.Collections;
using System.Xml.XPath;

namespace System.Xml
{
	internal class XmlIteratorNodeList : XmlNodeList
	{
		private class XPathNodeIteratorNodeListIterator : IEnumerator
		{
			private XPathNodeIterator iter;

			private XPathNodeIterator source;

			public object Current
			{
				get
				{
					return ((IHasXmlNode)iter.Current).GetNode();
				}
			}

			public XPathNodeIteratorNodeListIterator(XPathNodeIterator source)
			{
				this.source = source;
				Reset();
			}

			public bool MoveNext()
			{
				return iter.MoveNext();
			}

			public void Reset()
			{
				iter = source.Clone();
			}
		}

		private XPathNodeIterator source;

		private XPathNodeIterator iterator;

		private ArrayList list;

		private bool finished;

		public override int Count
		{
			get
			{
				return iterator.Count;
			}
		}

		public XmlIteratorNodeList(XPathNodeIterator iter)
		{
			source = iter;
			iterator = iter.Clone();
			list = new ArrayList();
		}

		public override IEnumerator GetEnumerator()
		{
			if (finished)
			{
				return list.GetEnumerator();
			}
			return new XPathNodeIteratorNodeListIterator(source);
		}

		public override XmlNode Item(int index)
		{
			if (index < 0)
			{
				return null;
			}
			if (index < list.Count)
			{
				return (XmlNode)list[index];
			}
			index++;
			while (iterator.CurrentPosition < index)
			{
				if (!iterator.MoveNext())
				{
					finished = true;
					return null;
				}
				list.Add(((IHasXmlNode)iterator.Current).GetNode());
			}
			return (XmlNode)list[index - 1];
		}
	}
}

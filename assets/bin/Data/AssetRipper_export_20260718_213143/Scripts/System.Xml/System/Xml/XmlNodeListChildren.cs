using System.Collections;

namespace System.Xml
{
	internal class XmlNodeListChildren : XmlNodeList
	{
		private class Enumerator : IEnumerator
		{
			private IHasXmlChildNode parent;

			private XmlLinkedNode currentChild;

			private bool passedLastNode;

			public virtual object Current
			{
				get
				{
					if (currentChild == null || parent.LastLinkedChild == null || passedLastNode)
					{
						throw new InvalidOperationException();
					}
					return currentChild;
				}
			}

			internal Enumerator(IHasXmlChildNode parent)
			{
				currentChild = null;
				this.parent = parent;
				passedLastNode = false;
			}

			public virtual bool MoveNext()
			{
				bool result = true;
				if (parent.LastLinkedChild == null)
				{
					result = false;
				}
				else if (currentChild == null)
				{
					currentChild = parent.LastLinkedChild.NextLinkedSibling;
				}
				else if (object.ReferenceEquals(currentChild, parent.LastLinkedChild))
				{
					result = false;
					passedLastNode = true;
				}
				else
				{
					currentChild = currentChild.NextLinkedSibling;
				}
				return result;
			}

			public virtual void Reset()
			{
				currentChild = null;
			}
		}

		private IHasXmlChildNode parent;

		public override int Count
		{
			get
			{
				int num = 0;
				if (parent.LastLinkedChild != null)
				{
					XmlLinkedNode nextLinkedSibling = parent.LastLinkedChild.NextLinkedSibling;
					num = 1;
					while (!object.ReferenceEquals(nextLinkedSibling, parent.LastLinkedChild))
					{
						nextLinkedSibling = nextLinkedSibling.NextLinkedSibling;
						num++;
					}
				}
				return num;
			}
		}

		public XmlNodeListChildren(IHasXmlChildNode parent)
		{
			this.parent = parent;
		}

		public override IEnumerator GetEnumerator()
		{
			return new Enumerator(parent);
		}

		public override XmlNode Item(int index)
		{
			XmlNode result = null;
			if (Count <= index)
			{
				return null;
			}
			if (index >= 0 && parent.LastLinkedChild != null)
			{
				XmlLinkedNode nextLinkedSibling = parent.LastLinkedChild.NextLinkedSibling;
				int i;
				for (i = 0; i < index; i++)
				{
					if (object.ReferenceEquals(nextLinkedSibling, parent.LastLinkedChild))
					{
						break;
					}
					nextLinkedSibling = nextLinkedSibling.NextLinkedSibling;
				}
				if (i == index)
				{
					result = nextLinkedSibling;
				}
			}
			return result;
		}
	}
}

namespace System.Xml
{
	public abstract class XmlLinkedNode : XmlNode
	{
		private XmlLinkedNode nextSibling;

		internal bool IsRooted
		{
			get
			{
				for (XmlNode xmlNode = ParentNode; xmlNode != null; xmlNode = xmlNode.ParentNode)
				{
					if (xmlNode.NodeType == XmlNodeType.Document)
					{
						return true;
					}
				}
				return false;
			}
		}

		public override XmlNode NextSibling
		{
			get
			{
				return (ParentNode != null && ParentNode.LastChild != this) ? nextSibling : null;
			}
		}

		internal XmlLinkedNode NextLinkedSibling
		{
			get
			{
				return nextSibling;
			}
			set
			{
				nextSibling = value;
			}
		}

		public override XmlNode PreviousSibling
		{
			get
			{
				if (ParentNode != null)
				{
					XmlNode firstChild = ParentNode.FirstChild;
					if (firstChild != this)
					{
						do
						{
							if (firstChild.NextSibling == this)
							{
								return firstChild;
							}
						}
						while ((firstChild = firstChild.NextSibling) != null);
					}
				}
				return null;
			}
		}

		internal XmlLinkedNode(XmlDocument doc)
			: base(doc)
		{
		}
	}
}

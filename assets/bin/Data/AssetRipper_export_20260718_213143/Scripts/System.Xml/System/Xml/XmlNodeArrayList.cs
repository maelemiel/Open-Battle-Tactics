using System.Collections;

namespace System.Xml
{
	internal class XmlNodeArrayList : XmlNodeList
	{
		private ArrayList _rgNodes;

		public override int Count
		{
			get
			{
				return _rgNodes.Count;
			}
		}

		public XmlNodeArrayList(ArrayList rgNodes)
		{
			_rgNodes = rgNodes;
		}

		public override IEnumerator GetEnumerator()
		{
			return _rgNodes.GetEnumerator();
		}

		public override XmlNode Item(int index)
		{
			if (index < 0 || _rgNodes.Count <= index)
			{
				return null;
			}
			return (XmlNode)_rgNodes[index];
		}
	}
}

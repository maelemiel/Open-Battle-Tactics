namespace System.Xml.Serialization
{
	public struct XmlDeserializationEvents
	{
		private XmlAttributeEventHandler onUnknownAttribute;

		private XmlElementEventHandler onUnknownElement;

		private XmlNodeEventHandler onUnknownNode;

		private UnreferencedObjectEventHandler onUnreferencedObject;

		public XmlAttributeEventHandler OnUnknownAttribute
		{
			get
			{
				return onUnknownAttribute;
			}
			set
			{
				onUnknownAttribute = value;
			}
		}

		public XmlElementEventHandler OnUnknownElement
		{
			get
			{
				return onUnknownElement;
			}
			set
			{
				onUnknownElement = value;
			}
		}

		public XmlNodeEventHandler OnUnknownNode
		{
			get
			{
				return onUnknownNode;
			}
			set
			{
				onUnknownNode = value;
			}
		}

		public UnreferencedObjectEventHandler OnUnreferencedObject
		{
			get
			{
				return onUnreferencedObject;
			}
			set
			{
				onUnreferencedObject = value;
			}
		}
	}
}

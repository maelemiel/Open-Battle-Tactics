namespace System.Xml
{
	public class XmlNodeChangedEventArgs : EventArgs
	{
		private XmlNode _oldParent;

		private XmlNode _newParent;

		private XmlNodeChangedAction _action;

		private XmlNode _node;

		private string _oldValue;

		private string _newValue;

		public XmlNodeChangedAction Action
		{
			get
			{
				return _action;
			}
		}

		public XmlNode Node
		{
			get
			{
				return _node;
			}
		}

		public XmlNode OldParent
		{
			get
			{
				return _oldParent;
			}
		}

		public XmlNode NewParent
		{
			get
			{
				return _newParent;
			}
		}

		public string OldValue
		{
			get
			{
				return (_oldValue == null) ? _node.Value : _oldValue;
			}
		}

		public string NewValue
		{
			get
			{
				return (_newValue == null) ? _node.Value : _newValue;
			}
		}

		public XmlNodeChangedEventArgs(XmlNode node, XmlNode oldParent, XmlNode newParent, string oldValue, string newValue, XmlNodeChangedAction action)
		{
			_node = node;
			_oldParent = oldParent;
			_newParent = newParent;
			_oldValue = oldValue;
			_newValue = newValue;
			_action = action;
		}
	}
}

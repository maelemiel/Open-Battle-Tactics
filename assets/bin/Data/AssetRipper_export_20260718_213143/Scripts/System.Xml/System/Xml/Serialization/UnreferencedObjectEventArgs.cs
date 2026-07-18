namespace System.Xml.Serialization
{
	public class UnreferencedObjectEventArgs : EventArgs
	{
		private object unreferencedObject;

		private string unreferencedId;

		public string UnreferencedId
		{
			get
			{
				return unreferencedId;
			}
		}

		public object UnreferencedObject
		{
			get
			{
				return unreferencedObject;
			}
		}

		public UnreferencedObjectEventArgs(object o, string id)
		{
			unreferencedObject = o;
			unreferencedId = id;
		}
	}
}

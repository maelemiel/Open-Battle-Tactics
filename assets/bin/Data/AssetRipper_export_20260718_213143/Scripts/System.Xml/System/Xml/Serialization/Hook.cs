namespace System.Xml.Serialization
{
	[XmlType("hook")]
	internal class Hook
	{
		[XmlAttribute("type")]
		public HookType HookType;

		[XmlElement("select")]
		public Select Select;

		[XmlElement("insertBefore")]
		public string InsertBefore;

		[XmlElement("insertAfter")]
		public string InsertAfter;

		[XmlElement("replace")]
		public string Replace;

		public string GetCode(HookAction action)
		{
			switch (action)
			{
			case HookAction.InsertBefore:
				return InsertBefore;
			case HookAction.InsertAfter:
				return InsertAfter;
			default:
				return Replace;
			}
		}
	}
}

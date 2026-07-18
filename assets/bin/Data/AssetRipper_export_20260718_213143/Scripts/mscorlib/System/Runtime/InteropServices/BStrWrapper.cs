namespace System.Runtime.InteropServices
{
	[Serializable]
	[ComVisible(true)]
	public sealed class BStrWrapper
	{
		private string _value;

		public string WrappedObject
		{
			get
			{
				return _value;
			}
		}

		public BStrWrapper(string value)
		{
			_value = value;
		}
	}
}

namespace System.ComponentModel
{
	public class PropertyChangedEventArgs : EventArgs
	{
		private string propertyName;

		public string PropertyName
		{
			get
			{
				return propertyName;
			}
		}

		public PropertyChangedEventArgs(string name)
		{
			propertyName = name;
		}
	}
}

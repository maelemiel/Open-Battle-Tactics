namespace LitJson
{
	internal class WriterContext
	{
		public int Count;

		private bool _inArray;

		private bool _inObject;

		private bool _expectingValue;

		private bool _simpleValue = true;

		public int Padding;

		public bool InArray
		{
			get
			{
				return _inArray;
			}
			set
			{
				_inArray = value;
				_simpleValue = false;
			}
		}

		public bool InObject
		{
			get
			{
				return _inObject;
			}
			set
			{
				_inObject = value;
				_simpleValue = false;
			}
		}

		public bool ExpectingValue
		{
			get
			{
				return _expectingValue;
			}
			set
			{
				_expectingValue = value;
				_simpleValue = false;
			}
		}

		public bool SimpleValue
		{
			get
			{
				return _simpleValue;
			}
		}
	}
}

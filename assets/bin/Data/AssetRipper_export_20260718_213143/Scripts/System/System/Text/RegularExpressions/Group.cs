namespace System.Text.RegularExpressions
{
	[Serializable]
	public class Group : Capture
	{
		internal static Group Fail = new Group();

		private bool success;

		private CaptureCollection captures;

		public CaptureCollection Captures
		{
			get
			{
				return captures;
			}
		}

		public bool Success
		{
			get
			{
				return success;
			}
		}

		internal Group(string text, int index, int length, int n_caps)
			: base(text, index, length)
		{
			success = true;
			captures = new CaptureCollection(n_caps);
			captures.SetValue(this, n_caps - 1);
		}

		internal Group(string text, int index, int length)
			: base(text, index, length)
		{
			success = true;
		}

		internal Group()
			: base(string.Empty)
		{
			success = false;
			captures = new CaptureCollection(0);
		}

		[System.MonoTODO("not thread-safe")]
		public static Group Synchronized(Group inner)
		{
			if (inner == null)
			{
				throw new ArgumentNullException("inner");
			}
			return inner;
		}
	}
}

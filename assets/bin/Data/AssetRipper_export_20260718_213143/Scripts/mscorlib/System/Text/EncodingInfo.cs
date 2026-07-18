namespace System.Text
{
	[Serializable]
	public sealed class EncodingInfo
	{
		private readonly int codepage;

		private Encoding encoding;

		public int CodePage
		{
			get
			{
				return codepage;
			}
		}

		[MonoTODO]
		public string DisplayName
		{
			get
			{
				return Name;
			}
		}

		public string Name
		{
			get
			{
				if (encoding == null)
				{
					encoding = GetEncoding();
				}
				return encoding.WebName;
			}
		}

		internal EncodingInfo(int cp)
		{
			codepage = cp;
		}

		public override bool Equals(object value)
		{
			EncodingInfo encodingInfo = value as EncodingInfo;
			return encodingInfo != null && encodingInfo.codepage == codepage;
		}

		public override int GetHashCode()
		{
			return codepage;
		}

		public Encoding GetEncoding()
		{
			return Encoding.GetEncoding(codepage);
		}
	}
}

using System;

namespace ICSharpCode.SharpZipLib.Zip
{
	public class RawTaggedData : ITaggedData
	{
		protected short tag_;

		private byte[] data_;

		public short TagID
		{
			get
			{
				return tag_;
			}
			set
			{
				tag_ = value;
			}
		}

		public byte[] Data
		{
			get
			{
				return data_;
			}
			set
			{
				data_ = value;
			}
		}

		public RawTaggedData(short tag)
		{
			tag_ = tag;
		}

		public void SetData(byte[] data, int offset, int count)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			data_ = new byte[count];
			Array.Copy(data, offset, data_, 0, count);
		}

		public byte[] GetData()
		{
			return data_;
		}
	}
}

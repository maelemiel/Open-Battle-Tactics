using System;
using System.IO;

namespace ICSharpCode.SharpZipLib.Zip
{
	public class ExtendedUnixData : ITaggedData
	{
		[Flags]
		public enum Flags : byte
		{
			ModificationTime = 1,
			AccessTime = 2,
			CreateTime = 4
		}

		private Flags flags_;

		private DateTime modificationTime_ = new DateTime(1970, 1, 1);

		private DateTime lastAccessTime_ = new DateTime(1970, 1, 1);

		private DateTime createTime_ = new DateTime(1970, 1, 1);

		public short TagID
		{
			get
			{
				return 21589;
			}
		}

		public DateTime ModificationTime
		{
			get
			{
				return modificationTime_;
			}
			set
			{
				if (!IsValidValue(value))
				{
					throw new ArgumentOutOfRangeException("value");
				}
				flags_ |= Flags.ModificationTime;
				modificationTime_ = value;
			}
		}

		public DateTime AccessTime
		{
			get
			{
				return lastAccessTime_;
			}
			set
			{
				if (!IsValidValue(value))
				{
					throw new ArgumentOutOfRangeException("value");
				}
				flags_ |= Flags.AccessTime;
				lastAccessTime_ = value;
			}
		}

		public DateTime CreateTime
		{
			get
			{
				return createTime_;
			}
			set
			{
				if (!IsValidValue(value))
				{
					throw new ArgumentOutOfRangeException("value");
				}
				flags_ |= Flags.CreateTime;
				createTime_ = value;
			}
		}

		private Flags Include
		{
			get
			{
				return flags_;
			}
			set
			{
				flags_ = value;
			}
		}

		public void SetData(byte[] data, int index, int count)
		{
			using (MemoryStream stream = new MemoryStream(data, index, count, false))
			{
				using (ZipHelperStream zipHelperStream = new ZipHelperStream(stream))
				{
					flags_ = (Flags)zipHelperStream.ReadByte();
					if ((flags_ & Flags.ModificationTime) != 0 && count >= 5)
					{
						int seconds = zipHelperStream.ReadLEInt();
						modificationTime_ = (new DateTime(1970, 1, 1, 0, 0, 0).ToUniversalTime() + new TimeSpan(0, 0, 0, seconds, 0)).ToLocalTime();
					}
					if ((flags_ & Flags.AccessTime) != 0)
					{
						int seconds2 = zipHelperStream.ReadLEInt();
						lastAccessTime_ = (new DateTime(1970, 1, 1, 0, 0, 0).ToUniversalTime() + new TimeSpan(0, 0, 0, seconds2, 0)).ToLocalTime();
					}
					if ((flags_ & Flags.CreateTime) != 0)
					{
						int seconds3 = zipHelperStream.ReadLEInt();
						createTime_ = (new DateTime(1970, 1, 1, 0, 0, 0).ToUniversalTime() + new TimeSpan(0, 0, 0, seconds3, 0)).ToLocalTime();
					}
				}
			}
		}

		public byte[] GetData()
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (ZipHelperStream zipHelperStream = new ZipHelperStream(memoryStream))
				{
					zipHelperStream.IsStreamOwner = false;
					zipHelperStream.WriteByte((byte)flags_);
					if ((flags_ & Flags.ModificationTime) != 0)
					{
						int value = (int)(modificationTime_.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0).ToUniversalTime()).TotalSeconds;
						zipHelperStream.WriteLEInt(value);
					}
					if ((flags_ & Flags.AccessTime) != 0)
					{
						int value2 = (int)(lastAccessTime_.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0).ToUniversalTime()).TotalSeconds;
						zipHelperStream.WriteLEInt(value2);
					}
					if ((flags_ & Flags.CreateTime) != 0)
					{
						int value3 = (int)(createTime_.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0).ToUniversalTime()).TotalSeconds;
						zipHelperStream.WriteLEInt(value3);
					}
					return memoryStream.ToArray();
				}
			}
		}

		public static bool IsValidValue(DateTime value)
		{
			return value >= new DateTime(1901, 12, 13, 20, 45, 52) || value <= new DateTime(2038, 1, 19, 3, 14, 7);
		}
	}
}

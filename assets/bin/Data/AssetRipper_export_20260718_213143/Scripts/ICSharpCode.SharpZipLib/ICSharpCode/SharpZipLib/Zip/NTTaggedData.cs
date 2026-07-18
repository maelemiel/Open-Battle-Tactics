using System;
using System.IO;

namespace ICSharpCode.SharpZipLib.Zip
{
	public class NTTaggedData : ITaggedData
	{
		private DateTime lastAccessTime_ = DateTime.FromFileTime(0L);

		private DateTime lastModificationTime_ = DateTime.FromFileTime(0L);

		private DateTime createTime_ = DateTime.FromFileTime(0L);

		public short TagID
		{
			get
			{
				return 10;
			}
		}

		public DateTime LastModificationTime
		{
			get
			{
				return lastModificationTime_;
			}
			set
			{
				if (!IsValidValue(value))
				{
					throw new ArgumentOutOfRangeException("value");
				}
				lastModificationTime_ = value;
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
				createTime_ = value;
			}
		}

		public DateTime LastAccessTime
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
				lastAccessTime_ = value;
			}
		}

		public void SetData(byte[] data, int index, int count)
		{
			using (MemoryStream stream = new MemoryStream(data, index, count, false))
			{
				using (ZipHelperStream zipHelperStream = new ZipHelperStream(stream))
				{
					zipHelperStream.ReadLEInt();
					while (zipHelperStream.Position < zipHelperStream.Length)
					{
						int num = zipHelperStream.ReadLEShort();
						int num2 = zipHelperStream.ReadLEShort();
						if (num == 1)
						{
							if (num2 >= 24)
							{
								long fileTime = zipHelperStream.ReadLELong();
								lastModificationTime_ = DateTime.FromFileTime(fileTime);
								long fileTime2 = zipHelperStream.ReadLELong();
								lastAccessTime_ = DateTime.FromFileTime(fileTime2);
								long fileTime3 = zipHelperStream.ReadLELong();
								createTime_ = DateTime.FromFileTime(fileTime3);
							}
							break;
						}
						zipHelperStream.Seek(num2, SeekOrigin.Current);
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
					zipHelperStream.WriteLEInt(0);
					zipHelperStream.WriteLEShort(1);
					zipHelperStream.WriteLEShort(24);
					zipHelperStream.WriteLELong(lastModificationTime_.ToFileTime());
					zipHelperStream.WriteLELong(lastAccessTime_.ToFileTime());
					zipHelperStream.WriteLELong(createTime_.ToFileTime());
					return memoryStream.ToArray();
				}
			}
		}

		public static bool IsValidValue(DateTime value)
		{
			bool result = true;
			try
			{
				value.ToFileTimeUtc();
			}
			catch
			{
				result = false;
			}
			return result;
		}
	}
}

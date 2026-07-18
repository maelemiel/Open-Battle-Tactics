using System;
using System.IO;

namespace ICSharpCode.SharpZipLib.Zip
{
	public sealed class ZipExtraData : IDisposable
	{
		private int index_;

		private int readValueStart_;

		private int readValueLength_;

		private MemoryStream newEntry_;

		private byte[] data_;

		public int Length
		{
			get
			{
				return data_.Length;
			}
		}

		public int ValueLength
		{
			get
			{
				return readValueLength_;
			}
		}

		public int CurrentReadIndex
		{
			get
			{
				return index_;
			}
		}

		public int UnreadCount
		{
			get
			{
				if (readValueStart_ > data_.Length || readValueStart_ < 4)
				{
					throw new ZipException("Find must be called before calling a Read method");
				}
				return readValueStart_ + readValueLength_ - index_;
			}
		}

		public ZipExtraData()
		{
			Clear();
		}

		public ZipExtraData(byte[] data)
		{
			if (data == null)
			{
				data_ = new byte[0];
			}
			else
			{
				data_ = data;
			}
		}

		public byte[] GetEntryData()
		{
			if (Length > 65535)
			{
				throw new ZipException("Data exceeds maximum length");
			}
			return (byte[])data_.Clone();
		}

		public void Clear()
		{
			if (data_ == null || data_.Length != 0)
			{
				data_ = new byte[0];
			}
		}

		public Stream GetStreamForTag(int tag)
		{
			Stream result = null;
			if (Find(tag))
			{
				result = new MemoryStream(data_, index_, readValueLength_, false);
			}
			return result;
		}

		private ITaggedData GetData(short tag)
		{
			ITaggedData result = null;
			if (Find(tag))
			{
				result = Create(tag, data_, readValueStart_, readValueLength_);
			}
			return result;
		}

		private static ITaggedData Create(short tag, byte[] data, int offset, int count)
		{
			ITaggedData taggedData = null;
			switch (tag)
			{
			case 10:
				taggedData = new NTTaggedData();
				break;
			case 21589:
				taggedData = new ExtendedUnixData();
				break;
			default:
				taggedData = new RawTaggedData(tag);
				break;
			}
			taggedData.SetData(data, offset, count);
			return taggedData;
		}

		public bool Find(int headerID)
		{
			readValueStart_ = data_.Length;
			readValueLength_ = 0;
			index_ = 0;
			int num = readValueStart_;
			int num2 = headerID - 1;
			while (num2 != headerID && index_ < data_.Length - 3)
			{
				num2 = ReadShortInternal();
				num = ReadShortInternal();
				if (num2 != headerID)
				{
					index_ += num;
				}
			}
			bool flag = num2 == headerID && index_ + num <= data_.Length;
			if (flag)
			{
				readValueStart_ = index_;
				readValueLength_ = num;
			}
			return flag;
		}

		public void AddEntry(ITaggedData taggedData)
		{
			if (taggedData == null)
			{
				throw new ArgumentNullException("taggedData");
			}
			AddEntry(taggedData.TagID, taggedData.GetData());
		}

		public void AddEntry(int headerID, byte[] fieldData)
		{
			if (headerID > 65535 || headerID < 0)
			{
				throw new ArgumentOutOfRangeException("headerID");
			}
			int num = ((fieldData != null) ? fieldData.Length : 0);
			if (num > 65535)
			{
				throw new ArgumentOutOfRangeException("fieldData", "exceeds maximum length");
			}
			int num2 = data_.Length + num + 4;
			if (Find(headerID))
			{
				num2 -= ValueLength + 4;
			}
			if (num2 > 65535)
			{
				throw new ZipException("Data exceeds maximum length");
			}
			Delete(headerID);
			byte[] array = new byte[num2];
			data_.CopyTo(array, 0);
			int index = data_.Length;
			data_ = array;
			SetShort(ref index, headerID);
			SetShort(ref index, num);
			if (fieldData != null)
			{
				fieldData.CopyTo(array, index);
			}
		}

		public void StartNewEntry()
		{
			newEntry_ = new MemoryStream();
		}

		public void AddNewEntry(int headerID)
		{
			byte[] fieldData = newEntry_.ToArray();
			newEntry_ = null;
			AddEntry(headerID, fieldData);
		}

		public void AddData(byte data)
		{
			newEntry_.WriteByte(data);
		}

		public void AddData(byte[] data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			newEntry_.Write(data, 0, data.Length);
		}

		public void AddLeShort(int toAdd)
		{
			newEntry_.WriteByte((byte)toAdd);
			newEntry_.WriteByte((byte)(toAdd >> 8));
		}

		public void AddLeInt(int toAdd)
		{
			AddLeShort((short)toAdd);
			AddLeShort((short)(toAdd >> 16));
		}

		public void AddLeLong(long toAdd)
		{
			AddLeInt((int)(toAdd & 0xFFFFFFFFu));
			AddLeInt((int)(toAdd >> 32));
		}

		public bool Delete(int headerID)
		{
			bool result = false;
			if (Find(headerID))
			{
				result = true;
				int num = readValueStart_ - 4;
				byte[] destinationArray = new byte[data_.Length - (ValueLength + 4)];
				Array.Copy(data_, 0, destinationArray, 0, num);
				int num2 = num + ValueLength + 4;
				Array.Copy(data_, num2, destinationArray, num, data_.Length - num2);
				data_ = destinationArray;
			}
			return result;
		}

		public long ReadLong()
		{
			ReadCheck(8);
			return (ReadInt() & 0xFFFFFFFFu) | ((long)ReadInt() << 32);
		}

		public int ReadInt()
		{
			ReadCheck(4);
			int result = data_[index_] + (data_[index_ + 1] << 8) + (data_[index_ + 2] << 16) + (data_[index_ + 3] << 24);
			index_ += 4;
			return result;
		}

		public int ReadShort()
		{
			ReadCheck(2);
			int result = data_[index_] + (data_[index_ + 1] << 8);
			index_ += 2;
			return result;
		}

		public int ReadByte()
		{
			int result = -1;
			if (index_ < data_.Length && readValueStart_ + readValueLength_ > index_)
			{
				result = data_[index_];
				index_++;
			}
			return result;
		}

		public void Skip(int amount)
		{
			ReadCheck(amount);
			index_ += amount;
		}

		private void ReadCheck(int length)
		{
			if (readValueStart_ > data_.Length || readValueStart_ < 4)
			{
				throw new ZipException("Find must be called before calling a Read method");
			}
			if (index_ > readValueStart_ + readValueLength_ - length)
			{
				throw new ZipException("End of extra data");
			}
		}

		private int ReadShortInternal()
		{
			if (index_ > data_.Length - 2)
			{
				throw new ZipException("End of extra data");
			}
			int result = data_[index_] + (data_[index_ + 1] << 8);
			index_ += 2;
			return result;
		}

		private void SetShort(ref int index, int source)
		{
			data_[index] = (byte)source;
			data_[index + 1] = (byte)(source >> 8);
			index += 2;
		}

		public void Dispose()
		{
			if (newEntry_ != null)
			{
				newEntry_.Close();
			}
		}
	}
}

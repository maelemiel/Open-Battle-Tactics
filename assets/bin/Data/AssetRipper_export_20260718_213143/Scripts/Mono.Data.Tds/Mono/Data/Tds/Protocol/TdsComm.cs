using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Mono.Data.Tds.Protocol
{
	internal sealed class TdsComm
	{
		private NetworkStream stream;

		private int packetSize;

		private TdsPacketType packetType;

		private bool connReset;

		private Encoding encoder;

		private string dataSource;

		private int commandTimeout;

		private byte[] outBuffer;

		private int outBufferLength;

		private int nextOutBufferIndex;

		private bool lsb;

		private byte[] inBuffer;

		private int inBufferLength;

		private int inBufferIndex;

		private static int headerLength = 8;

		private byte[] tmpBuf = new byte[8];

		private byte[] resBuffer = new byte[256];

		private int packetsSent;

		private int packetsReceived;

		private Socket socket;

		private TdsVersion tdsVersion;

		public int CommandTimeout
		{
			get
			{
				return commandTimeout;
			}
			set
			{
				commandTimeout = value;
			}
		}

		internal Encoding Encoder
		{
			get
			{
				return encoder;
			}
			set
			{
				encoder = value;
			}
		}

		public int PacketSize
		{
			get
			{
				return packetSize;
			}
			set
			{
				packetSize = value;
			}
		}

		public bool TdsByteOrder
		{
			get
			{
				return !lsb;
			}
			set
			{
				lsb = !value;
			}
		}

		public bool ResetConnection
		{
			get
			{
				return connReset;
			}
			set
			{
				connReset = value;
			}
		}

		public TdsComm(string dataSource, int port, int packetSize, int timeout, TdsVersion tdsVersion)
		{
			this.packetSize = packetSize;
			this.tdsVersion = tdsVersion;
			this.dataSource = dataSource;
			outBuffer = new byte[packetSize];
			inBuffer = new byte[packetSize];
			outBufferLength = packetSize;
			inBufferLength = packetSize;
			lsb = true;
			bool flag = false;
			IPEndPoint end_point;
			try
			{
				IPAddress address;
				if (IPAddress.TryParse(this.dataSource, out address))
				{
					end_point = new IPEndPoint(address, port);
				}
				else
				{
					IPHostEntry hostEntry = Dns.GetHostEntry(this.dataSource);
					end_point = new IPEndPoint(hostEntry.AddressList[0], port);
				}
			}
			catch (SocketException innerException)
			{
				throw new TdsInternalException("Server does not exist or connection refused.", innerException);
			}
			try
			{
				this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				IAsyncResult asyncResult = this.socket.BeginConnect(end_point, null, null);
				int num = timeout * 1000;
				if (timeout > 0 && !asyncResult.IsCompleted && !asyncResult.AsyncWaitHandle.WaitOne(num, false))
				{
					throw Tds.CreateTimeoutException(dataSource, "Open()");
				}
				this.socket.EndConnect(asyncResult);
				try
				{
					this.socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, 1);
				}
				catch (SocketException)
				{
				}
				try
				{
					this.socket.NoDelay = true;
					this.socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, num);
					this.socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, num);
				}
				catch
				{
				}
				stream = new NetworkStream(this.socket, true);
			}
			catch (SocketException innerException2)
			{
				flag = true;
				throw new TdsInternalException("Server does not exist or connection refused.", innerException2);
			}
			catch (Exception)
			{
				flag = true;
				throw;
			}
			finally
			{
				if (flag && this.socket != null)
				{
					try
					{
						Socket socket = this.socket;
						this.socket = null;
						socket.Close();
					}
					catch
					{
					}
				}
			}
			if (!this.socket.Connected)
			{
				throw new TdsInternalException("Server does not exist or connection refused.", null);
			}
			packetsSent = 1;
		}

		public byte[] Swap(byte[] toswap)
		{
			byte[] array = new byte[toswap.Length];
			for (int i = 0; i < toswap.Length; i++)
			{
				array[toswap.Length - i - 1] = toswap[i];
			}
			return array;
		}

		public void SendIfFull()
		{
			if (nextOutBufferIndex == outBufferLength)
			{
				SendPhysicalPacket(false);
				nextOutBufferIndex = headerLength;
			}
		}

		public void SendIfFull(int reserve)
		{
			if (nextOutBufferIndex + reserve > outBufferLength)
			{
				SendPhysicalPacket(false);
				nextOutBufferIndex = headerLength;
			}
		}

		public void Append(object o)
		{
			if (o == null || o == DBNull.Value)
			{
				Append((byte)0);
				return;
			}
			switch (Type.GetTypeCode(o.GetType()))
			{
			case TypeCode.Byte:
				Append((byte)o);
				break;
			case TypeCode.Boolean:
				if ((bool)o)
				{
					Append((byte)1);
				}
				else
				{
					Append((byte)0);
				}
				break;
			case TypeCode.Object:
				if (o is byte[])
				{
					Append((byte[])o);
				}
				break;
			case TypeCode.Int16:
				Append((short)o);
				break;
			case TypeCode.Int32:
				Append((int)o);
				break;
			case TypeCode.String:
				Append((string)o);
				break;
			case TypeCode.Double:
				Append((double)o);
				break;
			case TypeCode.Single:
				Append((float)o);
				break;
			case TypeCode.Int64:
				Append((long)o);
				break;
			case TypeCode.Decimal:
				Append((decimal)o, 17);
				break;
			case TypeCode.DateTime:
				Append((DateTime)o, 8);
				break;
			default:
				throw new InvalidOperationException(string.Format("Object Type :{0} , not being appended", o.GetType()));
			}
		}

		public void Append(byte b)
		{
			SendIfFull();
			Store(nextOutBufferIndex, b);
			nextOutBufferIndex++;
		}

		public void Append(DateTime t, int bytes)
		{
			DateTime dateTime = new DateTime(1900, 1, 1);
			TimeSpan timeSpan = t - dateTime;
			int days = timeSpan.Days;
			int num = 0;
			SendIfFull(bytes);
			switch (bytes)
			{
			case 8:
			{
				long num2 = (long)(timeSpan.Hours * 3600 + timeSpan.Minutes * 60 + timeSpan.Seconds) * 1000L + timeSpan.Milliseconds;
				num = (int)(num2 * 300 / 1000);
				AppendInternal(days);
				AppendInternal(num);
				break;
			}
			case 4:
				num = timeSpan.Hours * 60 + timeSpan.Minutes;
				AppendInternal((short)days);
				AppendInternal((short)num);
				break;
			default:
				throw new Exception("Invalid No of bytes");
			}
		}

		public void Append(byte[] b)
		{
			Append(b, b.Length, 0);
		}

		public void Append(byte[] b, int len, byte pad)
		{
			int num = System.Math.Min(b.Length, len);
			int num2 = len - num;
			int num3 = 0;
			while (num > 0)
			{
				SendIfFull();
				int val = outBufferLength - nextOutBufferIndex;
				int num4 = System.Math.Min(val, num);
				Buffer.BlockCopy(b, num3, outBuffer, nextOutBufferIndex, num4);
				nextOutBufferIndex += num4;
				num -= num4;
				num3 += num4;
			}
			while (num2 > 0)
			{
				SendIfFull();
				int val2 = outBufferLength - nextOutBufferIndex;
				int num5 = System.Math.Min(val2, num2);
				for (int i = 0; i < num5; i++)
				{
					outBuffer[nextOutBufferIndex++] = pad;
				}
				num2 -= num5;
			}
		}

		private void AppendInternal(short s)
		{
			if (!lsb)
			{
				outBuffer[nextOutBufferIndex++] = (byte)((byte)(s >> 8) & 0xFF);
				outBuffer[nextOutBufferIndex++] = (byte)(s & 0xFF);
			}
			else
			{
				outBuffer[nextOutBufferIndex++] = (byte)(s & 0xFF);
				outBuffer[nextOutBufferIndex++] = (byte)((byte)(s >> 8) & 0xFF);
			}
		}

		public void Append(short s)
		{
			SendIfFull(2);
			AppendInternal(s);
		}

		public void Append(ushort s)
		{
			SendIfFull(2);
			AppendInternal((short)s);
		}

		private void AppendInternal(int i)
		{
			if (!lsb)
			{
				AppendInternal((short)((short)(i >> 16) & 0xFFFF));
				AppendInternal((short)(i & 0xFFFF));
			}
			else
			{
				AppendInternal((short)(i & 0xFFFF));
				AppendInternal((short)((short)(i >> 16) & 0xFFFF));
			}
		}

		public void Append(int i)
		{
			SendIfFull(4);
			AppendInternal(i);
		}

		public void Append(string s)
		{
			if (tdsVersion < TdsVersion.tds70)
			{
				Append(encoder.GetBytes(s));
				return;
			}
			int num = s.Length * 2;
			int num2 = num / outBufferLength;
			int num3 = 0;
			int num4 = 0;
			if (num % outBufferLength > 0)
			{
				num2++;
			}
			num4 = outBufferLength - nextOutBufferIndex;
			for (int i = 0; i < num2; i++)
			{
				int num5 = System.Math.Min(num4, num);
				int num6 = 0;
				while (num6 < num5)
				{
					AppendInternal((short)s[num3]);
					num6 += 2;
					num3++;
				}
				num -= System.Math.Min(num4, num);
				SendIfFull(num + 2);
			}
		}

		public byte[] Append(string s, int len, byte pad)
		{
			if (s == null)
			{
				return new byte[0];
			}
			byte[] bytes = encoder.GetBytes(s);
			Append(bytes, len, pad);
			return bytes;
		}

		public void Append(double value)
		{
			if (!lsb)
			{
				Append(Swap(BitConverter.GetBytes(value)), 8, 0);
			}
			else
			{
				Append(BitConverter.GetBytes(value), 8, 0);
			}
		}

		public void Append(float value)
		{
			if (!lsb)
			{
				Append(Swap(BitConverter.GetBytes(value)), 4, 0);
			}
			else
			{
				Append(BitConverter.GetBytes(value), 4, 0);
			}
		}

		public void Append(long l)
		{
			SendIfFull(8);
			if (!lsb)
			{
				AppendInternal((int)((int)(l >> 32) & 0xFFFFFFFFu));
				AppendInternal((int)(l & 0xFFFFFFFFu));
			}
			else
			{
				AppendInternal((int)(l & 0xFFFFFFFFu));
				AppendInternal((int)((int)(l >> 32) & 0xFFFFFFFFu));
			}
		}

		public void Append(decimal d, int bytes)
		{
			int[] bits = decimal.GetBits(d);
			byte b = (byte)((d > 0m) ? 1 : 0);
			SendIfFull(bytes);
			Append(b);
			AppendInternal(bits[0]);
			AppendInternal(bits[1]);
			AppendInternal(bits[2]);
			AppendInternal(0);
		}

		public void Close()
		{
			if (stream != null)
			{
				connReset = false;
				socket = null;
				try
				{
					stream.Close();
				}
				catch
				{
				}
				stream = null;
			}
		}

		public bool IsConnected()
		{
			return socket != null && socket.Connected && (!socket.Poll(0, SelectMode.SelectRead) || socket.Available != 0);
		}

		public byte GetByte()
		{
			if (inBufferIndex >= inBufferLength)
			{
				GetPhysicalPacket();
			}
			return inBuffer[inBufferIndex++];
		}

		public byte[] GetBytes(int len, bool exclusiveBuffer)
		{
			byte[] array = null;
			if (exclusiveBuffer || len > 16384)
			{
				array = new byte[len];
			}
			else
			{
				if (resBuffer.Length < len)
				{
					resBuffer = new byte[len];
				}
				array = resBuffer;
			}
			int num = 0;
			while (num < len)
			{
				if (inBufferIndex >= inBufferLength)
				{
					GetPhysicalPacket();
				}
				int num2 = inBufferLength - inBufferIndex;
				num2 = ((num2 <= len - num) ? num2 : (len - num));
				Buffer.BlockCopy(inBuffer, inBufferIndex, array, num, num2);
				num += num2;
				inBufferIndex += num2;
			}
			return array;
		}

		public string GetString(int len, Encoding enc)
		{
			if (tdsVersion >= TdsVersion.tds70)
			{
				return GetString(len, true, null);
			}
			return GetString(len, false, null);
		}

		public string GetString(int len)
		{
			if (tdsVersion >= TdsVersion.tds70)
			{
				return GetString(len, true);
			}
			return GetString(len, false);
		}

		public string GetString(int len, bool wide, Encoding enc)
		{
			if (wide)
			{
				char[] array = new char[len];
				for (int i = 0; i < len; i++)
				{
					int num = GetByte() & 0xFF;
					int num2 = GetByte() & 0xFF;
					array[i] = (char)(num | (num2 << 8));
				}
				return new string(array);
			}
			byte[] array2 = new byte[len];
			Array.Copy(GetBytes(len, false), array2, len);
			if (enc != null)
			{
				return enc.GetString(array2);
			}
			return encoder.GetString(array2);
		}

		public string GetString(int len, bool wide)
		{
			return GetString(len, wide, null);
		}

		public int GetNetShort()
		{
			return Ntohs(new byte[2]
			{
				GetByte(),
				GetByte()
			}, 0);
		}

		public short GetTdsShort()
		{
			byte[] array = new byte[2];
			for (int i = 0; i < 2; i++)
			{
				array[i] = GetByte();
			}
			if (!BitConverter.IsLittleEndian)
			{
				return BitConverter.ToInt16(Swap(array), 0);
			}
			return BitConverter.ToInt16(array, 0);
		}

		public int GetTdsInt()
		{
			byte[] array = new byte[4];
			for (int i = 0; i < 4; i++)
			{
				array[i] = GetByte();
			}
			if (!BitConverter.IsLittleEndian)
			{
				return BitConverter.ToInt32(Swap(array), 0);
			}
			return BitConverter.ToInt32(array, 0);
		}

		public long GetTdsInt64()
		{
			byte[] array = new byte[8];
			for (int i = 0; i < 8; i++)
			{
				array[i] = GetByte();
			}
			if (!BitConverter.IsLittleEndian)
			{
				return BitConverter.ToInt64(Swap(array), 0);
			}
			return BitConverter.ToInt64(array, 0);
		}

		private void GetPhysicalPacket()
		{
			int physicalPacketHeader = GetPhysicalPacketHeader();
			GetPhysicalPacketData(physicalPacketHeader);
		}

		private int Read(byte[] buffer, int offset, int count)
		{
			try
			{
				return stream.Read(buffer, offset, count);
			}
			catch
			{
				socket = null;
				stream.Close();
				throw;
			}
		}

		private int GetPhysicalPacketHeader()
		{
			int num;
			for (int i = 0; i < 8; i += num)
			{
				num = Read(tmpBuf, i, 8 - i);
				if (num <= 0)
				{
					socket = null;
					stream.Close();
					throw new IOException((num != 0) ? "Connection error" : "Connection lost");
				}
			}
			TdsPacketType tdsPacketType = (TdsPacketType)tmpBuf[0];
			if (tdsPacketType != TdsPacketType.Logon && tdsPacketType != TdsPacketType.Query && tdsPacketType != TdsPacketType.Reply)
			{
				throw new Exception(string.Format("Unknown packet type {0}", tmpBuf[0]));
			}
			int num2 = Ntohs(tmpBuf, 2) - 8;
			if (num2 >= inBuffer.Length)
			{
				inBuffer = new byte[num2];
			}
			if (num2 < 0)
			{
				throw new Exception(string.Format("Confused by a length of {0}", num2));
			}
			return num2;
		}

		private void GetPhysicalPacketData(int length)
		{
			int num;
			for (int i = 0; i < length; i += num)
			{
				num = Read(inBuffer, i, length - i);
				if (num <= 0)
				{
					socket = null;
					stream.Close();
					throw new IOException((num != 0) ? "Connection error" : "Connection lost");
				}
			}
			packetsReceived++;
			inBufferLength = length;
			inBufferIndex = 0;
		}

		private static int Ntohs(byte[] buf, int offset)
		{
			int num = buf[offset + 1] & 0xFF;
			int num2 = (buf[offset] & 0xFF) << 8;
			return num2 | num;
		}

		public byte Peek()
		{
			if (inBufferIndex >= inBufferLength)
			{
				GetPhysicalPacket();
			}
			return inBuffer[inBufferIndex];
		}

		public bool Poll(int seconds, SelectMode selectMode)
		{
			return Poll(socket, seconds, selectMode);
		}

		private bool Poll(Socket s, int seconds, SelectMode selectMode)
		{
			long num = seconds * 1000000;
			bool flag = false;
			while (num > int.MaxValue)
			{
				if (s.Poll(int.MaxValue, selectMode))
				{
					return true;
				}
				num -= int.MaxValue;
			}
			return s.Poll((int)num, selectMode);
		}

		internal void ResizeOutBuf(int newSize)
		{
			if (newSize != outBufferLength)
			{
				byte[] dst = new byte[newSize];
				Buffer.BlockCopy(outBuffer, 0, dst, 0, newSize);
				outBufferLength = newSize;
				outBuffer = dst;
			}
		}

		public void SendPacket()
		{
			if (packetType != TdsPacketType.Query && packetType != TdsPacketType.Proc)
			{
				connReset = false;
			}
			SendPhysicalPacket(true);
			nextOutBufferIndex = 0;
			packetType = TdsPacketType.None;
			connReset = false;
			packetsSent = 1;
		}

		private void SendPhysicalPacket(bool isLastSegment)
		{
			if (nextOutBufferIndex > headerLength || packetType == TdsPacketType.Cancel)
			{
				byte value = (byte)((isLastSegment ? 1 : 0) | (connReset ? 8 : 0));
				Store(0, (byte)packetType);
				Store(1, value);
				Store(2, (short)nextOutBufferIndex);
				Store(4, 0);
				Store(5, 0);
				if (tdsVersion >= TdsVersion.tds70)
				{
					Store(6, (byte)packetsSent);
				}
				else
				{
					Store(6, 0);
				}
				Store(7, 0);
				stream.Write(outBuffer, 0, nextOutBufferIndex);
				stream.Flush();
				packetsSent++;
			}
		}

		public void Skip(long i)
		{
			while (i > 0)
			{
				GetByte();
				i--;
			}
		}

		public void StartPacket(TdsPacketType type)
		{
			if (type != TdsPacketType.Cancel && inBufferIndex != inBufferLength)
			{
				inBufferIndex = inBufferLength;
			}
			packetType = type;
			nextOutBufferIndex = headerLength;
		}

		private void Store(int index, byte value)
		{
			outBuffer[index] = value;
		}

		private void Store(int index, short value)
		{
			outBuffer[index] = (byte)((byte)(value >> 8) & 0xFF);
			outBuffer[index + 1] = (byte)((byte)value & 0xFF);
		}

		public IAsyncResult BeginReadPacket(AsyncCallback callback, object stateObject)
		{
			TdsAsyncResult tdsAsyncResult = new TdsAsyncResult(callback, stateObject);
			stream.BeginRead(tmpBuf, 0, 8, OnReadPacketCallback, tdsAsyncResult);
			return tdsAsyncResult;
		}

		public int EndReadPacket(IAsyncResult ar)
		{
			if (!ar.IsCompleted)
			{
				ar.AsyncWaitHandle.WaitOne();
			}
			return (int)((TdsAsyncResult)ar).ReturnValue;
		}

		public void OnReadPacketCallback(IAsyncResult socketAsyncResult)
		{
			TdsAsyncResult tdsAsyncResult = (TdsAsyncResult)socketAsyncResult.AsyncState;
			int num;
			for (int i = stream.EndRead(socketAsyncResult); i < 8; i += num)
			{
				num = Read(tmpBuf, i, 8 - i);
				if (num <= 0)
				{
					socket = null;
					stream.Close();
					throw new IOException((num != 0) ? "Connection error" : "Connection lost");
				}
			}
			TdsPacketType tdsPacketType = (TdsPacketType)tmpBuf[0];
			if (tdsPacketType != TdsPacketType.Logon && tdsPacketType != TdsPacketType.Query && tdsPacketType != TdsPacketType.Reply)
			{
				throw new Exception(string.Format("Unknown packet type {0}", tmpBuf[0]));
			}
			int num2 = Ntohs(tmpBuf, 2) - 8;
			if (num2 >= inBuffer.Length)
			{
				inBuffer = new byte[num2];
			}
			if (num2 < 0)
			{
				throw new Exception(string.Format("Confused by a length of {0}", num2));
			}
			GetPhysicalPacketData(num2);
			int num3 = num2 + 8;
			tdsAsyncResult.ReturnValue = num3;
			tdsAsyncResult.MarkComplete();
		}
	}
}

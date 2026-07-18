using System;

namespace Mono.Security.Protocol.Ntlm
{
	public abstract class MessageBase
	{
		private static byte[] header = new byte[8] { 78, 84, 76, 77, 83, 83, 80, 0 };

		private int _type;

		private NtlmFlags _flags;

		public NtlmFlags Flags
		{
			get
			{
				return _flags;
			}
			set
			{
				_flags = value;
			}
		}

		public int Type
		{
			get
			{
				return _type;
			}
		}

		protected MessageBase(int messageType)
		{
			_type = messageType;
		}

		protected byte[] PrepareMessage(int messageSize)
		{
			byte[] array = new byte[messageSize];
			Buffer.BlockCopy(header, 0, array, 0, 8);
			array[8] = (byte)_type;
			array[9] = (byte)(_type >> 8);
			array[10] = (byte)(_type >> 16);
			array[11] = (byte)(_type >> 24);
			return array;
		}

		protected virtual void Decode(byte[] message)
		{
			if (message == null)
			{
				throw new ArgumentNullException("message");
			}
			if (message.Length < 12)
			{
				string text = global::Locale.GetText("Minimum message length is 12 bytes.");
				throw new ArgumentOutOfRangeException("message", message.Length, text);
			}
			if (!CheckHeader(message))
			{
				string message2 = string.Format(global::Locale.GetText("Invalid Type{0} message."), _type);
				throw new ArgumentException(message2, "message");
			}
		}

		protected bool CheckHeader(byte[] message)
		{
			for (int i = 0; i < header.Length; i++)
			{
				if (message[i] != header[i])
				{
					return false;
				}
			}
			return Mono.Security.BitConverterLE.ToUInt32(message, 8) == _type;
		}

		public abstract byte[] GetBytes();
	}
}

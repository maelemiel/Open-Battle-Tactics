using System;
using System.Security.Cryptography;

namespace Mono.Security.Protocol.Ntlm
{
	public class Type2Message : MessageBase
	{
		private byte[] _nonce;

		public byte[] Nonce
		{
			get
			{
				return (byte[])_nonce.Clone();
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("Nonce");
				}
				if (value.Length != 8)
				{
					string text = global::Locale.GetText("Invalid Nonce Length (should be 8 bytes).");
					throw new ArgumentException(text, "Nonce");
				}
				_nonce = (byte[])value.Clone();
			}
		}

		public Type2Message()
			: base(2)
		{
			_nonce = new byte[8];
			RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
			randomNumberGenerator.GetBytes(_nonce);
			base.Flags = NtlmFlags.NegotiateUnicode | NtlmFlags.NegotiateNtlm | NtlmFlags.NegotiateAlwaysSign;
		}

		public Type2Message(byte[] message)
			: base(2)
		{
			_nonce = new byte[8];
			Decode(message);
		}

		~Type2Message()
		{
			if (_nonce != null)
			{
				Array.Clear(_nonce, 0, _nonce.Length);
			}
		}

		protected override void Decode(byte[] message)
		{
			base.Decode(message);
			base.Flags = (NtlmFlags)Mono.Security.BitConverterLE.ToUInt32(message, 20);
			Buffer.BlockCopy(message, 24, _nonce, 0, 8);
		}

		public override byte[] GetBytes()
		{
			byte[] array = PrepareMessage(40);
			short num = (short)array.Length;
			array[16] = (byte)num;
			array[17] = (byte)(num >> 8);
			array[20] = (byte)base.Flags;
			array[21] = (byte)((uint)base.Flags >> 8);
			array[22] = (byte)((uint)base.Flags >> 16);
			array[23] = (byte)((uint)base.Flags >> 24);
			Buffer.BlockCopy(_nonce, 0, array, 24, _nonce.Length);
			return array;
		}
	}
}

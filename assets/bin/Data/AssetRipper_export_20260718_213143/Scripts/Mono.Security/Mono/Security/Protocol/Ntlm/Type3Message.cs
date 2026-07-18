using System;
using System.Globalization;
using System.Text;

namespace Mono.Security.Protocol.Ntlm
{
	public class Type3Message : MessageBase
	{
		private byte[] _challenge;

		private string _host;

		private string _domain;

		private string _username;

		private string _password;

		private byte[] _lm;

		private byte[] _nt;

		public byte[] Challenge
		{
			get
			{
				if (_challenge == null)
				{
					return null;
				}
				return (byte[])_challenge.Clone();
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("Challenge");
				}
				if (value.Length != 8)
				{
					string text = global::Locale.GetText("Invalid Challenge Length (should be 8 bytes).");
					throw new ArgumentException(text, "Challenge");
				}
				_challenge = (byte[])value.Clone();
			}
		}

		public string Domain
		{
			get
			{
				return _domain;
			}
			set
			{
				_domain = value;
			}
		}

		public string Host
		{
			get
			{
				return _host;
			}
			set
			{
				_host = value;
			}
		}

		public string Password
		{
			get
			{
				return _password;
			}
			set
			{
				_password = value;
			}
		}

		public string Username
		{
			get
			{
				return _username;
			}
			set
			{
				_username = value;
			}
		}

		public byte[] LM
		{
			get
			{
				return _lm;
			}
		}

		public byte[] NT
		{
			get
			{
				return _nt;
			}
		}

		public Type3Message()
			: base(3)
		{
			_domain = Environment.UserDomainName;
			_host = Environment.MachineName;
			_username = Environment.UserName;
			base.Flags = NtlmFlags.NegotiateUnicode | NtlmFlags.NegotiateNtlm | NtlmFlags.NegotiateAlwaysSign;
		}

		public Type3Message(byte[] message)
			: base(3)
		{
			Decode(message);
		}

		~Type3Message()
		{
			if (_challenge != null)
			{
				Array.Clear(_challenge, 0, _challenge.Length);
			}
			if (_lm != null)
			{
				Array.Clear(_lm, 0, _lm.Length);
			}
			if (_nt != null)
			{
				Array.Clear(_nt, 0, _nt.Length);
			}
		}

		protected override void Decode(byte[] message)
		{
			base.Decode(message);
			if (Mono.Security.BitConverterLE.ToUInt16(message, 56) != message.Length)
			{
				string text = global::Locale.GetText("Invalid Type3 message length.");
				throw new ArgumentException(text, "message");
			}
			_password = null;
			int count = Mono.Security.BitConverterLE.ToUInt16(message, 28);
			int index = 64;
			_domain = Encoding.Unicode.GetString(message, index, count);
			int count2 = Mono.Security.BitConverterLE.ToUInt16(message, 44);
			int index2 = Mono.Security.BitConverterLE.ToUInt16(message, 48);
			_host = Encoding.Unicode.GetString(message, index2, count2);
			int count3 = Mono.Security.BitConverterLE.ToUInt16(message, 36);
			int index3 = Mono.Security.BitConverterLE.ToUInt16(message, 40);
			_username = Encoding.Unicode.GetString(message, index3, count3);
			_lm = new byte[24];
			int srcOffset = Mono.Security.BitConverterLE.ToUInt16(message, 16);
			Buffer.BlockCopy(message, srcOffset, _lm, 0, 24);
			_nt = new byte[24];
			int srcOffset2 = Mono.Security.BitConverterLE.ToUInt16(message, 24);
			Buffer.BlockCopy(message, srcOffset2, _nt, 0, 24);
			if (message.Length >= 64)
			{
				base.Flags = (NtlmFlags)Mono.Security.BitConverterLE.ToUInt32(message, 60);
			}
		}

		public override byte[] GetBytes()
		{
			byte[] bytes = Encoding.Unicode.GetBytes(_domain.ToUpper(CultureInfo.InvariantCulture));
			byte[] bytes2 = Encoding.Unicode.GetBytes(_username);
			byte[] bytes3 = Encoding.Unicode.GetBytes(_host.ToUpper(CultureInfo.InvariantCulture));
			byte[] array = PrepareMessage(64 + bytes.Length + bytes2.Length + bytes3.Length + 24 + 24);
			short num = (short)(64 + bytes.Length + bytes2.Length + bytes3.Length);
			array[12] = 24;
			array[13] = 0;
			array[14] = 24;
			array[15] = 0;
			array[16] = (byte)num;
			array[17] = (byte)(num >> 8);
			short num2 = (short)(num + 24);
			array[20] = 24;
			array[21] = 0;
			array[22] = 24;
			array[23] = 0;
			array[24] = (byte)num2;
			array[25] = (byte)(num2 >> 8);
			short num3 = (short)bytes.Length;
			short num4 = 64;
			array[28] = (byte)num3;
			array[29] = (byte)(num3 >> 8);
			array[30] = array[28];
			array[31] = array[29];
			array[32] = (byte)num4;
			array[33] = (byte)(num4 >> 8);
			short num5 = (short)bytes2.Length;
			short num6 = (short)(num4 + num3);
			array[36] = (byte)num5;
			array[37] = (byte)(num5 >> 8);
			array[38] = array[36];
			array[39] = array[37];
			array[40] = (byte)num6;
			array[41] = (byte)(num6 >> 8);
			short num7 = (short)bytes3.Length;
			short num8 = (short)(num6 + num5);
			array[44] = (byte)num7;
			array[45] = (byte)(num7 >> 8);
			array[46] = array[44];
			array[47] = array[45];
			array[48] = (byte)num8;
			array[49] = (byte)(num8 >> 8);
			short num9 = (short)array.Length;
			array[56] = (byte)num9;
			array[57] = (byte)(num9 >> 8);
			array[60] = (byte)base.Flags;
			array[61] = (byte)((uint)base.Flags >> 8);
			array[62] = (byte)((uint)base.Flags >> 16);
			array[63] = (byte)((uint)base.Flags >> 24);
			Buffer.BlockCopy(bytes, 0, array, num4, bytes.Length);
			Buffer.BlockCopy(bytes2, 0, array, num6, bytes2.Length);
			Buffer.BlockCopy(bytes3, 0, array, num8, bytes3.Length);
			using (ChallengeResponse challengeResponse = new ChallengeResponse(_password, _challenge))
			{
				Buffer.BlockCopy(challengeResponse.LM, 0, array, num, 24);
				Buffer.BlockCopy(challengeResponse.NT, 0, array, num2, 24);
				return array;
			}
		}
	}
}

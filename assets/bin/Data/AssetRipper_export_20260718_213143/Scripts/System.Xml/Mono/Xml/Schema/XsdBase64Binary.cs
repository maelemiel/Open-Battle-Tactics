using System;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace Mono.Xml.Schema
{
	internal class XsdBase64Binary : XsdString
	{
		private static string ALPHABET;

		private static byte[] decodeTable;

		public override XmlTypeCode TypeCode
		{
			get
			{
				return XmlTypeCode.Base64Binary;
			}
		}

		public override Type ValueType
		{
			get
			{
				return typeof(byte[]);
			}
		}

		internal XsdBase64Binary()
		{
		}

		static XsdBase64Binary()
		{
			ALPHABET = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
			int length = ALPHABET.Length;
			decodeTable = new byte[123];
			for (int i = 0; i < decodeTable.Length; i++)
			{
				decodeTable[i] = byte.MaxValue;
			}
			for (int j = 0; j < length; j++)
			{
				char c = ALPHABET[j];
				decodeTable[(uint)c] = (byte)j;
			}
		}

		public override object ParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr)
		{
			byte[] bytes = new ASCIIEncoding().GetBytes(s);
			FromBase64Transform fromBase64Transform = new FromBase64Transform();
			return fromBase64Transform.TransformFinalBlock(bytes, 0, bytes.Length);
		}

		internal override int Length(string s)
		{
			int num = 0;
			int num2 = 0;
			int length = s.Length;
			for (int i = 0; i < length; i++)
			{
				char c = s[i];
				if (char.IsWhiteSpace(c))
				{
					continue;
				}
				if (isData(c))
				{
					num++;
					continue;
				}
				if (isPad(c))
				{
					num2++;
					continue;
				}
				return -1;
			}
			if (num2 > 2)
			{
				return -1;
			}
			if (num2 > 0)
			{
				num2 = 3 - num2;
			}
			return num / 4 * 3 + num2;
		}

		protected static bool isPad(char octect)
		{
			return octect == '=';
		}

		protected static bool isData(char octect)
		{
			return octect <= 'z' && decodeTable[(uint)octect] != byte.MaxValue;
		}

		internal override ValueType ParseValueType(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr)
		{
			return new StringValueType(ParseValue(s, nameTable, nsmgr) as string);
		}
	}
}

using System;
using System.Collections;
using System.IO;
using System.Text;

namespace Mono.Security
{
	internal class ASN1
	{
		private byte m_nTag;

		private byte[] m_aValue;

		private ArrayList elist;

		public int Count
		{
			get
			{
				if (elist == null)
				{
					return 0;
				}
				return elist.Count;
			}
		}

		public byte Tag
		{
			get
			{
				return m_nTag;
			}
		}

		public int Length
		{
			get
			{
				if (m_aValue != null)
				{
					return m_aValue.Length;
				}
				return 0;
			}
		}

		public byte[] Value
		{
			get
			{
				if (m_aValue == null)
				{
					GetBytes();
				}
				return (byte[])m_aValue.Clone();
			}
			set
			{
				if (value != null)
				{
					m_aValue = (byte[])value.Clone();
				}
			}
		}

		public ASN1 this[int index]
		{
			get
			{
				try
				{
					if (elist == null || index >= elist.Count)
					{
						return null;
					}
					return (ASN1)elist[index];
				}
				catch (ArgumentOutOfRangeException)
				{
					return null;
				}
			}
		}

		public ASN1()
			: this(0, null)
		{
		}

		public ASN1(byte tag)
			: this(tag, null)
		{
		}

		public ASN1(byte tag, byte[] data)
		{
			m_nTag = tag;
			m_aValue = data;
		}

		public ASN1(byte[] data)
		{
			m_nTag = data[0];
			int num = 0;
			int num2 = data[1];
			if (num2 > 128)
			{
				num = num2 - 128;
				num2 = 0;
				for (int i = 0; i < num; i++)
				{
					num2 *= 256;
					num2 += data[i + 2];
				}
			}
			else if (num2 == 128)
			{
				throw new NotSupportedException("Undefined length encoding.");
			}
			m_aValue = new byte[num2];
			Buffer.BlockCopy(data, 2 + num, m_aValue, 0, num2);
			if ((m_nTag & 0x20) == 32)
			{
				int anPos = 2 + num;
				Decode(data, ref anPos, data.Length);
			}
		}

		private bool CompareArray(byte[] array1, byte[] array2)
		{
			bool flag = array1.Length == array2.Length;
			if (flag)
			{
				for (int i = 0; i < array1.Length; i++)
				{
					if (array1[i] != array2[i])
					{
						return false;
					}
				}
			}
			return flag;
		}

		public bool Equals(byte[] asn1)
		{
			return CompareArray(GetBytes(), asn1);
		}

		public bool CompareValue(byte[] value)
		{
			return CompareArray(m_aValue, value);
		}

		public ASN1 Add(ASN1 asn1)
		{
			if (asn1 != null)
			{
				if (elist == null)
				{
					elist = new ArrayList();
				}
				elist.Add(asn1);
			}
			return asn1;
		}

		public virtual byte[] GetBytes()
		{
			byte[] array = null;
			if (Count > 0)
			{
				int num = 0;
				ArrayList arrayList = new ArrayList();
				foreach (ASN1 item in elist)
				{
					byte[] bytes = item.GetBytes();
					arrayList.Add(bytes);
					num += bytes.Length;
				}
				array = new byte[num];
				int num2 = 0;
				for (int i = 0; i < elist.Count; i++)
				{
					byte[] array2 = (byte[])arrayList[i];
					Buffer.BlockCopy(array2, 0, array, num2, array2.Length);
					num2 += array2.Length;
				}
			}
			else if (m_aValue != null)
			{
				array = m_aValue;
			}
			int num3 = 0;
			byte[] array3;
			if (array != null)
			{
				int num4 = array.Length;
				if (num4 > 127)
				{
					if (num4 <= 255)
					{
						array3 = new byte[3 + num4];
						Buffer.BlockCopy(array, 0, array3, 3, num4);
						num3 = 129;
						array3[2] = (byte)num4;
					}
					else if (num4 <= 65535)
					{
						array3 = new byte[4 + num4];
						Buffer.BlockCopy(array, 0, array3, 4, num4);
						num3 = 130;
						array3[2] = (byte)(num4 >> 8);
						array3[3] = (byte)num4;
					}
					else if (num4 <= 16777215)
					{
						array3 = new byte[5 + num4];
						Buffer.BlockCopy(array, 0, array3, 5, num4);
						num3 = 131;
						array3[2] = (byte)(num4 >> 16);
						array3[3] = (byte)(num4 >> 8);
						array3[4] = (byte)num4;
					}
					else
					{
						array3 = new byte[6 + num4];
						Buffer.BlockCopy(array, 0, array3, 6, num4);
						num3 = 132;
						array3[2] = (byte)(num4 >> 24);
						array3[3] = (byte)(num4 >> 16);
						array3[4] = (byte)(num4 >> 8);
						array3[5] = (byte)num4;
					}
				}
				else
				{
					array3 = new byte[2 + num4];
					Buffer.BlockCopy(array, 0, array3, 2, num4);
					num3 = num4;
				}
				if (m_aValue == null)
				{
					m_aValue = array;
				}
			}
			else
			{
				array3 = new byte[2];
			}
			array3[0] = m_nTag;
			array3[1] = (byte)num3;
			return array3;
		}

		protected void Decode(byte[] asn1, ref int anPos, int anLength)
		{
			while (anPos < anLength - 1)
			{
				byte tag;
				int length;
				byte[] content;
				DecodeTLV(asn1, ref anPos, out tag, out length, out content);
				if (tag != 0)
				{
					ASN1 aSN = Add(new ASN1(tag, content));
					if ((tag & 0x20) == 32)
					{
						int anPos2 = anPos;
						aSN.Decode(asn1, ref anPos2, anPos2 + length);
					}
					anPos += length;
				}
			}
		}

		protected void DecodeTLV(byte[] asn1, ref int pos, out byte tag, out int length, out byte[] content)
		{
			tag = asn1[pos++];
			length = asn1[pos++];
			if ((length & 0x80) == 128)
			{
				int num = length & 0x7F;
				length = 0;
				for (int i = 0; i < num; i++)
				{
					length = length * 256 + asn1[pos++];
				}
			}
			content = new byte[length];
			Buffer.BlockCopy(asn1, pos, content, 0, length);
		}

		public ASN1 Element(int index, byte anTag)
		{
			try
			{
				if (elist == null || index >= elist.Count)
				{
					return null;
				}
				ASN1 aSN = (ASN1)elist[index];
				if (aSN.Tag == anTag)
				{
					return aSN;
				}
				return null;
			}
			catch (ArgumentOutOfRangeException)
			{
				return null;
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("Tag: {0} {1}", m_nTag.ToString("X2"), Environment.NewLine);
			stringBuilder.AppendFormat("Length: {0} {1}", Value.Length, Environment.NewLine);
			stringBuilder.Append("Value: ");
			stringBuilder.Append(Environment.NewLine);
			for (int i = 0; i < Value.Length; i++)
			{
				stringBuilder.AppendFormat("{0} ", Value[i].ToString("X2"));
				if ((i + 1) % 16 == 0)
				{
					stringBuilder.AppendFormat(Environment.NewLine);
				}
			}
			return stringBuilder.ToString();
		}

		public void SaveToFile(string filename)
		{
			if (filename == null)
			{
				throw new ArgumentNullException("filename");
			}
			using (FileStream fileStream = File.Create(filename))
			{
				byte[] bytes = GetBytes();
				fileStream.Write(bytes, 0, bytes.Length);
			}
		}
	}
}

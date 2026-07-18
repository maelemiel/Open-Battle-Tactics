using System.Text;

namespace System.Collections.Specialized
{
	public struct BitVector32
	{
		public struct Section
		{
			private short mask;

			private short offset;

			public short Mask
			{
				get
				{
					return mask;
				}
			}

			public short Offset
			{
				get
				{
					return offset;
				}
			}

			internal Section(short mask, short offset)
			{
				this.mask = mask;
				this.offset = offset;
			}

			public bool Equals(Section obj)
			{
				return mask == obj.mask && offset == obj.offset;
			}

			public override bool Equals(object o)
			{
				if (!(o is Section))
				{
					return false;
				}
				Section section = (Section)o;
				return mask == section.mask && offset == section.offset;
			}

			public override int GetHashCode()
			{
				return mask << (int)offset;
			}

			public override string ToString()
			{
				return ToString(this);
			}

			public static string ToString(Section value)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("Section{0x");
				stringBuilder.Append(Convert.ToString(value.Mask, 16));
				stringBuilder.Append(", 0x");
				stringBuilder.Append(Convert.ToString(value.Offset, 16));
				stringBuilder.Append("}");
				return stringBuilder.ToString();
			}

			public static bool operator ==(Section v1, Section v2)
			{
				return v1.mask == v2.mask && v1.offset == v2.offset;
			}

			public static bool operator !=(Section v1, Section v2)
			{
				return v1.mask != v2.mask || v1.offset != v2.offset;
			}
		}

		private int bits;

		public int Data
		{
			get
			{
				return bits;
			}
		}

		public int this[Section section]
		{
			get
			{
				return (bits >> (int)section.Offset) & section.Mask;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentException("Section can't hold negative values");
				}
				if (value > section.Mask)
				{
					throw new ArgumentException("Value too large to fit in section");
				}
				bits &= ~(section.Mask << (int)section.Offset);
				bits |= value << (int)section.Offset;
			}
		}

		public bool this[int mask]
		{
			get
			{
				return (bits & mask) == mask;
			}
			set
			{
				if (value)
				{
					bits |= mask;
				}
				else
				{
					bits &= ~mask;
				}
			}
		}

		public BitVector32(BitVector32 source)
		{
			bits = source.bits;
		}

		public BitVector32(int init)
		{
			bits = init;
		}

		public static int CreateMask()
		{
			return 1;
		}

		public static int CreateMask(int prev)
		{
			switch (prev)
			{
			case 0:
				return 1;
			case int.MinValue:
				throw new InvalidOperationException("all bits set");
			default:
				return prev << 1;
			}
		}

		public static Section CreateSection(short maxValue)
		{
			return CreateSection(maxValue, new Section(0, 0));
		}

		public static Section CreateSection(short maxValue, Section previous)
		{
			if (maxValue < 1)
			{
				throw new ArgumentException("maxValue");
			}
			int num = HighestSetBit(maxValue);
			int num2 = (1 << num) - 1;
			int num3 = previous.Offset + HighestSetBit(previous.Mask);
			if (num3 + num > 32)
			{
				throw new ArgumentException("Sections cannot exceed 32 bits in total");
			}
			return new Section((short)num2, (short)num3);
		}

		public override bool Equals(object o)
		{
			return o is BitVector32 && bits == ((BitVector32)o).bits;
		}

		public override int GetHashCode()
		{
			return bits.GetHashCode();
		}

		public override string ToString()
		{
			return ToString(this);
		}

		public static string ToString(BitVector32 value)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("BitVector32{");
			for (long num = 2147483648L; num > 0; num >>= 1)
			{
				stringBuilder.Append(((value.bits & num) != 0L) ? '1' : '0');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		private static int HighestSetBit(int i)
		{
			int j;
			for (j = 0; i >> j != 0; j++)
			{
			}
			return j;
		}
	}
}

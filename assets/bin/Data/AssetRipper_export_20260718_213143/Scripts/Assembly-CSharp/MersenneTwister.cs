public class MersenneTwister : IRandomProvider
{
	private const int N = 624;

	private const int M = 397;

	private const uint MATRIX_A = 2567483615u;

	private const uint UPPER_MASK = 2147483648u;

	private const uint LOWER_MASK = 2147483647u;

	private uint[] mt = new uint[624];

	private short mti;

	private static uint[] mag01 = new uint[2] { 0u, 2567483615u };

	private int counter;

	public int GeneratedNumbers
	{
		get
		{
			return counter;
		}
	}

	public MersenneTwister(uint seed)
	{
		if (seed == 0)
		{
			seed = 1u;
		}
		mt[0] = seed & 0xFFFFFFFFu;
		for (mti = 1; mti < 624; mti++)
		{
			mt[mti] = (69069 * mt[mti - 1]) & 0xFFFFFFFFu;
		}
	}

	private uint GenerateUInt()
	{
		counter++;
		uint num2;
		if (mti >= 624)
		{
			short num;
			for (num = 0; num < 227; num++)
			{
				num2 = (mt[num] & 0x80000000u) | (mt[num + 1] & 0x7FFFFFFF);
				mt[num] = mt[num + 397] ^ (num2 >> 1) ^ mag01[num2 & 1];
			}
			while (num < 623)
			{
				num2 = (mt[num] & 0x80000000u) | (mt[num + 1] & 0x7FFFFFFF);
				mt[num] = mt[num + -227] ^ (num2 >> 1) ^ mag01[num2 & 1];
				num++;
			}
			num2 = (mt[623] & 0x80000000u) | (mt[0] & 0x7FFFFFFF);
			mt[623] = mt[396] ^ (num2 >> 1) ^ mag01[num2 & 1];
			mti = 0;
		}
		num2 = mt[mti++];
		num2 ^= num2 >> 11;
		num2 ^= (num2 << 7) & 0x9D2C5680u;
		num2 ^= (num2 << 15) & 0xEFC60000u;
		return num2 ^ (num2 >> 18);
	}

	public int Next(int max)
	{
		return (int)(GenerateUInt() % (uint)max);
	}

	public uint Next(uint max)
	{
		return GenerateUInt() % max;
	}

	public IRandomProvider Clone()
	{
		MersenneTwister mersenneTwister = new MersenneTwister(0u);
		mersenneTwister.mt = new uint[mt.Length];
		for (int i = 0; i < mersenneTwister.mt.Length; i++)
		{
			mersenneTwister.mt[i] = mt[i];
		}
		mersenneTwister.mti = mti;
		return mersenneTwister;
	}
}

public class MicrosoftRNG : IRandomProvider
{
	private ulong seed;

	private int counter;

	public int GeneratedNumbers
	{
		get
		{
			return counter;
		}
	}

	public MicrosoftRNG(uint seed)
	{
		this.seed = seed;
	}

	public uint Next(uint max)
	{
		counter++;
		seed = seed * 214013 + 2531011;
		return (uint)(seed / 65536 % max);
	}

	public int Next(int max)
	{
		counter++;
		seed = seed * 214013 + 2531011;
		return (int)(seed / 65536 % (ulong)max);
	}

	public IRandomProvider Clone()
	{
		MicrosoftRNG microsoftRNG = new MicrosoftRNG((uint)seed);
		microsoftRNG.seed = seed;
		return microsoftRNG;
	}
}

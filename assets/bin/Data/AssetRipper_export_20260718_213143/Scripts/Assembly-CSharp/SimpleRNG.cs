public class SimpleRNG : IRandomProvider
{
	private uint seed;

	private int counter;

	public int GeneratedNumbers
	{
		get
		{
			return counter;
		}
	}

	public SimpleRNG(uint seed)
	{
		this.seed = seed;
	}

	public uint Next(uint max)
	{
		counter++;
		seed = seed * 1103515245 + 12345;
		return seed % max;
	}

	public int Next(int max)
	{
		counter++;
		seed = seed * 1103515245 + 12345;
		return (int)(seed % max);
	}

	public IRandomProvider Clone()
	{
		SimpleRNG simpleRNG = new SimpleRNG(seed);
		simpleRNG.seed = seed;
		return simpleRNG;
	}
}

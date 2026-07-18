using System.Collections.Generic;

public class FakeSequenceRNG : IRandomProvider
{
	private List<int> values;

	private uint defaultValue;

	public int index;

	private int counter;

	public int GeneratedNumbers
	{
		get
		{
			return counter;
		}
	}

	public FakeSequenceRNG(List<int> values, uint defaultValue = 0)
	{
		this.values = values;
		this.defaultValue = defaultValue;
		index = 0;
	}

	public uint Next(uint max)
	{
		counter++;
		if (index < values.Count)
		{
			uint num = (uint)values[index];
			index++;
			return num % max;
		}
		return defaultValue;
	}

	public int Next(int max)
	{
		return (int)Next((uint)max);
	}

	public IRandomProvider Clone()
	{
		FakeSequenceRNG fakeSequenceRNG = new FakeSequenceRNG(values, defaultValue);
		fakeSequenceRNG.index = index;
		return fakeSequenceRNG;
	}
}

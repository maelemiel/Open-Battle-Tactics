using System;
using System.Collections;
using System.Reflection;

namespace NUnit.Framework
{
	public class Randomizer : Random
	{
		private static Random seedGenerator = new Random();

		private static Hashtable randomizers = new Hashtable();

		public static int RandomSeed
		{
			get
			{
				return seedGenerator.Next();
			}
		}

		public static Randomizer GetRandomizer(MemberInfo member)
		{
			Randomizer randomizer = (Randomizer)randomizers[member];
			if (randomizer == null)
			{
				randomizer = (Randomizer)(randomizers[member] = new Randomizer());
			}
			return randomizer;
		}

		public static Randomizer GetRandomizer(ParameterInfo parameter)
		{
			return GetRandomizer(parameter.Member);
		}

		public Randomizer()
			: base(RandomSeed)
		{
		}

		public Randomizer(int seed)
			: base(seed)
		{
		}

		public double[] GetDoubles(int count)
		{
			double[] array = new double[count];
			for (int i = 0; i < count; i++)
			{
				array[i] = NextDouble();
			}
			return array;
		}

		public double[] GetDoubles(double min, double max, int count)
		{
			double num = max - min;
			double[] array = new double[count];
			for (int i = 0; i < count; i++)
			{
				array[i] = NextDouble() * num + min;
			}
			return array;
		}

		public int[] GetInts(int min, int max, int count)
		{
			int[] array = new int[count];
			for (int i = 0; i < count; i++)
			{
				array[i] = Next(min, max);
			}
			return array;
		}
	}
}

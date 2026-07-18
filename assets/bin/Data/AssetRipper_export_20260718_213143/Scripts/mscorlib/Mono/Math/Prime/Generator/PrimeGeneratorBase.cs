namespace Mono.Math.Prime.Generator
{
	internal abstract class PrimeGeneratorBase
	{
		public virtual ConfidenceFactor Confidence
		{
			get
			{
				return ConfidenceFactor.Medium;
			}
		}

		public virtual PrimalityTest PrimalityTest
		{
			get
			{
				return PrimalityTests.RabinMillerTest;
			}
		}

		public virtual int TrialDivisionBounds
		{
			get
			{
				return 4000;
			}
		}

		protected bool PostTrialDivisionTests(BigInteger bi)
		{
			return PrimalityTest(bi, Confidence);
		}

		public abstract BigInteger GenerateNewPrime(int bits);
	}
}

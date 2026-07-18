namespace System.Security.Cryptography
{
	public abstract class RandomNumberGenerator
	{
		public static RandomNumberGenerator Create()
		{
			return Create("System.Security.Cryptography.RandomNumberGenerator");
		}

		public static RandomNumberGenerator Create(string rngName)
		{
			return (RandomNumberGenerator)CryptoConfig.CreateFromName(rngName);
		}

		public abstract void GetBytes(byte[] data);

		public abstract void GetNonZeroBytes(byte[] data);
	}
}

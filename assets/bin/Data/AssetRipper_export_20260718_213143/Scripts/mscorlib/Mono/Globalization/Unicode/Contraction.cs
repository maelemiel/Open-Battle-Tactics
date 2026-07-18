namespace Mono.Globalization.Unicode
{
	internal class Contraction
	{
		public readonly char[] Source;

		public readonly string Replacement;

		public readonly byte[] SortKey;

		public Contraction(char[] source, string replacement, byte[] sortkey)
		{
			Source = source;
			Replacement = replacement;
			SortKey = sortkey;
		}
	}
}

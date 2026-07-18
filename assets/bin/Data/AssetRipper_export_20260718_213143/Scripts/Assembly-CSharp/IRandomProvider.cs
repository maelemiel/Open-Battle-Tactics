public interface IRandomProvider
{
	int GeneratedNumbers { get; }

	uint Next(uint max);

	IRandomProvider Clone();
}

public interface IPartMetadata
{
	string ID { get; }

	int PartType { get; }

	int DropChance { get; }

	int DropMin { get; }

	int DropMax { get; }
}

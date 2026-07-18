public interface IAbilityMetadata
{
	string ID { get; }

	string Name { get; }

	string Type { get; }

	string ShortDescription { get; }

	int Cost { get; }

	TargetType TargetSelectType { get; }

	string TargetSelectText { get; }

	bool LimitOnePerRound { get; }

	int ExecutionOrder { get; }

	string ButtonIconArtName { get; }

	int BoostValue { get; }

	float BoostValueFloat { get; }

	int SecondaryBoostValue { get; }

	float SecondaryBoostValueFloat { get; }

	bool IsUnitAbility { get; }
}

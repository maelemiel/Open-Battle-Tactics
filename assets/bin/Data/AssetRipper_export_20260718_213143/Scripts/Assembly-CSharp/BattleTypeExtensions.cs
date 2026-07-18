public static class BattleTypeExtensions
{
	public static string GetDieFaceName(this DieFaceType type)
	{
		switch (type)
		{
		case DieFaceType.DirectDamage:
			return "metadata_dice_type_damage".Localize("DAMAGE");
		case DieFaceType.Initiative:
			return "metadata_dice_type_initiative".Localize("INITIATIVE");
		case DieFaceType.ArmourPiercing:
			return "metadata_dice_type_armour_piercing".Localize("ARMOUR PIERCING");
		case DieFaceType.Special:
			return "metadata_dice_type_special".Localize("SPECIAL");
		case DieFaceType.AcidStrike:
			return "metadata_dice_type_acid_strike".Localize("ACID STRIKE");
		default:
			return "metadata_dice_type_damage".Localize("DAMAGE");
		}
	}
}

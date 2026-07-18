public static class AbilitySortTypes
{
	public static SortType Locked = new SortTypeInt
	{
		label = "ui_editabilities_locked".Localize("Locked"),
		sortHandler = (object x) => UserProfile.player.HasUnlockedAbility(((AbilityDataModel)x).ID) ? 1 : (-1)
	};

	public static SortType EnergySort = new SortTypeInt
	{
		label = "ui_editabilities_energycost".Localize("Energy Cost"),
		sortHandler = (object x) => ((AbilityDataModel)x).actionPoint
	};

	public static SortType UnlockTierSort = new SortTypeInt
	{
		label = "ui_editabilities_highesttier".Localize("Highest Tier"),
		sortHandler = (object x) => ((AbilityDataModel)x).unlockTier
	};

	public static SortType UnlockTierSortAscending = new SortTypeInt
	{
		label = "ui_editabilities_lowesttier".Localize("Lowest Tier"),
		sortHandler = (object x) => -((AbilityDataModel)x).unlockTier
	};

	public static SortType Close = new SortTypeInt
	{
		label = "ui_editabilities_cancel".Localize("Cancel")
	};
}

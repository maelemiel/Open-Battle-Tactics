public static class UnitSortTypes
{
	public static SortType RaritySort = new SortTypeInt
	{
		label = "ui_editteam_sort_rarity".Localize("Rarity"),
		sortHandler = (object unit) => ((UserUnit)unit).Rarity
	};

	public static SortType LevelSort = new SortTypeInt
	{
		label = "ui_editteam_sort_level".Localize("Level"),
		sortHandler = (object unit) => ((UserUnit)unit).level
	};

	public static SortType HealthSort = new SortTypeInt
	{
		label = "ui_editteam_sort_health".Localize("Health"),
		sortHandler = (object unit) => ((UserUnit)unit).StartingHealth
	};

	public static SortType DamageSort = new SortTypeInt
	{
		label = "ui_editteam_sort_damage".Localize("Damage"),
		sortHandler = (object unit) => (int)(GetAverageRollValue((UserUnit)unit, DieFaceType.DirectDamage) * 1000f) + (int)(GetAverageRollValue((UserUnit)unit, DieFaceType.Initiative) * 1000f)
	};

	public static SortType FirstStrikeSort = new SortTypeInt
	{
		label = "ui_editteam_sort_firststrike".Localize("First Strike"),
		sortHandler = (object unit) => (int)(GetAverageRollValue((UserUnit)unit, DieFaceType.Initiative) * 1000f)
	};

	public static SortType SpecialSort = new SortTypeInt
	{
		label = "ui_editteam_sort_special".Localize("Special"),
		sortHandler = (object unit) => GetRollTypeCount((UserUnit)unit, DieFaceType.Special)
	};

	public static SortType HighValueSort = new SortTypeInt
	{
		label = "ui_editteam_sort_highvaluesort".Localize("HighValueSort"),
		sortHandler = (object unit) => GetRollValueTotal((UserUnit)unit)
	};

	public static SortType ByNameSort = new SortTypeString
	{
		label = "ui_editteam_sort_byname".Localize("NAME"),
		sortHandler = (object unit) => ((UserUnit)unit).Name
	};

	public static SortType PartialLevel = new SortTypeInt
	{
		label = "ui_editteam_sort_by_partial".Localize("UPGRADES"),
		sortHandler = (object unit) => ((UserUnit)unit).partialLevel
	};

	public static SortType ResetSort = new SortTypeInt
	{
		label = "ui_editteam_sort_newest".Localize("Newest"),
		sortHandler = (object unit) => 0
	};

	public static SortType Close = new SortTypeInt
	{
		label = "ui_editteam_sort_cancel".Localize("Cancel")
	};

	public static int GetHighestRollValue(UserUnit unit, DieFaceType faceType)
	{
		int num = 0;
		for (int i = 0; i < unit.RollValues.Length; i++)
		{
			if (unit.RollTypes[i] == faceType && unit.RollValues[i] > num)
			{
				num = unit.RollValues[i];
			}
		}
		return num;
	}

	public static float GetAverageRollValue(UserUnit unit, DieFaceType faceType)
	{
		int num = 0;
		for (int i = 0; i < unit.RollValues.Length; i++)
		{
			if (unit.RollTypes[i] == faceType)
			{
				num += unit.RollValues[i];
			}
		}
		return (float)num / (float)unit.RollValues.Length;
	}

	public static int GetRollTypeCount(UserUnit unit, DieFaceType faceType)
	{
		int num = 0;
		for (int i = 0; i < unit.RollTypes.Length; i++)
		{
			if (unit.RollTypes[i] == faceType)
			{
				num++;
			}
		}
		return num;
	}

	public static int GetRollValueTotal(UserUnit unit)
	{
		int num = 0;
		for (int i = 0; i < unit.RollValues.Length; i++)
		{
			num = ((unit.RollTypes[i] != DieFaceType.Special) ? (num + unit.RollValues[i]) : (num + (10 + unit.GetAbilityBoostValueA(0))));
		}
		return num;
	}
}

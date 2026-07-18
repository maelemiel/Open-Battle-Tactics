public static class TutorialConstants
{
	public static int[] FIRST_UNIT_OPTIONS = new int[3] { FIRST_UNIT_OPTION_1, FIRST_UNIT_OPTION_2, FIRST_UNIT_OPTION_3 };

	public static int FIRST_UNIT_OPTION_1
	{
		get
		{
			return CacheManager.GetConstantInt("starter_unit_1", 210010101);
		}
	}

	public static int FIRST_UNIT_OPTION_2
	{
		get
		{
			return CacheManager.GetConstantInt("starter_unit_2", 210010101);
		}
	}

	public static int FIRST_UNIT_OPTION_3
	{
		get
		{
			return CacheManager.GetConstantInt("starter_unit_3", 210010101);
		}
	}

	public static int SECOND_UNIT
	{
		get
		{
			return CacheManager.GetConstantInt("tutorial_unit_1", 130010101);
		}
	}

	public static int THIRD_UNIT
	{
		get
		{
			return CacheManager.GetConstantInt("tutorial_unit_2", 120010101);
		}
	}

	public static int FOURTH_UNIT
	{
		get
		{
			return CacheManager.GetConstantInt("tutorial_unit_3", 110010101);
		}
	}

	public static int TUTORIAL_NME_1_1
	{
		get
		{
			return CacheManager.GetConstantInt("tutorial_enemy_1_1", 110050101);
		}
	}

	public static int TUTORIAL_NME_2_1
	{
		get
		{
			return CacheManager.GetConstantInt("tutorial_enemy_2_1", 110050101);
		}
	}

	public static int TUTORIAL_NME_2_2
	{
		get
		{
			return CacheManager.GetConstantInt("tutorial_enemy_2_2", 110060101);
		}
	}

	public static int TUTORIAL_NME_3_1
	{
		get
		{
			return CacheManager.GetConstantInt("tutorial_enemy_3_1", 110050101);
		}
	}

	public static int TUTORIAL_NME_3_2
	{
		get
		{
			return CacheManager.GetConstantInt("tutorial_enemy_3_2", 110060101);
		}
	}

	public static int TUTORIAL_NME_3_3
	{
		get
		{
			return CacheManager.GetConstantInt("tutorial_enemy_3_3", 130010101);
		}
	}

	public static int TUTORIAL_NME_4_1
	{
		get
		{
			return CacheManager.GetConstantInt("tutorial_enemy_4_1", 220020101);
		}
	}

	public static bool SHOW_CHANGE_NAME
	{
		get
		{
			return CacheManager.GetConstantString("tutorial_show_change_name", "TRUE") == "TRUE";
		}
	}
}

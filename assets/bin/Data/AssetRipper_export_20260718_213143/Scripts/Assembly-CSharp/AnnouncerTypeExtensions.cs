public static class AnnouncerTypeExtensions
{
	public static AnnouncerType GetAnnouncerType(string announcerType)
	{
		switch (announcerType)
		{
		case "male":
			return AnnouncerType.MALE_ANNOUNCER_BUILTIN;
		case "female":
			return AnnouncerType.FEMALE_ANNOUNCER_BUILTIN;
		case "rival":
			return AnnouncerType.RIVAL_ANNOUNCER;
		case "rival_win":
			return AnnouncerType.RIVAL_ANNOUNCER_WIN;
		case "rival_lose":
			return AnnouncerType.RIVAL_ANNOUNCER_LOSE;
		case "bambi":
			return AnnouncerType.BAMBI_ANNOUNCER;
		case "male_asset_bundle":
			return AnnouncerType.MALE_ANNOUNCER;
		case "female_asset_bundle":
			return AnnouncerType.FEMALE_ANNOUNCER;
		case "announcer_0":
			return AnnouncerType.ANNOUNCER_0;
		case "announcer_1":
			return AnnouncerType.ANNOUNCER_1;
		case "announcer_2":
			return AnnouncerType.ANNOUNCER_2;
		case "announcer_3":
			return AnnouncerType.ANNOUNCER_3;
		case "announcer_4":
			return AnnouncerType.ANNOUNCER_4;
		case "announcer_5":
			return AnnouncerType.ANNOUNCER_5;
		case "announcer_6":
			return AnnouncerType.ANNOUNCER_6;
		case "announcer_7":
			return AnnouncerType.ANNOUNCER_7;
		case "announcer_8":
			return AnnouncerType.ANNOUNCER_8;
		case "announcer_9":
			return AnnouncerType.ANNOUNCER_9;
		default:
			return AnnouncerType.NONE;
		}
	}

	public static EffectType GetEffectType(this AnnouncerType announcerType)
	{
		switch (announcerType)
		{
		case AnnouncerType.MALE_ANNOUNCER:
			return EffectType.ANNOUNCER_FRANK;
		case AnnouncerType.FEMALE_ANNOUNCER:
			return EffectType.ANNOUNCER_PEPPER;
		case AnnouncerType.MALE_ANNOUNCER_BUILTIN:
			return EffectType.ANNOUNCER_FRANK_BUILTIN;
		case AnnouncerType.FEMALE_ANNOUNCER_BUILTIN:
			return EffectType.ANNOUNCER_PEPPER_BUILTIN;
		case AnnouncerType.RIVAL_ANNOUNCER:
			return EffectType.ANNOUNCER_VIKKI;
		case AnnouncerType.BAMBI_ANNOUNCER:
			return EffectType.ANNOUNCER_BAMBI;
		case AnnouncerType.ANNOUNCER_0:
			return EffectType.ANNOUNCER_0;
		case AnnouncerType.ANNOUNCER_1:
			return EffectType.ANNOUNCER_1;
		case AnnouncerType.ANNOUNCER_2:
			return EffectType.ANNOUNCER_2;
		case AnnouncerType.ANNOUNCER_3:
			return EffectType.ANNOUNCER_3;
		case AnnouncerType.ANNOUNCER_4:
			return EffectType.ANNOUNCER_4;
		case AnnouncerType.ANNOUNCER_5:
			return EffectType.ANNOUNCER_5;
		case AnnouncerType.ANNOUNCER_6:
			return EffectType.ANNOUNCER_6;
		case AnnouncerType.ANNOUNCER_7:
			return EffectType.ANNOUNCER_7;
		case AnnouncerType.ANNOUNCER_8:
			return EffectType.ANNOUNCER_8;
		case AnnouncerType.ANNOUNCER_9:
			return EffectType.ANNOUNCER_9;
		default:
			return EffectType.NONE;
		}
	}
}

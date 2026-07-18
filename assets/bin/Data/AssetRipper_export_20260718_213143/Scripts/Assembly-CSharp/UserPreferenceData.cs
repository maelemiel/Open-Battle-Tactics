using System;

public class UserPreferenceData
{
	public enum PUSH_NOTIF_TYPE
	{
		TANKS_BUILD = 0,
		TANKS_REPAIRED = 1,
		TICKET_RECHARGED = 2,
		PRIZES_READY = 3,
		CLUB_CRATES = 4,
		EVENT_REWARDS = 5,
		TEAM_REPORT_CARDS = 6,
		ALL = 7
	}

	private const string KEY_BATTERY_SAVER = "batterySaverOn";

	private const string KEY_KAMCORD = "kamcordOn";

	private const string KEY_MUSIC_VOLUME = "musicVolume";

	private const string sfxVolumeKey = "sfxVolume";

	private KeyValueStorage preferenceKVS;

	private bool _batterySaverOn;

	private bool _kamcordOn;

	private bool[] _pushNotifArray;

	public bool BatterySaverOn
	{
		get
		{
			return _batterySaverOn;
		}
		set
		{
			preferenceKVS.SetValueAsync("batterySaverOn", value);
			_batterySaverOn = value;
		}
	}

	public bool KamcordOn
	{
		get
		{
			return _kamcordOn;
		}
		set
		{
			preferenceKVS.SetValueAsync("kamcordOn", value);
			_kamcordOn = value;
		}
	}

	public bool NotifTanksBuild
	{
		get
		{
			return _pushNotifArray[0];
		}
		set
		{
			_pushNotifArray[0] = value;
		}
	}

	public bool NotifTanksRepaired
	{
		get
		{
			return _pushNotifArray[1];
		}
		set
		{
			_pushNotifArray[1] = value;
		}
	}

	public bool NotifTicketsRecharged
	{
		get
		{
			return _pushNotifArray[2];
		}
		set
		{
			_pushNotifArray[2] = value;
		}
	}

	public bool NotifPrizesReady
	{
		get
		{
			return _pushNotifArray[3];
		}
		set
		{
			_pushNotifArray[3] = value;
		}
	}

	public bool NotifClubCrates
	{
		get
		{
			return _pushNotifArray[4];
		}
		set
		{
			_pushNotifArray[4] = value;
		}
	}

	public bool NotifEventRewards
	{
		get
		{
			return _pushNotifArray[5];
		}
		set
		{
			_pushNotifArray[5] = value;
		}
	}

	public bool NotifTeamReportCards
	{
		get
		{
			return _pushNotifArray[6];
		}
		set
		{
			_pushNotifArray[6] = value;
		}
	}

	public bool NotifAll
	{
		get
		{
			return _pushNotifArray[7];
		}
		set
		{
			_pushNotifArray[7] = value;
		}
	}

	public UserPreferenceData()
	{
		preferenceKVS = KeyValueStorage.Instance(KeyValueStorage.Storage.PREFERENCES);
		_batterySaverOn = InitKey("batterySaverOn", false);
		_kamcordOn = InitKey("kamcordOn", false);
		_pushNotifArray = new bool[Enum.GetNames(typeof(PUSH_NOTIF_TYPE)).Length];
		Singleton<SessionManager>.instance.GetAllPushNotifEnableStatus();
	}

	private T InitKey<T>(string key, T def)
	{
		if (preferenceKVS.ContainsKey(key))
		{
			return preferenceKVS.GetValue<T>(key);
		}
		return def;
	}
}

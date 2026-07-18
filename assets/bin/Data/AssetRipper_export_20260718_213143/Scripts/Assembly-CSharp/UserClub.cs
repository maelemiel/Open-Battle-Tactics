using System.Collections;
using System.Collections.Generic;
using LitJson0;

public class UserClub
{
	public class UserClubEventData
	{
		public Dictionary<string, int> userPoints;

		public UserClubEventData()
		{
			userPoints = new Dictionary<string, int>();
		}

		public int GetTotal()
		{
			int num = 0;
			foreach (KeyValuePair<string, int> userPoint in userPoints)
			{
				num += userPoint.Value;
			}
			return num;
		}

		public int GetUserPoints(string id)
		{
			if (userPoints != null && userPoints.ContainsKey(id))
			{
				return userPoints[id];
			}
			return 0;
		}
	}

	public string clubID;

	public string leaderID;

	public string name;

	public string description;

	public string password;

	public int minTier;

	public ClubTypes teamType;

	public string teamBadge;

	public List<ClubMember> members;

	private Dictionary<string, UserClubEventData> userClubEvents;

	private ProgressionDivisionDataModel cachedDivision;

	private AssetLinkageDataModel cachedTeamBadge;

	public ProgressionDivisionDataModel Division
	{
		get
		{
			if (cachedDivision == null)
			{
				cachedDivision = ProgressionDivisionDataModel.GetSingle(minTier);
			}
			return cachedDivision;
		}
	}

	public AssetLinkageDataModel MinTierAssetLinkage
	{
		get
		{
			if (Division != null)
			{
				return Division.BadgeLinkage;
			}
			return null;
		}
	}

	public AssetLinkageDataModel TeamBadgeAssetLinkage
	{
		get
		{
			if (cachedTeamBadge == null)
			{
				cachedTeamBadge = AssetLinkageDataModel.GetSingle(teamBadge);
			}
			return cachedTeamBadge;
		}
	}

	public UserClub()
	{
	}

	public UserClub(string name, string description, string password, int minTier, ClubTypes teamType, string teamBadge, string leaderID)
	{
		this.name = name;
		this.description = description;
		this.password = password;
		this.minTier = minTier;
		this.teamType = teamType;
		this.teamBadge = teamBadge;
		this.leaderID = leaderID;
		members = new List<ClubMember>();
	}

	public string GetLocalizatedTeamType()
	{
		string empty = string.Empty;
		if (teamType == ClubTypes.PRIVATE)
		{
			return "ui_clubs_create_club_private".Localize("PRIVATE");
		}
		if (teamType == ClubTypes.PUBLIC)
		{
			return "ui_clubs_create_club_public".Localize("PUBLIC");
		}
		return "NOT DEFINED!";
	}

	public int GetCurrentEventUserClubPoints(string id)
	{
		EventDataModel activeEvent = UserProfile.player.GetActiveEvent();
		if (userClubEvents != null && activeEvent != null && userClubEvents.ContainsKey(activeEvent.id))
		{
			return userClubEvents[activeEvent.id].GetUserPoints(id);
		}
		return 0;
	}

	public int GetTotalEventClubPoints()
	{
		EventDataModel activeEvent = UserProfile.player.GetActiveEvent();
		if (userClubEvents != null && activeEvent != null && userClubEvents.ContainsKey(activeEvent.id))
		{
			return userClubEvents[activeEvent.id].GetTotal();
		}
		return 0;
	}

	public static UserClub FromJSON(JsonObject clubJsonObject, UserClub clubToUpdate = null, bool parseMembers = true)
	{
		UserClub userClub = null;
		List<ClubMember> list = null;
		if (clubJsonObject != null && clubJsonObject.Dictionary != null)
		{
			userClub = ((clubToUpdate == null) ? new UserClub() : clubToUpdate);
			userClub.clubID = clubJsonObject.GetString("_id");
			userClub.leaderID = clubJsonObject.GetString("leader_id");
			userClub.name = clubJsonObject.GetString("name");
			userClub.description = clubJsonObject.GetString("description");
			userClub.teamType = (ClubTypes)clubJsonObject.GetInt("type");
			userClub.minTier = clubJsonObject.GetInt("tier_requirement");
			userClub.teamBadge = clubJsonObject.GetInt("badge_id").ToString();
			userClub.password = clubJsonObject.GetString("password");
			userClub.cachedDivision = null;
			userClub.cachedTeamBadge = null;
			if (parseMembers && clubJsonObject.Contains("members"))
			{
				list = ClubMember.FromJSON(clubJsonObject, userClub);
				if (list != null)
				{
					userClub.members = list;
				}
			}
			if (clubJsonObject.Contains("event_info"))
			{
				userClub.userClubEvents = new Dictionary<string, UserClubEventData>();
				IList objectList = clubJsonObject.GetObject("event_info").GetObjectList("leaderboard");
				if (objectList != null)
				{
					foreach (JsonObject item in objectList)
					{
						if (!item.Contains("leaderboard_id") || !item.Contains("member_points"))
						{
							continue;
						}
						string key = item.GetString("leaderboard_id");
						UserClubEventData userClubEventData = new UserClubEventData();
						JsonObject jsonObject2 = item.GetObject("member_points");
						foreach (KeyValuePair<string, object> item2 in jsonObject2.Dictionary)
						{
							if (!userClubEventData.userPoints.ContainsKey(item2.Key))
							{
								if (item2.Value == null)
								{
									userClubEventData.userPoints.Add(item2.Key, 0);
								}
								else
								{
									userClubEventData.userPoints.Add(item2.Key, jsonObject2.GetInt(item2.Key));
								}
							}
						}
						userClub.userClubEvents.Add(key, userClubEventData);
					}
				}
			}
		}
		return userClub;
	}

	public static List<UserClub> FromJSONList(JsonObject json)
	{
		List<UserClub> list = new List<UserClub>();
		List<JsonObject> objectList = json.GetObjectList("results");
		if (objectList.Count == 0)
		{
			return list;
		}
		UserClub userClub = null;
		foreach (JsonObject item in objectList)
		{
			if (item != null && item.Dictionary != null)
			{
				userClub = new UserClub();
				userClub.clubID = item.GetString("_id");
				userClub.leaderID = item.GetString("leader_id");
				userClub.name = item.GetString("name");
				userClub.description = item.GetString("description");
				userClub.teamType = (ClubTypes)item.GetInt("type");
				userClub.minTier = item.GetInt("tier_requirement");
				userClub.teamBadge = item.GetInt("badge_id").ToString();
				userClub.password = item.GetString("password");
				if (item.Contains("members"))
				{
					userClub.members = ClubMember.FromJSON(item, userClub);
				}
				list.Add(userClub);
			}
		}
		return list;
	}

	public bool UserIsMember(string userID)
	{
		ClubMember clubMember = members.Find((ClubMember x) => x.ID == userID);
		return clubMember != null;
	}

	public bool UserIsLeader(string userID)
	{
		return leaderID == userID;
	}

	public bool IsFull()
	{
		return members.Count >= Constants.ClubsTotalMembers;
	}
}

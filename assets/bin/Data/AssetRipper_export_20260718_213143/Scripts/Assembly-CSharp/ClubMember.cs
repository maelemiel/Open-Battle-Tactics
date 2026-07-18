using System.Collections.Generic;
using LitJson0;

public class ClubMember : IPlayerClubMetadata
{
	private string id;

	private int mobageId;

	private string name;

	private int pvpRating;

	private int tier;

	private string thumbnailURL;

	private ProgressionDivisionDataModel cachedDivision;

	private AssetLinkageDataModel cachedTeamBadge;

	public UserClub Club { get; set; }

	public string ID
	{
		get
		{
			return id;
		}
	}

	public int MobageID
	{
		get
		{
			return mobageId;
		}
	}

	public string Name
	{
		get
		{
			return name;
		}
	}

	public int PVPRating
	{
		get
		{
			return pvpRating;
		}
	}

	public string ThumbnailURL
	{
		get
		{
			return thumbnailURL;
		}
	}

	public ProgressionDivisionDataModel Division
	{
		get
		{
			if (cachedDivision == null)
			{
				cachedDivision = ProgressionDivisionDataModel.GetSingle(tier);
			}
			return cachedDivision;
		}
	}

	public AssetLinkageDataModel TierAssetLinkage
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

	public ClubMember()
	{
	}

	public ClubMember(string id, int mobageId, string name, int pvpRating, int tier, string thumbnailURL)
	{
		this.id = id;
		this.mobageId = mobageId;
		this.name = name;
		this.pvpRating = pvpRating;
		this.tier = tier;
		this.thumbnailURL = thumbnailURL;
	}

	public static List<ClubMember> FromJSON(JsonObject jsonObject, UserClub parentClub)
	{
		List<ClubMember> list = new List<ClubMember>();
		ClubMember clubMember = null;
		if (jsonObject != null && jsonObject.Dictionary != null)
		{
			foreach (JsonObject @object in jsonObject.GetObjectList("members"))
			{
				string text = @object.GetString("member_id");
				JsonObject jsonObject2 = @object.GetObject("user");
				if (jsonObject2 == null)
				{
					clubMember = new ClubMember();
					clubMember.id = text;
					clubMember.Club = parentClub;
					list.Add(clubMember);
					continue;
				}
				string text2 = jsonObject2.GetString("nickname");
				int num = jsonObject2.GetInt("pvp_rating");
				string text3 = string.Empty;
				int num2 = 0;
				if (jsonObject2 != null)
				{
					JsonObject jsonObject3 = jsonObject2.GetObject("mobage");
					if (jsonObject3 != null)
					{
						text3 = jsonObject3.GetString("thumbnail");
						num2 = jsonObject3.GetInt("id");
					}
				}
				int num3 = 0;
				JsonObject jsonObject4 = jsonObject2.GetObject("stats");
				if (jsonObject4 != null)
				{
					JsonObject jsonObject5 = jsonObject4.GetObject("division");
					if (jsonObject5 != null)
					{
						num3 = int.Parse(jsonObject5.GetString("division_id"));
					}
				}
				clubMember = new ClubMember(text, num2, text2, num, num3, text3);
				clubMember.Club = parentClub;
				list.Add(clubMember);
			}
		}
		return list;
	}
}

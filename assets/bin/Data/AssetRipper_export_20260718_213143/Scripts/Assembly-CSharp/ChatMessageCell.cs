using System;
using UnityEngine;

public class ChatMessageCell : ScrollableCell
{
	private string ADM_ASSET_ID = "6501";

	private string BOT_ASSET_ID = "6502";

	[SerializeField]
	private tk2dTextMesh nicknameField;

	[SerializeField]
	private tk2dTextMesh messageField;

	[SerializeField]
	private tk2dTextMesh dateField;

	[SerializeField]
	private tk2dTextMesh pvpRatingField;

	[SerializeField]
	private GameObject background;

	[SerializeField]
	private PrefabProxy badgeProxy;

	public bool useOddColoring;

	public Color REGULAR_MESSAGE_COLOR = Color.white;

	public Color USER_MESSAGE_COLOR = Color.white;

	private int defaultSortOrder;

	public int SortOrder
	{
		get
		{
			return nicknameField.renderer.sortingOrder;
		}
		set
		{
			Renderer obj = nicknameField.renderer;
			int num = value;
			dateField.renderer.sortingOrder = num;
			num = num;
			messageField.renderer.sortingOrder = num;
			obj.sortingOrder = num;
		}
	}

	private void Awake()
	{
		defaultSortOrder = nicknameField.SortingOrder;
	}

	public override void ConfigureCellData()
	{
		SortOrder = defaultSortOrder;
		ChatMessage chatMessage = base.DataObject as ChatMessage;
		if (chatMessage == null)
		{
			return;
		}
		nicknameField.text = chatMessage.nickname;
		pvpRatingField.text = "[" + chatMessage.pvpRating + "]";
		messageField.text = chatMessage.message;
		dateField.text = GetChatTimestampString(chatMessage.createdAt);
		bool flag = chatMessage.userID.Equals(UserProfile.player.id);
		nicknameField.color = ((!flag) ? REGULAR_MESSAGE_COLOR : USER_MESSAGE_COLOR);
		if ((bool)badgeProxy)
		{
			badgeProxy.StopAllCoroutines();
			if (chatMessage.isAdmin)
			{
				StartCoroutine(badgeProxy.ChangeAssetCoroutine(AssetLinkageDataModel.GetSingle(ADM_ASSET_ID)));
			}
			else if (chatMessage.nickname.Equals(Constants.ChatGodUsername))
			{
				StartCoroutine(badgeProxy.ChangeAssetCoroutine(AssetLinkageDataModel.GetSingle(BOT_ASSET_ID)));
			}
			else
			{
				ProgressionDivisionDataModel single = ProgressionDivisionDataModel.GetSingle(chatMessage.tier);
				if (single != null)
				{
					StartCoroutine(badgeProxy.ChangeAssetCoroutine(single.BadgeLinkage));
				}
			}
		}
		if (useOddColoring)
		{
			if (dataIndex % 2 == 0)
			{
				background.SetActive(false);
			}
			else
			{
				background.SetActive(true);
			}
		}
	}

	public string GetChatTimestampString(long time)
	{
		long num = Math.Abs(NonUnitySingleton<TimeManager>.instance.GetTimeDelta(time));
		long num2 = num / 60000 % 60;
		long num3 = num / 3600000 % 24;
		long num4 = num / 86400000;
		string result = string.Format("ui_chat_time_day".Localize("{0} D Ago"), num4);
		string result2 = string.Format("ui_chat_time_hour".Localize("{0} H Ago"), num3);
		string result3 = string.Format("ui_chat_time_min".Localize("{0} M Ago"), num2);
		if (num4 > 0)
		{
			return result;
		}
		if (num3 > 0)
		{
			return result2;
		}
		if (num2 > 0)
		{
			return result3;
		}
		return string.Empty;
	}
}

using System.Collections;
using System.Collections.Generic;
using Holoville.HOTween;
using UnityEngine;

public class ClubCratesPopUpController : PopupController
{
	[SerializeField]
	private GameObject[] platforms;

	[SerializeField]
	private GachaItemController[] gachaItemControllers;

	[SerializeField]
	private tk2dTextMesh[] fromNameLabels;

	[SerializeField]
	private ObjectShaker screenshakeTarget;

	[SerializeField]
	private PrefabProxy teamBadgePrefabProxy;

	[SerializeField]
	private tk2dTextMesh clubNameLabel;

	private List<ClubCrateDataModel> clubCratesData;

	private Dictionary<int, ClubCrateDataModel> availableCratesMapping;

	private int boxesOpened;

	protected override void Awake()
	{
		base.Awake();
		SceneController.resumeCallbackEnable = false;
	}

	protected override void Start()
	{
		base.Start();
		clubCratesData = (List<ClubCrateDataModel>)model.payload;
		for (int i = 0; i < gachaItemControllers.Length; i++)
		{
			gachaItemControllers[i].buttonPressedCallback = ClubCratesTankRequest;
		}
		if (clubCratesData != null)
		{
			UserProfile player = UserProfile.player;
			List<ClubMember> list = new List<ClubMember>(3);
			for (int j = 0; j < player.userClub.members.Count; j++)
			{
				ClubMember clubMember = player.userClub.members[j];
				if (clubMember.ID != player.id)
				{
					list.Add(clubMember);
				}
			}
			for (int k = 0; k < gachaItemControllers.Length; k++)
			{
				gachaItemControllers[k].uiButton.enabled = false;
			}
			availableCratesMapping = new Dictionary<int, ClubCrateDataModel>();
			for (int l = 0; l < platforms.Length; l++)
			{
				if (l < list.Count)
				{
					ClubMember clubMember2 = list[l];
					fromNameLabels[l].text = clubMember2.Name;
					ClubCrateDataModel clubCrateDataModel = clubCratesData.Find((ClubCrateDataModel clubDataEntry) => clubDataEntry.FromUser == clubMember2.ID);
					if (clubCrateDataModel != null)
					{
						availableCratesMapping[l] = clubCrateDataModel;
					}
				}
				else
				{
					fromNameLabels[l].text = string.Empty;
				}
			}
		}
		UserClub userClub = UserProfile.player.userClub;
		if (teamBadgePrefabProxy != null && userClub != null && userClub.TeamBadgeAssetLinkage != null)
		{
			teamBadgePrefabProxy.ChangeAsset(UserProfile.player.userClub.TeamBadgeAssetLinkage);
		}
		if (clubNameLabel != null && userClub != null)
		{
			clubNameLabel.text = userClub.name;
		}
		StartCoroutine(IntroAnimation());
	}

	private IEnumerator IntroAnimation()
	{
		float waitTime = 0.8f;
		float delayTime = 0f;
		for (int i = 0; i < gachaItemControllers.Length; i++)
		{
			if (availableCratesMapping.ContainsKey(i))
			{
				GachaItemController itemController = gachaItemControllers[i];
				Vector3 originalPosition = itemController.transform.localPosition;
				itemController.transform.localPosition = originalPosition + new Vector3(0f, 1000f, 0f);
				HOTween.To(itemController.transform, waitTime, new TweenParms().Prop("localPosition", originalPosition).Delay(delayTime).Ease(EaseType.EaseInExpo)
					.OnComplete(OnBoxLanded));
				delayTime += Random.Range(0.1f, 0.3f);
			}
			else
			{
				gachaItemControllers[i].gameObject.SetActive(false);
			}
		}
		waitTime += delayTime;
		yield return new WaitForSeconds(waitTime + 0.3f);
		for (int j = 0; j < gachaItemControllers.Length; j++)
		{
			if (availableCratesMapping.ContainsKey(j))
			{
				gachaItemControllers[j].uiButton.enabled = true;
			}
		}
		AudioTrigger.CrowdCheering.Play();
	}

	private void OnBoxLanded()
	{
		screenshakeTarget.Shake();
		AudioTrigger.CrateLand.Play();
	}

	public void ClubCratesTankRequest(GachaItemController itemController)
	{
		int num = -1;
		for (int i = 0; i < gachaItemControllers.Length; i++)
		{
			if (gachaItemControllers[i] == itemController)
			{
				num = i;
				break;
			}
		}
		if (num != -1)
		{
			AudioTrigger.CrateBreak.Play();
			AudioTrigger.CrowdExcited.Play();
			ClubCrateDataModel clubCrateDataModel = availableCratesMapping[num];
			itemController.GachaResult(clubCrateDataModel.ItemCollection.items[0]);
			boxesOpened++;
			if (boxesOpened >= clubCratesData.Count)
			{
				StartCoroutine(ShowSummaryPopUpWithDelay(4f));
			}
		}
	}

	private IEnumerator ShowSummaryPopUpWithDelay(float delay)
	{
		if (delay > 0f)
		{
			yield return new WaitForSeconds(delay);
		}
		for (int i = 0; i < platforms.Length; i++)
		{
			platforms[i].SetActive(false);
		}
		ItemCollectionDataModel items = new ItemCollectionDataModel();
		for (int j = 0; j < clubCratesData.Count; j++)
		{
			items.AddItem(clubCratesData[j].ItemCollection.items[0]);
		}
		GachaRewardsSceneModel gachaResultsPayload = new GachaRewardsSceneModel(GachaTypes.NONE, items, null);
		SceneModel sceneModel = new SceneModel(gachaResultsPayload);
		SceneController.resumeCallbackEnable = true;
		PopupManager.ShowPopup(PopupDataModel.GachaResult((GachaRewardsSceneModel)sceneModel.payload, "ui_gacha_result_title_2", OnCloseButton));
	}
}

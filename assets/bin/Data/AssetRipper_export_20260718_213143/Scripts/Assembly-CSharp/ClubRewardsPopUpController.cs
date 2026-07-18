using System.Collections;
using Holoville.HOTween;
using UnityEngine;

public class ClubRewardsPopUpController : PopupController
{
	[SerializeField]
	private PriceLabelController _firstReward;

	[SerializeField]
	private PriceLabelController _secondReward;

	[SerializeField]
	private PriceLabelController _restReward;

	[SerializeField]
	private GameObject _mainReward;

	[SerializeField]
	private GameObject _additionalReward;

	[SerializeField]
	private Announcer _announcer;

	[SerializeField]
	private tk2dTextMesh _addRewardsTitle;

	[SerializeField]
	private GameObject _explosionGO;

	[SerializeField]
	private tk2dSpineAnimation _explosion;

	private LeaderboardRewardsSceneModel _leaderboardRewardsDataModel;

	private GameObject _currentAnnouncer;

	private tk2dSpineAnimation _skeleton;

	protected override void Start()
	{
		base.Start();
		Singleton<AudioCacheManager>.instance.RegisterSingleAudioClip(AudioTrigger.CrowdCheering);
		Singleton<AudioCacheManager>.instance.RegisterSingleAudioClip(AudioTrigger.MiscExplosions);
		_leaderboardRewardsDataModel = (LeaderboardRewardsSceneModel)model.payload;
		if (_leaderboardRewardsDataModel != null)
		{
			UpdateRewardsPopUp();
		}
	}

	public void UpdateRewardsPopUp()
	{
		StartCoroutine(ShowAnnouncer(AnnouncerType.BAMBI_ANNOUNCER));
		if ((bool)_title)
		{
			_title.text = "ui_leaderboard_club_rewards_title".Localize("YOUR REWARDS");
			_addRewardsTitle.text = "ui_leaderboard_club_rewards_text01".Localize("ADDITONAL PRIZES");
		}
		if ((bool)_firstReward)
		{
			if (_leaderboardRewardsDataModel.leaderboardRewards.items.Count <= 0)
			{
				return;
			}
			ItemCollectionDataModel itemCollectionDataModel = new ItemCollectionDataModel();
			itemCollectionDataModel.AddItem(_leaderboardRewardsDataModel.leaderboardRewards.items[0]);
			_firstReward.ConfigurePriceLabel(itemCollectionDataModel);
		}
		if ((bool)_secondReward)
		{
			ItemCollectionDataModel itemCollectionDataModel2 = new ItemCollectionDataModel();
			if (_leaderboardRewardsDataModel.leaderboardRewards.items.Count <= 1)
			{
				return;
			}
			itemCollectionDataModel2.AddItem(_leaderboardRewardsDataModel.leaderboardRewards.items[1]);
			_secondReward.ConfigurePriceLabel(itemCollectionDataModel2);
		}
		if ((bool)_restReward)
		{
			ItemCollectionDataModel itemCollectionDataModel3 = new ItemCollectionDataModel();
			int num = 2;
			int num2 = 0;
			while (num < _leaderboardRewardsDataModel.leaderboardRewards.items.Count && num2 < 6)
			{
				itemCollectionDataModel3.AddItem(_leaderboardRewardsDataModel.leaderboardRewards.items[num]);
				num++;
				num2++;
			}
			if (itemCollectionDataModel3.items.Count > 0)
			{
				_restReward.ConfigurePriceLabel(itemCollectionDataModel3);
			}
		}
	}

	private IEnumerator ShowAnnouncer(AnnouncerType announcerType)
	{
		if (announcerType != AnnouncerType.NONE)
		{
			Vector3 initialPosition = _announcer.transform.localPosition;
			Vector3 localScale = _announcer.transform.localScale;
			AudioTrigger.CrowdCheering.Play();
			_announcer.GetAnnouncer(announcerType, delegate(EffectInstance result)
			{
				_currentAnnouncer = result.gameObject;
			});
			while (_currentAnnouncer == null)
			{
				yield return 0;
			}
			_announcer.MoveAndStay(announcerType, initialPosition, new Vector3(-414f, initialPosition.y, initialPosition.z), localScale, 1f);
			_skeleton = _currentAnnouncer.GetComponent<tk2dSpineAnimation>();
			yield return new WaitForSeconds(1f);
			if ((bool)_skeleton)
			{
				yield return StartCoroutine(_skeleton.PlayAnimCoroutine("Bambi_Celebration"));
			}
			_explosionGO.SetActive(true);
			_explosion.Skeleton.SortOrder = 15;
			AudioTrigger.MiscExplosions.Play();
			_mainReward.transform.TweenLocalScale(1f, 1.5f, EaseType.EaseOutBack);
			yield return StartCoroutine(_explosion.PlayAnimCoroutine("Attack Effect"));
			AudioTrigger.CrowdCheering.Play();
			_additionalReward.transform.TweenLocalXPosition(0f, 1f, EaseType.EaseOutBack);
			if ((bool)_skeleton)
			{
				yield return StartCoroutine(_skeleton.PlayAnimCoroutine("Bambi_OOOOOOOO....1"));
				_skeleton.loop = true;
				_skeleton.animationName = "Bambi_Idle_1_Blink";
			}
		}
	}

	private void ClosePopUpButton()
	{
		PopupManager.DestroyPopup(model);
	}
}

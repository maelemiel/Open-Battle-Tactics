using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiTeamFinalReportItemViewController : MonoBehaviour
{
	[SerializeField]
	private tk2dTextMesh teamLabel;

	[SerializeField]
	private tk2dTextMesh eventPointsValue;

	[SerializeField]
	private tk2dTextMesh bonusPointsValue;

	[SerializeField]
	private tk2dBaseSprite rankingSprite;

	public float animationDuration = 0.5f;

	public AudioTrigger soundToPlay;

	private List<string> rankingSpriteNames = new List<string> { "First_Rank", "Second_Rank", "Third_Rank" };

	public void Init(int teamIndex)
	{
		if ((bool)teamLabel)
		{
			teamLabel.text = string.Format("ui_editteam_team_number".Localize("Team {0}"), teamIndex);
		}
		if ((bool)eventPointsValue)
		{
			eventPointsValue.Alpha = 0f;
		}
		if ((bool)bonusPointsValue)
		{
			bonusPointsValue.Alpha = 0f;
		}
		if ((bool)rankingSprite)
		{
			rankingSprite.Alpha = 0f;
		}
		Singleton<AudioCacheManager>.instance.RegisterSingleAudioClip(soundToPlay);
		Singleton<AudioCacheManager>.instance.RegisterSingleAudioClip(AudioTrigger.SpecialResult);
		Singleton<GlobalEffectsManager>.instance.EnsurePoolCapacity(EffectType.DICE_EFFECT, 2);
	}

	public IEnumerator ShowValues(int eventPoints, int bonusPoints, bool showEffect = true)
	{
		bool noPointsEarned = eventPoints == 0 && bonusPoints == 0;
		Vector3 initialScale = Vector3.one * 2f;
		if ((bool)eventPointsValue)
		{
			ShowValue(eventPointsValue, eventPoints, showEffect, noPointsEarned);
			yield return new WaitForSeconds(animationDuration * 0.5f);
		}
		if ((bool)bonusPointsValue)
		{
			ShowValue(bonusPointsValue, bonusPoints, showEffect, noPointsEarned);
			yield return new WaitForSeconds(animationDuration * 0.5f);
		}
	}

	private void ShowValue(tk2dTextMesh textMesh, int value, bool showEffect, bool noPointsEarned)
	{
		Vector3 initialScale = Vector3.one * 2f;
		textMesh.text = ((!noPointsEarned) ? ("+" + value) : "ui_multiteam_non_applicable".Localize("N/A"));
		SimpleTween.Start(0f, 1f, animationDuration, delegate(float val)
		{
			textMesh.Alpha = val;
			textMesh.scale = initialScale - Vector3.one * val;
			textMesh.Commit();
		});
		if (showEffect)
		{
			EffectInstance effectInstance = GlobalEffectsManager.Create(EffectType.CANNON_SR_FIRE, textMesh.transform.position, base.gameObject);
			effectInstance.SpineAnimation.Skeleton.SortOrder = textMesh.SortingOrder + 1;
			effectInstance.transform.SetLocalXScale(-1f);
			effectInstance.AutoDestroy();
		}
		soundToPlay.Play();
	}

	public IEnumerator ShowRanking(int ranking)
	{
		string rankingSpriteName = string.Empty;
		if (rankingSpriteNames.Count - 1 < ranking)
		{
			ranking = rankingSpriteNames.Count - 1;
		}
		else if (ranking < 0)
		{
			ranking = 0;
		}
		rankingSpriteName = rankingSpriteNames[ranking];
		rankingSprite.SetSprite(rankingSpriteName);
		Vector3 initialScale = Vector3.one * 2f;
		SimpleTween.Start(0f, 1f, animationDuration, delegate(float val)
		{
			rankingSprite.Alpha = val;
			rankingSprite.scale = initialScale - Vector3.one * val;
		});
		AudioTrigger.SpecialResult.Play();
		yield break;
	}
}

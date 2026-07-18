using System.Collections;
using Spine;
using UnityEngine;

public class MiniBossManager : MonoBehaviour
{
	private BattleController battleController;

	[SerializeField]
	private UnitProxy bossProxy1;

	[SerializeField]
	private UnitProxy bossProxy2;

	public void Init(BattleController battleController)
	{
		this.battleController = battleController;
		if (battleController.enemyTeam.type == TeamType.Boss)
		{
		}
	}

	private void Startup()
	{
		battleController.BattleHooks = new RaidBossBattleHooks();
		Singleton<GlobalEffectsManager>.instance.EnsurePoolCapacity(EffectType.BOSS_MOMENT, 1);
	}

	public IEnumerator AnimateMiniBossMoment()
	{
		bool waitForAnim = true;
		battleController.hud.battleText.ShowMessage("battle_mini_boss_warning".Localize("WARNING MINI BOSS!"), Color.yellow, InBattleMessageType.BOSS_MOMENT);
		UnitState bossUnit1 = battleController.enemyTeam.units[0];
		UnitState bossUnit2 = battleController.enemyTeam.units[1];
		bossProxy1.ChangeAsset("Prefab.prefab", bossUnit1.metadata.AssetBundleID);
		bossProxy2.ChangeAsset("Prefab.prefab", bossUnit2.metadata.AssetBundleID);
		AudioTrigger.MiniBossEntrance.Play();
		yield return new WaitForSeconds(2f);
		AudioTrigger.MiniBossMoment.Play();
		EffectInstance effect = null;
		yield return StartCoroutine(GlobalEffectsManager.CreateCoroutine(EffectType.BOSS_MOMENT, base.transform.position, base.gameObject, delegate(EffectInstance x)
		{
			effect = x;
		}));
		effect.SpineAnimation.AnimationName = "Mini Boss Battle Moment";
		Bone bossBone1 = effect.SpineAnimation.Skeleton.skeleton.FindBone("Mini Boss 1");
		Bone bossBone2 = effect.SpineAnimation.Skeleton.skeleton.FindBone("Mini Boss 2");
		bossProxy1.transform.parent = effect.transform;
		bossProxy2.transform.parent = effect.transform;
		if ((bool)effect.SpineAnimation)
		{
			effect.SpineAnimation.AnimationComplete += delegate
			{
				waitForAnim = false;
			};
		}
		battleController.hud.battleText.ShowMessage(bossUnit1.UserUnitMetadata.Name, Color.white, InBattleMessageType.NONE, 250f);
		while (waitForAnim)
		{
			bossProxy1.transform.localPosition = new Vector3(bossBone1.X, bossBone1.Y, 0f);
			bossProxy2.transform.localPosition = new Vector3(bossBone2.X, bossBone2.Y, 0f);
			yield return new WaitForEndOfFrame();
		}
		bossProxy1.gameObject.SetActive(false);
		bossProxy2.gameObject.SetActive(false);
		AudioTrigger.BattleBackground_Music.PlayMusic();
	}
}

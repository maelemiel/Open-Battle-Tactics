using System.Collections;
using Holoville.HOTween;
using UnityEngine;

public class SelfDestructAnimationHandler : AbilityAnimationHandler
{
	private UnitView kamikaze;

	private float dirScaler;

	private TeamState targetTeam;

	protected override void LoadDependencies()
	{
		Singleton<GlobalEffectsManager>.instance.EnsurePoolCapacity(EffectType.DEBRIS, 3);
		Singleton<GlobalEffectsManager>.instance.EnsurePoolCapacity(EffectType.DEBRIS_BOUNCE, 3);
	}

	public override IEnumerator ActivationAnimation(UnitState target)
	{
		if (abilityState.team == battleController.playerTeam)
		{
			battleController.CubeBar.GoToTextState("ui_battle_selfdestructactivated".Localize("Self-Destruct Initialized!"));
			yield return new WaitForSeconds(2f);
			battleController.CubeBar.GoToMainState();
		}
	}

	public void SetKamikazeToEnemySide()
	{
		kamikaze.gameObject.transform.parent = targetTeam.battleField.gameObject.transform;
		kamikaze.gameObject.SetLayerRecursively(kamikaze.gameObject.transform.parent.gameObject.layer);
		kamikaze.gameObject.transform.localPosition = new Vector3(0f, 50f, kamikaze.gameObject.transform.localPosition.z);
		kamikaze.PossibleRollsSimple.SetVisible(false);
		kamikaze.HealthUI.SetVisible(false);
	}

	public override IEnumerator TeamBeginAttackAnimation(TeamState team)
	{
		BattleController bc = battleController;
		TeamState activationTeam = abilityState.team;
		targetTeam = activationTeam.otherTeam;
		dirScaler = ((!targetTeam.IsEnemy) ? (-1f) : 1f);
		kamikaze = abilityState.target.unitView;
		bc.CubeBar.UpdateTextState("ui_battle_selfdestructused".Localize("For king and Country!"));
		HOTween.To(kamikaze.transform, 0.75f, new TweenParms().Prop("localPosition", new Vector3(kamikaze.transform.localPosition.x + 650f * dirScaler, kamikaze.transform.localPosition.y, kamikaze.transform.localPosition.z)).Ease(EaseType.EaseInExpo));
		yield return new WaitForSeconds(0.75f);
		GameObject parentBattlefield = targetTeam.battleField.gameObject;
		int directionSign = ((abilityState.team == battleController.playerTeam) ? 1 : (-1));
		CreateDebrisEffect(bc, parentBattlefield, EffectType.DEBRIS, new Vector3(-200f, -212f, 0f), new Vector3(0f, 0f, 300f), "Debris Trail 1", directionSign, 1.1f);
		CreateDebrisEffect(bc, parentBattlefield, EffectType.DEBRIS, new Vector3(45f, -212f, 0f), new Vector3(0f, 0f, 340f), "Debris Trail 2", directionSign, 1f);
		CreateDebrisEffect(bc, parentBattlefield, EffectType.DEBRIS, new Vector3(70f, -175f, 0f), new Vector3(0f, 0f, 10f), "Debris Trail 3", directionSign, 0.9f);
		CreateDebrisEffect(bc, parentBattlefield, EffectType.DEBRIS_BOUNCE, new Vector3(-320f, -200f, 0f), new Vector3(0f, 0f, 345f), "Debris Bounce Trail 1", directionSign, 1f);
		CreateDebrisEffect(bc, parentBattlefield, EffectType.DEBRIS_BOUNCE, new Vector3(55f, 0f, 0f), new Vector3(0f, 0f, 5f), "Debris Bounce Trail 2", directionSign, 0.6f);
		CreateDebrisEffect(bc, parentBattlefield, EffectType.DEBRIS_BOUNCE, new Vector3(226f, 55f, 0f), new Vector3(0f, 0f, 20f), "Debris Bounce Trail 3", directionSign, 0.5f);
		kamikaze.TakeDamage(999);
		targetTeam.battleField.shaker.Shake(10f, 0f);
		yield return new WaitForSeconds(0.5f);
		foreach (UnitView unit in bc.GetUnitsByTeam(targetTeam))
		{
			unit.TakeDamage(GetNextDamage());
		}
		yield return new WaitForSeconds(1f);
		bc.CubeBar.UpdateTextState(string.Empty);
	}

	private void CreateDebrisEffect(BattleController bc, GameObject parentBattlefield, EffectType effectName, Vector3 positionOffset, Vector3 rotation, string name, int directionSign, float scaleMultiplier)
	{
		Vector3 b = new Vector3(directionSign, 1f, 1f);
		Vector3 vector = Vector3.Scale(positionOffset, b);
		if (directionSign == -1)
		{
			vector += new Vector3(460f, 0f, 0f);
			rotation = new Vector3(360f, 0f, 0f) - rotation;
		}
		Quaternion localRotation = Quaternion.Euler(rotation);
		EffectInstance effectInstance = GlobalEffectsManager.Create(effectName, parentBattlefield.transform.position + vector, parentBattlefield).AutoDestroy();
		effectInstance.gameObject.name = name;
		Transform transform = effectInstance.transform;
		transform.localRotation = localRotation;
		transform.localScale *= scaleMultiplier;
		transform.localScale = Vector3.Scale(transform.localScale, b);
	}
}

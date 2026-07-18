using Holoville.HOTween;
using Holoville.HOTween.Core;
using UnityEngine;

public class AnimationHandlerUtil
{
	private static Vector3[] planeOffsets = new Vector3[3]
	{
		new Vector3(-0.25f, -0.1f, 0f),
		new Vector3(0.25f, 0.1f, 0f),
		new Vector3(-0.25f, 0.4f, 0f)
	};

	public static void CreateDefaultPlaneFlyover(TeamState forTeam)
	{
		AudioTrigger.PlaneBy.Play();
		CreatePlaneFlyover(forTeam, planeOffsets[0]);
		CreatePlaneFlyover(forTeam, planeOffsets[1]);
		CreatePlaneFlyover(forTeam, planeOffsets[2]);
	}

	public static void CreatePlaneFlyover(TeamState forTeam, Vector3 viewportOffset)
	{
		Camera unityCamera = forTeam.battleField.unityCamera;
		BattleField battleField = forTeam.battleField;
		float dirScaler = battleField.directionScaler;
		Vector3 position = new Vector3(0.5f + dirScaler * 1.2f, 1.2f, 0f);
		position += viewportOffset;
		position = unityCamera.ViewportToWorldPoint(position);
		Vector3 toViewportPos = new Vector3(0.5f + dirScaler * -1.2f, -0.19999999f, 0f);
		toViewportPos += viewportOffset;
		toViewportPos += new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f));
		toViewportPos = unityCamera.ViewportToWorldPoint(toViewportPos);
		EffectInstance planeEffect = null;
		battleField.StartCoroutine(GlobalEffectsManager.CreateCoroutine(EffectType.PLANE_FLYOVER, position, battleField.gameObject, delegate(EffectInstance effectInstance)
		{
			planeEffect = effectInstance;
			tk2dBaseSprite component = planeEffect.GetComponent<tk2dBaseSprite>();
			if ((bool)component)
			{
				Vector3 scale = component.scale;
				component.scale = new Vector3(Mathf.Abs(scale.x) * dirScaler, scale.y, scale.z);
			}
			HOTween.To(planeEffect.transform, 2.5f, new TweenParms().Prop("position", toViewportPos).Ease(EaseType.Linear).OnComplete((TweenDelegate.TweenCallback)delegate
			{
				planeEffect.Destroy();
			}));
		}));
	}
}

using System.Collections;
using UnityEngine;

public class EventLogoController : MonoBehaviour
{
	public bool autoLoad;

	public bool checkUserInClub;

	public bool IsAnimating;

	[SerializeField]
	private PrefabProxy logoPrefabProxy;

	[SerializeField]
	private float eventLogoVerticalPosition = 25f;

	private void Start()
	{
		if (autoLoad)
		{
			LoadLogo();
		}
	}

	public IEnumerator InitBattleEventLogo(BattleController battleController)
	{
		BattleSceneModel sceneModel = battleController.SceneModel;
		if (sceneModel.activeEvent != null)
		{
			IsAnimating = true;
			yield return StartCoroutine(LoadLogoCoroutine(sceneModel.activeEvent));
			float initialVertical = base.gameObject.transform.localPosition.y;
			base.gameObject.transform.TweenLocalYPosition(eventLogoVerticalPosition, 1f);
			yield return new WaitForSeconds(3.75f);
			base.gameObject.transform.TweenLocalYPosition(initialVertical, 1f);
			yield return new WaitForSeconds(1f);
			IsAnimating = false;
		}
	}

	public IEnumerator LoadLogoCoroutine(EventDataModel eventDataModel)
	{
		if (eventDataModel != null && (bool)logoPrefabProxy && eventDataModel.LogoAssetLinkage != null)
		{
			yield return StartCoroutine(logoPrefabProxy.ChangeAssetCoroutine(eventDataModel.LogoAssetLinkage));
		}
	}

	public void LoadLogo()
	{
		EventDataModel activeOnCooldownEvent = UserProfile.player.GetActiveOnCooldownEvent(checkUserInClub);
		StartCoroutine(LoadLogoCoroutine(activeOnCooldownEvent));
	}
}

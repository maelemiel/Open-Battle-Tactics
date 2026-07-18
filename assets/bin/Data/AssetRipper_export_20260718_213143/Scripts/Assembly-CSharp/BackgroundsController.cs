using System.Collections;
using UnityEngine;

public class BackgroundsController : MonoBehaviour
{
	[SerializeField]
	private tk2dBaseSprite playerBackground;

	[SerializeField]
	private tk2dBaseSprite enemyBackground;

	[Range(0f, 1f)]
	[SerializeField]
	private float eventBackgroundChance = 0.3f;

	public BattleBackgroundModel[] battleBackgroundConfigurations;

	public EventBackgroundController[] eventBackgroundControllers;

	public IEnumerator InitBackgrounds(BattleController battleController)
	{
		BaseMatchHandler MatchHandler = battleController.MatchHandler;
		BattleSceneModel SceneModel = battleController.SceneModel;
		float randomChanceToEventBackground = Random.Range(0f, 1f);
		if (MatchHandler.IsEventMatch && randomChanceToEventBackground <= eventBackgroundChance && SceneModel.activeEvent.BackgroundAssetLinkage != null && UserProfile.player.divisionInt >= Constants.MinTierEventContent)
		{
			yield return StartCoroutine(InitEventBackground(SceneModel.activeEvent));
		}
		else if (SceneModel.matchType == MatchData.Type.TUTORIAL)
		{
			SetBackgroundsWithIndex(1);
		}
		else
		{
			SetBackgroundsRandom();
		}
	}

	public IEnumerator InitEventBackground(EventDataModel activeEvent)
	{
		for (int i = 0; i < eventBackgroundControllers.Length; i++)
		{
			if ((bool)eventBackgroundControllers[i])
			{
				yield return StartCoroutine(eventBackgroundControllers[i].LoadBackgroundCoroutine(activeEvent));
			}
		}
	}

	public void SetBackgroundsRandom()
	{
		int num = battleBackgroundConfigurations.Length;
		if (num > 0)
		{
			int index = Random.Range(0, num);
			_SetBackgroundsWithIndex(index);
		}
	}

	public void SetBackgroundsWithIndex(int index)
	{
		if (battleBackgroundConfigurations.Length > 0)
		{
			_SetBackgroundsWithIndex(index);
		}
	}

	private void _SetBackgroundsWithIndex(int index)
	{
		index = Mathf.Clamp(index, 0, battleBackgroundConfigurations.Length - 1);
		BattleBackgroundModel battleBackgroundModel = battleBackgroundConfigurations[index];
		tk2dSpriteCollectionData newCollection = Resources.Load<tk2dSpriteCollectionData>(battleBackgroundModel.playerBackgroundSpriteCollectionName);
		playerBackground.SetSprite(newCollection, battleBackgroundModel.playerBackgroundName);
		newCollection = Resources.Load<tk2dSpriteCollectionData>(battleBackgroundModel.enemyBackgroundSpriteCollectionName);
		enemyBackground.SetSprite(newCollection, battleBackgroundModel.enemyBackgroundName);
		Resources.UnloadUnusedAssets();
	}
}

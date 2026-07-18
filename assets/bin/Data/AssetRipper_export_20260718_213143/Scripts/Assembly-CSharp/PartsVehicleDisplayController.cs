using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartsVehicleDisplayController : MonoBehaviour
{
	public class QueueContainer
	{
		public int tankId;

		public UnitPartTypesDataModel partId;

		public int amount;

		public QueueContainer(int tankId, UnitPartTypesDataModel partId, int amount)
		{
			this.tankId = tankId;
			this.partId = partId;
			this.amount = amount;
		}
	}

	public List<QueueContainer> partDisplayQueue = new List<QueueContainer>();

	public bool finishedAnimating = true;

	[SerializeField]
	private tk2dTextMesh unitName;

	[SerializeField]
	private UnitInfoView unitView;

	[SerializeField]
	private Transform contentContainer;

	[SerializeField]
	private PriceLabelController priceLabel;

	[SerializeField]
	private PrefabProxy movingItem;

	[SerializeField]
	private Transform offScreenRight;

	[SerializeField]
	private Transform offScreenLeft;

	[SerializeField]
	private GameObject animationContainer;

	[SerializeField]
	private tk2dSpineAnimation[] fireworkAnimations;

	[SerializeField]
	private ObjectShaker shaker;

	private bool animRunning;

	private bool waitOnTap;

	private bool loopStarted;

	private Dictionary<string, int> alreadyCollectedParts = new Dictionary<string, int>();

	private Dictionary<string, int> grantedParts = new Dictionary<string, int>();

	public IEnumerator DisplayNewParts(int tankId, UnitPartTypesDataModel part, int amount)
	{
		animRunning = true;
		if (!loopStarted)
		{
			animationContainer.transform.position = offScreenRight.position;
		}
		contentContainer.position = offScreenLeft.position;
		contentContainer.gameObject.SetActive(true);
		movingItem.transform.localPosition = Vector3.down * 500f;
		int grantedPartsCount = (grantedParts.ContainsKey(part.id) ? grantedParts[part.id] : 0);
		int alreadyCollectedCount = (alreadyCollectedParts.ContainsKey(part.id) ? alreadyCollectedParts[part.id] : 0);
		int currentAmount = UserProfile.player.inventory.GetItem(UserInventory.ItemType.Parts, int.Parse(part.id));
		currentAmount -= grantedPartsCount;
		currentAmount += alreadyCollectedCount;
		if (alreadyCollectedParts.ContainsKey(part.id))
		{
			Dictionary<string, int> dictionary2;
			Dictionary<string, int> dictionary = (dictionary2 = alreadyCollectedParts);
			string id;
			string key = (id = part.id);
			int num = dictionary2[id];
			dictionary[key] = num + amount;
		}
		else
		{
			alreadyCollectedParts[part.id] = amount;
		}
		yield return new WaitForSeconds(0.5f);
		animationContainer.SetActive(true);
		movingItem.gameObject.SetActive(true);
		UnitDataModel unit = UnitDataModel.GetSingle(tankId);
		unitName.text = unit.name;
		unitView.ConfigureUnitView(unit);
		priceLabel.ConfigurePriceLabel(unit.GetResearchCost(UserProfile.player));
		StartCoroutine(movingItem.ChangeAssetCoroutine(part.AssetLinkage));
		PartsLabelItemView priceItemView = (PartsLabelItemView)priceLabel.GetPriceItem(UserInventory.ItemType.Parts, part.id);
		ItemCollectionDataModel.Item priceItem = priceItemView.item;
		priceItemView.priceLabel.text = currentAmount + "/" + priceItem.amount;
		Vector3 startPosition;
		if (!loopStarted)
		{
			loopStarted = true;
			AudioTrigger.ScreenTransition.Play();
			startPosition = animationContainer.transform.localPosition;
			SimpleTween.Start(0f, 1f, 0.5f, delegate(float current)
			{
				animationContainer.transform.localPosition = Vector3.Lerp(startPosition, Vector3.zero, current);
			});
			yield return new WaitForSeconds(0.5f);
		}
		else
		{
			animationContainer.transform.localPosition = Vector3.zero;
		}
		startPosition = contentContainer.localPosition;
		AudioTrigger.RevealTank.Play();
		SimpleTween.Start(0f, 1f, 0.5f, delegate(float current)
		{
			if ((bool)contentContainer)
			{
				contentContainer.localPosition = Vector3.Lerp(startPosition, Vector3.zero, current);
			}
		});
		yield return new WaitForSeconds(0.5f);
		Vector3 targetPosition = priceItemView.prefabProxy.transform.position;
		yield return new WaitForSeconds(0.25f);
		for (int i = 0; i < amount; i++)
		{
			movingItem.gameObject.SetActive(true);
			SimpleTween.Start(0f, 1f, 0.25f, delegate(float current)
			{
				movingItem.transform.position = Vector3.Lerp(Vector3.down * 500f, targetPosition, current);
			});
			SimpleTween.Start(0f, 1f, 0.25f, delegate(float current)
			{
				movingItem.transform.localScale = Vector3.Lerp(Vector3.one * 3f, Vector3.one, current);
			});
			yield return new WaitForSeconds(0.25f);
			AudioTrigger.SpecialResult.Play();
			movingItem.gameObject.SetActive(false);
			StartCoroutine(ShowFireworksAnimation());
			priceItemView.priceLabel.text = currentAmount + i + 1 + "/" + priceItem.amount;
			ShakePriceItems();
			shaker.Shake();
			yield return StartCoroutine(BounceAnim(priceItemView.prefabProxy.transform, 0.25f));
		}
		AudioTrigger.CrowdCheering.Play();
		yield return new WaitForSeconds(1.25f);
		startPosition = contentContainer.position;
		AudioTrigger.EngineRev.Play();
		SimpleTween.Start(0f, 1f, 0.5f, delegate(float current)
		{
			contentContainer.position = Vector3.Lerp(startPosition, offScreenRight.position, current);
		});
		yield return new WaitForSeconds(0.5f);
		contentContainer.gameObject.SetActive(false);
		if (partDisplayQueue.Count == 0)
		{
			startPosition = animationContainer.transform.position;
			SimpleTween.Start(0f, 1f, 0.5f, delegate(float current)
			{
				animationContainer.transform.position = Vector3.Lerp(startPosition, offScreenLeft.position, current);
			});
			AudioTrigger.ScreenTransition.Play();
			yield return new WaitForSeconds(0.5f);
			animationContainer.SetActive(false);
		}
		animRunning = false;
	}

	private void ShakePriceItems()
	{
		for (int i = 0; i < priceLabel.ItemsCount; i++)
		{
			StartCoroutine(ShakeObject(priceLabel.GetPriceItem(i).transform, 1.5f, 0.1f));
		}
	}

	private IEnumerator ShakeObject(Transform target, float intensity, float decay)
	{
		Vector3 origin = target.localPosition;
		while (intensity > 0f)
		{
			target.localPosition = origin + new Vector3(UnityEngine.Random.Range(0f - intensity, intensity), UnityEngine.Random.Range(0f - intensity, intensity), 0f);
			intensity -= decay;
			yield return new WaitForEndOfFrame();
		}
		target.localPosition = origin;
	}

	private IEnumerator BounceAnim(Transform target, float time)
	{
		for (float currentTime = 0f; currentTime < time; currentTime += Time.deltaTime)
		{
			target.transform.localScale = Vector3.one + Vector3.one * 0.75f * Mathf.Sin(currentTime / time * (float)Math.PI);
			yield return new WaitForEndOfFrame();
		}
		target.localScale = Vector3.one;
	}

	public void AddItemToQueue(int unitId, UnitPartTypesDataModel part, int amount)
	{
		if (grantedParts.ContainsKey(part.id))
		{
			Dictionary<string, int> dictionary2;
			Dictionary<string, int> dictionary = (dictionary2 = grantedParts);
			string id;
			string key = (id = part.id);
			int num = dictionary2[id];
			dictionary[key] = num + amount;
		}
		else
		{
			grantedParts[part.id] = amount;
		}
		finishedAnimating = false;
		partDisplayQueue.Add(new QueueContainer(unitId, part, amount));
	}

	private void Update()
	{
		if (!animRunning)
		{
			if (partDisplayQueue.Count > 0)
			{
				animRunning = true;
				StartCoroutine(DisplayNewParts(partDisplayQueue[0].tankId, partDisplayQueue[0].partId, partDisplayQueue[0].amount));
				partDisplayQueue.RemoveAt(0);
			}
			else
			{
				finishedAnimating = true;
			}
		}
		else if ((Input.touchCount > 0 || Input.GetMouseButton(0)) && !waitOnTap)
		{
			waitOnTap = true;
			StopAllCoroutines();
			animationContainer.SetActive(false);
			movingItem.gameObject.SetActive(false);
			animRunning = false;
		}
		else if (Input.touchCount == 0 && !Input.GetMouseButton(0))
		{
			waitOnTap = false;
		}
	}

	private IEnumerator ShowFireworksAnimation()
	{
		AudioTrigger.Fireworks.Play();
		for (int i = 0; i < fireworkAnimations.Length; i++)
		{
			ActivateAndAutoDeactivateAnimation(fireworkAnimations[i]);
			yield return new WaitForSeconds(0.5f);
		}
	}

	private void ActivateAndAutoDeactivateAnimation(tk2dSpineAnimation spineAnimation)
	{
		spineAnimation.AnimationComplete += ResetAnimation;
		spineAnimation.gameObject.SetActive(true);
	}

	private void ResetAnimation(tk2dSpineAnimation spineAnimation)
	{
		if ((bool)spineAnimation)
		{
			spineAnimation.AnimationComplete -= ResetAnimation;
			spineAnimation.Reset();
			spineAnimation.gameObject.SetActive(false);
		}
	}
}

using System;
using System.Collections;
using Holoville.HOTween;
using UnityEngine;

public class CurrencyEffect : MonoBehaviour
{
	private const float ANIMATE_CURRENCY_ICON_TIME = 1f;

	private int iconCount;

	private UserInventory.ItemType itemType;

	private int totalAmount;

	public int stackSizeOverride = -1;

	[SerializeField]
	private tk2dSprite[] currencyIcons;

	[SerializeField]
	private float burstRadius = 5f;

	[SerializeField]
	private tk2dTextMesh currencyLabel;

	private Vector3[] initialPositions;

	private bool showText = true;

	public int SortingOrder = 10;

	public bool animating;

	public Vector3 EndPoint { get; set; }

	public bool ShowText
	{
		get
		{
			return showText;
		}
		set
		{
			showText = value;
		}
	}

	private void Awake()
	{
		initialPositions = new Vector3[currencyIcons.Length];
		for (int i = 0; i < currencyIcons.Length; i++)
		{
			initialPositions[i] = currencyIcons[i].transform.localPosition;
			currencyIcons[i].SortingOrder = SortingOrder;
		}
		EndPoint = new Vector3(2000f, 400f, 0f);
	}

	private void Start()
	{
		for (int i = 0; i < currencyIcons.Length; i++)
		{
			currencyIcons[i].SortingOrder = SortingOrder;
		}
	}

	[ContextMenu("PlayAnimation")]
	public void PlayAnimation()
	{
		StartCoroutine(PlayAnimationInternal());
	}

	private IEnumerator PlayAnimationInternal()
	{
		animating = true;
		AnimateText();
		for (int i = 0; i < iconCount; i++)
		{
			StartCoroutine(AnimateIconBurst(i, 2f));
		}
		yield return new WaitForSeconds(1f);
		for (int j = 0; j < iconCount; j++)
		{
			AnimateCashIcon(j, 1f);
			yield return new WaitForSeconds(0.1f);
		}
		animating = false;
	}

	private void AnimateText()
	{
		if ((bool)currencyLabel)
		{
			if (showText)
			{
				currencyLabel.gameObject.SetActive(true);
				currencyLabel.text = totalAmount + " " + itemType.GetLocalizedName() + "!";
				currencyLabel.Alpha = 0f;
				currencyLabel.TweenAlpha(1f, 1f);
				currencyLabel.transform.TweenLocalPosition(new Vector3(0f, (0f - burstRadius) * 0.75f, 0f), 1f);
			}
			else
			{
				currencyLabel.gameObject.SetActive(false);
			}
		}
	}

	private IEnumerator AnimateIconBurst(int iconIndex, float animationTime)
	{
		tk2dBaseSprite icon = currencyIcons[iconIndex];
		int degrees = UnityEngine.Random.Range(0, 180);
		float cos = Mathf.Cos((float)degrees * ((float)Math.PI / 180f));
		float sin = Mathf.Sin((float)degrees * ((float)Math.PI / 180f));
		Vector2 rightVector = Vector2.right;
		Vector2 finalPosition = new Vector2
		{
			x = rightVector.x * cos - rightVector.y * sin,
			y = rightVector.x * sin + rightVector.y * cos
		};
		finalPosition *= UnityEngine.Random.Range(0f, burstRadius);
		Extensions.TweenLocalPosition(newLocalPosition: new Vector3(finalPosition.x, finalPosition.y, base.gameObject.transform.position.z), transform: icon.transform, duration: animationTime, easeType: EaseType.EaseOutBack);
		yield return new WaitForSeconds(animationTime);
	}

	private void AnimateCashIcon(int iconIndex, float animationTime)
	{
		tk2dSprite icon = currencyIcons[iconIndex];
		icon.transform.TweenPosition(EndPoint, animationTime, EaseType.EaseOutBack, delegate
		{
			icon.gameObject.SetActive(false);
			if ((bool)currencyLabel)
			{
				currencyLabel.transform.localPosition = Vector3.zero;
				currencyLabel.gameObject.SetActive(false);
			}
			if (iconIndex == iconCount - 1)
			{
				GlobalEffectsManager.Return(base.gameObject);
			}
		});
		if (itemType == UserInventory.ItemType.Coins)
		{
			AudioTrigger.CoinsEarned.Play();
		}
		else if (itemType == UserInventory.ItemType.PremiumCurrency)
		{
			AudioTrigger.PremiumCurrencyEarned.Play();
		}
		else if (itemType == UserInventory.ItemType.Parts)
		{
			AudioTrigger.PartEarned.Play();
		}
	}

	public void ConfigureEffect(UserInventory.ItemType itemType, int amount)
	{
		this.itemType = itemType;
		totalAmount = amount;
		int stackSize = itemType.GetStackSize();
		if (stackSizeOverride != -1)
		{
			stackSize = stackSizeOverride;
		}
		iconCount = Math.Min(currencyIcons.Length, Math.Max(1, amount / stackSize));
		for (int i = 0; i < iconCount; i++)
		{
			currencyIcons[i].gameObject.SetActive(true);
		}
		for (int j = iconCount; j < currencyIcons.Length; j++)
		{
			currencyIcons[j].gameObject.SetActive(false);
		}
		for (int k = 0; k < currencyIcons.Length; k++)
		{
			currencyIcons[k].SetSprite(itemType.GetIconName());
			currencyIcons[k].transform.localPosition = Vector3.zero;
			if (itemType == UserInventory.ItemType.VictoryPoint)
			{
				currencyIcons[k].transform.localScale = Vector3.one * 0.5f;
			}
		}
		StartCoroutine(PlayAnimationInternal());
	}

	public static CurrencyEffect Create(UserInventory.ItemType itemType, int itemAmount, int stackSizeOverride = -1)
	{
		Vector3 startPosition = TopBarController.instance.TopBarCamera.ScreenCamera.ScreenToWorldPoint(tk2dUIManager.Instance.lastTouchPos);
		Vector3 topBarPosition = itemType.GetTopBarPosition();
		return Create(itemType, itemAmount, stackSizeOverride, startPosition, topBarPosition, TopBarController.instance.gameObject);
	}

	public static CurrencyEffect Create(UserInventory.ItemType itemType, int itemAmount, Vector3 startPos)
	{
		Vector3 topBarPosition = itemType.GetTopBarPosition();
		return Create(itemType, itemAmount, -1, startPos, topBarPosition, TopBarController.instance.gameObject);
	}

	public static CurrencyEffect Create(UserInventory.ItemType itemType, int itemAmount, int stackSizeOverride, Vector3 startPosition, Vector3 endPosition, GameObject parentObject)
	{
		if (itemAmount == 0)
		{
			return null;
		}
		CurrencyEffect component = GlobalEffectsManager.Create(EffectType.CASH, startPosition).GetComponent<CurrencyEffect>();
		if (parentObject != null)
		{
			component.gameObject.SetLayerRecursively(parentObject.layer);
		}
		component.EndPoint = endPosition;
		component.stackSizeOverride = stackSizeOverride;
		component.ConfigureEffect(itemType, itemAmount);
		return component;
	}
}

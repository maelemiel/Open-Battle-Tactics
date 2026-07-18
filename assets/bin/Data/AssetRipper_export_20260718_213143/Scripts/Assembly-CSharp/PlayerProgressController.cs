using System.Collections;
using UnityEngine;

public class PlayerProgressController : MonoBehaviour
{
	[SerializeField]
	private PrefabProxy badgeProxy;

	[SerializeField]
	private ChartBarView divisionStars;

	[SerializeField]
	private tk2dUIProgressBar progressBar;

	[SerializeField]
	private Transform bannerContainer;

	[SerializeField]
	private tk2dTextMesh pvpRatingTextMesh;

	public float badgeStarsSize = 0.09f;

	[SerializeField]
	private float openY;

	[SerializeField]
	private float closedY;

	public float timeToOpen = 0.5f;

	private string starSpriteSuffix = "_Star";

	private UserProfile userProfile;

	private tk2dSpriteCollectionData spriteCollection;

	private string starSpriteName;

	private bool _isOpen = true;

	public bool IsOpen
	{
		get
		{
			return _isOpen;
		}
		set
		{
			SetOpen(value);
			_isOpen = value;
		}
	}

	public void SetOpen(bool isOpen)
	{
		StopAllCoroutines();
		StartCoroutine(OpenCloseSequence(isOpen));
	}

	private IEnumerator OpenCloseSequence(bool isOpen)
	{
		Extensions.TweenLocalYPosition(newLocalYPosition: (!isOpen) ? closedY : openY, transform: bannerContainer, duration: timeToOpen);
		yield break;
	}

	public void Toggle()
	{
		IsOpen = !IsOpen;
	}

	public void SetProgress(UserProfile userProfile)
	{
		if (userProfile != null)
		{
			this.userProfile = userProfile;
			float progress = (float)userProfile.points / (float)userProfile.CurrentDivision.totalPointToPromotionSeries;
			SetProgress(progress);
			int num = int.Parse(userProfile.CurrentDivision.id);
			SetDivisionStars((num - 1) % 5);
			SetPVPRating(userProfile.pvpRating);
			if ((bool)badgeProxy)
			{
				StartCoroutine(SetBadge());
			}
		}
	}

	public IEnumerator SetBadge()
	{
		yield return StartCoroutine(badgeProxy.ChangeAssetCoroutine(userProfile.CurrentDivision.BadgeLinkage));
		yield return StartCoroutine(badgeProxy.WaitForAssetReady());
		tk2dSprite badgeSprite = badgeProxy.Prefab.GetComponent<tk2dSprite>();
		spriteCollection = badgeSprite.Collection;
		starSpriteName = badgeSprite.CurrentSprite.name + starSpriteSuffix;
		if (spriteCollection != null && !string.IsNullOrEmpty(starSpriteName))
		{
			tk2dSprite[] starSprites = divisionStars.GetComponentsInChildren<tk2dSprite>();
			for (int i = 0; i < starSprites.Length; i++)
			{
				starSprites[i].SetSprite(spriteCollection, starSpriteName);
				starSprites[i].scale = Vector3.one * badgeStarsSize;
			}
		}
	}

	private void SetDivisionStars(int divisionIndex)
	{
		if ((bool)divisionStars)
		{
			divisionStars.SetBarLevel(divisionIndex);
		}
	}

	private void SetProgress(float progressBarValue)
	{
		progressBarValue = Mathf.Clamp01(progressBarValue);
		if ((bool)progressBar)
		{
			progressBar.Value = progressBarValue;
		}
	}

	private void SetPVPRating(int pvpValue)
	{
		if ((bool)pvpRatingTextMesh)
		{
			pvpRatingTextMesh.text = pvpValue.ToString();
		}
	}
}

using System.Collections;
using Holoville.HOTween;
using UnityEngine;

public class DamageScoreboard : MonoBehaviour
{
	[SerializeField]
	private tk2dTextMesh damageText;

	[SerializeField]
	private tk2dTextMesh nextRewardText;

	[SerializeField]
	private Transform targetPosition;

	[SerializeField]
	private float countUpSpeed;

	private Vector3 originalPosition;

	private bool isShowing;

	private float currentDamageValue;

	private float goalDamageValue;

	private Tweener damageTween;

	public bool IsShowing
	{
		get
		{
			return isShowing;
		}
	}

	private void Awake()
	{
		originalPosition = base.transform.position;
		currentDamageValue = 0f;
		goalDamageValue = 0f;
		damageText.text = "0";
		StartCoroutine(UpdateCurrentDamage());
	}

	public void SetValue(int newValue)
	{
		goalDamageValue = newValue;
	}

	public void SetNextRewardValue(int newValue)
	{
		nextRewardText.text = newValue.ToString();
	}

	private IEnumerator UpdateCurrentDamage()
	{
		while (true)
		{
			if (currentDamageValue != goalDamageValue)
			{
				currentDamageValue = Mathf.Min(currentDamageValue + countUpSpeed * Time.deltaTime, goalDamageValue);
				damageText.text = Mathf.FloorToInt(currentDamageValue).ToString();
			}
			yield return new WaitForEndOfFrame();
		}
	}

	public void Show()
	{
		base.transform.TweenPosition(targetPosition.position, 1f);
		isShowing = true;
	}

	public void Hide()
	{
		base.transform.TweenPosition(originalPosition, 1f);
		isShowing = false;
	}
}

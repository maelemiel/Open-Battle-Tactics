using System.Collections;
using Holoville.HOTween;
using UnityEngine;

public class BouncingArrow : MonoBehaviour
{
	[SerializeField]
	private float bounceOffset = 35f;

	[SerializeField]
	private GameObject[] arrows;

	private Vector3 bouncePosition;

	private Vector3 originalPosition;

	[HideInInspector]
	public bool isBouncing;

	private void Start()
	{
		originalPosition = base.transform.localPosition;
		bouncePosition = base.transform.position;
		Hide();
	}

	public void Bounce()
	{
		base.transform.localPosition = originalPosition;
		base.gameObject.SetActive(true);
		isBouncing = true;
		Vector3 vector = new Vector3(bouncePosition.x, bouncePosition.y + bounceOffset, bouncePosition.z);
		HOTween.To(base.transform, 0.5f, new TweenParms().Prop("position", vector).Loops(100000, LoopType.Yoyo));
		if (arrows.Length > 0)
		{
			StartCoroutine(BounceArrows());
		}
	}

	private IEnumerator BounceArrows()
	{
		GameObject[] array = arrows;
		foreach (GameObject arrow in array)
		{
			yield return new WaitForSeconds(0.1f);
			Vector3 vec = new Vector3(arrow.transform.localPosition.x, arrow.transform.localPosition.y + 20f, arrow.transform.localPosition.z);
			HOTween.To(arrow.transform, 0.5f, new TweenParms().Prop("localPosition", vec).Loops(100000, LoopType.Yoyo));
		}
	}

	public void ToggleArrows(bool[] visibleFlags)
	{
		for (int i = 0; i < arrows.Length; i++)
		{
			GameObject gameObject = arrows[i];
			gameObject.SetActive(visibleFlags[i]);
			if (visibleFlags[i])
			{
				tk2dSprite component = gameObject.GetComponent<tk2dSprite>();
				component.Alpha = 0f;
				component.TweenAlpha(1f, 1f);
			}
		}
	}

	public void Hide()
	{
		base.gameObject.SetActive(false);
		StopAllCoroutines();
	}

	public void StartBouncingAfterSeconds(float sec)
	{
		base.gameObject.SetActive(true);
		StartCoroutine(_StartBouncingAfterSeconds(sec));
	}

	public IEnumerator _StartBouncingAfterSeconds(float sec)
	{
		yield return new WaitForSeconds(sec);
		Bounce();
	}

	public void StartBouncingAfterSeconds(float sec, bool[] visibleFlags)
	{
		GameObject[] array = arrows;
		foreach (GameObject gameObject in array)
		{
			gameObject.SetActive(false);
		}
		base.gameObject.SetActive(true);
		StartCoroutine(_StartBouncingAfterSeconds(sec, visibleFlags));
	}

	public IEnumerator _StartBouncingAfterSeconds(float sec, bool[] visibleFlags)
	{
		yield return new WaitForSeconds(sec);
		ToggleArrows(visibleFlags);
		Bounce();
	}
}

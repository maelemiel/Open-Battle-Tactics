using System.Collections;
using Holoville.HOTween;
using UnityEngine;

public class LoadingPopupController : MonoBehaviour
{
	private static float _animationTime = 1f;

	private static bool _fadeIn = true;

	private void Awake()
	{
		if (_fadeIn)
		{
			StartCoroutine(ShowCoroutine());
		}
		if (!LoadingPopupManager.RegisterLoadingPopupController(this))
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void Update()
	{
	}

	public void Destroy()
	{
		StartCoroutine(HideCoroutine());
	}

	public void Init(LoadingPopupManager.LoadingType loadingType)
	{
		switch (loadingType)
		{
		case LoadingPopupManager.LoadingType.altPosition:
			break;
		case LoadingPopupManager.LoadingType.blackBackground:
			break;
		case LoadingPopupManager.LoadingType.loadingBarOnly:
			break;
		}
	}

	private IEnumerator ShowCoroutine()
	{
		Sequence seq = new Sequence();
		tk2dSprite[] componentsInChildren = GetComponentsInChildren<tk2dSprite>();
		foreach (tk2dSprite sprite in componentsInChildren)
		{
			seq.Insert(0f, HOTween.To(sprite, _animationTime, "Alpha", 0.35f));
		}
		seq.Play();
		yield return StartCoroutine(seq.WaitForCompletion());
	}

	private IEnumerator HideCoroutine()
	{
		Sequence seq = new Sequence();
		tk2dSprite[] componentsInChildren = GetComponentsInChildren<tk2dSprite>();
		foreach (tk2dSprite sprite in componentsInChildren)
		{
			HOTween.Kill(sprite);
			seq.Insert(0f, HOTween.To(sprite, _animationTime, "Alpha", 0f));
		}
		tk2dTextMesh[] componentsInChildren2 = GetComponentsInChildren<tk2dTextMesh>();
		foreach (tk2dTextMesh text in componentsInChildren2)
		{
			Object.Destroy(text.gameObject);
		}
		seq.Play();
		yield return StartCoroutine(seq.WaitForCompletion());
		Object.Destroy(base.gameObject);
	}
}

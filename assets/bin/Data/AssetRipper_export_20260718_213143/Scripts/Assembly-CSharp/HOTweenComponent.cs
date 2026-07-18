using System;
using System.Collections.Generic;
using Holoville.HOTween;
using UnityEngine;

[AddComponentMenu("HOTween/HOTweenComponent")]
public class HOTweenComponent : ABSHOTweenEditorElement
{
	public bool destroyAfterSetup = true;

	[NonSerialized]
	public List<Tweener> generatedTweeners;

	private void Awake()
	{
		HOTween.Init(true, true, true);
		generatedTweeners = new List<Tweener>();
		foreach (HOTweenManager.HOTweenData tweenData in tweenDatas)
		{
			generatedTweeners.Add(HOTweenManager.CreateTween(tweenData, globalDelay, globalTimeScale));
		}
		if (destroyAfterSetup)
		{
			DoDestroy();
		}
	}

	private void OnDestroy()
	{
		DoDestroy();
	}

	protected override void DoDestroy()
	{
		if (!destroyed)
		{
			base.DoDestroy();
			generatedTweeners = null;
			UnityEngine.Object.Destroy(this);
		}
	}
}

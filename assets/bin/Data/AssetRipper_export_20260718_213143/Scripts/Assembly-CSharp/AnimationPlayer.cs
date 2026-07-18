using System;
using System.Collections;
using UnityEngine;

public class AnimationPlayer : MonoBehaviour
{
	private tk2dSpineAnimation _anim;

	private tk2dSpineSkeleton _skeleton;

	private string _skeletonData;

	private void Awake()
	{
		_skeletonData = "skeletonDataAsset";
	}

	public void Init()
	{
		_anim = base.transform.GetComponentInChildren<tk2dSpineAnimation>();
		_skeleton = base.transform.GetComponentInChildren<tk2dSpineSkeleton>();
		if (!_anim)
		{
			Debug.LogError("tk2dSpineAnimation not found!");
		}
		if (!_skeleton)
		{
			Debug.LogError("tk2dSpineSkeleton not found!");
		}
	}

	public void PlayAnimation(string clipName, bool loop = true)
	{
		if (_skeletonData == null)
		{
			Debug.LogError("SkeletonData not set");
		}
		PlayAnimation(_skeletonData, clipName, loop);
	}

	public IEnumerator PlayAnimation(string clipName, Action finishCallback)
	{
		PlayAnimation(clipName, false);
		while (!_anim.state.IsComplete())
		{
			yield return 0;
		}
		finishCallback();
		yield return 1;
	}

	public IEnumerator PlayAnimationCoroutine(string clipName)
	{
		PlayAnimation(clipName, false);
		while (!_anim.state.IsComplete())
		{
			yield return 0;
		}
		yield return 1;
	}

	public void PlayAnimation(string skeletonData, string clipName, bool loop = true)
	{
		if (skeletonData != _skeletonData)
		{
			_skeletonData = skeletonData;
		}
		_anim.state.Time = 0f;
		_anim.loop = loop;
		_anim.animationName = clipName;
	}

	public void Enabled(bool show)
	{
		_anim.gameObject.SetActive(show);
	}

	public void ChangeAnimationSpeed(float speed)
	{
		_anim.animationSpeed = speed;
	}
}

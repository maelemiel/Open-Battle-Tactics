using System;
using System.Collections;
using System.Linq;
using Spine;
using UnityEngine;

[RequireComponent(typeof(tk2dSpineSkeleton))]
public class tk2dSpineAnimation : MonoBehaviour
{
	public string animationName;

	public bool loop;

	public float animationSpeed = 1f;

	public Spine.AnimationState state;

	private tk2dSpineSkeleton cachedSpineSkeleton;

	private bool _isComplete;

	public bool IsComplete
	{
		get
		{
			return _isComplete;
		}
	}

	public string AnimationName
	{
		get
		{
			return animationName;
		}
		set
		{
			ClearAnimation();
			animationName = value;
		}
	}

	public tk2dSpineSkeleton Skeleton
	{
		get
		{
			return cachedSpineSkeleton;
		}
	}

	private event Action<tk2dSpineAnimation> _AnimationComplete;

	public event Action<tk2dSpineAnimation> AnimationComplete
	{
		add
		{
			this._AnimationComplete = (Action<tk2dSpineAnimation>)Delegate.Combine(this._AnimationComplete, value);
			if (state != null && state.IsComplete())
			{
				value(this);
			}
		}
		remove
		{
			this._AnimationComplete = (Action<tk2dSpineAnimation>)Delegate.Remove(this._AnimationComplete, value);
		}
	}

	public void ResetAnimationComplete()
	{
		this._AnimationComplete = null;
	}

	private void Awake()
	{
		cachedSpineSkeleton = GetComponent<tk2dSpineSkeleton>();
		state = new Spine.AnimationState(cachedSpineSkeleton.skeletonDataAsset.GetAnimationStateData());
		UpdateAnimation();
	}

	private void Update()
	{
		UpdateAnimation();
	}

	private void UpdateAnimation()
	{
		if (state.Animation != null && animationName == null)
		{
			state.ClearAnimation();
			_isComplete = false;
		}
		else if (state.Animation == null || animationName != state.Animation.Name)
		{
			Spine.Animation animation = cachedSpineSkeleton.skeleton.Data.FindAnimation(animationName);
			if (animation != null)
			{
				cachedSpineSkeleton.ResetData();
				state.SetAnimation(animation, loop);
			}
			_isComplete = false;
		}
		state.Loop = loop;
		cachedSpineSkeleton.skeleton.Update(Time.deltaTime * animationSpeed);
		state.Update(Time.deltaTime * animationSpeed);
		state.Apply(cachedSpineSkeleton.skeleton);
		CheckComplete();
	}

	public void ClearAnimation()
	{
		if (state != null)
		{
			state.ClearAnimation();
		}
		if (cachedSpineSkeleton != null)
		{
		}
		_isComplete = false;
	}

	public string[] GetAnimationNames()
	{
		return state.Data.SkeletonData.Animations.Select((Spine.Animation x) => x.Name).ToArray();
	}

	private void CheckComplete()
	{
		if (state != null && state.IsComplete() && !_isComplete)
		{
			_isComplete = true;
			if (this._AnimationComplete != null)
			{
				this._AnimationComplete(this);
			}
		}
	}

	public void Reset()
	{
		if (state != null)
		{
			state.Time = 0f;
		}
		_isComplete = false;
		if (cachedSpineSkeleton != null)
		{
			cachedSpineSkeleton.skeleton.SetToSetupPose();
			cachedSpineSkeleton.Update();
		}
	}

	public void ForceRebuild()
	{
		cachedSpineSkeleton.Initialize();
	}

	public float GetAnimationDuration(string animationName)
	{
		Spine.Animation animation = state.Data.SkeletonData.FindAnimation(animationName);
		float result = 0f;
		if (animation != null)
		{
			result = animation.Duration;
		}
		return result;
	}

	public IEnumerator WaitForAnimationComplete()
	{
		while (!_isComplete)
		{
			yield return 0;
		}
	}

	public IEnumerator PlayAnimCoroutine(string animationName)
	{
		AnimationName = animationName;
		while (!_isComplete)
		{
			yield return 0;
		}
	}
}

using System;
using System.Collections.Generic;
using UnityEngine;

public class MixedAnimationPlayer : MonoBehaviour
{
	[Serializable]
	public class AnimData
	{
		public GameObject animObject;

		public float delay;

		public string tk2dAnimName;

		public bool setActive = true;
	}

	public List<AnimData> animations;

	private float _timer;

	private int _step;

	private Action _cb;

	private bool _callbackFlag;

	private void Awake()
	{
		animations.Sort((AnimData lhs, AnimData rhs) => (lhs.delay - rhs.delay >= 0f) ? 1 : (-1));
		_step = animations.Count;
	}

	private void Update()
	{
		if (_step < animations.Count)
		{
			_timer += Time.deltaTime;
			if (_timer >= animations[_step].delay)
			{
				GameObject animObject = animations[_step].animObject;
				if ((bool)animObject)
				{
					animObject.SetActive(animations[_step].setActive);
					if (animations[_step].setActive)
					{
						if ((bool)animObject.animation)
						{
							animObject.animation.Play();
						}
						tk2dSpriteAnimator component = animObject.GetComponent<tk2dSpriteAnimator>();
						if ((bool)component)
						{
							component.Play(animations[_step].tk2dAnimName);
						}
						if ((bool)animObject.particleSystem)
						{
							animObject.particleSystem.Play();
						}
					}
				}
				_step++;
			}
			if (_step == animations.Count)
			{
				_callbackFlag = true;
			}
		}
		if (!_callbackFlag)
		{
			return;
		}
		bool flag = false;
		for (int i = 0; i < animations.Count; i++)
		{
			GameObject animObject2 = animations[i].animObject;
			if (!animObject2)
			{
				continue;
			}
			animObject2.SetActive(animations[i].setActive);
			if (animations[i].setActive)
			{
				if ((bool)animObject2.animation)
				{
					flag = flag || animObject2.animation.isPlaying;
				}
				tk2dSpriteAnimator component2 = animObject2.GetComponent<tk2dSpriteAnimator>();
				if ((bool)component2)
				{
					flag = flag || component2.IsPlaying(animations[i].tk2dAnimName);
				}
				if ((bool)animObject2.particleSystem)
				{
					flag = flag || animObject2.particleSystem.isPlaying;
				}
			}
		}
		if (!flag)
		{
			_callbackFlag = false;
			if (_cb != null)
			{
				_cb();
			}
		}
	}

	public void StartAnimation()
	{
		_timer = 0f;
		_step = 0;
	}

	public void StartAnimation(Action cb)
	{
		_timer = 0f;
		_step = 0;
		_callbackFlag = false;
		_cb = cb;
	}
}

using System;
using Holoville.HOTween;
using Holoville.HOTween.Core;

public class SimpleTween
{
	public float val;

	public static Tweener Start(float fromVal, float toVal, float duration, Action<float> onUpdate)
	{
		return Start(fromVal, toVal, duration, 0f, EaseType.EaseOutExpo, onUpdate);
	}

	public static Tweener Start(float fromVal, float toVal, float duration, EaseType ease, Action<float> onUpdate)
	{
		return Start(fromVal, toVal, duration, 0f, ease, onUpdate);
	}

	public static Tweener Start(float fromVal, float toVal, float duration, EaseType ease, Action<float> onUpdate, Action onComplete)
	{
		return Start(fromVal, toVal, duration, 0f, ease, onUpdate, onComplete);
	}

	public static Tweener Start(float fromVal, float toVal, float duration, float delay, EaseType ease, Action<float> onUpdate, Action onComplete = null)
	{
		if (duration.Equals(0f))
		{
			onUpdate(toVal);
			if (onComplete != null)
			{
				onComplete();
			}
			return null;
		}
		SimpleTween st = new SimpleTween();
		st.val = fromVal;
		TweenParms tweenParms = new TweenParms();
		tweenParms.Ease(ease);
		tweenParms.Prop("val", toVal);
		tweenParms.OnUpdate((TweenDelegate.TweenCallback)delegate
		{
			onUpdate(st.val);
		});
		if (onComplete != null)
		{
			tweenParms.OnComplete((TweenDelegate.TweenCallback)delegate
			{
				onComplete();
			});
		}
		Tweener result = HOTween.To(st, duration, tweenParms);
		onUpdate(fromVal);
		return result;
	}

	public static Sequence StartInAndOut(float fromVal, float toVal, float duration, EaseType ease, Action<float> onUpdate)
	{
		if (duration.Equals(0f))
		{
			onUpdate(toVal);
			return null;
		}
		SimpleTween st = new SimpleTween();
		st.val = fromVal;
		Sequence sequence = new Sequence();
		sequence.Append(HOTween.To(st, duration * 0.5f, new TweenParms().Ease(ease).Prop("val", toVal).OnUpdate((TweenDelegate.TweenCallback)delegate
		{
			onUpdate(st.val);
		})));
		sequence.Append(HOTween.To(st, duration * 0.5f, new TweenParms().Ease(ease).Prop("val", fromVal).OnUpdate((TweenDelegate.TweenCallback)delegate
		{
			onUpdate(st.val);
		})));
		sequence.Play();
		onUpdate(fromVal);
		return sequence;
	}

	public static Sequence DelayedCall(float duration, Action onComplete)
	{
		Sequence sequence = new Sequence();
		sequence.AppendInterval(duration);
		sequence.AppendCallback((TweenDelegate.TweenCallback)delegate
		{
			onComplete();
		});
		sequence.Play();
		return sequence;
	}
}

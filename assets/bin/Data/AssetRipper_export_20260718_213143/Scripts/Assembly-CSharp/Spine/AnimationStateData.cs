using System;
using System.Collections.Generic;

namespace Spine
{
	public class AnimationStateData
	{
		private Dictionary<KeyValuePair<Animation, Animation>, float> animationToMixTime = new Dictionary<KeyValuePair<Animation, Animation>, float>();

		public SkeletonData SkeletonData { get; private set; }

		public float defaultMix { get; set; }

		public AnimationStateData(SkeletonData skeletonData)
		{
			SkeletonData = skeletonData;
		}

		public void SetMix(string fromName, string toName, float duration)
		{
			Animation animation = SkeletonData.FindAnimation(fromName);
			if (animation == null)
			{
				throw new ArgumentException("Animation not found: " + fromName);
			}
			Animation animation2 = SkeletonData.FindAnimation(toName);
			if (animation2 == null)
			{
				throw new ArgumentException("Animation not found: " + toName);
			}
			SetMix(animation, animation2, duration);
		}

		public void SetMix(Animation from, Animation to, float duration)
		{
			if (from == null)
			{
				throw new ArgumentNullException("from cannot be null.");
			}
			if (to == null)
			{
				throw new ArgumentNullException("to cannot be null.");
			}
			KeyValuePair<Animation, Animation> key = new KeyValuePair<Animation, Animation>(from, to);
			animationToMixTime.Remove(key);
			animationToMixTime.Add(key, duration);
		}

		public float GetMix(Animation from, Animation to)
		{
			KeyValuePair<Animation, Animation> key = new KeyValuePair<Animation, Animation>(from, to);
			float value;
			if (animationToMixTime.TryGetValue(key, out value))
			{
				return value;
			}
			return defaultMix;
		}
	}
}

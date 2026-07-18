using System;
using System.Collections.Generic;

namespace Spine
{
	public class AnimationState
	{
		public delegate void EventDelegate(AnimationState state, Event e);

		private Animation previous;

		private float previousTime;

		private bool previousLoop;

		private float lastPreviousTime;

		private float lastTime;

		private float mixTime;

		private float mixDuration;

		private List<QueueEntry> queue = new List<QueueEntry>();

		private List<Event> events = new List<Event>();

		public AnimationStateData Data { get; private set; }

		public Animation Animation { get; private set; }

		public float Time { get; set; }

		public bool Loop { get; set; }

		public event EventDelegate OnEvent;

		public AnimationState(AnimationStateData data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data cannot be null.");
			}
			Data = data;
		}

		public void Update(float delta)
		{
			lastTime = Time;
			lastPreviousTime = previousTime;
			Time += delta;
			previousTime += delta;
			mixTime += delta;
			if (queue.Count > 0)
			{
				QueueEntry queueEntry = queue[0];
				if (Time >= queueEntry.delay)
				{
					SetAnimationInternal(queueEntry.animation, queueEntry.loop);
					queue.RemoveAt(0);
				}
			}
		}

		public void Apply(Skeleton skeleton)
		{
			if (Animation == null)
			{
				return;
			}
			List<Event> list = events;
			list.Clear();
			if (previous != null)
			{
				previous.Apply(skeleton, lastPreviousTime, previousTime, previousLoop, list);
				float num = mixTime / mixDuration;
				if (num >= 1f)
				{
					num = 1f;
					previous = null;
				}
				Animation.Mix(skeleton, lastTime, Time, Loop, num, list);
			}
			else
			{
				Animation.Apply(skeleton, lastTime, Time, Loop, list);
			}
			int i = 0;
			for (int count = list.Count; i < count; i++)
			{
				Event e = list[i];
				if (this.OnEvent != null)
				{
					this.OnEvent(this, e);
				}
			}
		}

		public void AddAnimation(string animationName, bool loop)
		{
			AddAnimation(animationName, loop, 0f);
		}

		public void AddAnimation(string animationName, bool loop, float delay)
		{
			Animation animation = Data.SkeletonData.FindAnimation(animationName);
			if (animation == null)
			{
				throw new ArgumentException("Animation not found: " + animationName);
			}
			AddAnimation(animation, loop, delay);
		}

		public void AddAnimation(Animation animation, bool loop)
		{
			AddAnimation(animation, loop, 0f);
		}

		public void AddAnimation(Animation animation, bool loop, float delay)
		{
			QueueEntry queueEntry = new QueueEntry();
			queueEntry.animation = animation;
			queueEntry.loop = loop;
			if (delay <= 0f)
			{
				Animation animation2 = ((queue.Count != 0) ? queue[queue.Count - 1].animation : Animation);
				delay = ((animation2 == null) ? 0f : (animation2.Duration - Data.GetMix(animation2, animation) + delay));
			}
			queueEntry.delay = delay;
			queue.Add(queueEntry);
		}

		private void SetAnimationInternal(Animation animation, bool loop)
		{
			previous = null;
			if (animation != null && Animation != null)
			{
				mixDuration = Data.GetMix(Animation, animation);
				if (mixDuration > 0f)
				{
					mixTime = 0f;
					previous = Animation;
					previousTime = Time;
					previousLoop = Loop;
				}
			}
			Animation = animation;
			Loop = loop;
			Time = 0f;
		}

		public void SetAnimation(string animationName, bool loop)
		{
			Animation animation = Data.SkeletonData.FindAnimation(animationName);
			if (animation == null)
			{
				throw new ArgumentException("Animation not found: " + animationName);
			}
			SetAnimation(animation, loop);
		}

		public void SetAnimation(Animation animation, bool loop)
		{
			queue.Clear();
			SetAnimationInternal(animation, loop);
		}

		public void ClearAnimation()
		{
			Time = 0f;
			previous = null;
			Animation = null;
			queue.Clear();
		}

		public bool IsComplete()
		{
			return Animation != null && Time >= Animation.Duration;
		}

		public override string ToString()
		{
			return (Animation == null || Animation.Name == null) ? base.ToString() : Animation.Name;
		}
	}
}

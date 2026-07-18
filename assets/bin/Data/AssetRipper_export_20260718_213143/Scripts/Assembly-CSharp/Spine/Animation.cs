using System;
using System.Collections.Generic;

namespace Spine
{
	public class Animation
	{
		public string Name { get; private set; }

		public List<Timeline> Timelines { get; set; }

		public float Duration { get; set; }

		public Animation(string name, List<Timeline> timelines, float duration)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name cannot be null.");
			}
			if (timelines == null)
			{
				throw new ArgumentNullException("timelines cannot be null.");
			}
			Name = name;
			Timelines = timelines;
			Duration = duration;
		}

		public void Apply(Skeleton skeleton, float lastTime, float time, bool loop, List<Event> events)
		{
			if (skeleton == null)
			{
				throw new ArgumentNullException("skeleton cannot be null.");
			}
			if (loop && Duration != 0f)
			{
				time %= Duration;
			}
			List<Timeline> timelines = Timelines;
			int i = 0;
			for (int count = timelines.Count; i < count; i++)
			{
				timelines[i].Apply(skeleton, lastTime, time, events, 1f);
			}
		}

		public void Mix(Skeleton skeleton, float lastTime, float time, bool loop, float alpha, List<Event> events)
		{
			if (skeleton == null)
			{
				throw new ArgumentNullException("skeleton cannot be null.");
			}
			if (loop && Duration != 0f)
			{
				time %= Duration;
			}
			List<Timeline> timelines = Timelines;
			int i = 0;
			for (int count = timelines.Count; i < count; i++)
			{
				timelines[i].Apply(skeleton, lastTime, time, events, alpha);
			}
		}

		internal static int binarySearch(float[] values, float target, int step)
		{
			int num = 0;
			int num2 = values.Length / step - 2;
			if (num2 == 0)
			{
				return step;
			}
			int num3 = (int)((uint)num2 >> 1);
			while (true)
			{
				if (values[(num3 + 1) * step] <= target)
				{
					num = num3 + 1;
				}
				else
				{
					num2 = num3;
				}
				if (num == num2)
				{
					break;
				}
				num3 = (int)((uint)(num + num2) >> 1);
			}
			return (num + 1) * step;
		}

		internal static int linearSearch(float[] values, float target, int step)
		{
			int i = 0;
			for (int num = values.Length - step; i <= num; i += step)
			{
				if (values[i] > target)
				{
					return i;
				}
			}
			return -1;
		}
	}
}

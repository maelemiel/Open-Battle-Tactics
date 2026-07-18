using System.Collections.Generic;

namespace Spine
{
	public class ColorTimeline : CurveTimeline
	{
		protected static int LAST_FRAME_TIME = -5;

		protected static int FRAME_R = 1;

		protected static int FRAME_G = 2;

		protected static int FRAME_B = 3;

		protected static int FRAME_A = 4;

		public int SlotIndex { get; set; }

		public float[] Frames { get; private set; }

		public ColorTimeline(int frameCount)
			: base(frameCount)
		{
			Frames = new float[frameCount * 5];
		}

		public void setFrame(int frameIndex, float time, float r, float g, float b, float a)
		{
			frameIndex *= 5;
			Frames[frameIndex] = time;
			Frames[frameIndex + 1] = r;
			Frames[frameIndex + 2] = g;
			Frames[frameIndex + 3] = b;
			Frames[frameIndex + 4] = a;
		}

		public override void Apply(Skeleton skeleton, float lastTime, float time, List<Event> firedEvents, float alpha)
		{
			float[] frames = Frames;
			if (time < frames[0])
			{
				return;
			}
			Slot slot = skeleton.Slots[SlotIndex];
			if (time >= frames[frames.Length - 5])
			{
				int num = frames.Length - 1;
				slot.R = frames[num - 3];
				slot.G = frames[num - 2];
				slot.B = frames[num - 1];
				slot.A = frames[num];
				return;
			}
			int num2 = Animation.binarySearch(frames, time, 5);
			float num3 = frames[num2 - 4];
			float num4 = frames[num2 - 3];
			float num5 = frames[num2 - 2];
			float num6 = frames[num2 - 1];
			float num7 = frames[num2];
			float num8 = 1f - (time - num7) / (frames[num2 + LAST_FRAME_TIME] - num7);
			num8 = GetCurvePercent(num2 / 5 - 1, (num8 < 0f) ? 0f : ((!(num8 > 1f)) ? num8 : 1f));
			float num9 = num3 + (frames[num2 + FRAME_R] - num3) * num8;
			float num10 = num4 + (frames[num2 + FRAME_G] - num4) * num8;
			float num11 = num5 + (frames[num2 + FRAME_B] - num5) * num8;
			float num12 = num6 + (frames[num2 + FRAME_A] - num6) * num8;
			if (alpha < 1f)
			{
				slot.R += (num9 - slot.R) * alpha;
				slot.G += (num10 - slot.G) * alpha;
				slot.B += (num11 - slot.B) * alpha;
				slot.A += (num12 - slot.A) * alpha;
			}
			else
			{
				slot.R = num9;
				slot.G = num10;
				slot.B = num11;
				slot.A = num12;
			}
		}
	}
}

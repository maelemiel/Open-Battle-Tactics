using System.Collections.Generic;

namespace Spine
{
	public class RotateTimeline : CurveTimeline
	{
		protected static int LAST_FRAME_TIME = -2;

		protected static int FRAME_VALUE = 1;

		public int BoneIndex { get; set; }

		public float[] Frames { get; private set; }

		public RotateTimeline(int frameCount)
			: base(frameCount)
		{
			Frames = new float[frameCount * 2];
		}

		public void SetFrame(int frameIndex, float time, float angle)
		{
			frameIndex *= 2;
			Frames[frameIndex] = time;
			Frames[frameIndex + 1] = angle;
		}

		public override void Apply(Skeleton skeleton, float lastTime, float time, List<Event> firedEvents, float alpha)
		{
			float[] frames = Frames;
			if (time < frames[0])
			{
				return;
			}
			Bone bone = skeleton.Bones[BoneIndex];
			float num;
			if (time >= frames[frames.Length - 2])
			{
				for (num = bone.Data.Rotation + frames[frames.Length - 1] - bone.Rotation; num > 180f; num -= 360f)
				{
				}
				for (; num < -180f; num += 360f)
				{
				}
				bone.Rotation += num * alpha;
				return;
			}
			int num2 = Animation.binarySearch(frames, time, 2);
			float num3 = frames[num2 - 1];
			float num4 = frames[num2];
			float num5 = 1f - (time - num4) / (frames[num2 + LAST_FRAME_TIME] - num4);
			num5 = GetCurvePercent(num2 / 2 - 1, (num5 < 0f) ? 0f : ((!(num5 > 1f)) ? num5 : 1f));
			for (num = frames[num2 + FRAME_VALUE] - num3; num > 180f; num -= 360f)
			{
			}
			for (; num < -180f; num += 360f)
			{
			}
			for (num = bone.Data.Rotation + (num3 + num * num5) - bone.Rotation; num > 180f; num -= 360f)
			{
			}
			for (; num < -180f; num += 360f)
			{
			}
			bone.Rotation += num * alpha;
		}
	}
}

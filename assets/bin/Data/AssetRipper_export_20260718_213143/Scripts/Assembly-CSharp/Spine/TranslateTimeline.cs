using System.Collections.Generic;

namespace Spine
{
	public class TranslateTimeline : CurveTimeline
	{
		protected static int LAST_FRAME_TIME = -3;

		protected static int FRAME_X = 1;

		protected static int FRAME_Y = 2;

		public int BoneIndex { get; set; }

		public float[] Frames { get; private set; }

		public TranslateTimeline(int frameCount)
			: base(frameCount)
		{
			Frames = new float[frameCount * 3];
		}

		public void SetFrame(int frameIndex, float time, float x, float y)
		{
			frameIndex *= 3;
			Frames[frameIndex] = time;
			Frames[frameIndex + 1] = x;
			Frames[frameIndex + 2] = y;
		}

		public override void Apply(Skeleton skeleton, float lastTime, float time, List<Event> firedEvents, float alpha)
		{
			float[] frames = Frames;
			if (!(time < frames[0]))
			{
				Bone bone = skeleton.Bones[BoneIndex];
				if (time >= frames[frames.Length - 3])
				{
					bone.X += (bone.Data.X + frames[frames.Length - 2] - bone.X) * alpha;
					bone.Y += (bone.Data.Y + frames[frames.Length - 1] - bone.Y) * alpha;
					return;
				}
				int num = Animation.binarySearch(frames, time, 3);
				float num2 = frames[num - 2];
				float num3 = frames[num - 1];
				float num4 = frames[num];
				float num5 = 1f - (time - num4) / (frames[num + LAST_FRAME_TIME] - num4);
				num5 = GetCurvePercent(num / 3 - 1, (num5 < 0f) ? 0f : ((!(num5 > 1f)) ? num5 : 1f));
				bone.X += (bone.Data.X + num2 + (frames[num + FRAME_X] - num2) * num5 - bone.X) * alpha;
				bone.Y += (bone.Data.Y + num3 + (frames[num + FRAME_Y] - num3) * num5 - bone.Y) * alpha;
			}
		}
	}
}

using System.Collections.Generic;

namespace Spine
{
	public class ScaleTimeline : TranslateTimeline
	{
		public ScaleTimeline(int frameCount)
			: base(frameCount)
		{
		}

		public override void Apply(Skeleton skeleton, float lastTime, float time, List<Event> firedEvents, float alpha)
		{
			float[] frames = base.Frames;
			if (!(time < frames[0]))
			{
				Bone bone = skeleton.Bones[base.BoneIndex];
				if (time >= frames[frames.Length - 3])
				{
					bone.ScaleX += (bone.Data.ScaleX - 1f + frames[frames.Length - 2] - bone.ScaleX) * alpha;
					bone.ScaleY += (bone.Data.ScaleY - 1f + frames[frames.Length - 1] - bone.ScaleY) * alpha;
					return;
				}
				int num = Animation.binarySearch(frames, time, 3);
				float num2 = frames[num - 2];
				float num3 = frames[num - 1];
				float num4 = frames[num];
				float num5 = 1f - (time - num4) / (frames[num + TranslateTimeline.LAST_FRAME_TIME] - num4);
				num5 = GetCurvePercent(num / 3 - 1, (num5 < 0f) ? 0f : ((!(num5 > 1f)) ? num5 : 1f));
				bone.ScaleX += (bone.Data.ScaleX - 1f + num2 + (frames[num + TranslateTimeline.FRAME_X] - num2) * num5 - bone.ScaleX) * alpha;
				bone.ScaleY += (bone.Data.ScaleY - 1f + num3 + (frames[num + TranslateTimeline.FRAME_Y] - num3) * num5 - bone.ScaleY) * alpha;
			}
		}
	}
}

using System.Collections.Generic;

namespace Spine
{
	public abstract class CurveTimeline : Timeline
	{
		protected static float LINEAR;

		protected static float STEPPED = -1f;

		protected static int BEZIER_SEGMENTS = 10;

		private float[] curves;

		public int FrameCount
		{
			get
			{
				return curves.Length / 6 + 1;
			}
		}

		public CurveTimeline(int frameCount)
		{
			curves = new float[(frameCount - 1) * 6];
		}

		public abstract void Apply(Skeleton skeleton, float lastTime, float time, List<Event> firedEvents, float alpha);

		public void SetLinear(int frameIndex)
		{
			curves[frameIndex * 6] = LINEAR;
		}

		public void SetStepped(int frameIndex)
		{
			curves[frameIndex * 6] = STEPPED;
		}

		public void SetCurve(int frameIndex, float cx1, float cy1, float cx2, float cy2)
		{
			float num = 1f / (float)BEZIER_SEGMENTS;
			float num2 = num * num;
			float num3 = num2 * num;
			float num4 = 3f * num;
			float num5 = 3f * num2;
			float num6 = 6f * num2;
			float num7 = 6f * num3;
			float num8 = (0f - cx1) * 2f + cx2;
			float num9 = (0f - cy1) * 2f + cy2;
			float num10 = (cx1 - cx2) * 3f + 1f;
			float num11 = (cy1 - cy2) * 3f + 1f;
			int num12 = frameIndex * 6;
			float[] array = curves;
			array[num12] = cx1 * num4 + num8 * num5 + num10 * num3;
			array[num12 + 1] = cy1 * num4 + num9 * num5 + num11 * num3;
			array[num12 + 2] = num8 * num6 + num10 * num7;
			array[num12 + 3] = num9 * num6 + num11 * num7;
			array[num12 + 4] = num10 * num7;
			array[num12 + 5] = num11 * num7;
		}

		public float GetCurvePercent(int frameIndex, float percent)
		{
			int num = frameIndex * 6;
			float[] array = curves;
			float num2 = array[num];
			if (num2 == LINEAR)
			{
				return percent;
			}
			if (num2 == STEPPED)
			{
				return 0f;
			}
			float num3 = array[num + 1];
			float num4 = array[num + 2];
			float num5 = array[num + 3];
			float num6 = array[num + 4];
			float num7 = array[num + 5];
			float num8 = num2;
			float num9 = num3;
			int num10 = BEZIER_SEGMENTS - 2;
			while (true)
			{
				if (num8 >= percent)
				{
					float num11 = num8 - num2;
					float num12 = num9 - num3;
					return num12 + (num9 - num12) * (percent - num11) / (num8 - num11);
				}
				if (num10 == 0)
				{
					break;
				}
				num10--;
				num2 += num4;
				num3 += num5;
				num4 += num6;
				num5 += num7;
				num8 += num2;
				num9 += num3;
			}
			return num9 + (1f - num9) * (percent - num8) / (1f - num8);
		}
	}
}

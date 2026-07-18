using System;

namespace Spine
{
	public class RegionAttachment : Attachment
	{
		public const int X1 = 0;

		public const int Y1 = 1;

		public const int X2 = 2;

		public const int Y2 = 3;

		public const int X3 = 4;

		public const int Y3 = 5;

		public const int X4 = 6;

		public const int Y4 = 7;

		public float X { get; set; }

		public float Y { get; set; }

		public float ScaleX { get; set; }

		public float ScaleY { get; set; }

		public float Rotation { get; set; }

		public float Width { get; set; }

		public float Height { get; set; }

		public object RendererObject { get; set; }

		public float RegionOffsetX { get; set; }

		public float RegionOffsetY { get; set; }

		public float RegionWidth { get; set; }

		public float RegionHeight { get; set; }

		public float RegionOriginalWidth { get; set; }

		public float RegionOriginalHeight { get; set; }

		public float[] Offset { get; private set; }

		public float[] UVs { get; private set; }

		public RegionAttachment(string name)
			: base(name)
		{
			Offset = new float[8];
			UVs = new float[8];
			ScaleX = 1f;
			ScaleY = 1f;
		}

		public void SetUVs(float u, float v, float u2, float v2, bool rotate)
		{
			float[] uVs = UVs;
			if (rotate)
			{
				uVs[2] = u;
				uVs[3] = v2;
				uVs[4] = u;
				uVs[5] = v;
				uVs[6] = u2;
				uVs[7] = v;
				uVs[0] = u2;
				uVs[1] = v2;
			}
			else
			{
				uVs[0] = u;
				uVs[1] = v2;
				uVs[2] = u;
				uVs[3] = v;
				uVs[4] = u2;
				uVs[5] = v;
				uVs[6] = u2;
				uVs[7] = v2;
			}
		}

		public void UpdateOffset()
		{
			float width = Width;
			float height = Height;
			float scaleX = ScaleX;
			float scaleY = ScaleY;
			float num = width / RegionOriginalWidth * scaleX;
			float num2 = height / RegionOriginalHeight * scaleY;
			float num3 = (0f - width) / 2f * scaleX + RegionOffsetX * num;
			float num4 = (0f - height) / 2f * scaleY + RegionOffsetY * num2;
			float num5 = num3 + RegionWidth * num;
			float num6 = num4 + RegionHeight * num2;
			float num7 = Rotation * (float)Math.PI / 180f;
			float num8 = (float)Math.Cos(num7);
			float num9 = (float)Math.Sin(num7);
			float x = X;
			float y = Y;
			float num10 = num3 * num8 + x;
			float num11 = num3 * num9;
			float num12 = num4 * num8 + y;
			float num13 = num4 * num9;
			float num14 = num5 * num8 + x;
			float num15 = num5 * num9;
			float num16 = num6 * num8 + y;
			float num17 = num6 * num9;
			float[] offset = Offset;
			offset[0] = num10 - num13;
			offset[1] = num12 + num11;
			offset[2] = num10 - num17;
			offset[3] = num16 + num11;
			offset[4] = num14 - num17;
			offset[5] = num16 + num15;
			offset[6] = num14 - num13;
			offset[7] = num12 + num15;
		}

		public void ComputeVertices(float x, float y, Bone bone, float[] vertices)
		{
			x += bone.WorldX;
			y += bone.WorldY;
			float m = bone.M00;
			float m2 = bone.M01;
			float m3 = bone.M10;
			float m4 = bone.M11;
			float[] offset = Offset;
			vertices[0] = offset[0] * m + offset[1] * m2 + x;
			vertices[1] = offset[0] * m3 + offset[1] * m4 + y;
			vertices[2] = offset[2] * m + offset[3] * m2 + x;
			vertices[3] = offset[2] * m3 + offset[3] * m4 + y;
			vertices[4] = offset[4] * m + offset[5] * m2 + x;
			vertices[5] = offset[4] * m3 + offset[5] * m4 + y;
			vertices[6] = offset[6] * m + offset[7] * m2 + x;
			vertices[7] = offset[6] * m3 + offset[7] * m4 + y;
		}
	}
}

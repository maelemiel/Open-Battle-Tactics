using System;

namespace Spine
{
	public class Bone
	{
		public static bool yDown;

		public BoneData Data { get; private set; }

		public Bone Parent { get; private set; }

		public float X { get; set; }

		public float Y { get; set; }

		public float Rotation { get; set; }

		public float ScaleX { get; set; }

		public float ScaleY { get; set; }

		public float M00 { get; private set; }

		public float M01 { get; private set; }

		public float M10 { get; private set; }

		public float M11 { get; private set; }

		public float WorldX { get; private set; }

		public float WorldY { get; private set; }

		public float WorldRotation { get; private set; }

		public float WorldScaleX { get; private set; }

		public float WorldScaleY { get; private set; }

		public Bone(BoneData data, Bone parent)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data cannot be null.");
			}
			Data = data;
			Parent = parent;
			SetToSetupPose();
		}

		public void UpdateWorldTransform(bool flipX, bool flipY)
		{
			Bone parent = Parent;
			if (parent != null)
			{
				WorldX = X * parent.M00 + Y * parent.M01 + parent.WorldX;
				WorldY = X * parent.M10 + Y * parent.M11 + parent.WorldY;
				if (Data.InheritScale)
				{
					WorldScaleX = parent.WorldScaleX * ScaleX;
					WorldScaleY = parent.WorldScaleY * ScaleY;
				}
				else
				{
					WorldScaleX = ScaleX;
					WorldScaleY = ScaleY;
				}
				WorldRotation = ((!Data.InheritRotation) ? Rotation : (parent.WorldRotation + Rotation));
			}
			else
			{
				WorldX = ((!flipX) ? X : (0f - X));
				WorldY = ((!flipY) ? Y : (0f - Y));
				WorldScaleX = ScaleX;
				WorldScaleY = ScaleY;
				WorldRotation = Rotation;
			}
			float num = WorldRotation * (float)Math.PI / 180f;
			float num2 = (float)Math.Cos(num);
			float num3 = (float)Math.Sin(num);
			M00 = num2 * WorldScaleX;
			M10 = num3 * WorldScaleX;
			M01 = (0f - num3) * WorldScaleY;
			M11 = num2 * WorldScaleY;
			if (flipX)
			{
				M00 = 0f - M00;
				M01 = 0f - M01;
			}
			if (flipY ^ yDown)
			{
				M10 = 0f - M10;
				M11 = 0f - M11;
			}
		}

		public void SetToSetupPose()
		{
			BoneData data = Data;
			X = data.X;
			Y = data.Y;
			Rotation = data.Rotation;
			ScaleX = data.ScaleX;
			ScaleY = data.ScaleY;
		}

		public override string ToString()
		{
			return Data.Name;
		}
	}
}

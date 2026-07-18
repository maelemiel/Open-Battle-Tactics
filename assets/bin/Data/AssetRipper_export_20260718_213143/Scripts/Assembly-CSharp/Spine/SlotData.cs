using System;

namespace Spine
{
	public class SlotData
	{
		public string Name { get; private set; }

		public BoneData BoneData { get; private set; }

		public float R { get; set; }

		public float G { get; set; }

		public float B { get; set; }

		public float A { get; set; }

		public string AttachmentName { get; set; }

		public SlotData(string name, BoneData boneData)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name cannot be null.");
			}
			if (boneData == null)
			{
				throw new ArgumentNullException("boneData cannot be null.");
			}
			Name = name;
			BoneData = boneData;
			R = 1f;
			G = 1f;
			B = 1f;
			A = 1f;
		}

		public override string ToString()
		{
			return Name;
		}
	}
}

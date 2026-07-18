using System;

namespace Spine
{
	public class BoneData
	{
		public BoneData Parent { get; private set; }

		public string Name { get; private set; }

		public float Length { get; set; }

		public float X { get; set; }

		public float Y { get; set; }

		public float Rotation { get; set; }

		public float ScaleX { get; set; }

		public float ScaleY { get; set; }

		public bool InheritScale { get; set; }

		public bool InheritRotation { get; set; }

		public BoneData(string name, BoneData parent)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name cannot be null.");
			}
			Name = name;
			Parent = parent;
			ScaleX = 1f;
			ScaleY = 1f;
		}

		public override string ToString()
		{
			return Name;
		}
	}
}

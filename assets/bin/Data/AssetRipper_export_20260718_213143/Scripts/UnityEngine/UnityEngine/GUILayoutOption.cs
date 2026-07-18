namespace UnityEngine
{
	public sealed class GUILayoutOption
	{
		internal enum Type
		{
			fixedWidth = 0,
			fixedHeight = 1,
			minWidth = 2,
			maxWidth = 3,
			minHeight = 4,
			maxHeight = 5,
			stretchWidth = 6,
			stretchHeight = 7,
			alignStart = 8,
			alignMiddle = 9,
			alignEnd = 10,
			alignJustify = 11,
			equalSize = 12,
			spacing = 13
		}

		internal Type type;

		internal object value;

		internal GUILayoutOption(Type type, object value)
		{
			this.type = type;
			this.value = value;
		}
	}
}

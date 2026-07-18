namespace UnityEngine
{
	public struct TextGenerationSettings
	{
		public Color color;

		public int size;

		public FontStyle style;

		public bool richText;

		public TextAnchor anchor;

		public TextWrapMode wrapMode;

		public Vector2 extents;

		public Vector2 pivot;

		public Font font;

		private bool CompareColors(Color left, Color right)
		{
			Color32 color = left;
			Color32 color2 = right;
			return color.Equals(color2);
		}

		private bool CompareVector2(Vector2 left, Vector2 right)
		{
			return Mathf.Approximately(left.x, right.x) && Mathf.Approximately(left.y, right.y);
		}

		public bool Equals(TextGenerationSettings other)
		{
			return CompareColors(color, other.color) && size == other.size && style == other.style && richText == other.richText && anchor == other.anchor && wrapMode == other.wrapMode && CompareVector2(extents, other.extents) && CompareVector2(pivot, other.pivot) && font == other.font;
		}
	}
}

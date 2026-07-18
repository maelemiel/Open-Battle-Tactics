namespace UnityEngine
{
	internal sealed class GUIWordWrapSizer : GUILayoutEntry
	{
		private GUIContent content;

		private float forcedMinHeight;

		private float forcedMaxHeight;

		public GUIWordWrapSizer(GUIStyle _style, GUIContent _content, GUILayoutOption[] options)
			: base(0f, 0f, 0f, 0f, _style)
		{
			content = new GUIContent(_content);
			base.ApplyOptions(options);
			forcedMinHeight = minHeight;
			forcedMaxHeight = maxHeight;
		}

		public override void CalcWidth()
		{
			if (minWidth == 0f || maxWidth == 0f)
			{
				float num;
				float num2;
				base.style.CalcMinMaxWidth(content, out num, out num2);
				if (minWidth == 0f)
				{
					minWidth = num;
				}
				if (maxWidth == 0f)
				{
					maxWidth = num2;
				}
			}
		}

		public override void CalcHeight()
		{
			if (forcedMinHeight == 0f || forcedMaxHeight == 0f)
			{
				float num = base.style.CalcHeight(content, rect.width);
				if (forcedMinHeight == 0f)
				{
					minHeight = num;
				}
				else
				{
					minHeight = forcedMinHeight;
				}
				if (forcedMaxHeight == 0f)
				{
					maxHeight = num;
				}
				else
				{
					maxHeight = forcedMaxHeight;
				}
			}
		}
	}
}

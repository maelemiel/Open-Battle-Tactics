namespace UnityEngine
{
	internal sealed class GUIGridSizer : GUILayoutEntry
	{
		private int count;

		private int xCount;

		private float minButtonWidth = -1f;

		private float maxButtonWidth = -1f;

		private float minButtonHeight = -1f;

		private float maxButtonHeight = -1f;

		private int rows
		{
			get
			{
				int num = count / xCount;
				if (count % xCount != 0)
				{
					num++;
				}
				return num;
			}
		}

		private GUIGridSizer(GUIContent[] contents, int _xCount, GUIStyle buttonStyle, GUILayoutOption[] options)
			: base(0f, 0f, 0f, 0f, GUIStyle.none)
		{
			count = contents.Length;
			xCount = _xCount;
			ApplyStyleSettings(buttonStyle);
			ApplyOptions(options);
			if (_xCount == 0 || contents.Length == 0)
			{
				return;
			}
			float num = Mathf.Max(buttonStyle.margin.left, buttonStyle.margin.right) * (xCount - 1);
			float num2 = Mathf.Max(buttonStyle.margin.top, buttonStyle.margin.bottom) * (rows - 1);
			if (buttonStyle.fixedWidth != 0f)
			{
				minButtonWidth = (maxButtonWidth = buttonStyle.fixedWidth);
			}
			if (buttonStyle.fixedHeight != 0f)
			{
				minButtonHeight = (maxButtonHeight = buttonStyle.fixedHeight);
			}
			if (minButtonWidth == -1f)
			{
				if (minWidth != 0f)
				{
					minButtonWidth = (minWidth - num) / (float)xCount;
				}
				if (maxWidth != 0f)
				{
					maxButtonWidth = (maxWidth - num) / (float)xCount;
				}
			}
			if (minButtonHeight == -1f)
			{
				if (minHeight != 0f)
				{
					minButtonHeight = (minHeight - num2) / (float)rows;
				}
				if (maxHeight != 0f)
				{
					maxButtonHeight = (maxHeight - num2) / (float)rows;
				}
			}
			if (minButtonHeight == -1f || maxButtonHeight == -1f || minButtonWidth == -1f || maxButtonWidth == -1f)
			{
				float a = 0f;
				float a2 = 0f;
				foreach (GUIContent content in contents)
				{
					Vector2 vector = buttonStyle.CalcSize(content);
					a2 = Mathf.Max(a2, vector.x);
					a = Mathf.Max(a, vector.y);
				}
				if (minButtonWidth == -1f)
				{
					if (maxButtonWidth != -1f)
					{
						minButtonWidth = Mathf.Min(a2, maxButtonWidth);
					}
					else
					{
						minButtonWidth = a2;
					}
				}
				if (maxButtonWidth == -1f)
				{
					if (minButtonWidth != -1f)
					{
						maxButtonWidth = Mathf.Max(a2, minButtonWidth);
					}
					else
					{
						maxButtonWidth = a2;
					}
				}
				if (minButtonHeight == -1f)
				{
					if (maxButtonHeight != -1f)
					{
						minButtonHeight = Mathf.Min(a, maxButtonHeight);
					}
					else
					{
						minButtonHeight = a;
					}
				}
				if (maxButtonHeight == -1f)
				{
					if (minButtonHeight != -1f)
					{
						maxHeight = Mathf.Max(maxHeight, minButtonHeight);
					}
					maxButtonHeight = maxHeight;
				}
			}
			minWidth = minButtonWidth * (float)xCount + num;
			maxWidth = maxButtonWidth * (float)xCount + num;
			minHeight = minButtonHeight * (float)rows + num2;
			maxHeight = maxButtonHeight * (float)rows + num2;
		}

		public static Rect GetRect(GUIContent[] contents, int xCount, GUIStyle style, GUILayoutOption[] options)
		{
			Rect result = new Rect(0f, 0f, 0f, 0f);
			switch (Event.current.type)
			{
			case EventType.Layout:
				GUILayoutUtility.current.topLevel.Add(new GUIGridSizer(contents, xCount, style, options));
				return result;
			case EventType.Used:
				return GUILayoutEntry.kDummyRect;
			default:
				return GUILayoutUtility.current.topLevel.GetNext().rect;
			}
		}
	}
}

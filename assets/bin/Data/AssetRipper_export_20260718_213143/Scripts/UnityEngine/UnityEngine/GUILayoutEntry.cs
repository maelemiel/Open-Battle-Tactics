namespace UnityEngine
{
	internal class GUILayoutEntry
	{
		public float minWidth;

		public float maxWidth;

		public float minHeight;

		public float maxHeight;

		public Rect rect = new Rect(0f, 0f, 0f, 0f);

		public int stretchWidth;

		public int stretchHeight;

		private GUIStyle m_Style = GUIStyle.none;

		internal static Rect kDummyRect = new Rect(0f, 0f, 1f, 1f);

		protected static int indent = 0;

		public GUIStyle style
		{
			get
			{
				return m_Style;
			}
			set
			{
				m_Style = value;
				ApplyStyleSettings(value);
			}
		}

		public virtual RectOffset margin
		{
			get
			{
				return style.margin;
			}
		}

		public GUILayoutEntry(float _minWidth, float _maxWidth, float _minHeight, float _maxHeight, GUIStyle _style)
		{
			minWidth = _minWidth;
			maxWidth = _maxWidth;
			minHeight = _minHeight;
			maxHeight = _maxHeight;
			if (_style == null)
			{
				_style = GUIStyle.none;
			}
			style = _style;
		}

		public GUILayoutEntry(float _minWidth, float _maxWidth, float _minHeight, float _maxHeight, GUIStyle _style, GUILayoutOption[] options)
		{
			minWidth = _minWidth;
			maxWidth = _maxWidth;
			minHeight = _minHeight;
			maxHeight = _maxHeight;
			style = _style;
			ApplyOptions(options);
		}

		public virtual void CalcWidth()
		{
		}

		public virtual void CalcHeight()
		{
		}

		public virtual void SetHorizontal(float x, float width)
		{
			rect.x = x;
			rect.width = width;
		}

		public virtual void SetVertical(float y, float height)
		{
			rect.y = y;
			rect.height = height;
		}

		protected virtual void ApplyStyleSettings(GUIStyle style)
		{
			stretchWidth = ((style.fixedWidth == 0f && style.stretchWidth) ? 1 : 0);
			stretchHeight = ((style.fixedHeight == 0f && style.stretchHeight) ? 1 : 0);
			m_Style = style;
		}

		public virtual void ApplyOptions(GUILayoutOption[] options)
		{
			if (options == null)
			{
				return;
			}
			foreach (GUILayoutOption gUILayoutOption in options)
			{
				switch (gUILayoutOption.type)
				{
				case GUILayoutOption.Type.fixedWidth:
					minWidth = (maxWidth = (float)gUILayoutOption.value);
					stretchWidth = 0;
					break;
				case GUILayoutOption.Type.fixedHeight:
					minHeight = (maxHeight = (float)gUILayoutOption.value);
					stretchHeight = 0;
					break;
				case GUILayoutOption.Type.minWidth:
					minWidth = (float)gUILayoutOption.value;
					if (maxWidth < minWidth)
					{
						maxWidth = minWidth;
					}
					break;
				case GUILayoutOption.Type.maxWidth:
					maxWidth = (float)gUILayoutOption.value;
					if (minWidth > maxWidth)
					{
						minWidth = maxWidth;
					}
					stretchWidth = 0;
					break;
				case GUILayoutOption.Type.minHeight:
					minHeight = (float)gUILayoutOption.value;
					if (maxHeight < minHeight)
					{
						maxHeight = minHeight;
					}
					break;
				case GUILayoutOption.Type.maxHeight:
					maxHeight = (float)gUILayoutOption.value;
					if (minHeight > maxHeight)
					{
						minHeight = maxHeight;
					}
					stretchHeight = 0;
					break;
				case GUILayoutOption.Type.stretchWidth:
					stretchWidth = (int)gUILayoutOption.value;
					break;
				case GUILayoutOption.Type.stretchHeight:
					stretchHeight = (int)gUILayoutOption.value;
					break;
				}
			}
			if (maxWidth != 0f && maxWidth < minWidth)
			{
				maxWidth = minWidth;
			}
			if (maxHeight != 0f && maxHeight < minHeight)
			{
				maxHeight = minHeight;
			}
		}

		public override string ToString()
		{
			string text = string.Empty;
			for (int i = 0; i < indent; i++)
			{
				text += " ";
			}
			return text + UnityString.Format("{1}-{0} (x:{2}-{3}, y:{4}-{5})", (style == null) ? "NULL" : style.name, GetType(), rect.x, rect.xMax, rect.y, rect.yMax) + "   -   W: " + minWidth + "-" + maxWidth + ((stretchWidth == 0) ? string.Empty : "+") + ", H: " + minHeight + "-" + maxHeight + ((stretchHeight == 0) ? string.Empty : "+");
		}
	}
}

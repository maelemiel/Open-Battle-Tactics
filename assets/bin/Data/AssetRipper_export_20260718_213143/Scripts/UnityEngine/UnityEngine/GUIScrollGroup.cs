namespace UnityEngine
{
	internal sealed class GUIScrollGroup : GUILayoutGroup
	{
		public float calcMinWidth;

		public float calcMaxWidth;

		public float calcMinHeight;

		public float calcMaxHeight;

		public float clientWidth;

		public float clientHeight;

		public bool allowHorizontalScroll = true;

		public bool allowVerticalScroll = true;

		public bool needsHorizontalScrollbar;

		public bool needsVerticalScrollbar;

		public GUIStyle horizontalScrollbar;

		public GUIStyle verticalScrollbar;

		public override void CalcWidth()
		{
			float num = minWidth;
			float num2 = maxWidth;
			if (allowHorizontalScroll)
			{
				minWidth = 0f;
				maxWidth = 0f;
			}
			base.CalcWidth();
			calcMinWidth = minWidth;
			calcMaxWidth = maxWidth;
			if (allowHorizontalScroll)
			{
				if (minWidth > 32f)
				{
					minWidth = 32f;
				}
				if (num != 0f)
				{
					minWidth = num;
				}
				if (num2 != 0f)
				{
					maxWidth = num2;
					stretchWidth = 0;
				}
			}
		}

		public override void SetHorizontal(float x, float width)
		{
			float num = ((!needsVerticalScrollbar) ? width : (width - verticalScrollbar.fixedWidth - (float)verticalScrollbar.margin.left));
			if (allowHorizontalScroll && num < calcMinWidth)
			{
				needsHorizontalScrollbar = true;
				minWidth = calcMinWidth;
				maxWidth = calcMaxWidth;
				base.SetHorizontal(x, calcMinWidth);
				rect.width = width;
				clientWidth = calcMinWidth;
				return;
			}
			needsHorizontalScrollbar = false;
			if (allowHorizontalScroll)
			{
				minWidth = calcMinWidth;
				maxWidth = calcMaxWidth;
			}
			base.SetHorizontal(x, num);
			rect.width = width;
			clientWidth = num;
		}

		public override void CalcHeight()
		{
			float num = minHeight;
			float num2 = maxHeight;
			if (allowVerticalScroll)
			{
				minHeight = 0f;
				maxHeight = 0f;
			}
			base.CalcHeight();
			calcMinHeight = minHeight;
			calcMaxHeight = maxHeight;
			if (needsHorizontalScrollbar)
			{
				float num3 = horizontalScrollbar.fixedHeight + (float)horizontalScrollbar.margin.top;
				minHeight += num3;
				maxHeight += num3;
			}
			if (allowVerticalScroll)
			{
				if (minHeight > 32f)
				{
					minHeight = 32f;
				}
				if (num != 0f)
				{
					minHeight = num;
				}
				if (num2 != 0f)
				{
					maxHeight = num2;
					stretchHeight = 0;
				}
			}
		}

		public override void SetVertical(float y, float height)
		{
			float num = height;
			if (needsHorizontalScrollbar)
			{
				num -= horizontalScrollbar.fixedHeight + (float)horizontalScrollbar.margin.top;
			}
			if (allowVerticalScroll && num < calcMinHeight)
			{
				if (!needsHorizontalScrollbar && !needsVerticalScrollbar)
				{
					clientWidth = rect.width - verticalScrollbar.fixedWidth - (float)verticalScrollbar.margin.left;
					if (clientWidth < calcMinWidth)
					{
						clientWidth = calcMinWidth;
					}
					float width = rect.width;
					SetHorizontal(rect.x, clientWidth);
					CalcHeight();
					rect.width = width;
				}
				float num2 = minHeight;
				float num3 = maxHeight;
				minHeight = calcMinHeight;
				maxHeight = calcMaxHeight;
				base.SetVertical(y, calcMinHeight);
				minHeight = num2;
				maxHeight = num3;
				rect.height = height;
				clientHeight = calcMinHeight;
			}
			else
			{
				if (allowVerticalScroll)
				{
					minHeight = calcMinHeight;
					maxHeight = calcMaxHeight;
				}
				base.SetVertical(y, num);
				rect.height = height;
				clientHeight = num;
			}
		}
	}
}

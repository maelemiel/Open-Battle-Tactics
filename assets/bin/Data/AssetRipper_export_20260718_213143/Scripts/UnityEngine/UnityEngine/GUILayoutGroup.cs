using System;
using System.Collections.Generic;

namespace UnityEngine
{
	internal class GUILayoutGroup : GUILayoutEntry
	{
		public List<GUILayoutEntry> entries = new List<GUILayoutEntry>();

		public bool isVertical = true;

		public bool resetCoords;

		public float spacing;

		public bool sameSize = true;

		public bool isWindow;

		public int windowID = -1;

		private int cursor;

		protected int stretchableCountX = 100;

		protected int stretchableCountY = 100;

		protected bool userSpecifiedWidth;

		protected bool userSpecifiedHeight;

		protected float childMinWidth = 100f;

		protected float childMaxWidth = 100f;

		protected float childMinHeight = 100f;

		protected float childMaxHeight = 100f;

		private RectOffset m_Margin = new RectOffset();

		public override RectOffset margin
		{
			get
			{
				return m_Margin;
			}
		}

		public GUILayoutGroup()
			: base(0f, 0f, 0f, 0f, GUIStyle.none)
		{
		}

		public GUILayoutGroup(GUIStyle _style, GUILayoutOption[] options)
			: base(0f, 0f, 0f, 0f, _style)
		{
			if (options != null)
			{
				ApplyOptions(options);
			}
			m_Margin.left = _style.margin.left;
			m_Margin.right = _style.margin.right;
			m_Margin.top = _style.margin.top;
			m_Margin.bottom = _style.margin.bottom;
		}

		public override void ApplyOptions(GUILayoutOption[] options)
		{
			if (options == null)
			{
				return;
			}
			base.ApplyOptions(options);
			foreach (GUILayoutOption gUILayoutOption in options)
			{
				switch (gUILayoutOption.type)
				{
				case GUILayoutOption.Type.fixedWidth:
				case GUILayoutOption.Type.minWidth:
				case GUILayoutOption.Type.maxWidth:
					userSpecifiedHeight = true;
					break;
				case GUILayoutOption.Type.fixedHeight:
				case GUILayoutOption.Type.minHeight:
				case GUILayoutOption.Type.maxHeight:
					userSpecifiedWidth = true;
					break;
				case GUILayoutOption.Type.spacing:
					spacing = (int)gUILayoutOption.value;
					break;
				}
			}
		}

		protected override void ApplyStyleSettings(GUIStyle style)
		{
			base.ApplyStyleSettings(style);
			RectOffset rectOffset = style.margin;
			m_Margin.left = rectOffset.left;
			m_Margin.right = rectOffset.right;
			m_Margin.top = rectOffset.top;
			m_Margin.bottom = rectOffset.bottom;
		}

		public void ResetCursor()
		{
			cursor = 0;
		}

		public Rect PeekNext()
		{
			if (cursor < entries.Count)
			{
				GUILayoutEntry gUILayoutEntry = entries[cursor];
				return gUILayoutEntry.rect;
			}
			throw new ArgumentException(string.Concat("Getting control ", cursor, "'s position in a group with only ", entries.Count, " controls when doing ", Event.current.rawType, "\nAborting"));
		}

		public GUILayoutEntry GetNext()
		{
			if (cursor < entries.Count)
			{
				GUILayoutEntry result = entries[cursor];
				cursor++;
				return result;
			}
			throw new ArgumentException(string.Concat("Getting control ", cursor, "'s position in a group with only ", entries.Count, " controls when doing ", Event.current.rawType, "\nAborting"));
		}

		public Rect GetLast()
		{
			if (cursor == 0)
			{
				Debug.LogError("You cannot call GetLast immediately after beginning a group.");
				return GUILayoutEntry.kDummyRect;
			}
			if (cursor <= entries.Count)
			{
				GUILayoutEntry gUILayoutEntry = entries[cursor - 1];
				return gUILayoutEntry.rect;
			}
			Debug.LogError("Getting control " + cursor + "'s position in a group with only " + entries.Count + " controls when doing " + Event.current.type);
			return GUILayoutEntry.kDummyRect;
		}

		public void Add(GUILayoutEntry e)
		{
			entries.Add(e);
		}

		public override void CalcWidth()
		{
			if (entries.Count == 0)
			{
				maxWidth = (minWidth = base.style.padding.horizontal);
				return;
			}
			childMinWidth = 0f;
			childMaxWidth = 0f;
			int num = 0;
			int num2 = 0;
			stretchableCountX = 0;
			bool flag = true;
			if (isVertical)
			{
				foreach (GUILayoutEntry entry in entries)
				{
					entry.CalcWidth();
					RectOffset rectOffset = entry.margin;
					if (entry.style != GUILayoutUtility.spaceStyle)
					{
						if (!flag)
						{
							num = Mathf.Min(rectOffset.left, num);
							num2 = Mathf.Min(rectOffset.right, num2);
						}
						else
						{
							num = rectOffset.left;
							num2 = rectOffset.right;
							flag = false;
						}
						childMinWidth = Mathf.Max(entry.minWidth + (float)rectOffset.horizontal, childMinWidth);
						childMaxWidth = Mathf.Max(entry.maxWidth + (float)rectOffset.horizontal, childMaxWidth);
					}
					stretchableCountX += entry.stretchWidth;
				}
				childMinWidth -= num + num2;
				childMaxWidth -= num + num2;
			}
			else
			{
				int num3 = 0;
				foreach (GUILayoutEntry entry2 in entries)
				{
					entry2.CalcWidth();
					RectOffset rectOffset2 = entry2.margin;
					if (entry2.style != GUILayoutUtility.spaceStyle)
					{
						int num4;
						if (!flag)
						{
							num4 = ((num3 <= rectOffset2.left) ? rectOffset2.left : num3);
						}
						else
						{
							num4 = 0;
							flag = false;
						}
						childMinWidth += entry2.minWidth + spacing + (float)num4;
						childMaxWidth += entry2.maxWidth + spacing + (float)num4;
						num3 = rectOffset2.right;
						stretchableCountX += entry2.stretchWidth;
					}
					else
					{
						childMinWidth += entry2.minWidth;
						childMaxWidth += entry2.maxWidth;
						stretchableCountX += entry2.stretchWidth;
					}
				}
				childMinWidth -= spacing;
				childMaxWidth -= spacing;
				if (entries.Count != 0)
				{
					num = entries[0].margin.left;
					num2 = num3;
				}
				else
				{
					num = (num2 = 0);
				}
			}
			float num5 = 0f;
			float num6 = 0f;
			if (base.style != GUIStyle.none || userSpecifiedWidth)
			{
				num5 = Mathf.Max(base.style.padding.left, num);
				num6 = Mathf.Max(base.style.padding.right, num2);
			}
			else
			{
				m_Margin.left = num;
				m_Margin.right = num2;
				num5 = (num6 = 0f);
			}
			minWidth = Mathf.Max(minWidth, childMinWidth + num5 + num6);
			if (maxWidth == 0f)
			{
				stretchWidth += stretchableCountX + (base.style.stretchWidth ? 1 : 0);
				maxWidth = childMaxWidth + num5 + num6;
			}
			else
			{
				stretchWidth = 0;
			}
			maxWidth = Mathf.Max(maxWidth, minWidth);
			if (base.style.fixedWidth != 0f)
			{
				maxWidth = (minWidth = base.style.fixedWidth);
				stretchWidth = 0;
			}
		}

		public override void SetHorizontal(float x, float width)
		{
			base.SetHorizontal(x, width);
			if (resetCoords)
			{
				x = 0f;
			}
			RectOffset padding = base.style.padding;
			if (isVertical)
			{
				if (base.style != GUIStyle.none)
				{
					foreach (GUILayoutEntry entry in entries)
					{
						float num = Mathf.Max(entry.margin.left, padding.left);
						float x2 = x + num;
						float num2 = width - (float)Mathf.Max(entry.margin.right, padding.right) - num;
						if (entry.stretchWidth != 0)
						{
							entry.SetHorizontal(x2, num2);
						}
						else
						{
							entry.SetHorizontal(x2, Mathf.Clamp(num2, entry.minWidth, entry.maxWidth));
						}
					}
					return;
				}
				float num3 = x - (float)margin.left;
				float num4 = width + (float)margin.horizontal;
				{
					foreach (GUILayoutEntry entry2 in entries)
					{
						if (entry2.stretchWidth != 0)
						{
							entry2.SetHorizontal(num3 + (float)entry2.margin.left, num4 - (float)entry2.margin.horizontal);
						}
						else
						{
							entry2.SetHorizontal(num3 + (float)entry2.margin.left, Mathf.Clamp(num4 - (float)entry2.margin.horizontal, entry2.minWidth, entry2.maxWidth));
						}
					}
					return;
				}
			}
			if (base.style != GUIStyle.none)
			{
				float num5 = padding.left;
				float num6 = padding.right;
				if (entries.Count != 0)
				{
					num5 = Mathf.Max(num5, entries[0].margin.left);
					num6 = Mathf.Max(num6, entries[entries.Count - 1].margin.right);
				}
				x += num5;
				width -= num6 + num5;
			}
			float num7 = width - spacing * (float)(entries.Count - 1);
			float t = 0f;
			if (childMinWidth != childMaxWidth)
			{
				t = Mathf.Clamp((num7 - childMinWidth) / (childMaxWidth - childMinWidth), 0f, 1f);
			}
			float num8 = 0f;
			if (num7 > childMaxWidth && stretchableCountX > 0)
			{
				num8 = (num7 - childMaxWidth) / (float)stretchableCountX;
			}
			int num9 = 0;
			bool flag = true;
			foreach (GUILayoutEntry entry3 in entries)
			{
				float num10 = Mathf.Lerp(entry3.minWidth, entry3.maxWidth, t);
				num10 += num8 * (float)entry3.stretchWidth;
				if (entry3.style != GUILayoutUtility.spaceStyle)
				{
					int num11 = entry3.margin.left;
					if (flag)
					{
						num11 = 0;
						flag = false;
					}
					int num12 = ((num9 <= num11) ? num11 : num9);
					x += (float)num12;
					num9 = entry3.margin.right;
				}
				entry3.SetHorizontal(Mathf.Round(x), Mathf.Round(num10));
				x += num10 + spacing;
			}
		}

		public override void CalcHeight()
		{
			if (entries.Count == 0)
			{
				maxHeight = (minHeight = base.style.padding.vertical);
				return;
			}
			childMinHeight = (childMaxHeight = 0f);
			int num = 0;
			int num2 = 0;
			stretchableCountY = 0;
			if (isVertical)
			{
				int num3 = 0;
				bool flag = true;
				foreach (GUILayoutEntry entry in entries)
				{
					entry.CalcHeight();
					RectOffset rectOffset = entry.margin;
					if (entry.style != GUILayoutUtility.spaceStyle)
					{
						int num4;
						if (!flag)
						{
							num4 = Mathf.Max(num3, rectOffset.top);
						}
						else
						{
							num4 = 0;
							flag = false;
						}
						childMinHeight += entry.minHeight + spacing + (float)num4;
						childMaxHeight += entry.maxHeight + spacing + (float)num4;
						num3 = rectOffset.bottom;
						stretchableCountY += entry.stretchHeight;
					}
					else
					{
						childMinHeight += entry.minHeight;
						childMaxHeight += entry.maxHeight;
						stretchableCountY += entry.stretchHeight;
					}
				}
				childMinHeight -= spacing;
				childMaxHeight -= spacing;
				if (entries.Count != 0)
				{
					num = entries[0].margin.top;
					num2 = num3;
				}
				else
				{
					num2 = (num = 0);
				}
			}
			else
			{
				bool flag2 = true;
				foreach (GUILayoutEntry entry2 in entries)
				{
					entry2.CalcHeight();
					RectOffset rectOffset2 = entry2.margin;
					if (entry2.style != GUILayoutUtility.spaceStyle)
					{
						if (!flag2)
						{
							num = Mathf.Min(rectOffset2.top, num);
							num2 = Mathf.Min(rectOffset2.bottom, num2);
						}
						else
						{
							num = rectOffset2.top;
							num2 = rectOffset2.bottom;
							flag2 = false;
						}
						childMinHeight = Mathf.Max(entry2.minHeight, childMinHeight);
						childMaxHeight = Mathf.Max(entry2.maxHeight, childMaxHeight);
					}
					stretchableCountY += entry2.stretchHeight;
				}
			}
			float num5 = 0f;
			float num6 = 0f;
			if (base.style != GUIStyle.none || userSpecifiedHeight)
			{
				num5 = Mathf.Max(base.style.padding.top, num);
				num6 = Mathf.Max(base.style.padding.bottom, num2);
			}
			else
			{
				m_Margin.top = num;
				m_Margin.bottom = num2;
				num5 = (num6 = 0f);
			}
			minHeight = Mathf.Max(minHeight, childMinHeight + num5 + num6);
			if (maxHeight == 0f)
			{
				stretchHeight += stretchableCountY + (base.style.stretchHeight ? 1 : 0);
				maxHeight = childMaxHeight + num5 + num6;
			}
			else
			{
				stretchHeight = 0;
			}
			maxHeight = Mathf.Max(maxHeight, minHeight);
			if (base.style.fixedHeight != 0f)
			{
				maxHeight = (minHeight = base.style.fixedHeight);
				stretchHeight = 0;
			}
		}

		public override void SetVertical(float y, float height)
		{
			base.SetVertical(y, height);
			if (entries.Count == 0)
			{
				return;
			}
			RectOffset padding = base.style.padding;
			if (resetCoords)
			{
				y = 0f;
			}
			if (isVertical)
			{
				if (base.style != GUIStyle.none)
				{
					float num = padding.top;
					float num2 = padding.bottom;
					if (entries.Count != 0)
					{
						num = Mathf.Max(num, entries[0].margin.top);
						num2 = Mathf.Max(num2, entries[entries.Count - 1].margin.bottom);
					}
					y += num;
					height -= num2 + num;
				}
				float num3 = height - spacing * (float)(entries.Count - 1);
				float t = 0f;
				if (childMinHeight != childMaxHeight)
				{
					t = Mathf.Clamp((num3 - childMinHeight) / (childMaxHeight - childMinHeight), 0f, 1f);
				}
				float num4 = 0f;
				if (num3 > childMaxHeight && stretchableCountY > 0)
				{
					num4 = (num3 - childMaxHeight) / (float)stretchableCountY;
				}
				int num5 = 0;
				bool flag = true;
				{
					foreach (GUILayoutEntry entry in entries)
					{
						float num6 = Mathf.Lerp(entry.minHeight, entry.maxHeight, t);
						num6 += num4 * (float)entry.stretchHeight;
						if (entry.style != GUILayoutUtility.spaceStyle)
						{
							int num7 = entry.margin.top;
							if (flag)
							{
								num7 = 0;
								flag = false;
							}
							int num8 = ((num5 <= num7) ? num7 : num5);
							y += (float)num8;
							num5 = entry.margin.bottom;
						}
						entry.SetVertical(Mathf.Round(y), Mathf.Round(num6));
						y += num6 + spacing;
					}
					return;
				}
			}
			if (base.style != GUIStyle.none)
			{
				foreach (GUILayoutEntry entry2 in entries)
				{
					float num9 = Mathf.Max(entry2.margin.top, padding.top);
					float y2 = y + num9;
					float num10 = height - (float)Mathf.Max(entry2.margin.bottom, padding.bottom) - num9;
					if (entry2.stretchHeight != 0)
					{
						entry2.SetVertical(y2, num10);
					}
					else
					{
						entry2.SetVertical(y2, Mathf.Clamp(num10, entry2.minHeight, entry2.maxHeight));
					}
				}
				return;
			}
			float num11 = y - (float)margin.top;
			float num12 = height + (float)margin.vertical;
			foreach (GUILayoutEntry entry3 in entries)
			{
				if (entry3.stretchHeight != 0)
				{
					entry3.SetVertical(num11 + (float)entry3.margin.top, num12 - (float)entry3.margin.vertical);
				}
				else
				{
					entry3.SetVertical(num11 + (float)entry3.margin.top, Mathf.Clamp(num12 - (float)entry3.margin.vertical, entry3.minHeight, entry3.maxHeight));
				}
			}
		}

		public override string ToString()
		{
			string empty = string.Empty;
			string text = string.Empty;
			for (int i = 0; i < GUILayoutEntry.indent; i++)
			{
				text += " ";
			}
			string text2 = empty;
			empty = text2 + base.ToString() + " Margins: " + childMinHeight + " {\n";
			GUILayoutEntry.indent += 4;
			foreach (GUILayoutEntry entry in entries)
			{
				empty = empty + entry.ToString() + "\n";
			}
			empty = empty + text + "}";
			GUILayoutEntry.indent -= 4;
			return empty;
		}
	}
}

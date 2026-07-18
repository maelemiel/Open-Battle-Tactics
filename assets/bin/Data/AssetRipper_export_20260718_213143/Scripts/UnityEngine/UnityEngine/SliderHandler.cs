namespace UnityEngine
{
	internal struct SliderHandler
	{
		private readonly Rect position;

		private readonly float currentValue;

		private readonly float size;

		private readonly float start;

		private readonly float end;

		private readonly GUIStyle slider;

		private readonly GUIStyle thumb;

		private readonly bool horiz;

		private readonly int id;

		public SliderHandler(Rect position, float currentValue, float size, float start, float end, GUIStyle slider, GUIStyle thumb, bool horiz, int id)
		{
			this.position = position;
			this.currentValue = currentValue;
			this.size = size;
			this.start = start;
			this.end = end;
			this.slider = slider;
			this.thumb = thumb;
			this.horiz = horiz;
			this.id = id;
		}

		public float Handle()
		{
			if (slider == null || thumb == null)
			{
				return currentValue;
			}
			switch (CurrentEventType())
			{
			case EventType.MouseDown:
				return OnMouseDown();
			case EventType.MouseDrag:
				return OnMouseDrag();
			case EventType.MouseUp:
				return OnMouseUp();
			case EventType.Repaint:
				return OnRepaint();
			default:
				return currentValue;
			}
		}

		private float OnMouseDown()
		{
			if (!position.Contains(CurrentEvent().mousePosition) || IsEmptySlider())
			{
				return currentValue;
			}
			GUI.scrollTroughSide = 0;
			GUIUtility.hotControl = id;
			CurrentEvent().Use();
			if (ThumbSelectionRect().Contains(CurrentEvent().mousePosition))
			{
				StartDraggingWithValue(ClampedCurrentValue());
				return currentValue;
			}
			GUI.changed = true;
			if (SupportsPageMovements())
			{
				SliderState().isDragging = false;
				GUI.nextScrollStepTime = SystemClock.now.AddMilliseconds(250.0);
				GUI.scrollTroughSide = CurrentScrollTroughSide();
				return PageMovementValue();
			}
			float num = ValueForCurrentMousePosition();
			StartDraggingWithValue(num);
			return Clamp(num);
		}

		private float OnMouseDrag()
		{
			if (GUIUtility.hotControl != id)
			{
				return currentValue;
			}
			SliderState sliderState = SliderState();
			if (!sliderState.isDragging)
			{
				return currentValue;
			}
			GUI.changed = true;
			CurrentEvent().Use();
			float num = MousePosition() - sliderState.dragStartPos;
			float value = sliderState.dragStartValue + num / ValuesPerPixel();
			return Clamp(value);
		}

		private float OnMouseUp()
		{
			if (GUIUtility.hotControl == id)
			{
				CurrentEvent().Use();
				GUIUtility.hotControl = 0;
			}
			return currentValue;
		}

		private float OnRepaint()
		{
			slider.Draw(position, GUIContent.none, id);
			thumb.Draw(ThumbRect(), GUIContent.none, id);
			if (GUIUtility.hotControl != id || !position.Contains(CurrentEvent().mousePosition) || IsEmptySlider())
			{
				return currentValue;
			}
			if (ThumbRect().Contains(CurrentEvent().mousePosition))
			{
				if (GUI.scrollTroughSide != 0)
				{
					GUIUtility.hotControl = 0;
				}
				return currentValue;
			}
			GUI.InternalRepaintEditorWindow();
			if (SystemClock.now < GUI.nextScrollStepTime)
			{
				return currentValue;
			}
			if (CurrentScrollTroughSide() != GUI.scrollTroughSide)
			{
				return currentValue;
			}
			GUI.nextScrollStepTime = SystemClock.now.AddMilliseconds(30.0);
			if (SupportsPageMovements())
			{
				SliderState().isDragging = false;
				GUI.changed = true;
				return PageMovementValue();
			}
			return ClampedCurrentValue();
		}

		private EventType CurrentEventType()
		{
			return CurrentEvent().GetTypeForControl(id);
		}

		private int CurrentScrollTroughSide()
		{
			float num = ((!horiz) ? CurrentEvent().mousePosition.y : CurrentEvent().mousePosition.x);
			float num2 = ((!horiz) ? ThumbRect().y : ThumbRect().x);
			return (num > num2) ? 1 : (-1);
		}

		private bool IsEmptySlider()
		{
			return start == end;
		}

		private bool SupportsPageMovements()
		{
			return size != 0f && GUI.usePageScrollbars;
		}

		private float PageMovementValue()
		{
			float num = currentValue;
			int num2 = ((!(start > end)) ? 1 : (-1));
			num = ((!(MousePosition() > PageUpMovementBound())) ? (num - size * (float)num2 * 0.9f) : (num + size * (float)num2 * 0.9f));
			return Clamp(num);
		}

		private float PageUpMovementBound()
		{
			if (horiz)
			{
				return ThumbRect().xMax - position.x;
			}
			return ThumbRect().yMax - position.y;
		}

		private Event CurrentEvent()
		{
			return Event.current;
		}

		private float ValueForCurrentMousePosition()
		{
			if (horiz)
			{
				return (MousePosition() - ThumbRect().width * 0.5f) / ValuesPerPixel() + start - size * 0.5f;
			}
			return (MousePosition() - ThumbRect().height * 0.5f) / ValuesPerPixel() + start - size * 0.5f;
		}

		private float Clamp(float value)
		{
			return Mathf.Clamp(value, MinValue(), MaxValue());
		}

		private Rect ThumbSelectionRect()
		{
			return ThumbRect();
		}

		private void StartDraggingWithValue(float dragStartValue)
		{
			SliderState sliderState = SliderState();
			sliderState.dragStartPos = MousePosition();
			sliderState.dragStartValue = dragStartValue;
			sliderState.isDragging = true;
		}

		private SliderState SliderState()
		{
			return (SliderState)GUIUtility.GetStateObject(typeof(SliderState), id);
		}

		private Rect ThumbRect()
		{
			return (!horiz) ? VerticalThumbRect() : HorizontalThumbRect();
		}

		private Rect VerticalThumbRect()
		{
			float num = ValuesPerPixel();
			if (start < end)
			{
				return new Rect(position.x + (float)slider.padding.left, (ClampedCurrentValue() - start) * num + position.y + (float)slider.padding.top, position.width - (float)slider.padding.horizontal, size * num + ThumbSize());
			}
			return new Rect(position.x + (float)slider.padding.left, (ClampedCurrentValue() + size - start) * num + position.y + (float)slider.padding.top, position.width - (float)slider.padding.horizontal, size * (0f - num) + ThumbSize());
		}

		private Rect HorizontalThumbRect()
		{
			float num = ValuesPerPixel();
			if (start < end)
			{
				return new Rect((ClampedCurrentValue() - start) * num + position.x + (float)slider.padding.left, position.y + (float)slider.padding.top, size * num + ThumbSize(), position.height - (float)slider.padding.vertical);
			}
			return new Rect((ClampedCurrentValue() + size - start) * num + position.x + (float)slider.padding.left, position.y, size * (0f - num) + ThumbSize(), position.height);
		}

		private float ClampedCurrentValue()
		{
			return Clamp(currentValue);
		}

		private float MousePosition()
		{
			if (horiz)
			{
				return CurrentEvent().mousePosition.x - position.x;
			}
			return CurrentEvent().mousePosition.y - position.y;
		}

		private float ValuesPerPixel()
		{
			if (horiz)
			{
				return (position.width - (float)slider.padding.horizontal - ThumbSize()) / (end - start);
			}
			return (position.height - (float)slider.padding.vertical - ThumbSize()) / (end - start);
		}

		private float ThumbSize()
		{
			if (horiz)
			{
				return (thumb.fixedWidth == 0f) ? ((float)thumb.padding.horizontal) : thumb.fixedWidth;
			}
			return (thumb.fixedHeight == 0f) ? ((float)thumb.padding.vertical) : thumb.fixedHeight;
		}

		private float MaxValue()
		{
			return Mathf.Max(start, end) - size;
		}

		private float MinValue()
		{
			return Mathf.Min(start, end);
		}
	}
}

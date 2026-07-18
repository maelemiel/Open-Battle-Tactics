using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityEngine
{
	[Serializable]
	[ExecuteInEditMode]
	public sealed class GUISkin : ScriptableObject
	{
		internal delegate void SkinChangedDelegate();

		[SerializeField]
		private Font m_Font;

		[SerializeField]
		private GUIStyle m_box;

		[SerializeField]
		private GUIStyle m_button;

		[SerializeField]
		private GUIStyle m_toggle;

		[SerializeField]
		private GUIStyle m_label;

		[SerializeField]
		private GUIStyle m_textField;

		[SerializeField]
		private GUIStyle m_textArea;

		[SerializeField]
		private GUIStyle m_window;

		[SerializeField]
		private GUIStyle m_horizontalSlider;

		[SerializeField]
		private GUIStyle m_horizontalSliderThumb;

		[SerializeField]
		private GUIStyle m_verticalSlider;

		[SerializeField]
		private GUIStyle m_verticalSliderThumb;

		[SerializeField]
		private GUIStyle m_horizontalScrollbar;

		[SerializeField]
		private GUIStyle m_horizontalScrollbarThumb;

		[SerializeField]
		private GUIStyle m_horizontalScrollbarLeftButton;

		[SerializeField]
		private GUIStyle m_horizontalScrollbarRightButton;

		[SerializeField]
		private GUIStyle m_verticalScrollbar;

		[SerializeField]
		private GUIStyle m_verticalScrollbarThumb;

		[SerializeField]
		private GUIStyle m_verticalScrollbarUpButton;

		[SerializeField]
		private GUIStyle m_verticalScrollbarDownButton;

		[SerializeField]
		private GUIStyle m_ScrollView;

		[SerializeField]
		internal GUIStyle[] m_CustomStyles;

		[SerializeField]
		private GUISettings m_Settings = new GUISettings();

		internal static GUIStyle ms_Error;

		private Dictionary<string, GUIStyle> styles;

		internal static SkinChangedDelegate m_SkinChanged;

		internal static GUISkin current;

		public Font font
		{
			get
			{
				return m_Font;
			}
			set
			{
				m_Font = value;
				if (current == this)
				{
					GUIStyle.SetDefaultFont(m_Font);
				}
				Apply();
			}
		}

		public GUIStyle box
		{
			get
			{
				return m_box;
			}
			set
			{
				m_box = value;
				Apply();
			}
		}

		public GUIStyle label
		{
			get
			{
				return m_label;
			}
			set
			{
				m_label = value;
				Apply();
			}
		}

		public GUIStyle textField
		{
			get
			{
				return m_textField;
			}
			set
			{
				m_textField = value;
				Apply();
			}
		}

		public GUIStyle textArea
		{
			get
			{
				return m_textArea;
			}
			set
			{
				m_textArea = value;
				Apply();
			}
		}

		public GUIStyle button
		{
			get
			{
				return m_button;
			}
			set
			{
				m_button = value;
				Apply();
			}
		}

		public GUIStyle toggle
		{
			get
			{
				return m_toggle;
			}
			set
			{
				m_toggle = value;
				Apply();
			}
		}

		public GUIStyle window
		{
			get
			{
				return m_window;
			}
			set
			{
				m_window = value;
				Apply();
			}
		}

		public GUIStyle horizontalSlider
		{
			get
			{
				return m_horizontalSlider;
			}
			set
			{
				m_horizontalSlider = value;
				Apply();
			}
		}

		public GUIStyle horizontalSliderThumb
		{
			get
			{
				return m_horizontalSliderThumb;
			}
			set
			{
				m_horizontalSliderThumb = value;
				Apply();
			}
		}

		public GUIStyle verticalSlider
		{
			get
			{
				return m_verticalSlider;
			}
			set
			{
				m_verticalSlider = value;
				Apply();
			}
		}

		public GUIStyle verticalSliderThumb
		{
			get
			{
				return m_verticalSliderThumb;
			}
			set
			{
				m_verticalSliderThumb = value;
				Apply();
			}
		}

		public GUIStyle horizontalScrollbar
		{
			get
			{
				return m_horizontalScrollbar;
			}
			set
			{
				m_horizontalScrollbar = value;
				Apply();
			}
		}

		public GUIStyle horizontalScrollbarThumb
		{
			get
			{
				return m_horizontalScrollbarThumb;
			}
			set
			{
				m_horizontalScrollbarThumb = value;
				Apply();
			}
		}

		public GUIStyle horizontalScrollbarLeftButton
		{
			get
			{
				return m_horizontalScrollbarLeftButton;
			}
			set
			{
				m_horizontalScrollbarLeftButton = value;
				Apply();
			}
		}

		public GUIStyle horizontalScrollbarRightButton
		{
			get
			{
				return m_horizontalScrollbarRightButton;
			}
			set
			{
				m_horizontalScrollbarRightButton = value;
				Apply();
			}
		}

		public GUIStyle verticalScrollbar
		{
			get
			{
				return m_verticalScrollbar;
			}
			set
			{
				m_verticalScrollbar = value;
				Apply();
			}
		}

		public GUIStyle verticalScrollbarThumb
		{
			get
			{
				return m_verticalScrollbarThumb;
			}
			set
			{
				m_verticalScrollbarThumb = value;
				Apply();
			}
		}

		public GUIStyle verticalScrollbarUpButton
		{
			get
			{
				return m_verticalScrollbarUpButton;
			}
			set
			{
				m_verticalScrollbarUpButton = value;
				Apply();
			}
		}

		public GUIStyle verticalScrollbarDownButton
		{
			get
			{
				return m_verticalScrollbarDownButton;
			}
			set
			{
				m_verticalScrollbarDownButton = value;
				Apply();
			}
		}

		public GUIStyle scrollView
		{
			get
			{
				return m_ScrollView;
			}
			set
			{
				m_ScrollView = value;
				Apply();
			}
		}

		public GUIStyle[] customStyles
		{
			get
			{
				return m_CustomStyles;
			}
			set
			{
				m_CustomStyles = value;
				Apply();
			}
		}

		public GUISettings settings
		{
			get
			{
				return m_Settings;
			}
		}

		internal static GUIStyle error
		{
			get
			{
				if (ms_Error == null)
				{
					ms_Error = new GUIStyle();
				}
				return ms_Error;
			}
		}

		public GUISkin()
		{
			m_CustomStyles = new GUIStyle[1];
		}

		internal void OnEnable()
		{
			Apply();
		}

		internal void Apply()
		{
			if (m_CustomStyles == null)
			{
				Debug.Log("custom styles is null");
			}
			BuildStyleCache();
		}

		private void BuildStyleCache()
		{
			if (m_box == null)
			{
				m_box = new GUIStyle();
			}
			if (m_button == null)
			{
				m_button = new GUIStyle();
			}
			if (m_toggle == null)
			{
				m_toggle = new GUIStyle();
			}
			if (m_label == null)
			{
				m_label = new GUIStyle();
			}
			if (m_window == null)
			{
				m_window = new GUIStyle();
			}
			if (m_textField == null)
			{
				m_textField = new GUIStyle();
			}
			if (m_textArea == null)
			{
				m_textArea = new GUIStyle();
			}
			if (m_horizontalSlider == null)
			{
				m_horizontalSlider = new GUIStyle();
			}
			if (m_horizontalSliderThumb == null)
			{
				m_horizontalSliderThumb = new GUIStyle();
			}
			if (m_verticalSlider == null)
			{
				m_verticalSlider = new GUIStyle();
			}
			if (m_verticalSliderThumb == null)
			{
				m_verticalSliderThumb = new GUIStyle();
			}
			if (m_horizontalScrollbar == null)
			{
				m_horizontalScrollbar = new GUIStyle();
			}
			if (m_horizontalScrollbarThumb == null)
			{
				m_horizontalScrollbarThumb = new GUIStyle();
			}
			if (m_horizontalScrollbarLeftButton == null)
			{
				m_horizontalScrollbarLeftButton = new GUIStyle();
			}
			if (m_horizontalScrollbarRightButton == null)
			{
				m_horizontalScrollbarRightButton = new GUIStyle();
			}
			if (m_verticalScrollbar == null)
			{
				m_verticalScrollbar = new GUIStyle();
			}
			if (m_verticalScrollbarThumb == null)
			{
				m_verticalScrollbarThumb = new GUIStyle();
			}
			if (m_verticalScrollbarUpButton == null)
			{
				m_verticalScrollbarUpButton = new GUIStyle();
			}
			if (m_verticalScrollbarDownButton == null)
			{
				m_verticalScrollbarDownButton = new GUIStyle();
			}
			if (m_ScrollView == null)
			{
				m_ScrollView = new GUIStyle();
			}
			styles = new Dictionary<string, GUIStyle>(StringComparer.OrdinalIgnoreCase);
			styles["box"] = m_box;
			m_box.name = "box";
			styles["button"] = m_button;
			m_button.name = "button";
			styles["toggle"] = m_toggle;
			m_toggle.name = "toggle";
			styles["label"] = m_label;
			m_label.name = "label";
			styles["window"] = m_window;
			m_window.name = "window";
			styles["textfield"] = m_textField;
			m_textField.name = "textfield";
			styles["textarea"] = m_textArea;
			m_textArea.name = "textarea";
			styles["horizontalslider"] = m_horizontalSlider;
			m_horizontalSlider.name = "horizontalslider";
			styles["horizontalsliderthumb"] = m_horizontalSliderThumb;
			m_horizontalSliderThumb.name = "horizontalsliderthumb";
			styles["verticalslider"] = m_verticalSlider;
			m_verticalSlider.name = "verticalslider";
			styles["verticalsliderthumb"] = m_verticalSliderThumb;
			m_verticalSliderThumb.name = "verticalsliderthumb";
			styles["horizontalscrollbar"] = m_horizontalScrollbar;
			m_horizontalScrollbar.name = "horizontalscrollbar";
			styles["horizontalscrollbarthumb"] = m_horizontalScrollbarThumb;
			m_horizontalScrollbarThumb.name = "horizontalscrollbarthumb";
			styles["horizontalscrollbarleftbutton"] = m_horizontalScrollbarLeftButton;
			m_horizontalScrollbarLeftButton.name = "horizontalscrollbarleftbutton";
			styles["horizontalscrollbarrightbutton"] = m_horizontalScrollbarRightButton;
			m_horizontalScrollbarRightButton.name = "horizontalscrollbarrightbutton";
			styles["verticalscrollbar"] = m_verticalScrollbar;
			m_verticalScrollbar.name = "verticalscrollbar";
			styles["verticalscrollbarthumb"] = m_verticalScrollbarThumb;
			m_verticalScrollbarThumb.name = "verticalscrollbarthumb";
			styles["verticalscrollbarupbutton"] = m_verticalScrollbarUpButton;
			m_verticalScrollbarUpButton.name = "verticalscrollbarupbutton";
			styles["verticalscrollbardownbutton"] = m_verticalScrollbarDownButton;
			m_verticalScrollbarDownButton.name = "verticalscrollbardownbutton";
			styles["scrollview"] = m_ScrollView;
			m_ScrollView.name = "scrollview";
			if (m_CustomStyles != null)
			{
				for (int i = 0; i < m_CustomStyles.Length; i++)
				{
					if (m_CustomStyles[i] != null)
					{
						styles[m_CustomStyles[i].name] = m_CustomStyles[i];
					}
				}
			}
			error.stretchHeight = true;
			error.normal.textColor = Color.red;
		}

		public GUIStyle GetStyle(string styleName)
		{
			GUIStyle gUIStyle = FindStyle(styleName);
			if (gUIStyle != null)
			{
				return gUIStyle;
			}
			Debug.LogWarning("Unable to find style '" + styleName + "' in skin '" + base.name + "' " + Event.current.type);
			return error;
		}

		public GUIStyle FindStyle(string styleName)
		{
			if (this == null)
			{
				Debug.LogError("GUISkin is NULL");
				return null;
			}
			if (styles == null)
			{
				BuildStyleCache();
			}
			GUIStyle value;
			if (styles.TryGetValue(styleName, out value))
			{
				return value;
			}
			return null;
		}

		internal void MakeCurrent()
		{
			current = this;
			GUIStyle.SetDefaultFont(font);
			if (m_SkinChanged != null)
			{
				m_SkinChanged();
			}
		}

		public IEnumerator GetEnumerator()
		{
			if (styles == null)
			{
				BuildStyleCache();
			}
			return styles.Values.GetEnumerator();
		}
	}
}

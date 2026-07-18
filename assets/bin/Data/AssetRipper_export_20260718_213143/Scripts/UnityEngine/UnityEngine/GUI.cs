using System;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;
using UnityEngineInternal;

namespace UnityEngine
{
	public class GUI
	{
		internal sealed class ScrollViewState
		{
			public Rect position;

			public Rect visibleRect;

			public Rect viewRect;

			public Vector2 scrollPosition;

			public bool apply;

			public bool hasScrollTo;

			internal void ScrollTo(Rect position)
			{
				ScrollTowards(position, float.PositiveInfinity);
			}

			internal bool ScrollTowards(Rect position, float maxDelta)
			{
				Vector2 vector = ScrollNeeded(position);
				if (vector.sqrMagnitude < 0.0001f)
				{
					return false;
				}
				if (maxDelta == 0f)
				{
					return true;
				}
				if (vector.magnitude > maxDelta)
				{
					vector = vector.normalized * maxDelta;
				}
				scrollPosition += vector;
				apply = true;
				return true;
			}

			internal Vector2 ScrollNeeded(Rect position)
			{
				Rect rect = visibleRect;
				rect.x += scrollPosition.x;
				rect.y += scrollPosition.y;
				float num = position.width - visibleRect.width;
				if (num > 0f)
				{
					position.width -= num;
					position.x += num * 0.5f;
				}
				num = position.height - visibleRect.height;
				if (num > 0f)
				{
					position.height -= num;
					position.y += num * 0.5f;
				}
				Vector2 zero = Vector2.zero;
				if (position.xMax > rect.xMax)
				{
					zero.x += position.xMax - rect.xMax;
				}
				else if (position.xMin < rect.xMin)
				{
					zero.x -= rect.xMin - position.xMin;
				}
				if (position.yMax > rect.yMax)
				{
					zero.y += position.yMax - rect.yMax;
				}
				else if (position.yMin < rect.yMin)
				{
					zero.y -= rect.yMin - position.yMin;
				}
				Rect rect2 = viewRect;
				rect2.width = Mathf.Max(rect2.width, visibleRect.width);
				rect2.height = Mathf.Max(rect2.height, visibleRect.height);
				zero.x = Mathf.Clamp(zero.x, rect2.xMin - scrollPosition.x, rect2.xMax - visibleRect.width - scrollPosition.x);
				zero.y = Mathf.Clamp(zero.y, rect2.yMin - scrollPosition.y, rect2.yMax - visibleRect.height - scrollPosition.y);
				return zero;
			}
		}

		public delegate void WindowFunction(int id);

		private static float scrollStepSize;

		private static int scrollControlID;

		private static int hotTextField;

		private static GUISkin s_Skin;

		internal static Rect s_ToolTipRect;

		private static int boxHash;

		private static int repeatButtonHash;

		private static int toggleHash;

		private static int buttonGridHash;

		private static int sliderHash;

		private static int beginGroupHash;

		private static int scrollviewHash;

		private static GenericStack s_ScrollViewStates;

		internal static DateTime nextScrollStepTime { get; set; }

		internal static int scrollTroughSide { get; set; }

		public static GUISkin skin
		{
			get
			{
				GUIUtility.CheckOnGUI();
				return s_Skin;
			}
			set
			{
				GUIUtility.CheckOnGUI();
				if (!value)
				{
					value = GUIUtility.GetDefaultSkin();
				}
				s_Skin = value;
				value.MakeCurrent();
			}
		}

		public static Color color
		{
			get
			{
				Color value;
				INTERNAL_get_color(out value);
				return value;
			}
			set
			{
				INTERNAL_set_color(ref value);
			}
		}

		public static Color backgroundColor
		{
			get
			{
				Color value;
				INTERNAL_get_backgroundColor(out value);
				return value;
			}
			set
			{
				INTERNAL_set_backgroundColor(ref value);
			}
		}

		public static Color contentColor
		{
			get
			{
				Color value;
				INTERNAL_get_contentColor(out value);
				return value;
			}
			set
			{
				INTERNAL_set_contentColor(ref value);
			}
		}

		public static extern bool changed
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public static extern bool enabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public static Matrix4x4 matrix
		{
			get
			{
				return GUIClip.GetMatrix();
			}
			set
			{
				GUIClip.SetMatrix(value);
			}
		}

		public static string tooltip
		{
			get
			{
				string text = Internal_GetTooltip();
				if (text != null)
				{
					return text;
				}
				return string.Empty;
			}
			set
			{
				Internal_SetTooltip(value);
			}
		}

		protected static string mouseTooltip
		{
			get
			{
				return Internal_GetMouseTooltip();
			}
		}

		protected static Rect tooltipRect
		{
			get
			{
				return s_ToolTipRect;
			}
			set
			{
				s_ToolTipRect = value;
			}
		}

		public static extern int depth
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		private static extern Material blendMaterial
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		private static extern Material blitMaterial
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		internal static extern bool usePageScrollbars
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		static GUI()
		{
			scrollStepSize = 10f;
			hotTextField = -1;
			boxHash = "Box".GetHashCode();
			repeatButtonHash = "repeatButton".GetHashCode();
			toggleHash = "Toggle".GetHashCode();
			buttonGridHash = "ButtonGrid".GetHashCode();
			sliderHash = "Slider".GetHashCode();
			beginGroupHash = "BeginGroup".GetHashCode();
			scrollviewHash = "scrollView".GetHashCode();
			s_ScrollViewStates = new GenericStack();
			nextScrollStepTime = DateTime.Now;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_get_color(out Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_set_color(ref Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_get_backgroundColor(out Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_set_backgroundColor(ref Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_get_contentColor(out Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_set_contentColor(ref Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern string Internal_GetTooltip();

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void Internal_SetTooltip(string value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern string Internal_GetMouseTooltip();

		public static void Label(Rect position, string text)
		{
			Label(position, GUIContent.Temp(text), s_Skin.label);
		}

		public static void Label(Rect position, Texture image)
		{
			Label(position, GUIContent.Temp(image), s_Skin.label);
		}

		public static void Label(Rect position, GUIContent content)
		{
			Label(position, content, s_Skin.label);
		}

		public static void Label(Rect position, string text, GUIStyle style)
		{
			Label(position, GUIContent.Temp(text), style);
		}

		public static void Label(Rect position, Texture image, GUIStyle style)
		{
			Label(position, GUIContent.Temp(image), style);
		}

		public static void Label(Rect position, GUIContent content, GUIStyle style)
		{
			DoLabel(position, content, style.m_Ptr);
		}

		private static void DoLabel(Rect position, GUIContent content, IntPtr style)
		{
			INTERNAL_CALL_DoLabel(ref position, content, style);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_DoLabel(ref Rect position, GUIContent content, IntPtr style);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void InitializeGUIClipTexture();

		[ExcludeFromDocs]
		public static void DrawTexture(Rect position, Texture image, ScaleMode scaleMode, bool alphaBlend)
		{
			float imageAspect = 0f;
			DrawTexture(position, image, scaleMode, alphaBlend, imageAspect);
		}

		[ExcludeFromDocs]
		public static void DrawTexture(Rect position, Texture image, ScaleMode scaleMode)
		{
			float imageAspect = 0f;
			bool alphaBlend = true;
			DrawTexture(position, image, scaleMode, alphaBlend, imageAspect);
		}

		[ExcludeFromDocs]
		public static void DrawTexture(Rect position, Texture image)
		{
			float imageAspect = 0f;
			bool alphaBlend = true;
			ScaleMode scaleMode = ScaleMode.StretchToFill;
			DrawTexture(position, image, scaleMode, alphaBlend, imageAspect);
		}

		public static void DrawTexture(Rect position, Texture image, [DefaultValue("ScaleMode.StretchToFill")] ScaleMode scaleMode, [DefaultValue("true")] bool alphaBlend, [DefaultValue("0")] float imageAspect)
		{
			if (Event.current.type != EventType.Repaint)
			{
				return;
			}
			if (image == null)
			{
				Debug.LogWarning("null texture passed to GUI.DrawTexture");
				return;
			}
			if (imageAspect == 0f)
			{
				imageAspect = (float)image.width / (float)image.height;
			}
			Material mat = ((!alphaBlend) ? blitMaterial : blendMaterial);
			float num = position.width / position.height;
			InternalDrawTextureArguments arguments = new InternalDrawTextureArguments
			{
				texture = image,
				leftBorder = 0,
				rightBorder = 0,
				topBorder = 0,
				bottomBorder = 0,
				color = color,
				mat = mat
			};
			switch (scaleMode)
			{
			case ScaleMode.StretchToFill:
				arguments.screenRect = position;
				arguments.sourceRect = new Rect(0f, 0f, 1f, 1f);
				Graphics.DrawTexture(ref arguments);
				break;
			case ScaleMode.ScaleAndCrop:
				if (num > imageAspect)
				{
					float num4 = imageAspect / num;
					arguments.screenRect = position;
					arguments.sourceRect = new Rect(0f, (1f - num4) * 0.5f, 1f, num4);
					Graphics.DrawTexture(ref arguments);
				}
				else
				{
					float num5 = num / imageAspect;
					arguments.screenRect = position;
					arguments.sourceRect = new Rect(0.5f - num5 * 0.5f, 0f, num5, 1f);
					Graphics.DrawTexture(ref arguments);
				}
				break;
			case ScaleMode.ScaleToFit:
				if (num > imageAspect)
				{
					float num2 = imageAspect / num;
					arguments.screenRect = new Rect(position.xMin + position.width * (1f - num2) * 0.5f, position.yMin, num2 * position.width, position.height);
					arguments.sourceRect = new Rect(0f, 0f, 1f, 1f);
					Graphics.DrawTexture(ref arguments);
				}
				else
				{
					float num3 = num / imageAspect;
					arguments.screenRect = new Rect(position.xMin, position.yMin + position.height * (1f - num3) * 0.5f, position.width, num3 * position.height);
					arguments.sourceRect = new Rect(0f, 0f, 1f, 1f);
					Graphics.DrawTexture(ref arguments);
				}
				break;
			}
		}

		internal static bool CalculateScaledTextureRects(Rect position, ScaleMode scaleMode, float imageAspect, ref Rect outScreenRect, ref Rect outSourceRect)
		{
			float num = position.width / position.height;
			bool result = false;
			switch (scaleMode)
			{
			case ScaleMode.StretchToFill:
				outScreenRect = position;
				outSourceRect = new Rect(0f, 0f, 1f, 1f);
				result = true;
				break;
			case ScaleMode.ScaleAndCrop:
				if (num > imageAspect)
				{
					float num4 = imageAspect / num;
					outScreenRect = position;
					outSourceRect = new Rect(0f, (1f - num4) * 0.5f, 1f, num4);
					result = true;
				}
				else
				{
					float num5 = num / imageAspect;
					outScreenRect = position;
					outSourceRect = new Rect(0.5f - num5 * 0.5f, 0f, num5, 1f);
					result = true;
				}
				break;
			case ScaleMode.ScaleToFit:
				if (num > imageAspect)
				{
					float num2 = imageAspect / num;
					outScreenRect = new Rect(position.xMin + position.width * (1f - num2) * 0.5f, position.yMin, num2 * position.width, position.height);
					outSourceRect = new Rect(0f, 0f, 1f, 1f);
					result = true;
				}
				else
				{
					float num3 = num / imageAspect;
					outScreenRect = new Rect(position.xMin, position.yMin + position.height * (1f - num3) * 0.5f, position.width, num3 * position.height);
					outSourceRect = new Rect(0f, 0f, 1f, 1f);
					result = true;
				}
				break;
			}
			return result;
		}

		[ExcludeFromDocs]
		public static void DrawTextureWithTexCoords(Rect position, Texture image, Rect texCoords)
		{
			bool alphaBlend = true;
			DrawTextureWithTexCoords(position, image, texCoords, alphaBlend);
		}

		public static void DrawTextureWithTexCoords(Rect position, Texture image, Rect texCoords, [DefaultValue("true")] bool alphaBlend)
		{
			if (Event.current.type == EventType.Repaint)
			{
				Material mat = ((!alphaBlend) ? blitMaterial : blendMaterial);
				InternalDrawTextureArguments arguments = new InternalDrawTextureArguments
				{
					texture = image,
					leftBorder = 0,
					rightBorder = 0,
					topBorder = 0,
					bottomBorder = 0,
					color = color,
					mat = mat,
					screenRect = position,
					sourceRect = texCoords
				};
				Graphics.DrawTexture(ref arguments);
			}
		}

		public static void Box(Rect position, string text)
		{
			Box(position, GUIContent.Temp(text), s_Skin.box);
		}

		public static void Box(Rect position, Texture image)
		{
			Box(position, GUIContent.Temp(image), s_Skin.box);
		}

		public static void Box(Rect position, GUIContent content)
		{
			Box(position, content, s_Skin.box);
		}

		public static void Box(Rect position, string text, GUIStyle style)
		{
			Box(position, GUIContent.Temp(text), style);
		}

		public static void Box(Rect position, Texture image, GUIStyle style)
		{
			Box(position, GUIContent.Temp(image), style);
		}

		public static void Box(Rect position, GUIContent content, GUIStyle style)
		{
			GUIUtility.CheckOnGUI();
			int controlID = GUIUtility.GetControlID(boxHash, FocusType.Passive);
			if (Event.current.type == EventType.Repaint)
			{
				style.Draw(position, content, controlID);
			}
		}

		public static bool Button(Rect position, string text)
		{
			return DoButton(position, GUIContent.Temp(text), s_Skin.button.m_Ptr);
		}

		public static bool Button(Rect position, Texture image)
		{
			return DoButton(position, GUIContent.Temp(image), s_Skin.button.m_Ptr);
		}

		public static bool Button(Rect position, GUIContent content)
		{
			return DoButton(position, content, s_Skin.button.m_Ptr);
		}

		public static bool Button(Rect position, string text, GUIStyle style)
		{
			return DoButton(position, GUIContent.Temp(text), style.m_Ptr);
		}

		public static bool Button(Rect position, Texture image, GUIStyle style)
		{
			return DoButton(position, GUIContent.Temp(image), style.m_Ptr);
		}

		public static bool Button(Rect position, GUIContent content, GUIStyle style)
		{
			return DoButton(position, content, style.m_Ptr);
		}

		private static bool DoButton(Rect position, GUIContent content, IntPtr style)
		{
			return INTERNAL_CALL_DoButton(ref position, content, style);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern bool INTERNAL_CALL_DoButton(ref Rect position, GUIContent content, IntPtr style);

		public static bool RepeatButton(Rect position, string text)
		{
			return DoRepeatButton(position, GUIContent.Temp(text), s_Skin.button, FocusType.Native);
		}

		public static bool RepeatButton(Rect position, Texture image)
		{
			return DoRepeatButton(position, GUIContent.Temp(image), s_Skin.button, FocusType.Native);
		}

		public static bool RepeatButton(Rect position, GUIContent content)
		{
			return DoRepeatButton(position, content, s_Skin.button, FocusType.Native);
		}

		public static bool RepeatButton(Rect position, string text, GUIStyle style)
		{
			return DoRepeatButton(position, GUIContent.Temp(text), style, FocusType.Native);
		}

		public static bool RepeatButton(Rect position, Texture image, GUIStyle style)
		{
			return DoRepeatButton(position, GUIContent.Temp(image), style, FocusType.Native);
		}

		public static bool RepeatButton(Rect position, GUIContent content, GUIStyle style)
		{
			return DoRepeatButton(position, content, style, FocusType.Native);
		}

		private static bool DoRepeatButton(Rect position, GUIContent content, GUIStyle style, FocusType focusType)
		{
			GUIUtility.CheckOnGUI();
			int controlID = GUIUtility.GetControlID(repeatButtonHash, focusType, position);
			switch (Event.current.GetTypeForControl(controlID))
			{
			case EventType.MouseDown:
				if (position.Contains(Event.current.mousePosition))
				{
					GUIUtility.hotControl = controlID;
					Event.current.Use();
				}
				return false;
			case EventType.MouseUp:
				if (GUIUtility.hotControl == controlID)
				{
					GUIUtility.hotControl = 0;
					Event.current.Use();
					return position.Contains(Event.current.mousePosition);
				}
				return false;
			case EventType.Repaint:
				style.Draw(position, content, controlID);
				return controlID == GUIUtility.hotControl && position.Contains(Event.current.mousePosition);
			default:
				return false;
			}
		}

		public static string TextField(Rect position, string text)
		{
			GUIContent gUIContent = GUIContent.Temp(text);
			DoTextField(position, GUIUtility.GetControlID(FocusType.Keyboard, position), gUIContent, false, -1, skin.textField);
			return gUIContent.text;
		}

		public static string TextField(Rect position, string text, int maxLength)
		{
			GUIContent gUIContent = GUIContent.Temp(text);
			DoTextField(position, GUIUtility.GetControlID(FocusType.Keyboard, position), gUIContent, false, maxLength, skin.textField);
			return gUIContent.text;
		}

		public static string TextField(Rect position, string text, GUIStyle style)
		{
			GUIContent gUIContent = GUIContent.Temp(text);
			DoTextField(position, GUIUtility.GetControlID(FocusType.Keyboard, position), gUIContent, false, -1, style);
			return gUIContent.text;
		}

		public static string TextField(Rect position, string text, int maxLength, GUIStyle style)
		{
			GUIContent gUIContent = GUIContent.Temp(text);
			DoTextField(position, GUIUtility.GetControlID(FocusType.Keyboard, position), gUIContent, true, maxLength, style);
			return gUIContent.text;
		}

		public static string PasswordField(Rect position, string password, char maskChar)
		{
			return PasswordField(position, password, maskChar, -1, skin.textField);
		}

		public static string PasswordField(Rect position, string password, char maskChar, int maxLength)
		{
			return PasswordField(position, password, maskChar, maxLength, skin.textField);
		}

		public static string PasswordField(Rect position, string password, char maskChar, GUIStyle style)
		{
			return PasswordField(position, password, maskChar, -1, style);
		}

		public static string PasswordField(Rect position, string password, char maskChar, int maxLength, GUIStyle style)
		{
			string t = PasswordFieldGetStrToShow(password, maskChar);
			GUIContent gUIContent = GUIContent.Temp(t);
			bool flag = changed;
			changed = false;
			DoTextField(position, GUIUtility.GetControlID(FocusType.Keyboard), gUIContent, false, maxLength, style, password, maskChar);
			t = ((!changed) ? password : gUIContent.text);
			changed |= flag;
			return t;
		}

		internal static string PasswordFieldGetStrToShow(string password, char maskChar)
		{
			return (Event.current.type != EventType.Repaint && Event.current.type != EventType.MouseDown) ? password : string.Empty.PadRight(password.Length, maskChar);
		}

		public static string TextArea(Rect position, string text)
		{
			GUIContent gUIContent = GUIContent.Temp(text);
			DoTextField(position, GUIUtility.GetControlID(FocusType.Keyboard, position), gUIContent, true, -1, skin.textArea);
			return gUIContent.text;
		}

		public static string TextArea(Rect position, string text, int maxLength)
		{
			GUIContent gUIContent = GUIContent.Temp(text);
			DoTextField(position, GUIUtility.GetControlID(FocusType.Keyboard, position), gUIContent, true, maxLength, skin.textArea);
			return gUIContent.text;
		}

		public static string TextArea(Rect position, string text, GUIStyle style)
		{
			GUIContent gUIContent = GUIContent.Temp(text);
			DoTextField(position, GUIUtility.GetControlID(FocusType.Keyboard, position), gUIContent, true, -1, style);
			return gUIContent.text;
		}

		public static string TextArea(Rect position, string text, int maxLength, GUIStyle style)
		{
			GUIContent gUIContent = GUIContent.Temp(text);
			DoTextField(position, GUIUtility.GetControlID(FocusType.Keyboard, position), gUIContent, false, maxLength, style);
			return gUIContent.text;
		}

		private static string TextArea(Rect position, GUIContent content, int maxLength, GUIStyle style)
		{
			GUIContent gUIContent = GUIContent.Temp(content.text, content.image);
			DoTextField(position, GUIUtility.GetControlID(FocusType.Keyboard, position), gUIContent, false, maxLength, style);
			return gUIContent.text;
		}

		internal static void DoTextField(Rect position, int id, GUIContent content, bool multiline, int maxLength, GUIStyle style, string secureText = null, char maskChar = '\0')
		{
			if (maxLength >= 0 && content.text.Length > maxLength)
			{
				content.text = content.text.Substring(0, maxLength);
			}
			GUIUtility.CheckOnGUI();
			TextEditor textEditor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), id);
			textEditor.content.text = content.text;
			textEditor.SaveBackup();
			textEditor.position = position;
			textEditor.style = style;
			textEditor.multiline = multiline;
			textEditor.controlID = id;
			textEditor.ClampPos();
			if (GUIUtility.keyboardControl == id && Event.current.type != EventType.Layout)
			{
				textEditor.UpdateScrollOffsetIfNeeded();
			}
			Event current = Event.current;
			switch (current.type)
			{
			case EventType.MouseDown:
				if (position.Contains(current.mousePosition))
				{
					GUIUtility.hotControl = id;
					if (hotTextField != -1 && hotTextField != id)
					{
						TextEditor textEditor2 = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), hotTextField);
						textEditor2.keyboardOnScreen = null;
					}
					hotTextField = id;
					if (GUIUtility.keyboardControl != id)
					{
						GUIUtility.keyboardControl = id;
					}
					textEditor.keyboardOnScreen = TouchScreenKeyboard.Open((secureText == null) ? content.text : secureText, TouchScreenKeyboardType.Default, true, multiline, secureText != null);
					current.Use();
				}
				break;
			case EventType.Repaint:
			{
				if (textEditor.keyboardOnScreen != null)
				{
					content.text = textEditor.keyboardOnScreen.text;
					if (maxLength >= 0 && content.text.Length > maxLength)
					{
						content.text = content.text.Substring(0, maxLength);
					}
					if (textEditor.keyboardOnScreen.done)
					{
						textEditor.keyboardOnScreen = null;
						changed = true;
					}
				}
				string text = content.text;
				if (secureText != null)
				{
					content.text = PasswordFieldGetStrToShow(text, maskChar);
				}
				style.Draw(position, content, id, false);
				content.text = text;
				break;
			}
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern void SetNextControlName(string name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern string GetNameOfFocusedControl();

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern void FocusControl(string name);

		public static bool Toggle(Rect position, bool value, string text)
		{
			return Toggle(position, value, GUIContent.Temp(text), s_Skin.toggle);
		}

		public static bool Toggle(Rect position, bool value, Texture image)
		{
			return Toggle(position, value, GUIContent.Temp(image), s_Skin.toggle);
		}

		public static bool Toggle(Rect position, bool value, GUIContent content)
		{
			return Toggle(position, value, content, s_Skin.toggle);
		}

		public static bool Toggle(Rect position, bool value, string text, GUIStyle style)
		{
			return Toggle(position, value, GUIContent.Temp(text), style);
		}

		public static bool Toggle(Rect position, bool value, Texture image, GUIStyle style)
		{
			return Toggle(position, value, GUIContent.Temp(image), style);
		}

		public static bool Toggle(Rect position, bool value, GUIContent content, GUIStyle style)
		{
			return DoToggle(position, GUIUtility.GetControlID(toggleHash, FocusType.Native, position), value, content, style.m_Ptr);
		}

		public static bool Toggle(Rect position, int id, bool value, GUIContent content, GUIStyle style)
		{
			return DoToggle(position, id, value, content, style.m_Ptr);
		}

		internal static bool DoToggle(Rect position, int id, bool value, GUIContent content, IntPtr style)
		{
			return INTERNAL_CALL_DoToggle(ref position, id, value, content, style);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern bool INTERNAL_CALL_DoToggle(ref Rect position, int id, bool value, GUIContent content, IntPtr style);

		public static int Toolbar(Rect position, int selected, string[] texts)
		{
			return Toolbar(position, selected, GUIContent.Temp(texts), s_Skin.button);
		}

		public static int Toolbar(Rect position, int selected, Texture[] images)
		{
			return Toolbar(position, selected, GUIContent.Temp(images), s_Skin.button);
		}

		public static int Toolbar(Rect position, int selected, GUIContent[] content)
		{
			return Toolbar(position, selected, content, s_Skin.button);
		}

		public static int Toolbar(Rect position, int selected, string[] texts, GUIStyle style)
		{
			return Toolbar(position, selected, GUIContent.Temp(texts), style);
		}

		public static int Toolbar(Rect position, int selected, Texture[] images, GUIStyle style)
		{
			return Toolbar(position, selected, GUIContent.Temp(images), style);
		}

		public static int Toolbar(Rect position, int selected, GUIContent[] contents, GUIStyle style)
		{
			GUIStyle firstStyle;
			GUIStyle midStyle;
			GUIStyle lastStyle;
			FindStyles(ref style, out firstStyle, out midStyle, out lastStyle, "left", "mid", "right");
			return DoButtonGrid(position, selected, contents, contents.Length, style, firstStyle, midStyle, lastStyle);
		}

		public static int SelectionGrid(Rect position, int selected, string[] texts, int xCount)
		{
			return SelectionGrid(position, selected, GUIContent.Temp(texts), xCount, null);
		}

		public static int SelectionGrid(Rect position, int selected, Texture[] images, int xCount)
		{
			return SelectionGrid(position, selected, GUIContent.Temp(images), xCount, null);
		}

		public static int SelectionGrid(Rect position, int selected, GUIContent[] content, int xCount)
		{
			return SelectionGrid(position, selected, content, xCount, null);
		}

		public static int SelectionGrid(Rect position, int selected, string[] texts, int xCount, GUIStyle style)
		{
			return SelectionGrid(position, selected, GUIContent.Temp(texts), xCount, style);
		}

		public static int SelectionGrid(Rect position, int selected, Texture[] images, int xCount, GUIStyle style)
		{
			return SelectionGrid(position, selected, GUIContent.Temp(images), xCount, style);
		}

		public static int SelectionGrid(Rect position, int selected, GUIContent[] contents, int xCount, GUIStyle style)
		{
			if (style == null)
			{
				style = s_Skin.button;
			}
			return DoButtonGrid(position, selected, contents, xCount, style, style, style, style);
		}

		internal static void FindStyles(ref GUIStyle style, out GUIStyle firstStyle, out GUIStyle midStyle, out GUIStyle lastStyle, string first, string mid, string last)
		{
			if (style == null)
			{
				style = skin.button;
			}
			string name = style.name;
			midStyle = skin.FindStyle(name + mid);
			if (midStyle == null)
			{
				midStyle = style;
			}
			firstStyle = skin.FindStyle(name + first);
			if (firstStyle == null)
			{
				firstStyle = midStyle;
			}
			lastStyle = skin.FindStyle(name + last);
			if (lastStyle == null)
			{
				lastStyle = midStyle;
			}
		}

		internal static int CalcTotalHorizSpacing(int xCount, GUIStyle style, GUIStyle firstStyle, GUIStyle midStyle, GUIStyle lastStyle)
		{
			if (xCount < 2)
			{
				return 0;
			}
			if (xCount == 2)
			{
				return Mathf.Max(firstStyle.margin.right, lastStyle.margin.left);
			}
			int num = Mathf.Max(midStyle.margin.left, midStyle.margin.right);
			return Mathf.Max(firstStyle.margin.right, midStyle.margin.left) + Mathf.Max(midStyle.margin.right, lastStyle.margin.left) + num * (xCount - 3);
		}

		private static int DoButtonGrid(Rect position, int selected, GUIContent[] contents, int xCount, GUIStyle style, GUIStyle firstStyle, GUIStyle midStyle, GUIStyle lastStyle)
		{
			GUIUtility.CheckOnGUI();
			int num = contents.Length;
			if (num == 0)
			{
				return selected;
			}
			if (xCount <= 0)
			{
				Debug.LogWarning("You are trying to create a SelectionGrid with zero or less elements to be displayed in the horizontal direction. Set xCount to a positive value.");
				return selected;
			}
			int controlID = GUIUtility.GetControlID(buttonGridHash, FocusType.Native, position);
			int num2 = num / xCount;
			if (num % xCount != 0)
			{
				num2++;
			}
			float num3 = CalcTotalHorizSpacing(xCount, style, firstStyle, midStyle, lastStyle);
			float num4 = Mathf.Max(style.margin.top, style.margin.bottom) * (num2 - 1);
			float elemWidth = (position.width - num3) / (float)xCount;
			float elemHeight = (position.height - num4) / (float)num2;
			if (style.fixedWidth != 0f)
			{
				elemWidth = style.fixedWidth;
			}
			if (style.fixedHeight != 0f)
			{
				elemHeight = style.fixedHeight;
			}
			switch (Event.current.GetTypeForControl(controlID))
			{
			case EventType.MouseDown:
				if (position.Contains(Event.current.mousePosition))
				{
					Rect[] array = CalcMouseRects(position, num, xCount, elemWidth, elemHeight, style, firstStyle, midStyle, lastStyle, false);
					if (GetButtonGridMouseSelection(array, Event.current.mousePosition, true) != -1)
					{
						GUIUtility.hotControl = controlID;
						Event.current.Use();
					}
				}
				break;
			case EventType.MouseDrag:
				if (GUIUtility.hotControl == controlID)
				{
					Event.current.Use();
				}
				break;
			case EventType.MouseUp:
				if (GUIUtility.hotControl == controlID)
				{
					GUIUtility.hotControl = 0;
					Event.current.Use();
					Rect[] array = CalcMouseRects(position, num, xCount, elemWidth, elemHeight, style, firstStyle, midStyle, lastStyle, false);
					int buttonGridMouseSelection2 = GetButtonGridMouseSelection(array, Event.current.mousePosition, true);
					changed = true;
					return buttonGridMouseSelection2;
				}
				break;
			case EventType.Repaint:
			{
				GUIStyle gUIStyle = null;
				GUIClip.Push(position, Vector2.zero, Vector2.zero, false);
				position = new Rect(0f, 0f, position.width, position.height);
				Rect[] array = CalcMouseRects(position, num, xCount, elemWidth, elemHeight, style, firstStyle, midStyle, lastStyle, false);
				int buttonGridMouseSelection = GetButtonGridMouseSelection(array, Event.current.mousePosition, controlID == GUIUtility.hotControl);
				bool flag = position.Contains(Event.current.mousePosition);
				GUIUtility.mouseUsed |= flag;
				for (int i = 0; i < num; i++)
				{
					GUIStyle gUIStyle2 = null;
					gUIStyle2 = ((i == 0) ? firstStyle : midStyle);
					if (i == num - 1)
					{
						gUIStyle2 = lastStyle;
					}
					if (num == 1)
					{
						gUIStyle2 = style;
					}
					if (i != selected)
					{
						gUIStyle2.Draw(array[i], contents[i], i == buttonGridMouseSelection && (enabled || controlID == GUIUtility.hotControl) && (controlID == GUIUtility.hotControl || GUIUtility.hotControl == 0), controlID == GUIUtility.hotControl && enabled, false, false);
					}
					else
					{
						gUIStyle = gUIStyle2;
					}
				}
				if (selected < num && selected > -1)
				{
					gUIStyle.Draw(array[selected], contents[selected], selected == buttonGridMouseSelection && (enabled || controlID == GUIUtility.hotControl) && (controlID == GUIUtility.hotControl || GUIUtility.hotControl == 0), controlID == GUIUtility.hotControl, true, false);
				}
				if (buttonGridMouseSelection >= 0)
				{
					tooltip = contents[buttonGridMouseSelection].tooltip;
				}
				GUIClip.Pop();
				break;
			}
			}
			return selected;
		}

		private static Rect[] CalcMouseRects(Rect position, int count, int xCount, float elemWidth, float elemHeight, GUIStyle style, GUIStyle firstStyle, GUIStyle midStyle, GUIStyle lastStyle, bool addBorders)
		{
			int num = 0;
			int num2 = 0;
			float num3 = position.xMin;
			float num4 = position.yMin;
			GUIStyle gUIStyle = style;
			Rect[] array = new Rect[count];
			if (count > 1)
			{
				gUIStyle = firstStyle;
			}
			for (int i = 0; i < count; i++)
			{
				if (!addBorders)
				{
					array[i] = new Rect(num3, num4, elemWidth, elemHeight);
				}
				else
				{
					array[i] = gUIStyle.margin.Add(new Rect(num3, num4, elemWidth, elemHeight));
				}
				array[i].width = Mathf.Round(array[i].xMax) - Mathf.Round(array[i].x);
				array[i].x = Mathf.Round(array[i].x);
				GUIStyle gUIStyle2 = midStyle;
				if (i == count - 2)
				{
					gUIStyle2 = lastStyle;
				}
				num3 += elemWidth + (float)Mathf.Max(gUIStyle.margin.right, gUIStyle2.margin.left);
				num2++;
				if (num2 >= xCount)
				{
					num++;
					num2 = 0;
					num4 += elemHeight + (float)Mathf.Max(style.margin.top, style.margin.bottom);
					num3 = position.xMin;
				}
			}
			return array;
		}

		private static int GetButtonGridMouseSelection(Rect[] buttonRects, Vector2 mousePos, bool findNearest)
		{
			for (int i = 0; i < buttonRects.Length; i++)
			{
				if (buttonRects[i].Contains(mousePos))
				{
					return i;
				}
			}
			if (!findNearest)
			{
				return -1;
			}
			float num = 10000000f;
			int result = -1;
			for (int j = 0; j < buttonRects.Length; j++)
			{
				Rect rect = buttonRects[j];
				Vector2 vector = new Vector2(Mathf.Clamp(mousePos.x, rect.xMin, rect.xMax), Mathf.Clamp(mousePos.y, rect.yMin, rect.yMax));
				float sqrMagnitude = (mousePos - vector).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					result = j;
					num = sqrMagnitude;
				}
			}
			return result;
		}

		public static float HorizontalSlider(Rect position, float value, float leftValue, float rightValue)
		{
			return Slider(position, value, 0f, leftValue, rightValue, skin.horizontalSlider, skin.horizontalSliderThumb, true, GUIUtility.GetControlID(sliderHash, FocusType.Native, position));
		}

		public static float HorizontalSlider(Rect position, float value, float leftValue, float rightValue, GUIStyle slider, GUIStyle thumb)
		{
			return Slider(position, value, 0f, leftValue, rightValue, slider, thumb, true, GUIUtility.GetControlID(sliderHash, FocusType.Native, position));
		}

		public static float VerticalSlider(Rect position, float value, float topValue, float bottomValue)
		{
			return Slider(position, value, 0f, topValue, bottomValue, skin.verticalSlider, skin.verticalSliderThumb, false, GUIUtility.GetControlID(sliderHash, FocusType.Native, position));
		}

		public static float VerticalSlider(Rect position, float value, float topValue, float bottomValue, GUIStyle slider, GUIStyle thumb)
		{
			return Slider(position, value, 0f, topValue, bottomValue, slider, thumb, false, GUIUtility.GetControlID(sliderHash, FocusType.Native, position));
		}

		public static float Slider(Rect position, float value, float size, float start, float end, GUIStyle slider, GUIStyle thumb, bool horiz, int id)
		{
			GUIUtility.CheckOnGUI();
			return new SliderHandler(position, value, size, start, end, slider, thumb, horiz, id).Handle();
		}

		public static float HorizontalScrollbar(Rect position, float value, float size, float leftValue, float rightValue)
		{
			return Scroller(position, value, size, leftValue, rightValue, skin.horizontalScrollbar, skin.horizontalScrollbarThumb, skin.horizontalScrollbarLeftButton, skin.horizontalScrollbarRightButton, true);
		}

		public static float HorizontalScrollbar(Rect position, float value, float size, float leftValue, float rightValue, GUIStyle style)
		{
			return Scroller(position, value, size, leftValue, rightValue, style, skin.GetStyle(style.name + "thumb"), skin.GetStyle(style.name + "leftbutton"), skin.GetStyle(style.name + "rightbutton"), true);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		internal static extern void InternalRepaintEditorWindow();

		internal static bool ScrollerRepeatButton(int scrollerID, Rect rect, GUIStyle style)
		{
			bool result = false;
			if (DoRepeatButton(rect, GUIContent.none, style, FocusType.Passive))
			{
				bool flag = scrollControlID != scrollerID;
				scrollControlID = scrollerID;
				if (flag)
				{
					result = true;
					nextScrollStepTime = DateTime.Now.AddMilliseconds(250.0);
				}
				else if (DateTime.Now >= nextScrollStepTime)
				{
					result = true;
					nextScrollStepTime = DateTime.Now.AddMilliseconds(30.0);
				}
				if (Event.current.type == EventType.Repaint)
				{
					InternalRepaintEditorWindow();
				}
			}
			return result;
		}

		public static float VerticalScrollbar(Rect position, float value, float size, float topValue, float bottomValue)
		{
			return Scroller(position, value, size, topValue, bottomValue, skin.verticalScrollbar, skin.verticalScrollbarThumb, skin.verticalScrollbarUpButton, skin.verticalScrollbarDownButton, false);
		}

		public static float VerticalScrollbar(Rect position, float value, float size, float topValue, float bottomValue, GUIStyle style)
		{
			return Scroller(position, value, size, topValue, bottomValue, style, skin.GetStyle(style.name + "thumb"), skin.GetStyle(style.name + "upbutton"), skin.GetStyle(style.name + "downbutton"), false);
		}

		private static float Scroller(Rect position, float value, float size, float leftValue, float rightValue, GUIStyle slider, GUIStyle thumb, GUIStyle leftButton, GUIStyle rightButton, bool horiz)
		{
			GUIUtility.CheckOnGUI();
			int controlID = GUIUtility.GetControlID(sliderHash, FocusType.Passive, position);
			Rect position2;
			Rect rect;
			Rect rect2;
			if (horiz)
			{
				position2 = new Rect(position.x + leftButton.fixedWidth, position.y, position.width - leftButton.fixedWidth - rightButton.fixedWidth, position.height);
				rect = new Rect(position.x, position.y, leftButton.fixedWidth, position.height);
				rect2 = new Rect(position.xMax - rightButton.fixedWidth, position.y, rightButton.fixedWidth, position.height);
			}
			else
			{
				position2 = new Rect(position.x, position.y + leftButton.fixedHeight, position.width, position.height - leftButton.fixedHeight - rightButton.fixedHeight);
				rect = new Rect(position.x, position.y, position.width, leftButton.fixedHeight);
				rect2 = new Rect(position.x, position.yMax - rightButton.fixedHeight, position.width, rightButton.fixedHeight);
			}
			value = Slider(position2, value, size, leftValue, rightValue, slider, thumb, horiz, controlID);
			bool flag = false;
			if (Event.current.type == EventType.MouseUp)
			{
				flag = true;
			}
			if (ScrollerRepeatButton(controlID, rect, leftButton))
			{
				value -= scrollStepSize * ((!(leftValue < rightValue)) ? (-1f) : 1f);
			}
			if (ScrollerRepeatButton(controlID, rect2, rightButton))
			{
				value += scrollStepSize * ((!(leftValue < rightValue)) ? (-1f) : 1f);
			}
			if (flag && Event.current.type == EventType.Used)
			{
				scrollControlID = 0;
			}
			value = ((!(leftValue < rightValue)) ? Mathf.Clamp(value, rightValue, leftValue - size) : Mathf.Clamp(value, leftValue, rightValue - size));
			return value;
		}

		public static void BeginGroup(Rect position)
		{
			BeginGroup(position, GUIContent.none, GUIStyle.none);
		}

		public static void BeginGroup(Rect position, string text)
		{
			BeginGroup(position, GUIContent.Temp(text), GUIStyle.none);
		}

		public static void BeginGroup(Rect position, Texture image)
		{
			BeginGroup(position, GUIContent.Temp(image), GUIStyle.none);
		}

		public static void BeginGroup(Rect position, GUIContent content)
		{
			BeginGroup(position, content, GUIStyle.none);
		}

		public static void BeginGroup(Rect position, GUIStyle style)
		{
			BeginGroup(position, GUIContent.none, style);
		}

		public static void BeginGroup(Rect position, string text, GUIStyle style)
		{
			BeginGroup(position, GUIContent.Temp(text), style);
		}

		public static void BeginGroup(Rect position, Texture image, GUIStyle style)
		{
			BeginGroup(position, GUIContent.Temp(image), style);
		}

		public static void BeginGroup(Rect position, GUIContent content, GUIStyle style)
		{
			GUIUtility.CheckOnGUI();
			int controlID = GUIUtility.GetControlID(beginGroupHash, FocusType.Passive);
			if (content != GUIContent.none || style != GUIStyle.none)
			{
				EventType type = Event.current.type;
				if (type == EventType.Repaint)
				{
					style.Draw(position, content, controlID);
				}
				else if (position.Contains(Event.current.mousePosition))
				{
					GUIUtility.mouseUsed = true;
				}
			}
			GUIClip.Push(position, Vector2.zero, Vector2.zero, false);
		}

		public static void EndGroup()
		{
			GUIClip.Pop();
		}

		public static Vector2 BeginScrollView(Rect position, Vector2 scrollPosition, Rect viewRect)
		{
			return BeginScrollView(position, scrollPosition, viewRect, false, false, skin.horizontalScrollbar, skin.verticalScrollbar, skin.scrollView);
		}

		public static Vector2 BeginScrollView(Rect position, Vector2 scrollPosition, Rect viewRect, bool alwaysShowHorizontal, bool alwaysShowVertical)
		{
			return BeginScrollView(position, scrollPosition, viewRect, alwaysShowHorizontal, alwaysShowVertical, skin.horizontalScrollbar, skin.verticalScrollbar, skin.scrollView);
		}

		public static Vector2 BeginScrollView(Rect position, Vector2 scrollPosition, Rect viewRect, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar)
		{
			return BeginScrollView(position, scrollPosition, viewRect, false, false, horizontalScrollbar, verticalScrollbar, skin.scrollView);
		}

		public static Vector2 BeginScrollView(Rect position, Vector2 scrollPosition, Rect viewRect, bool alwaysShowHorizontal, bool alwaysShowVertical, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar)
		{
			return BeginScrollView(position, scrollPosition, viewRect, alwaysShowHorizontal, alwaysShowVertical, horizontalScrollbar, verticalScrollbar, null);
		}

		protected static Vector2 DoBeginScrollView(Rect position, Vector2 scrollPosition, Rect viewRect, bool alwaysShowHorizontal, bool alwaysShowVertical, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar, GUIStyle background)
		{
			return BeginScrollView(position, scrollPosition, viewRect, alwaysShowHorizontal, alwaysShowVertical, horizontalScrollbar, verticalScrollbar, background);
		}

		internal static Vector2 BeginScrollView(Rect position, Vector2 scrollPosition, Rect viewRect, bool alwaysShowHorizontal, bool alwaysShowVertical, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar, GUIStyle background)
		{
			GUIUtility.CheckOnGUI();
			int controlID = GUIUtility.GetControlID(scrollviewHash, FocusType.Passive);
			ScrollViewState scrollViewState = (ScrollViewState)GUIUtility.GetStateObject(typeof(ScrollViewState), controlID);
			if (scrollViewState.apply)
			{
				scrollPosition = scrollViewState.scrollPosition;
				scrollViewState.apply = false;
			}
			scrollViewState.position = position;
			scrollViewState.scrollPosition = scrollPosition;
			scrollViewState.visibleRect = (scrollViewState.viewRect = viewRect);
			scrollViewState.visibleRect.width = position.width;
			scrollViewState.visibleRect.height = position.height;
			s_ScrollViewStates.Push(scrollViewState);
			Rect screenRect = new Rect(position);
			switch (Event.current.type)
			{
			case EventType.Layout:
				GUIUtility.GetControlID(sliderHash, FocusType.Passive);
				GUIUtility.GetControlID(repeatButtonHash, FocusType.Passive);
				GUIUtility.GetControlID(repeatButtonHash, FocusType.Passive);
				GUIUtility.GetControlID(sliderHash, FocusType.Passive);
				GUIUtility.GetControlID(repeatButtonHash, FocusType.Passive);
				GUIUtility.GetControlID(repeatButtonHash, FocusType.Passive);
				break;
			default:
			{
				bool flag = alwaysShowVertical;
				bool flag2 = alwaysShowHorizontal;
				if (flag2 || viewRect.width > screenRect.width)
				{
					scrollViewState.visibleRect.height = position.height - horizontalScrollbar.fixedHeight + (float)horizontalScrollbar.margin.top;
					screenRect.height -= horizontalScrollbar.fixedHeight + (float)horizontalScrollbar.margin.top;
					flag2 = true;
				}
				if (flag || viewRect.height > screenRect.height)
				{
					scrollViewState.visibleRect.width = position.width - verticalScrollbar.fixedWidth + (float)verticalScrollbar.margin.left;
					screenRect.width -= verticalScrollbar.fixedWidth + (float)verticalScrollbar.margin.left;
					flag = true;
					if (!flag2 && viewRect.width > screenRect.width)
					{
						scrollViewState.visibleRect.height = position.height - horizontalScrollbar.fixedHeight + (float)horizontalScrollbar.margin.top;
						screenRect.height -= horizontalScrollbar.fixedHeight + (float)horizontalScrollbar.margin.top;
						flag2 = true;
					}
				}
				if (Event.current.type == EventType.Repaint && background != GUIStyle.none)
				{
					background.Draw(position, position.Contains(Event.current.mousePosition), false, flag2 && flag, false);
				}
				if (flag2 && horizontalScrollbar != GUIStyle.none)
				{
					scrollPosition.x = HorizontalScrollbar(new Rect(position.x, position.yMax - horizontalScrollbar.fixedHeight, screenRect.width, horizontalScrollbar.fixedHeight), scrollPosition.x, screenRect.width, 0f, viewRect.width, horizontalScrollbar);
				}
				else
				{
					GUIUtility.GetControlID(sliderHash, FocusType.Passive);
					GUIUtility.GetControlID(repeatButtonHash, FocusType.Passive);
					GUIUtility.GetControlID(repeatButtonHash, FocusType.Passive);
					if (horizontalScrollbar != GUIStyle.none)
					{
						scrollPosition.x = 0f;
					}
					else
					{
						scrollPosition.x = Mathf.Clamp(scrollPosition.x, 0f, Mathf.Max(viewRect.width - position.width, 0f));
					}
				}
				if (flag && verticalScrollbar != GUIStyle.none)
				{
					scrollPosition.y = VerticalScrollbar(new Rect(screenRect.xMax + (float)verticalScrollbar.margin.left, screenRect.y, verticalScrollbar.fixedWidth, screenRect.height), scrollPosition.y, screenRect.height, 0f, viewRect.height, verticalScrollbar);
					break;
				}
				GUIUtility.GetControlID(sliderHash, FocusType.Passive);
				GUIUtility.GetControlID(repeatButtonHash, FocusType.Passive);
				GUIUtility.GetControlID(repeatButtonHash, FocusType.Passive);
				if (verticalScrollbar != GUIStyle.none)
				{
					scrollPosition.y = 0f;
				}
				else
				{
					scrollPosition.y = Mathf.Clamp(scrollPosition.y, 0f, Mathf.Max(viewRect.height - position.height, 0f));
				}
				break;
			}
			case EventType.Used:
				break;
			}
			GUIClip.Push(screenRect, new Vector2(Mathf.Round(0f - scrollPosition.x - viewRect.x), Mathf.Round(0f - scrollPosition.y - viewRect.y)), Vector2.zero, false);
			return scrollPosition;
		}

		public static void EndScrollView()
		{
			EndScrollView(true);
		}

		public static void EndScrollView(bool handleScrollWheel)
		{
			ScrollViewState scrollViewState = (ScrollViewState)s_ScrollViewStates.Peek();
			GUIUtility.CheckOnGUI();
			GUIClip.Pop();
			s_ScrollViewStates.Pop();
			if (handleScrollWheel && Event.current.type == EventType.ScrollWheel && scrollViewState.position.Contains(Event.current.mousePosition))
			{
				scrollViewState.scrollPosition.x = Mathf.Clamp(scrollViewState.scrollPosition.x + Event.current.delta.x * 20f, 0f, scrollViewState.viewRect.width - scrollViewState.visibleRect.width);
				scrollViewState.scrollPosition.y = Mathf.Clamp(scrollViewState.scrollPosition.y + Event.current.delta.y * 20f, 0f, scrollViewState.viewRect.height - scrollViewState.visibleRect.height);
				scrollViewState.apply = true;
				Event.current.Use();
			}
		}

		internal static ScrollViewState GetTopScrollView()
		{
			if (s_ScrollViewStates.Count != 0)
			{
				return (ScrollViewState)s_ScrollViewStates.Peek();
			}
			return null;
		}

		public static void ScrollTo(Rect position)
		{
			ScrollViewState topScrollView = GetTopScrollView();
			if (topScrollView != null)
			{
				topScrollView.ScrollTo(position);
			}
		}

		public static bool ScrollTowards(Rect position, float maxDelta)
		{
			ScrollViewState topScrollView = GetTopScrollView();
			if (topScrollView == null)
			{
				return false;
			}
			return topScrollView.ScrollTowards(position, maxDelta);
		}

		public static Rect Window(int id, Rect clientRect, WindowFunction func, string text)
		{
			return DoWindow(id, clientRect, func, GUIContent.Temp(text), skin.window, skin, true);
		}

		public static Rect Window(int id, Rect clientRect, WindowFunction func, Texture image)
		{
			return DoWindow(id, clientRect, func, GUIContent.Temp(image), skin.window, skin, true);
		}

		public static Rect Window(int id, Rect clientRect, WindowFunction func, GUIContent content)
		{
			return DoWindow(id, clientRect, func, content, skin.window, skin, true);
		}

		public static Rect Window(int id, Rect clientRect, WindowFunction func, string text, GUIStyle style)
		{
			return DoWindow(id, clientRect, func, GUIContent.Temp(text), style, skin, true);
		}

		public static Rect Window(int id, Rect clientRect, WindowFunction func, Texture image, GUIStyle style)
		{
			return DoWindow(id, clientRect, func, GUIContent.Temp(image), style, skin, true);
		}

		public static Rect Window(int id, Rect clientRect, WindowFunction func, GUIContent title, GUIStyle style)
		{
			return DoWindow(id, clientRect, func, title, style, skin, true);
		}

		public static Rect ModalWindow(int id, Rect clientRect, WindowFunction func, string text)
		{
			return DoModalWindow(id, clientRect, func, GUIContent.Temp(text), skin.window, skin);
		}

		public static Rect ModalWindow(int id, Rect clientRect, WindowFunction func, Texture image)
		{
			return DoModalWindow(id, clientRect, func, GUIContent.Temp(image), skin.window, skin);
		}

		public static Rect ModalWindow(int id, Rect clientRect, WindowFunction func, GUIContent content)
		{
			return DoModalWindow(id, clientRect, func, content, skin.window, skin);
		}

		public static Rect ModalWindow(int id, Rect clientRect, WindowFunction func, string text, GUIStyle style)
		{
			return DoModalWindow(id, clientRect, func, GUIContent.Temp(text), style, skin);
		}

		public static Rect ModalWindow(int id, Rect clientRect, WindowFunction func, Texture image, GUIStyle style)
		{
			return DoModalWindow(id, clientRect, func, GUIContent.Temp(image), style, skin);
		}

		public static Rect ModalWindow(int id, Rect clientRect, WindowFunction func, GUIContent content, GUIStyle style)
		{
			return DoModalWindow(id, clientRect, func, content, style, skin);
		}

		private static Rect DoModalWindow(int id, Rect clientRect, WindowFunction func, GUIContent content, GUIStyle style, GUISkin skin)
		{
			return INTERNAL_CALL_DoModalWindow(id, ref clientRect, func, content, style, skin);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern Rect INTERNAL_CALL_DoModalWindow(int id, ref Rect clientRect, WindowFunction func, GUIContent content, GUIStyle style, GUISkin skin);

		internal static void CallWindowDelegate(WindowFunction func, int id, GUISkin _skin, int forceRect, float width, float height, GUIStyle style)
		{
			GUILayoutUtility.SelectIDList(id, true);
			GUISkin gUISkin = skin;
			if (Event.current.type == EventType.Layout)
			{
				if (forceRect != 0)
				{
					GUILayoutOption[] options = new GUILayoutOption[2]
					{
						GUILayout.Width(width),
						GUILayout.Height(height)
					};
					GUILayoutUtility.BeginWindow(id, style, options);
				}
				else
				{
					GUILayoutUtility.BeginWindow(id, style, null);
				}
			}
			skin = _skin;
			func(id);
			if (Event.current.type == EventType.Layout)
			{
				GUILayoutUtility.Layout();
			}
			skin = gUISkin;
		}

		private static Rect DoWindow(int id, Rect clientRect, WindowFunction func, GUIContent title, GUIStyle style, GUISkin skin, bool forceRectOnLayout)
		{
			return INTERNAL_CALL_DoWindow(id, ref clientRect, func, title, style, skin, forceRectOnLayout);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern Rect INTERNAL_CALL_DoWindow(int id, ref Rect clientRect, WindowFunction func, GUIContent title, GUIStyle style, GUISkin skin, bool forceRectOnLayout);

		public static void DragWindow(Rect position)
		{
			INTERNAL_CALL_DragWindow(ref position);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_DragWindow(ref Rect position);

		public static void DragWindow()
		{
			DragWindow(new Rect(0f, 0f, 10000f, 10000f));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern void BringWindowToFront(int windowID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern void BringWindowToBack(int windowID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern void FocusWindow(int windowID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern void UnfocusWindow();

		internal static void BeginWindows(int skinMode, int editorWindowInstanceID)
		{
			GUILayoutGroup topLevel = GUILayoutUtility.current.topLevel;
			GenericStack layoutGroups = GUILayoutUtility.current.layoutGroups;
			GUILayoutGroup windows = GUILayoutUtility.current.windows;
			Matrix4x4 matrix4x = matrix;
			Internal_BeginWindows();
			matrix = matrix4x;
			GUILayoutUtility.current.topLevel = topLevel;
			GUILayoutUtility.current.layoutGroups = layoutGroups;
			GUILayoutUtility.current.windows = windows;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void Internal_BeginWindows();

		internal static void EndWindows()
		{
			GUILayoutGroup topLevel = GUILayoutUtility.current.topLevel;
			GenericStack layoutGroups = GUILayoutUtility.current.layoutGroups;
			GUILayoutGroup windows = GUILayoutUtility.current.windows;
			Internal_EndWindows();
			GUILayoutUtility.current.topLevel = topLevel;
			GUILayoutUtility.current.layoutGroups = layoutGroups;
			GUILayoutUtility.current.windows = windows;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void Internal_EndWindows();
	}
}

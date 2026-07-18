using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public class GUIUtility
	{
		[NotRenamed]
		internal static int s_SkinMode;

		[NotRenamed]
		internal static int s_OriginalID;

		internal static Vector2 s_EditorScreenPointOffset = Vector2.zero;

		internal static bool s_HasKeyboardFocus = false;

		public static int hotControl
		{
			get
			{
				return Internal_GetHotControl();
			}
			set
			{
				Internal_SetHotControl(value);
			}
		}

		public static extern int keyboardControl
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		internal static extern string systemCopyBuffer
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		internal static extern bool mouseUsed
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public static extern bool hasModalWindow
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		internal static extern bool textFieldInput
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public static int GetControlID(FocusType focus)
		{
			return GetControlID(0, focus);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern int GetControlID(int hint, FocusType focus);

		public static int GetControlID(GUIContent contents, FocusType focus)
		{
			return GetControlID(contents.hash, focus);
		}

		public static int GetControlID(FocusType focus, Rect position)
		{
			return Internal_GetNextControlID2(0, focus, position);
		}

		public static int GetControlID(int hint, FocusType focus, Rect position)
		{
			return Internal_GetNextControlID2(hint, focus, position);
		}

		public static int GetControlID(GUIContent contents, FocusType focus, Rect position)
		{
			return Internal_GetNextControlID2(contents.hash, focus, position);
		}

		private static int Internal_GetNextControlID2(int hint, FocusType focusType, Rect rect)
		{
			return INTERNAL_CALL_Internal_GetNextControlID2(hint, focusType, ref rect);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern int INTERNAL_CALL_Internal_GetNextControlID2(int hint, FocusType focusType, ref Rect rect);

		public static object GetStateObject(Type t, int controlID)
		{
			return GUIStateObjects.GetStateObject(t, controlID);
		}

		public static object QueryStateObject(Type t, int controlID)
		{
			return GUIStateObjects.QueryStateObject(t, controlID);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		internal static extern int GetPermanentControlID();

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern int Internal_GetHotControl();

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void Internal_SetHotControl(int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		internal static extern void UpdateUndoName();

		public static void ExitGUI()
		{
			throw new ExitGUIException();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		internal static extern void SetDidGUIWindowsEatLastEvent(bool value);

		internal static GUISkin GetDefaultSkin()
		{
			return Internal_GetDefaultSkin(s_SkinMode);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern GUISkin Internal_GetDefaultSkin(int skinMode);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern Object Internal_GetBuiltinSkin(int skin);

		internal static GUISkin GetBuiltinSkin(int skin)
		{
			return Internal_GetBuiltinSkin(skin) as GUISkin;
		}

		internal static void BeginGUI(int skinMode, int instanceID, int useGUILayout)
		{
			s_SkinMode = skinMode;
			s_OriginalID = instanceID;
			GUI.skin = null;
			if (useGUILayout != 0)
			{
				GUILayoutUtility.SelectIDList(instanceID, false);
				GUILayoutUtility.Begin(instanceID);
			}
			GUI.changed = false;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void Internal_ExitGUI();

		internal static void EndGUI(int layoutType)
		{
			try
			{
				if (Event.current.type == EventType.Layout)
				{
					switch (layoutType)
					{
					case 1:
						GUILayoutUtility.Layout();
						break;
					case 2:
						GUILayoutUtility.LayoutFromEditorWindow();
						break;
					}
				}
				GUILayoutUtility.SelectIDList(s_OriginalID, false);
				GUIContent.ClearStaticCache();
			}
			finally
			{
				Internal_ExitGUI();
			}
		}

		internal static bool EndGUIFromException(Exception exception)
		{
			if (exception == null)
			{
				return false;
			}
			if (!(exception is ExitGUIException) && !(exception.InnerException is ExitGUIException))
			{
				return false;
			}
			Internal_ExitGUI();
			return true;
		}

		internal static void CheckOnGUI()
		{
			if (Internal_GetGUIDepth() <= 0)
			{
				throw new ArgumentException("You can only call GUI functions from inside OnGUI.");
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		internal static extern int Internal_GetGUIDepth();

		public static Vector2 GUIToScreenPoint(Vector2 guiPoint)
		{
			return GUIClip.Unclip(guiPoint) + s_EditorScreenPointOffset;
		}

		internal static Rect GUIToScreenRect(Rect guiRect)
		{
			Vector2 vector = GUIToScreenPoint(new Vector2(guiRect.x, guiRect.y));
			guiRect.x = vector.x;
			guiRect.y = vector.y;
			return guiRect;
		}

		public static Vector2 ScreenToGUIPoint(Vector2 screenPoint)
		{
			return GUIClip.Clip(screenPoint) - s_EditorScreenPointOffset;
		}

		public static Rect ScreenToGUIRect(Rect screenRect)
		{
			Vector2 vector = ScreenToGUIPoint(new Vector2(screenRect.x, screenRect.y));
			screenRect.x = vector.x;
			screenRect.y = vector.y;
			return screenRect;
		}

		public static void RotateAroundPivot(float angle, Vector2 pivotPoint)
		{
			Matrix4x4 matrix = GUI.matrix;
			GUI.matrix = Matrix4x4.identity;
			Vector2 vector = GUIClip.Unclip(pivotPoint);
			Matrix4x4 matrix4x = Matrix4x4.TRS(vector, Quaternion.Euler(0f, 0f, angle), Vector3.one) * Matrix4x4.TRS(-vector, Quaternion.identity, Vector3.one);
			GUI.matrix = matrix4x * matrix;
		}

		public static void ScaleAroundPivot(Vector2 scale, Vector2 pivotPoint)
		{
			Matrix4x4 matrix = GUI.matrix;
			Vector2 vector = GUIClip.Unclip(pivotPoint);
			Matrix4x4 matrix4x = Matrix4x4.TRS(vector, Quaternion.identity, new Vector3(scale.x, scale.y, 1f)) * Matrix4x4.TRS(-vector, Quaternion.identity, Vector3.one);
			GUI.matrix = matrix4x * matrix;
		}
	}
}

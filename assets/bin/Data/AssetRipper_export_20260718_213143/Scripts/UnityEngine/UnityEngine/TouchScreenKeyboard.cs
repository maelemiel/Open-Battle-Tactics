using System;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;

namespace UnityEngine
{
	public sealed class TouchScreenKeyboard
	{
		[NonSerialized]
		[NotRenamed]
		internal IntPtr m_Ptr;

		public extern string text
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public static extern bool hideInput
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern bool active
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern bool done
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern bool wasCanceled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public static extern Rect area
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public static extern bool visible
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public TouchScreenKeyboard(string text, TouchScreenKeyboardType keyboardType, bool autocorrection, bool multiline, bool secure, bool alert, string textPlaceholder)
		{
			TouchScreenKeyboard_InternalConstructorHelperArguments arguments = new TouchScreenKeyboard_InternalConstructorHelperArguments
			{
				keyboardType = Convert.ToUInt32(keyboardType),
				autocorrection = Convert.ToUInt32(autocorrection),
				multiline = Convert.ToUInt32(multiline),
				secure = Convert.ToUInt32(secure),
				alert = Convert.ToUInt32(alert)
			};
			TouchScreenKeyboard_InternalConstructorHelper(ref arguments, text, textPlaceholder);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void Destroy();

		~TouchScreenKeyboard()
		{
			Destroy();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void TouchScreenKeyboard_InternalConstructorHelper(ref TouchScreenKeyboard_InternalConstructorHelperArguments arguments, string text, string textPlaceholder);

		[ExcludeFromDocs]
		public static TouchScreenKeyboard Open(string text, TouchScreenKeyboardType keyboardType, bool autocorrection, bool multiline, bool secure, bool alert)
		{
			string empty = string.Empty;
			return Open(text, keyboardType, autocorrection, multiline, secure, alert, empty);
		}

		[ExcludeFromDocs]
		public static TouchScreenKeyboard Open(string text, TouchScreenKeyboardType keyboardType, bool autocorrection, bool multiline, bool secure)
		{
			string empty = string.Empty;
			bool alert = false;
			return Open(text, keyboardType, autocorrection, multiline, secure, alert, empty);
		}

		[ExcludeFromDocs]
		public static TouchScreenKeyboard Open(string text, TouchScreenKeyboardType keyboardType, bool autocorrection, bool multiline)
		{
			string empty = string.Empty;
			bool alert = false;
			bool secure = false;
			return Open(text, keyboardType, autocorrection, multiline, secure, alert, empty);
		}

		[ExcludeFromDocs]
		public static TouchScreenKeyboard Open(string text, TouchScreenKeyboardType keyboardType, bool autocorrection)
		{
			string empty = string.Empty;
			bool alert = false;
			bool secure = false;
			bool multiline = false;
			return Open(text, keyboardType, autocorrection, multiline, secure, alert, empty);
		}

		[ExcludeFromDocs]
		public static TouchScreenKeyboard Open(string text, TouchScreenKeyboardType keyboardType)
		{
			string empty = string.Empty;
			bool alert = false;
			bool secure = false;
			bool multiline = false;
			bool autocorrection = true;
			return Open(text, keyboardType, autocorrection, multiline, secure, alert, empty);
		}

		[ExcludeFromDocs]
		public static TouchScreenKeyboard Open(string text)
		{
			string empty = string.Empty;
			bool alert = false;
			bool secure = false;
			bool multiline = false;
			bool autocorrection = true;
			TouchScreenKeyboardType keyboardType = TouchScreenKeyboardType.Default;
			return Open(text, keyboardType, autocorrection, multiline, secure, alert, empty);
		}

		public static TouchScreenKeyboard Open(string text, [DefaultValue("TouchScreenKeyboardType.Default")] TouchScreenKeyboardType keyboardType, [DefaultValue("true")] bool autocorrection, [DefaultValue("false")] bool multiline, [DefaultValue("false")] bool secure, [DefaultValue("false")] bool alert, [DefaultValue("\"\"")] string textPlaceholder)
		{
			return new TouchScreenKeyboard(text, keyboardType, autocorrection, multiline, secure, alert, textPlaceholder);
		}
	}
}

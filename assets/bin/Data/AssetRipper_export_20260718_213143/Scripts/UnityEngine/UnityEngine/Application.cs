using System;
using System.Collections;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using UnityEngine.Internal;

namespace UnityEngine
{
	public sealed class Application
	{
		public delegate void LogCallback(string condition, string stackTrace, LogType type);

		private static volatile LogCallback s_LogCallback;

		public static extern int loadedLevel
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public static extern string loadedLevelName
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public static extern bool isLoadingLevel
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public static extern int levelCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public static extern int streamedBytes
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public static extern bool isPlaying
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public static extern bool isEditor
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public static extern bool isWebPlayer
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public static extern RuntimePlatform platform
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public static extern bool runInBackground
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		[Obsolete("use Application.isEditor instead")]
		public static bool isPlayer
		{
			get
			{
				return !isEditor;
			}
		}

		public static extern string dataPath
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public static extern string streamingAssetsPath
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		[SecurityCritical]
		public static extern string persistentDataPath
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public static extern string temporaryCachePath
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public static extern string srcValue
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public static extern string absoluteURL
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		[Obsolete("Please use absoluteURL instead")]
		public static string absoluteUrl
		{
			get
			{
				return absoluteURL;
			}
		}

		public static extern string unityVersion
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public static extern bool webSecurityEnabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public static extern string webSecurityHostUrl
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public static extern int targetFrameRate
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public static extern SystemLanguage systemLanguage
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public static extern ThreadPriority backgroundLoadingPriority
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public static extern NetworkReachability internetReachability
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public static extern bool genuine
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public static extern bool genuineCheckAvailable
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern void Quit();

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern void CancelQuit();

		public static void LoadLevel(int index)
		{
			LoadLevelAsync(null, index, false, true);
		}

		public static void LoadLevel(string name)
		{
			LoadLevelAsync(name, -1, false, true);
		}

		public static AsyncOperation LoadLevelAsync(int index)
		{
			return LoadLevelAsync(null, index, false, false);
		}

		public static AsyncOperation LoadLevelAsync(string levelName)
		{
			return LoadLevelAsync(levelName, -1, false, false);
		}

		public static AsyncOperation LoadLevelAdditiveAsync(int index)
		{
			return LoadLevelAsync(null, index, true, false);
		}

		public static AsyncOperation LoadLevelAdditiveAsync(string levelName)
		{
			return LoadLevelAsync(levelName, -1, true, false);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern AsyncOperation LoadLevelAsync(string monoLevelName, int index, bool additive, bool mustCompleteNextFrame);

		public static void LoadLevelAdditive(int index)
		{
			LoadLevelAsync(null, index, true, true);
		}

		public static void LoadLevelAdditive(string name)
		{
			LoadLevelAsync(name, -1, true, true);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern float GetStreamProgressForLevelByName(string levelName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern float GetStreamProgressForLevel(int levelIndex);

		public static float GetStreamProgressForLevel(string levelName)
		{
			return GetStreamProgressForLevelByName(levelName);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern bool CanStreamedLevelBeLoadedByName(string levelName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern bool CanStreamedLevelBeLoaded(int levelIndex);

		public static bool CanStreamedLevelBeLoaded(string levelName)
		{
			return CanStreamedLevelBeLoadedByName(levelName);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern void CaptureScreenshot(string filename, [DefaultValue("0")] int superSize);

		[ExcludeFromDocs]
		public static void CaptureScreenshot(string filename)
		{
			int superSize = 0;
			CaptureScreenshot(filename, superSize);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern bool HasProLicense();

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		internal static extern bool HasAdvancedLicense();

		[MethodImpl(MethodImplOptions.InternalCall)]
		[Obsolete("Use Object.DontDestroyOnLoad instead")]
		[WrapperlessIcall]
		public static extern void DontDestroyOnLoad(Object mono);

		private static string ObjectToJSString(object o)
		{
			if (o == null)
			{
				return "null";
			}
			if (o is string)
			{
				string text = o.ToString().Replace("\\", "\\\\");
				text = text.Replace("\"", "\\\"");
				text = text.Replace("\n", "\\n");
				text = text.Replace("\r", "\\r");
				text = text.Replace("\0", string.Empty);
				text = text.Replace("\u2028", string.Empty);
				text = text.Replace("\u2029", string.Empty);
				return '"' + text + '"';
			}
			if (o is int || o is short || o is uint || o is ushort || o is byte)
			{
				return o.ToString();
			}
			if (o is float)
			{
				NumberFormatInfo numberFormat = CultureInfo.InvariantCulture.NumberFormat;
				return ((float)o).ToString(numberFormat);
			}
			if (o is double)
			{
				NumberFormatInfo numberFormat2 = CultureInfo.InvariantCulture.NumberFormat;
				return ((double)o).ToString(numberFormat2);
			}
			if (o is char)
			{
				if ((char)o == '"')
				{
					return "\"\\\"\"";
				}
				return '"' + o.ToString() + '"';
			}
			if (o is IList)
			{
				IList list = (IList)o;
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("new Array(");
				int count = list.Count;
				for (int i = 0; i < count; i++)
				{
					if (i != 0)
					{
						stringBuilder.Append(", ");
					}
					stringBuilder.Append(ObjectToJSString(list[i]));
				}
				stringBuilder.Append(")");
				return stringBuilder.ToString();
			}
			return ObjectToJSString(o.ToString());
		}

		public static void ExternalCall(string functionName, params object[] args)
		{
			Internal_ExternalCall(BuildInvocationForArguments(functionName, args));
		}

		private static string BuildInvocationForArguments(string functionName, params object[] args)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(functionName);
			stringBuilder.Append('(');
			int num = args.Length;
			for (int i = 0; i < num; i++)
			{
				if (i != 0)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append(ObjectToJSString(args[i]));
			}
			stringBuilder.Append(')');
			stringBuilder.Append(';');
			return stringBuilder.ToString();
		}

		public static void ExternalEval(string script)
		{
			if (script.Length > 0 && script[script.Length - 1] != ';')
			{
				script += ';';
			}
			Internal_ExternalCall(script);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void Internal_ExternalCall(string script);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		internal static extern int GetBuildUnityVersion();

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		internal static extern int GetNumericUnityVersion(string version);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern void OpenURL(string url);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[Obsolete("For internal use only")]
		[WrapperlessIcall]
		public static extern void CommitSuicide(int mode);

		public static void RegisterLogCallback(LogCallback handler)
		{
			s_LogCallback = handler;
			SetLogCallbackDefined(handler != null, false);
		}

		public static void RegisterLogCallbackThreaded(LogCallback handler)
		{
			s_LogCallback = handler;
			SetLogCallbackDefined(handler != null, true);
		}

		private static void CallLogCallback(string logString, string stackTrace, LogType type)
		{
			if (s_LogCallback != null)
			{
				s_LogCallback(logString, stackTrace, type);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void SetLogCallbackDefined(bool defined, bool threaded);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern AsyncOperation RequestUserAuthorization(UserAuthorization mode);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern bool HasUserAuthorization(UserAuthorization mode);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		internal static extern void ReplyToUserAuthorizationRequest(bool reply, [DefaultValue("false")] bool remember);

		[ExcludeFromDocs]
		internal static void ReplyToUserAuthorizationRequest(bool reply)
		{
			bool remember = false;
			ReplyToUserAuthorizationRequest(reply, remember);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern int GetUserAuthorizationRequestMode_Internal();

		internal static UserAuthorization GetUserAuthorizationRequestMode()
		{
			return (UserAuthorization)GetUserAuthorizationRequestMode_Internal();
		}
	}
}

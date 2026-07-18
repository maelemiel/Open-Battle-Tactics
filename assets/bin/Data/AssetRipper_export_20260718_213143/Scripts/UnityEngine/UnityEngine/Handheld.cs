using System.Runtime.CompilerServices;
using UnityEngine.Internal;

namespace UnityEngine
{
	public sealed class Handheld
	{
		public static extern bool use32BitDisplayBuffer
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public static bool PlayFullScreenMovie(string path, [DefaultValue("Color.black")] Color bgColor, [DefaultValue("FullScreenMovieControlMode.Full")] FullScreenMovieControlMode controlMode, [DefaultValue("FullScreenMovieScalingMode.AspectFit")] FullScreenMovieScalingMode scalingMode)
		{
			return INTERNAL_CALL_PlayFullScreenMovie(path, ref bgColor, controlMode, scalingMode);
		}

		[ExcludeFromDocs]
		public static bool PlayFullScreenMovie(string path, Color bgColor, FullScreenMovieControlMode controlMode)
		{
			FullScreenMovieScalingMode scalingMode = FullScreenMovieScalingMode.AspectFit;
			return INTERNAL_CALL_PlayFullScreenMovie(path, ref bgColor, controlMode, scalingMode);
		}

		[ExcludeFromDocs]
		public static bool PlayFullScreenMovie(string path, Color bgColor)
		{
			FullScreenMovieScalingMode scalingMode = FullScreenMovieScalingMode.AspectFit;
			FullScreenMovieControlMode controlMode = FullScreenMovieControlMode.Full;
			return INTERNAL_CALL_PlayFullScreenMovie(path, ref bgColor, controlMode, scalingMode);
		}

		[ExcludeFromDocs]
		public static bool PlayFullScreenMovie(string path)
		{
			FullScreenMovieScalingMode scalingMode = FullScreenMovieScalingMode.AspectFit;
			FullScreenMovieControlMode controlMode = FullScreenMovieControlMode.Full;
			Color bgColor = Color.black;
			return INTERNAL_CALL_PlayFullScreenMovie(path, ref bgColor, controlMode, scalingMode);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern bool INTERNAL_CALL_PlayFullScreenMovie(string path, ref Color bgColor, FullScreenMovieControlMode controlMode, FullScreenMovieScalingMode scalingMode);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern void Vibrate();

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		internal static extern void SetActivityIndicatorStyleImpl(int style);

		public static void SetActivityIndicatorStyle(AndroidActivityIndicatorStyle style)
		{
			SetActivityIndicatorStyleImpl((int)style);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern int GetActivityIndicatorStyle();

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern void StartActivityIndicator();

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern void StopActivityIndicator();

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern void ClearShaderCache();
	}
}

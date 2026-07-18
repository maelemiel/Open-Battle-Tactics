using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class iPhoneUtils
	{
		[Obsolete("isApplicationGenuine property is deprecated. Please use Application.genuine instead.")]
		public static extern bool isApplicationGenuine
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		[Obsolete("isApplicationGenuineAvailable property is deprecated. Please use Application.genuineCheckAvailable instead.")]
		public static extern bool isApplicationGenuineAvailable
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		[Obsolete("PlayMovie method is deprecated. Please use Handheld.PlayFullScreenMovie instead.")]
		public static void PlayMovie(string path, Color bgColor, int controlMode, int scalingMode)
		{
			INTERNAL_CALL_PlayMovie(path, ref bgColor, controlMode, scalingMode);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_PlayMovie(string path, ref Color bgColor, int controlMode, int scalingMode);

		[Obsolete("PlayMovie method is deprecated. Please use Handheld.PlayFullScreenMovie instead.")]
		public static void PlayMovie(string path, Color bgColor, iPhoneMovieControlMode controlMode, iPhoneMovieScalingMode scalingMode)
		{
			PlayMovie(path, bgColor, (int)controlMode, (int)scalingMode);
		}

		[Obsolete("PlayMovie method is deprecated. Please use Handheld.PlayFullScreenMovie instead.")]
		public static void PlayMovie(string path, Color bgColor, iPhoneMovieControlMode controlMode)
		{
			PlayMovie(path, bgColor, (int)controlMode, 1);
		}

		[Obsolete("PlayMovie method is deprecated. Please use Handheld.PlayFullScreenMovie instead.")]
		public static void PlayMovie(string path, Color bgColor)
		{
			PlayMovie(path, bgColor, 0, 1);
		}

		[Obsolete("PlayMovieURL method is deprecated. Please use Handheld.PlayFullScreenMovie instead.")]
		public static void PlayMovieURL(string url, Color bgColor, int controlMode, int scalingMode)
		{
			INTERNAL_CALL_PlayMovieURL(url, ref bgColor, controlMode, scalingMode);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_PlayMovieURL(string url, ref Color bgColor, int controlMode, int scalingMode);

		[Obsolete("PlayMovieURL method is deprecated. Please use Handheld.PlayFullScreenMovie instead.")]
		public static void PlayMovieURL(string url, Color bgColor, iPhoneMovieControlMode controlMode, iPhoneMovieScalingMode scalingMode)
		{
			PlayMovieURL(url, bgColor, (int)controlMode, (int)scalingMode);
		}

		[Obsolete("PlayMovieURL method is deprecated. Please use Handheld.PlayFullScreenMovie instead.")]
		public static void PlayMovieURL(string url, Color bgColor, iPhoneMovieControlMode controlMode)
		{
			PlayMovieURL(url, bgColor, (int)controlMode, 1);
		}

		[Obsolete("PlayMovieURL method is deprecated. Please use Handheld.PlayFullScreenMovie instead.")]
		public static void PlayMovieURL(string url, Color bgColor)
		{
			PlayMovieURL(url, bgColor, 0, 1);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		[Obsolete("Vibrate method is deprecated. Please use Handheld.Vibrate instead.")]
		public static extern void Vibrate();
	}
}

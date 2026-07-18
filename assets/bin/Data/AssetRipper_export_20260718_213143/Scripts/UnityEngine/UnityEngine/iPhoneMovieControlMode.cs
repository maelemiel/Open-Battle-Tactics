using System;

namespace UnityEngine
{
	[Obsolete("iPhoneMovieControlMode enumeration is deprecated. Please use FullScreenMovieControlMode instead.")]
	public enum iPhoneMovieControlMode
	{
		Full = 0,
		Minimal = 1,
		CancelOnTouch = 2,
		Hidden = 3,
		[Obsolete("VolumeOnly is deprecated. Please use iPhoneMovieControlMode.Minimal instead.")]
		VolumeOnly = 1
	}
}

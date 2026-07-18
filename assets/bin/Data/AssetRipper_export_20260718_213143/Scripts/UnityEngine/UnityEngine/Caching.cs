using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class Caching
	{
		[Obsolete("this API is not for public use.")]
		public static extern CacheIndex[] index
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public static extern long spaceFree
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public static extern long maximumAvailableDiskSpace
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public static extern long spaceOccupied
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		[Obsolete("Please use Caching.spaceFree instead")]
		public static extern int spaceAvailable
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		[Obsolete("Please use Caching.spaceOccupied instead")]
		public static extern int spaceUsed
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public static extern int expirationDelay
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

		public static extern bool ready
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public static bool Authorize(string name, string domain, long size, string signature)
		{
			return Authorize(name, domain, size, -1, signature);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern bool Authorize(string name, string domain, long size, int expiration, string signature);

		[Obsolete("Size is now specified as a long")]
		public static bool Authorize(string name, string domain, int size, int expiration, string signature)
		{
			return Authorize(name, domain, (long)size, expiration, signature);
		}

		[Obsolete("Size is now specified as a long")]
		public static bool Authorize(string name, string domain, int size, string signature)
		{
			return Authorize(name, domain, (long)size, signature);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern bool CleanCache();

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		[Obsolete("this API is not for public use.")]
		public static extern bool CleanNamedCache(string name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		[Obsolete("This function is obsolete and has no effect.")]
		public static extern bool DeleteFromCache(string url);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		[Obsolete("This function is obsolete and will always return -1. Use IsVersionCached instead.")]
		public static extern int GetVersionFromCache(string url);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern bool IsVersionCached(string url, int version);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern bool MarkAsUsed(string url, int version);
	}
}

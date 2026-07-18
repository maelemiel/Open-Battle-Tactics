using System;

namespace UnityEngine
{
	public enum ConnectionTesterStatus
	{
		Error = -2,
		Undetermined = -1,
		[Obsolete("No longer returned, use newer connection tester enums instead.")]
		PrivateIPNoNATPunchthrough = 0,
		[Obsolete("No longer returned, use newer connection tester enums instead.")]
		PrivateIPHasNATPunchThrough = 1,
		PublicIPIsConnectable = 2,
		PublicIPPortBlocked = 3,
		PublicIPNoServerStarted = 4,
		LimitedNATPunchthroughPortRestricted = 5,
		LimitedNATPunchthroughSymmetric = 6,
		NATpunchthroughFullCone = 7,
		NATpunchthroughAddressRestrictedCone = 8
	}
}

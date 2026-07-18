using System;
using System.Collections.Generic;
using MobageEditor;

namespace Mobage
{
	public class Reward
	{
		public delegate void redeemRewardCode_onCompleteCallback(SimpleAPIStatus status, Error error, User inviter, int redemptions, string payload);

		private static int cbUidGenerator = 0;

		private static Dictionary<int, Delegate> pendingCallbacks = new Dictionary<int, Delegate>();
	}
}

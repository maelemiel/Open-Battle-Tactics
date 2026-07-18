namespace LCD.User
{
	public class StoreAccount
	{
		public enum StoreType
		{
			GOOGLE = 0,
			AMAZON = 1,
			APPLE = 2,
			UNITY_EDITOR = 3
		}

		public readonly StoreType storeType;

		public readonly string storeUserId;

		public readonly string advertisingId;

		public readonly string deviceToken;

		internal StoreAccount(StoreType storeType, string storeUserId, string advertisingId, string deviceToken)
		{
			this.storeType = storeType;
			this.storeUserId = storeUserId;
			this.advertisingId = advertisingId;
			this.deviceToken = deviceToken;
		}

		internal static StoreType getStoreType(string storeType)
		{
			if (storeType.Equals("GOOGLE"))
			{
				return StoreType.GOOGLE;
			}
			if (storeType.Equals("AMAZON"))
			{
				return StoreType.AMAZON;
			}
			if (storeType.Equals("APPLE"))
			{
				return StoreType.APPLE;
			}
			return StoreType.UNITY_EDITOR;
		}
	}
}

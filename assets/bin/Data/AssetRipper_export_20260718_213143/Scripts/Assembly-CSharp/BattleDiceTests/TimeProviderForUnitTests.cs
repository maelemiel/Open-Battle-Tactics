namespace BattleDiceTests
{
	internal class TimeProviderForUnitTests : TimeManager.ITimeProvider
	{
		private long _currentDeviceTime;

		public long CurrentDeviceTime
		{
			set
			{
				_currentDeviceTime = value;
			}
		}

		public long GetCurrentDevicetime()
		{
			return _currentDeviceTime;
		}
	}
}

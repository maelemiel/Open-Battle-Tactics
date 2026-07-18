namespace System.Net.NetworkInformation
{
	public sealed class NetworkChange
	{
		public static event NetworkAddressChangedEventHandler NetworkAddressChanged;

		public static event NetworkAvailabilityChangedEventHandler NetworkAvailabilityChanged;

		private NetworkChange()
		{
		}
	}
}

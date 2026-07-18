namespace System.Net
{
	public class IPHostEntry
	{
		private IPAddress[] addressList;

		private string[] aliases;

		private string hostName;

		public IPAddress[] AddressList
		{
			get
			{
				return addressList;
			}
			set
			{
				addressList = value;
			}
		}

		public string[] Aliases
		{
			get
			{
				return aliases;
			}
			set
			{
				aliases = value;
			}
		}

		public string HostName
		{
			get
			{
				return hostName;
			}
			set
			{
				hostName = value;
			}
		}
	}
}

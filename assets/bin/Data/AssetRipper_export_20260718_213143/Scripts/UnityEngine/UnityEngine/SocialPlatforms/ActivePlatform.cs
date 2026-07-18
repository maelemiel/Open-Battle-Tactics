namespace UnityEngine.SocialPlatforms
{
	internal static class ActivePlatform
	{
		private static ISocialPlatform _active;

		internal static ISocialPlatform Instance
		{
			get
			{
				if (_active == null)
				{
					_active = SelectSocialPlatform();
				}
				return _active;
			}
			set
			{
				_active = value;
			}
		}

		private static ISocialPlatform SelectSocialPlatform()
		{
			return new Local();
		}
	}
}

namespace Mono.Security.Protocol.Tls
{
	internal class Alert
	{
		private AlertLevel level;

		private AlertDescription description;

		public AlertLevel Level
		{
			get
			{
				return level;
			}
		}

		public AlertDescription Description
		{
			get
			{
				return description;
			}
		}

		public string Message
		{
			get
			{
				return GetAlertMessage(description);
			}
		}

		public bool IsWarning
		{
			get
			{
				return level == AlertLevel.Warning;
			}
		}

		public bool IsCloseNotify
		{
			get
			{
				if (IsWarning && description == AlertDescription.CloseNotify)
				{
					return true;
				}
				return false;
			}
		}

		public Alert(AlertDescription description)
		{
			inferAlertLevel();
			this.description = description;
		}

		public Alert(AlertLevel level, AlertDescription description)
		{
			this.level = level;
			this.description = description;
		}

		private void inferAlertLevel()
		{
			switch (description)
			{
			case AlertDescription.CloseNotify:
			case AlertDescription.UserCancelled:
			case AlertDescription.NoRenegotiation:
				level = AlertLevel.Warning;
				break;
			default:
				level = AlertLevel.Fatal;
				break;
			}
		}

		public static string GetAlertMessage(AlertDescription description)
		{
			return "The authentication or decryption has failed.";
		}
	}
}

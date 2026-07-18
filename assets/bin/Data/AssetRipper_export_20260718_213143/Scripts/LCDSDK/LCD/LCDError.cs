namespace LCD
{
	public class LCDError
	{
		public enum ErrorType
		{
			NETWORK_ERROR = 0,
			LCD_ERROR = 1,
			USER_CANCEL = 2
		}

		public readonly ErrorType errorType;

		public readonly int errorCode;

		public readonly string errorMessage;

		internal LCDError(ErrorType errorType, int errorCode, string errorMessage)
		{
			this.errorType = errorType;
			this.errorCode = errorCode;
			this.errorMessage = errorMessage;
		}

		internal LCDError(string errorType, int errorCode, string errorMessage)
		{
			if ("cancel".Equals(errorType))
			{
				this.errorType = ErrorType.USER_CANCEL;
			}
			else if ("USER_CANCEL".Equals(errorType))
			{
				this.errorType = ErrorType.USER_CANCEL;
			}
			else if ("NETWORK_ERROR".Equals(errorType))
			{
				this.errorType = ErrorType.NETWORK_ERROR;
			}
			else if ("network".Equals(errorType))
			{
				this.errorType = ErrorType.NETWORK_ERROR;
			}
			else
			{
				this.errorType = ErrorType.LCD_ERROR;
			}
			this.errorCode = errorCode;
			this.errorMessage = errorMessage;
		}
	}
}

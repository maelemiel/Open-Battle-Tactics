using LitJson0;

public class ErrorHandler : DeNetworkErrorHandler
{
	public bool CheckError<T>(TypedRestResponse<T> restResponse)
	{
		if (restResponse.Error != null)
		{
			restResponse.ThreadPoolManager.RunOnMainThread(delegate
			{
				UINetworkErrorHandler.DisplayMessage(restResponse as TypedRestResponse<JsonObject>);
			});
			return true;
		}
		return false;
	}

	public bool CheckError(RestResponse restResponse)
	{
		if (restResponse.Error != null)
		{
			restResponse.ThreadPoolManager.RunOnMainThread(delegate
			{
				UINetworkErrorHandler.DisplayMessage((string)restResponse.Error);
			});
			return true;
		}
		return false;
	}
}

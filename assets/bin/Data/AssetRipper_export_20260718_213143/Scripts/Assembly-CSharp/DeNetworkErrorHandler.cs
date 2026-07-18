public interface DeNetworkErrorHandler
{
	bool CheckError<T>(TypedRestResponse<T> restResponse);

	bool CheckError(RestResponse restResponse);
}

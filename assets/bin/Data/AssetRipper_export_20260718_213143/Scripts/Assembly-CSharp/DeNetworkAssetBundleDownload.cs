public class DeNetworkAssetBundleDownload
{
	public string Endpoint;

	public string LocalPath;

	public string Hash;

	public DeNetworkError Error;

	public DeNetworkAssetBundleDownload()
	{
	}

	public DeNetworkAssetBundleDownload(RestResponse response)
	{
		LocalPath = (string)response.Body;
	}

	public static DeNetworkAssetBundleDownload valueOfError(string message)
	{
		DeNetworkAssetBundleDownload deNetworkAssetBundleDownload = new DeNetworkAssetBundleDownload();
		deNetworkAssetBundleDownload.Error = new DeNetworkError(message);
		return deNetworkAssetBundleDownload;
	}
}

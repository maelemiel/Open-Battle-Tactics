using System;
using System.Collections.Generic;
using OAuth;

public class DeNetwork
{
	private enum Clients
	{
		Server = 0,
		AssetCDN = 1
	}

	private static string HEADER_SET_COOKIE = "set-cookie";

	private static string HEADER_COOKIE = "Cookie";

	private Dictionary<Clients, RestClient> _clients = new Dictionary<Clients, RestClient>();

	private ThreadPoolManager _threadPoolManager;

	private string _serverHost;

	private string _tempTokenPath;

	private string _tokenPath;

	private string _oauthKey;

	private string _oauthSecret;

	private int _maxLoginAttempts = 3;

	private string _sessionCookie;

	private string _filesPath;

	private DeNetworkLogin _networkLoginImpl;

	public DeNetwork(int concurrency)
	{
		_threadPoolManager = new ThreadPoolManager(concurrency);
	}

	public DeNetwork MaxLoginAttempts(int maxLoginAttempts)
	{
		_maxLoginAttempts = maxLoginAttempts;
		return this;
	}

	public DeNetwork SetupServer(string host, string tempTokenPath = "", string tokenPath = "")
	{
		_tempTokenPath = tempTokenPath;
		_tokenPath = tokenPath;
		AddClient(Clients.Server, host, _threadPoolManager);
		SetupBasicServerMiddleware();
		return this;
	}

	public DeNetwork SetupAssetBundles(string filesPath, string host)
	{
		return SetupAssetBundles(filesPath, host, _threadPoolManager);
	}

	public DeNetwork SetupAssetBundles(string filesPath, string host, ThreadPoolManager threadPoolManager)
	{
		_filesPath = filesPath;
		AddClient(Clients.AssetCDN, host, threadPoolManager);
		Assets().SetMediaType("text/plain", new BinaryDownloadMediaType(filesPath));
		Assets().BeforeRequest(delegate(RestRequest request)
		{
			request.Header("Accept-Encoding", "gzip");
		});
		Assets().UseLogMiddleware();
		return this;
	}

	public RestClient Server()
	{
		return _clients[Clients.Server];
	}

	public RestClient Assets()
	{
		return _clients[Clients.AssetCDN];
	}

	public void DownloadAsset(string uri, string md5, Action<DeNetworkAssetBundleDownload> callback)
	{
		DownloadAsset(uri, null, md5, callback);
	}

	public void DownloadAsset(string uri, string filename, string md5, Action<DeNetworkAssetBundleDownload> callback)
	{
		RestRequest restRequest = Assets().At(uri).Get();
		if (filename != null)
		{
			restRequest.Config("filename", filename);
		}
		if (md5 != null)
		{
			restRequest.Config("md5", md5);
		}
		restRequest.Response(delegate(RestResponse response)
		{
			if (response.StatusCode != 200)
			{
				callback(DeNetworkAssetBundleDownload.valueOfError("Error downloading asset bundle"));
			}
			else
			{
				callback(new DeNetworkAssetBundleDownload(response));
			}
		});
	}

	private void AddClient(Clients client, string host, ThreadPoolManager threadPoolManager)
	{
		_clients.Add(client, new RestClient(threadPoolManager).Host(host));
	}

	public DeNetwork StartLoginProcess(Action<DeNetworkError> callback)
	{
		_networkLoginImpl.Login(delegate(DeNetworkError loginError)
		{
			if (loginError != null)
			{
				callback(loginError);
			}
			else
			{
				RestRequest restRequest = Server().At(_tempTokenPath);
				restRequest.Header("Authorization", AuthorizationHeader(restRequest.Path, "GET", string.Empty, string.Empty));
				_networkLoginImpl.TemporaryTokenRequest(restRequest, delegate(DeNetworkError tempTokenError, string token)
				{
					if (tempTokenError != null)
					{
						callback(tempTokenError);
					}
					else
					{
						_networkLoginImpl.Authorize(token, delegate(DeNetworkError authorizeError, string verifier)
						{
							if (authorizeError != null)
							{
								callback(authorizeError);
							}
							else
							{
								RestRequest restRequest2 = Server().At(_tokenPath);
								restRequest2.Header("Authorization", AuthorizationHeader(restRequest2.Path, "GET", token, string.Empty));
								_networkLoginImpl.TokenRequest(restRequest2, verifier, callback);
							}
						});
					}
				});
			}
		});
		return this;
	}

	private void SetupBasicServerMiddleware()
	{
		Server().BeforeRequest(AddSessionCookie);
		Server().AfterResponse(SaveSessionCookie);
	}

	private void AddSessionCookie(RestRequest request)
	{
		if (_sessionCookie != null)
		{
			request.Header(HEADER_COOKIE, _sessionCookie);
		}
	}

	private void SaveSessionCookie(RestResponse response)
	{
		if (response.Headers.ContainsKey(HEADER_SET_COOKIE))
		{
			_sessionCookie = response.Headers[HEADER_SET_COOKIE].ToString();
		}
	}

	private string AuthorizationHeader(string url, string method, string oauthToken = "", string oauthTokenSecret = "")
	{
		string nonce = Manager.GenerateNonce();
		string timeStamp = Manager.GenerateTimeStamp();
		Uri url2 = new Uri(url);
		string signature = Manager.GenerateSignature(url2, _oauthKey, _oauthSecret, oauthToken, oauthTokenSecret, method, timeStamp, nonce);
		return Manager.GenerateAuthorizationHeader(url2, _oauthKey, oauthToken, timeStamp, nonce, "HMAC-SHA1", signature);
	}
}

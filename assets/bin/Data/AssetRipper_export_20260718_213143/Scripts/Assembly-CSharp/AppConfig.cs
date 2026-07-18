using System.Collections.Generic;
using com.adjust.sdk;

public class AppConfig
{
	public enum EnvironmentType
	{
		Local = 0,
		Vagrant = 1,
		Dev = 2,
		Staging = 3,
		Hotfix = 4,
		Dev3 = 5,
		Production = 6
	}

	public enum PlatformType
	{
		ios = 0,
		android = 1,
		amazon = 2
	}

	public const string REQUEST_SIGNING_KEY = "mSyRT3h6Qk";

	private const EnvironmentType DEFAULT_ENV = EnvironmentType.Production;

	public static string buildHash = "c0fec28";

	public static string buildDate = "2015-05-04 13:38:31";

	public static PlatformType platform = PlatformType.android;

	public static bool offlineMode = false;

	public static bool useNobage = false;

	public static bool keepLocalDataModelChanges = false;

	public static bool useLocalAssetBundles = offlineMode;

	public static string appId = "Super-Battle-Tactics-Android";

	public static string oauthKey = "F05U7HxnGoQjXgi2ml193w";

	public static string oauthKeySecret = "QNI54o33zuSYibw8p7JIw4Rr3hrgi3oHfqRdonlrlo";

	public static string premiumCurrency = "MobaCoins";

	public static SceneTransitionManager.Scene defaultScene = SceneTransitionManager.Scene.ArenaScene;

	public static int networkRetries = 3;

	public static int networkTimeout = 15000;

	public static string ndkVersion = "2.5.10";

	public static string adjustToken = "wzn4c2hjmgts";

	public static AdjustUtil.AdjustEnvironment adjustEnviroment = AdjustUtil.AdjustEnvironment.Production;

	public static AdjustUtil.LogLevel adjustLogInfo = AdjustUtil.LogLevel.Error;

	public static string specName = "Local";

	public static bool enableDebug = true;

	public static bool removeSocial;

	public static bool skipTutorial;

	public static bool skipPostTutorial;

	public static bool showDMandLAPopups = true;

	public static bool logAssetBundles;

	public static bool simulateBadNetwork;

	public static bool skipMobageLogin;

	public static bool enableGameCenter;

	public static bool traceNetworkRequests;

	public static bool enableFPS;

	public static bool verboseErrors;

	public static bool enableDebugMenu;

	public static string clientVersion = "1.1.5";

	private static KeyValueStorage _clientVersionKVS;

	private static KeyValueStorage _environmentKVS;

	public static string ServerProd = "https://sbt-green.denastudioscanada.com";

	public static string ProductionMapURL = "http://d3dhq3qiq7lleq.cloudfront.net/server.production.json";

	private static Dictionary<EnvironmentType, string> _serversList = new Dictionary<EnvironmentType, string> { 
	{
		EnvironmentType.Production,
		ServerProd
	} };

	public static string Server;

	public static EnvironmentType currentEnvironmentType = EnvironmentType.Production;

	public static string pubnubPublishKey = "pub-c-299e1de0-2968-4722-b32d-a39163999f52";

	public static string pubnubSubscribeKey = "sub-c-653f060e-004e-11e3-abed-02ee2ddab7fe";

	public static string pubnubSecretKey = string.Empty;

	public static string pubnubCipherKey = string.Empty;

	public static bool pubnubSsIOn;

	public static bool IsNewVersion
	{
		get
		{
			if (_clientVersionKVS == null)
			{
				_clientVersionKVS = KeyValueStorage.Instance(KeyValueStorage.Storage.CLIENT);
			}
			string value = _clientVersionKVS.GetValue<string>("version");
			if (value == clientVersion)
			{
				return false;
			}
			_clientVersionKVS.SetValue("version", clientVersion);
			if (string.IsNullOrEmpty(value))
			{
				return false;
			}
			return true;
		}
	}

	public static KeyValueStorage environmentKVS
	{
		get
		{
			_environmentKVS = KeyValueStorage.Instance(KeyValueStorage.Storage.ENVIRONMENT);
			return _environmentKVS;
		}
	}

	public static Dictionary<EnvironmentType, string> AvailableServers
	{
		get
		{
			return _serversList;
		}
	}

	public static InitializationManager InitManager { get; set; }

	public static Dictionary<EnvironmentType, string> ServersList
	{
		get
		{
			return _serversList;
		}
	}

	public static bool UseNobage()
	{
		if (currentEnvironmentType == EnvironmentType.Dev || currentEnvironmentType == EnvironmentType.Staging || currentEnvironmentType == EnvironmentType.Vagrant || currentEnvironmentType == EnvironmentType.Local || currentEnvironmentType == EnvironmentType.Dev3 || currentEnvironmentType == EnvironmentType.Hotfix)
		{
			return useNobage;
		}
		return false;
	}

	private static void SetProductionKeys()
	{
		oauthKey = "0cTO7BhGYxT7xgZdmgisw";
		oauthKeySecret = "9hVd1arzKPmjQ30hxMZkKIqpzfeVV3TuOmrXgoMz8";
	}

	public static void UpdateEnvironmentURL(EnvironmentType environmentType, string newURL)
	{
		if (_serversList.ContainsKey(environmentType))
		{
			_serversList[environmentType] = newURL;
		}
		else
		{
			Log.Warning("Trying to set a non-available EnvironmentType: " + environmentType);
		}
	}

	public static void InitEnvironment()
	{
		if (currentEnvironmentType != EnvironmentType.Production)
		{
			enableFPS = true;
			verboseErrors = true;
			enableDebugMenu = true;
			traceNetworkRequests = true;
			LoadCurrentEnvironment();
		}
		else
		{
			enableFPS = false;
			verboseErrors = false;
			enableDebugMenu = false;
			traceNetworkRequests = true;
			logAssetBundles = false;
			SetEnvironment(EnvironmentType.Production);
		}
	}

	public static void LoadCurrentEnvironment()
	{
		if (currentEnvironmentType != EnvironmentType.Production)
		{
			string value = environmentKVS.GetValue<string>("current");
			if (!string.IsNullOrEmpty(value))
			{
				SetEnvironment(ParseUtility.ParseEnum(value, EnvironmentType.Production));
			}
			else
			{
				SetEnvironment(EnvironmentType.Production);
			}
		}
	}

	public static void SaveCurrentEnvironment(EnvironmentType type)
	{
		environmentKVS.SetValue("current", type.ToString());
		SetEnvironment(type);
	}

	public static void SetEnvironment(EnvironmentType type)
	{
		currentEnvironmentType = type;
		Server = _serversList[type];
	}
}

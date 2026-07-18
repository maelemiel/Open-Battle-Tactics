using System;
using System.Collections.Generic;

public static class TestTimeUtility
{
	public class TestTimeContainer
	{
		public int quantity;

		public float totalTime;

		public float min;

		public float max;

		public float average
		{
			get
			{
				return totalTime / (float)quantity;
			}
		}
	}

	private static Dictionary<string, TestTimeContainer> requestsTimeDictionary = new Dictionary<string, TestTimeContainer>();

	private static Dictionary<string, TestTimeContainer> transitionsTimeDictionary = new Dictionary<string, TestTimeContainer>();

	private static List<string> allRequestsAndTransitions = new List<string>();

	public static void AddRequestTime(string url, float deltaTime)
	{
		AddTime(url, deltaTime, requestsTimeDictionary);
	}

	public static void AddTransitionTime(string sceneName, float deltaTime)
	{
		AddTime(sceneName, deltaTime, transitionsTimeDictionary);
	}

	private static void AddTime(string key, float deltaTime, Dictionary<string, TestTimeContainer> testDictionary)
	{
		if (!AppConfig.traceNetworkRequests)
		{
			return;
		}
		if (!testDictionary.ContainsKey(key))
		{
			TestTimeContainer testTimeContainer = new TestTimeContainer();
			testTimeContainer.quantity = 1;
			testTimeContainer.totalTime = deltaTime;
			testTimeContainer.min = deltaTime;
			testTimeContainer.max = deltaTime;
			TestTimeContainer value = testTimeContainer;
			testDictionary.Add(key, value);
			return;
		}
		testDictionary[key].quantity++;
		testDictionary[key].totalTime += deltaTime;
		if (deltaTime < testDictionary[key].min)
		{
			testDictionary[key].min = deltaTime;
		}
		if (deltaTime > testDictionary[key].max)
		{
			testDictionary[key].max = deltaTime;
		}
	}

	public static void AddRequest(string header, string url, string reqType)
	{
		if (AppConfig.traceNetworkRequests)
		{
			string text = DateTime.Now.ToLongTimeString();
			string text2 = "Start";
			if (SceneTransitionManager.readyToTransitionIn)
			{
				text2 = "Middle";
			}
			string empty = string.Empty;
			empty = ((SceneTransitionManager.CurrentSceneDM == null) ? ("Request:" + header + "," + text + "," + url + ",NoScene:" + text2 + ": " + reqType) : ("Request:" + header + "," + text + "," + url + "," + SceneTransitionManager.CurrentSceneDM._scene.ToString() + ":" + text2 + ": " + reqType));
			allRequestsAndTransitions.Add(empty);
		}
	}

	public static void AddTransition(string header, string fromScene, string toScene, string transitionType)
	{
		if (AppConfig.traceNetworkRequests)
		{
			string text = DateTime.Now.ToLongTimeString();
			string item = "Transition:" + header + "," + text + "," + fromScene + ": " + toScene + ": " + transitionType;
			allRequestsAndTransitions.Add(item);
		}
	}

	public static void AddLoginRequest(string header, string body)
	{
		if (AppConfig.traceNetworkRequests)
		{
			string text = DateTime.Now.ToLongTimeString();
			string item = "Login:" + header + "," + text + "," + body;
			allRequestsAndTransitions.Add(item);
		}
	}

	public static void PrintAllRequestsAndTransitions()
	{
		Log.Debug("LogAllRequestAndTransitions Start ----------------------------------------------------------------------------------------------------------------------------");
		foreach (string allRequestsAndTransition in allRequestsAndTransitions)
		{
			Log.Debug(allRequestsAndTransition);
		}
		Log.Debug("LogAllRequestAndTransitions End ----------------------------------------------------------------------------------------------------------------------------");
	}

	public static void PrintRequestsTimeDictionary()
	{
		Log.Debug("LogRequest Times Start ----------------------------------------------------------------------------------------------------------------------------");
		PrintTimeDictionary("URL", requestsTimeDictionary);
		Log.Debug("LogRequest Times END ----------------------------------------------------------------------------------------------------------------------------");
	}

	public static void PrintTranstionsTimeDictionary()
	{
		Log.Debug("LogTransition Times Start ----------------------------------------------------------------------------------------------------------------------------");
		PrintTimeDictionary("SCENE", transitionsTimeDictionary);
		Log.Debug("LogTransition Times End ----------------------------------------------------------------------------------------------------------------------------");
	}

	private static void PrintTimeDictionary(string header, Dictionary<string, TestTimeContainer> testDictionary)
	{
		Log.Debug("{0},Quantity,Average,Min,Max");
		foreach (KeyValuePair<string, TestTimeContainer> item in testDictionary)
		{
			Log.Debug(header + ": {0},{1},{2},{3},{4}", item.Key, item.Value.quantity, item.Value.average, item.Value.min, item.Value.max);
		}
	}
}

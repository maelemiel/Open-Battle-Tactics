using System;
using UnityEngine;

public class TestMatchConfiguration : MonoBehaviour
{
	public enum TestType
	{
		MatchJSON = 0,
		RandomMatch = 1,
		ConfiguredInCode = 2
	}

	public enum TestSpeed
	{
		Normal = 0,
		TimeX2 = 1,
		TimeX10 = 2,
		TimeX100 = 3
	}

	[Serializable]
	public class MatchJSONData
	{
		public TextAsset matchJson;

		public bool isPlayerTeamTwo;
	}

	public class CustomMatchData
	{
		[Serializable]
		public class TeamData
		{
			public string unitOneID;

			public string unitTwoID;

			public string unitThreeID;

			public string unitFourID;

			public string abilityOneID;

			public string abilityTwoID;

			public string abilityThreeID;

			public string abilityFourID;

			public int randomSeed;
		}

		public TeamData teamOne;

		public TeamData teamTwo;

		public int randomSeed;
	}

	[Serializable]
	public class RandomMatchData
	{
		public bool givePlayerAI;
	}

	private static TestMatchConfiguration _instance;

	public TestType type;

	public TestSpeed speed;

	public bool disablePoolWarnings;

	public bool overrideServer = true;

	public AppConfig.EnvironmentType serverEnvironment = AppConfig.EnvironmentType.Vagrant;

	public MatchJSONData matchJsonData;

	public CustomMatchData customMatchData;

	public RandomMatchData randomMatchData;

	private void Awake()
	{
		if (_instance != null)
		{
			UnityEngine.Object.DestroyImmediate(base.gameObject);
			return;
		}
		_instance = this;
		UnityEngine.Object.Destroy(base.gameObject);
	}
}

using System.Collections.Generic;
using UnityEngine;

public class TestConsole : Singleton<TestConsole>
{
	public class TestConsoleMessage
	{
		public string Description { get; set; }

		public string StackTrace { get; set; }

		public LogTypeExtended Type { get; set; }

		public TestConsoleMessage(string description, string stackTrace, LogTypeExtended logType)
		{
			Description = description;
			StackTrace = stackTrace;
			Type = logType;
		}
	}

	private const int MAX_TOUCHES_TO_ACTIVATE = 4;

	private const int MAX_ELEMENTS_AT_A_TIME = 1024;

	private int currentMessageIndex;

	private bool isConsoleActive;

	private List<TestConsoleMessage> consoleMessages = new List<TestConsoleMessage>(1024);

	public Dictionary<LogTypeExtended, bool> filters = new Dictionary<LogTypeExtended, bool>();

	private GUISkin currentGUISkin;

	private int currentFontSize = 16;

	private void Awake()
	{
		DisableConsole();
		RegisterLogTool.Instance.AddLogListener(HandleLog);
		filters.Add(LogTypeExtended.Assert, true);
		filters.Add(LogTypeExtended.Error, true);
		filters.Add(LogTypeExtended.Exception, true);
		filters.Add(LogTypeExtended.Log, true);
		filters.Add(LogTypeExtended.Warning, true);
		filters.Add(LogTypeExtended.Network, true);
		filters.Add(LogTypeExtended.Analytics, true);
	}

	private void EnableConsole()
	{
		isConsoleActive = true;
		currentMessageIndex = consoleMessages.Count - 5;
	}

	private void DisableConsole()
	{
		isConsoleActive = false;
	}

	private void OnDestroy()
	{
		RegisterLogTool.Instance.RemoveLogListener(HandleLog);
	}

	private void Update()
	{
		ApplyMovementInput();
		if (ActivationSequence())
		{
			ToggleConsole();
		}
	}

	private void ToggleConsole()
	{
		if (isConsoleActive)
		{
			DisableConsole();
		}
		else
		{
			EnableConsole();
		}
	}

	private bool ActivationSequence()
	{
		return (Input.touches.Length == 4 && Input.touches[3].phase == TouchPhase.Began) || Input.GetKeyDown(KeyCode.Tab);
	}

	private void ApplyMovementInput()
	{
		if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Moved)
		{
			currentMessageIndex += (int)Mathf.Sign(Input.touches[0].deltaPosition.y);
		}
		if (Input.GetAxis("Vertical") != 0f)
		{
			currentMessageIndex += (int)Mathf.Sign(Input.GetAxis("Vertical") * -1f);
		}
	}

	private void HandleLog(string logString, string stackTrace, LogType type)
	{
		HandleLogWithType(logString, stackTrace, (LogTypeExtended)type);
	}

	public void HandleLogWithType(string logString, string stackTrace, LogTypeExtended logType)
	{
		if (consoleMessages.Count == 1024)
		{
			consoleMessages.RemoveAt(0);
		}
		TestConsoleMessage item = new TestConsoleMessage(logString, stackTrace, logType);
		consoleMessages.Add(item);
	}

	private void OnGUI()
	{
		if (isConsoleActive)
		{
			currentGUISkin = GUI.skin;
			currentGUISkin.label.fontSize = currentFontSize;
			currentMessageIndex = Mathf.Clamp(currentMessageIndex, 0, consoleMessages.Count - 1);
			GUILayout.BeginArea(new Rect(0f, 0f, Screen.width, (float)Screen.height * 0.1f), string.Empty, "Box");
			DrawLogTypeExtendedCheckboxes();
			GUILayout.EndArea();
			GUILayout.BeginArea(new Rect(0f, (float)Screen.height * 0.1f, (float)Screen.width * 0.9f, (float)Screen.height - (float)Screen.height * 0.1f), string.Empty, "Box");
			GUILayout.BeginArea(new Rect(0f, 0f, (float)Screen.width * 0.9f, (float)Screen.height - (float)Screen.height * 0.1f), string.Empty, "Box");
			DrawConsoleText();
			GUILayout.EndArea();
			GUILayout.EndArea();
			GUILayout.BeginArea(new Rect((float)Screen.width * 0.9f, (float)Screen.height * 0.1f, (float)Screen.width * 0.1f, (float)Screen.height - (float)Screen.height * 0.1f), string.Empty, "Box");
			GUILayout.BeginArea(new Rect(0f, 0f, (float)Screen.width * 0.1f, (float)Screen.height - (float)Screen.height * 0.1f), string.Empty, "Box");
			DrawUpDownButtons();
			GUILayout.EndArea();
			GUILayout.EndArea();
		}
	}

	private void DrawConsoleText()
	{
		string empty = string.Empty;
		for (int i = currentMessageIndex; i < consoleMessages.Count; i++)
		{
			if (filters[consoleMessages[i].Type])
			{
				empty = ((consoleMessages[i].Type != LogTypeExtended.Error && consoleMessages[i].Type != LogTypeExtended.Exception && consoleMessages[i].Type != LogTypeExtended.Analytics) ? string.Empty : consoleMessages[i].StackTrace);
				GUI.color = GetColorByMessageType(consoleMessages[i].Type);
				GUILayout.Label(consoleMessages[i].Description + empty, GUILayout.ExpandWidth(true));
			}
		}
	}

	private void DrawLogTypeExtendedCheckboxes()
	{
		GUILayout.BeginHorizontal("Box");
		List<LogTypeExtended> list = new List<LogTypeExtended>(filters.Keys);
		for (int i = 0; i < list.Count; i++)
		{
			GUI.color = GetColorByMessageType(list[i]);
			filters[list[i]] = GUILayout.Toggle(filters[list[i]], list[i].ToString(), "Button", GUILayout.ExpandHeight(true));
		}
		GUILayout.EndHorizontal();
	}

	private void DrawUpDownButtons()
	{
		GUILayout.BeginVertical();
		GUI.color = Color.white;
		if (GUILayout.RepeatButton("UP", GUILayout.ExpandHeight(true)))
		{
			currentMessageIndex++;
		}
		if (GUILayout.RepeatButton("DOWN", GUILayout.ExpandHeight(true)))
		{
			currentMessageIndex--;
		}
		if (GUILayout.Button("Step \n UP", GUILayout.ExpandHeight(true)))
		{
			currentMessageIndex++;
		}
		if (GUILayout.Button("Step \n DOWN", GUILayout.ExpandHeight(true)))
		{
			currentMessageIndex--;
		}
		if (GUILayout.Button("Bigger \n Font", GUILayout.ExpandHeight(true)))
		{
			currentFontSize++;
		}
		if (GUILayout.Button("Smaller \n Font", GUILayout.ExpandHeight(true)))
		{
			currentFontSize--;
		}
		GUILayout.EndVertical();
	}

	private Color GetColorByMessageType(LogTypeExtended type)
	{
		switch (type)
		{
		case LogTypeExtended.Assert:
			return Color.green;
		case LogTypeExtended.Analytics:
			return Color.cyan;
		case LogTypeExtended.Error:
			return new Color(1f, 0.55f, 0f, 1f);
		case LogTypeExtended.Exception:
			return new Color(1f, 0.25f, 0f, 1f);
		case LogTypeExtended.Log:
			return Color.white;
		case LogTypeExtended.Warning:
			return Color.yellow;
		case LogTypeExtended.Network:
			return Color.blue;
		default:
			return Color.white;
		}
	}
}

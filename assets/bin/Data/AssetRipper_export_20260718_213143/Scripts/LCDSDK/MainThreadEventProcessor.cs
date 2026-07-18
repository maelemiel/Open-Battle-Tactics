using System;
using System.Collections.Generic;
using UnityEngine;

public class MainThreadEventProcessor : MonoBehaviour
{
	private static MainThreadEventProcessor instance;

	public static bool initialized;

	private object m_queueLock = new object();

	private List<Action> m_queuedEvents = new List<Action>();

	private List<Action> m_executingEvents = new List<Action>();

	public static MainThreadEventProcessor Instance
	{
		get
		{
			if (!initialized)
			{
				GameObject gameObject = new GameObject("MainThreadEventProcessorProxy");
				instance = gameObject.AddComponent<MainThreadEventProcessor>();
				initialized = true;
			}
			return instance;
		}
	}

	public static void Init()
	{
		GameObject gameObject = GameObject.Find("MainThreadEventProcessorProxy");
		if (gameObject == null)
		{
			gameObject = new GameObject("MainThreadEventProcessorProxy");
			instance = gameObject.AddComponent<MainThreadEventProcessor>();
		}
		else
		{
			instance = gameObject.GetComponent<MainThreadEventProcessor>();
		}
		initialized = true;
	}

	public void QueueEvent(Action action)
	{
		lock (m_queueLock)
		{
			m_queuedEvents.Add(action);
		}
	}

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	private void Start()
	{
		m_queuedEvents = new List<Action>();
		m_executingEvents = new List<Action>();
	}

	private void Update()
	{
		MoveQueuedEventsToExecuting();
		if (m_executingEvents != null)
		{
			while (m_executingEvents.Count > 0)
			{
				Action action = m_executingEvents[0];
				m_executingEvents.RemoveAt(0);
				action();
			}
		}
	}

	private void MoveQueuedEventsToExecuting()
	{
		lock (m_queueLock)
		{
			if (m_queuedEvents != null)
			{
				while (m_queuedEvents.Count > 0)
				{
					Action item = m_queuedEvents[0];
					m_executingEvents.Add(item);
					m_queuedEvents.RemoveAt(0);
				}
			}
		}
	}
}

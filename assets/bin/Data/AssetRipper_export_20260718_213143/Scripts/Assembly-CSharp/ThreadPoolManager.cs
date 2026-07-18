using System;
using System.Collections.Generic;

public class ThreadPoolManager
{
	public enum ThreadType
	{
		Normal = 0,
		HighPriority = 1
	}

	private Queue<Action<Action>> _allActions = new Queue<Action<Action>>();

	private int _concurrency;

	private List<JobThread> _threads;

	private bool _networkAvailable = true;

	private Action _lostNetworkConnectionCallback;

	private Action _recoveredNetworkConnectionCallback;

	public Action LostNetworkConnectionCallback
	{
		set
		{
			_lostNetworkConnectionCallback = value;
		}
	}

	public Action RecoveredNetworkConnectionCallback
	{
		set
		{
			_recoveredNetworkConnectionCallback = value;
		}
	}

	public ThreadPoolManager(int concurrency)
	{
		Singleton<ThreadPoolSingleton>.Instantiate();
		_concurrency = concurrency;
		setupThreads();
	}

	private void setupThreads()
	{
		_threads = new List<JobThread>();
		for (int i = 0; i < _concurrency; i++)
		{
			_threads.Add(new JobThread(this));
		}
	}

	public void Run(Action<Action> action)
	{
		Run(ThreadType.Normal, action);
	}

	public void Run(ThreadType threadType, Action<Action> action)
	{
		if (threadType == ThreadType.HighPriority)
		{
			UnshiftQueue(action);
		}
		else
		{
			_allActions.Enqueue(action);
		}
		Pulse();
	}

	public void Pulse()
	{
		if (_allActions.Count > 0)
		{
			JobThread jobThread = FindAvailableJobThread();
			if (jobThread != null)
			{
				jobThread.run(_allActions.Dequeue());
			}
		}
	}

	public void RunOnMainThread(Action action)
	{
		Singleton<ThreadPoolSingleton>.instance.EnqueueAction(action);
	}

	private JobThread FindAvailableJobThread()
	{
		if (_threads == null || !_networkAvailable)
		{
			return null;
		}
		foreach (JobThread thread in _threads)
		{
			if (thread.isAvailable())
			{
				return thread;
			}
		}
		return null;
	}

	public void NetworkNotAvailable()
	{
		Log.InfoTag("Lost internet connection", null, "NETWORK_STATUS");
		ToggleNetworkAvailable(false, _lostNetworkConnectionCallback);
	}

	public void InternetAvailable()
	{
		Log.InfoTag("Internet available", null, "NETWORK_STATUS");
		ToggleNetworkAvailable(true, _recoveredNetworkConnectionCallback);
		Pulse();
	}

	private void ToggleNetworkAvailable(bool status, Action action)
	{
		if (_networkAvailable != status && action != null)
		{
			RunOnMainThread(action);
		}
		_networkAvailable = status;
	}

	private void UnshiftQueue(Action<Action> action)
	{
		Action<Action>[] array = new Action<Action>[_allActions.Count];
		_allActions.CopyTo(array, 0);
		_allActions.Clear();
		_allActions.Enqueue(action);
		for (int i = 0; i < array.Length; i++)
		{
			_allActions.Enqueue(array[i]);
		}
	}
}

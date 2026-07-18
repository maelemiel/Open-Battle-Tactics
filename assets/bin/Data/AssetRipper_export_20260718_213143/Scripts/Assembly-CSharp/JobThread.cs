using System;
using System.Collections.Generic;
using System.Threading;

public class JobThread
{
	private Thread _thread;

	private Queue<Action<Action>> _actionQueue = new Queue<Action<Action>>();

	private bool _available = true;

	private ThreadPoolManager _manager;

	public JobThread(ThreadPoolManager manager)
	{
		_manager = manager;
		_thread = new Thread(threadFunc);
		_thread.Priority = ThreadPriority.BelowNormal;
		_thread.Start();
	}

	public bool isAvailable()
	{
		return _available;
	}

	public void run(Action<Action> action, bool withPriority = false)
	{
		lock (_actionQueue)
		{
			_actionQueue.Enqueue(action);
			Monitor.Pulse(_actionQueue);
		}
	}

	public void threadFunc()
	{
		while (true)
		{
			lock (_actionQueue)
			{
				while (_actionQueue.Count == 0)
				{
					Monitor.Wait(_actionQueue);
				}
				_available = false;
				Action<Action> action = _actionQueue.Dequeue();
				Action obj = delegate
				{
					_available = true;
					_manager.Pulse();
				};
				try
				{
					action(obj);
				}
				catch (Exception ex)
				{
					_available = true;
					Log.ErrorTag("Exception when running action: {0} \nKeep going as we cannot kill this thread.\n StackTrace: {1}", null, "Networklayer", ex.Message, ex.StackTrace);
				}
			}
		}
	}
}

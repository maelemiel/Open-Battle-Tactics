using System;
using System.Collections.Generic;

public class ThreadPoolSingleton : Singleton<ThreadPoolSingleton>
{
	private Queue<Action> _mainThreadQueue = new Queue<Action>();

	public void EnqueueAction(Action action)
	{
		lock (_mainThreadQueue)
		{
			_mainThreadQueue.Enqueue(action);
		}
	}

	private void Update()
	{
		Action[] array;
		lock (_mainThreadQueue)
		{
			int count = _mainThreadQueue.Count;
			if (count == 0)
			{
				return;
			}
			array = new Action[count];
			_mainThreadQueue.CopyTo(array, 0);
			_mainThreadQueue.Clear();
		}
		for (int i = 0; i < array.Length; i++)
		{
			array[i]();
		}
	}
}

using System;
using System.Collections;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : class
{
	protected static T _instance;

	public static T instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = SingletonManager.gameObject.AddComponent(typeof(T)) as T;
				SingletonManager.helperComponent = _instance as MonoBehaviour;
			}
			return _instance;
		}
	}

	public Singleton()
	{
		_instance = this as T;
	}

	public static bool IsInstantiated()
	{
		return _instance != null;
	}

	public static void Instantiate()
	{
		_instance = instance;
	}

	public void ExecuteAfterCoroutine(IEnumerator coroutine, Action action)
	{
		SingletonManager.ExecuteAfterCoroutine(coroutine, action);
	}
}

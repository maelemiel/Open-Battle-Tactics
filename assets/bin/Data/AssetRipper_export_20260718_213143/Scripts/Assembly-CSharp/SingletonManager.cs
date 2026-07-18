using System;
using System.Collections;
using UnityEngine;

public class SingletonManager
{
	private static MonoBehaviour _helperComponent;

	private static GameObject _gameObject;

	public static MonoBehaviour helperComponent
	{
		set
		{
			_helperComponent = value;
		}
	}

	public static GameObject gameObject
	{
		get
		{
			EnsureCreated();
			return _gameObject;
		}
	}

	public static void EnsureCreated()
	{
		if (_gameObject == null)
		{
			_gameObject = new GameObject("-SingletonManager");
			UnityEngine.Object.DontDestroyOnLoad(_gameObject);
		}
	}

	public static Coroutine StartCoroutine(IEnumerator e)
	{
		EnsureCreated();
		return _helperComponent.StartCoroutine(e);
	}

	public static void ExecuteAfterCoroutine(IEnumerator coroutine, Action action)
	{
		StartCoroutine(ExecuteAfterCoroutineActual(coroutine, action));
	}

	private static IEnumerator ExecuteAfterCoroutineActual(IEnumerator coroutine, Action action)
	{
		yield return StartCoroutine(coroutine);
		action();
	}
}

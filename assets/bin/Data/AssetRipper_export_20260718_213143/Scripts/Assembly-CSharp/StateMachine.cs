using System;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
	private bool _isInitialized;

	protected StateBehaviour _currentState;

	protected Dictionary<string, StateBehaviour> _registeredStates;

	public string State
	{
		get
		{
			if (_currentState != null)
			{
				return _currentState.name;
			}
			return null;
		}
	}

	public void Init()
	{
		_isInitialized = true;
		_currentState = null;
		_registeredStates = new Dictionary<string, StateBehaviour>();
	}

	public void RegisterState(string name, StateBehaviour _state)
	{
		try
		{
			if (!_isInitialized)
			{
				Init();
			}
			_registeredStates.Add(name, _state);
		}
		catch (ArgumentException)
		{
			Debug.LogError("Can't register state " + name + " more than once.");
		}
	}

	public void SetState(string name)
	{
		StateBehaviour currentState = _currentState;
		if (_currentState != null && name == _currentState.name)
		{
			return;
		}
		Debug.Log("Changing state to " + name);
		try
		{
			_currentState = _registeredStates[name];
		}
		catch (KeyNotFoundException ex)
		{
			Debug.LogError("Can't set unregistered state " + name + ". Exception: " + ex);
		}
		if (_currentState == null)
		{
			Debug.LogError("Can't set unregistered state " + name + ".");
			return;
		}
		if (currentState != null)
		{
			Debug.Log("Calling EndState on " + currentState.name);
			currentState.EndState();
		}
		Debug.Log("Calling StartState on " + _currentState.name);
		_currentState.StartState();
	}

	public void Update()
	{
		if (_currentState != null)
		{
			string state = _currentState.Run();
			SetState(state);
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineSoundEffectHandler : MonoBehaviour
{
	[SerializeField]
	private List<EngineSoundEffect> _items;

	public bool _carDamaged;

	public BattleController _controller;

	private AudioManager.RepeatingSfx _engine;

	private AudioManager.RepeatingSfx _engineSputter;

	private IEnumerator Start()
	{
		foreach (EngineSoundEffect effect in _items)
		{
			effect._handler = this;
		}
		while (_controller.phaseManager.currentPhase != Phase.INTRO)
		{
			yield return new WaitForFixedUpdate();
		}
		_engine = AudioTrigger.EngineStart.PlayRepeating();
	}

	private void FixedUpdate()
	{
		if (_carDamaged && _engineSputter == null)
		{
			_engineSputter = AudioTrigger.EngineSputter.PlayRepeating();
		}
		else if (!_carDamaged && _engineSputter != null)
		{
			_engineSputter.Stop();
			_engineSputter = null;
		}
	}

	private void OnDisable()
	{
		_engine.Stop();
		if (_engineSputter != null)
		{
			_engineSputter.Stop();
		}
	}

	public void RemoveEffect(EngineSoundEffect effect)
	{
		_items.Remove(effect);
		CheckCarDamaged();
	}

	public void CheckCarDamaged()
	{
		bool flag = false;
		foreach (EngineSoundEffect item in _items)
		{
			flag = flag || item.IsDamaged();
		}
		_carDamaged = flag;
	}
}

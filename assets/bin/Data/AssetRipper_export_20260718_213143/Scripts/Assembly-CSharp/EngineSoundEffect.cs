using UnityEngine;

public class EngineSoundEffect : MonoBehaviour
{
	public const float HPDAMAGED = 0.5f;

	[SerializeField]
	private UnitState _controller;

	public int _carHealth;

	public EngineSoundEffectHandler _handler;

	public void Init(UnitState state)
	{
		_controller = state;
		_carHealth = _controller.startingHp;
	}

	public void ChangeHealth(int health)
	{
		_carHealth = health;
		_handler.CheckCarDamaged();
	}

	private void OnDisable()
	{
		if ((bool)_handler)
		{
			_handler.RemoveEffect(this);
		}
	}

	public bool IsDamaged()
	{
		if (_controller == null)
		{
			return false;
		}
		return (float)_carHealth <= (float)_controller.startingHp * 0.5f;
	}
}

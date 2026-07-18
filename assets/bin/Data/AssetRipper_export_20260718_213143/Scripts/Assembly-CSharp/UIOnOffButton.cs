using UnityEngine;

public class UIOnOffButton : MonoBehaviour
{
	public enum StateType
	{
		On = 0,
		Off = 1
	}

	[SerializeField]
	public UIToggleButton _onButton;

	[SerializeField]
	private UIToggleButton _offButton;

	private StateType _state;

	public StateType State
	{
		get
		{
			return _state;
		}
	}

	public UIToggleButton OnButton
	{
		get
		{
			return _onButton;
		}
	}

	public UIToggleButton OffButton
	{
		get
		{
			return _offButton;
		}
	}

	private void Awake()
	{
		_onButton.ButtonPressedEvent += OnButtonChangeState;
		_offButton.ButtonPressedEvent += OffButtonChangeState;
	}

	public void Init(bool ifOnSelected)
	{
		if (ifOnSelected)
		{
			_onButton.SelectButton(true);
			_offButton.SelectButton();
		}
		else
		{
			_onButton.SelectButton();
			_offButton.SelectButton(true);
		}
		ChangeState(ifOnSelected);
	}

	private void OnButtonChangeState(tk2dButton source)
	{
		ChangeState(true);
	}

	private void OffButtonChangeState(tk2dButton source)
	{
		ChangeState(false);
	}

	private void ChangeState(bool ifOnSelected)
	{
		if (ifOnSelected)
		{
			_offButton.SelectButton();
			_onButton.enabled = false;
			_offButton.enabled = true;
			_state = StateType.On;
		}
		else
		{
			_onButton.SelectButton();
			_offButton.enabled = false;
			_onButton.enabled = true;
			_state = StateType.Off;
		}
	}
}

using System;
using UnityEngine;

public class PointBoostController : MonoBehaviour
{
	[SerializeField]
	private tk2dTextMesh _multiplierDamageText;

	[SerializeField]
	private tk2dTextMesh _multiplierSpecialText;

	[SerializeField]
	private tk2dTextMesh _buttonTextUp;

	[SerializeField]
	private tk2dTextMesh _buttonTextDown;

	private BoostDataModel _boostDM;

	private Action<BoostType, int> _callback;

	public void Init(BoostDataModel boostDM, Action<BoostType, int> callback)
	{
		_boostDM = boostDM;
		_callback = callback;
		if (_multiplierDamageText != null && boostDM != null)
		{
			_multiplierDamageText.text = boostDM.Multiplier1String;
			_multiplierDamageText.Commit();
		}
		if (_multiplierSpecialText != null && boostDM != null)
		{
			_multiplierSpecialText.text = boostDM.Multiplier2String;
			_multiplierSpecialText.Commit();
		}
		int num = 1;
		if (boostDM != null)
		{
			num = boostDM.Price.items[0].amount + 1;
		}
		if ((bool)_buttonTextUp)
		{
			if (num == 1)
			{
				_buttonTextUp.text = string.Format("ui_bigger_point_boost_01_ticket".Localize("{0} Ticket"), num);
			}
			else
			{
				_buttonTextUp.text = string.Format("ui_bigger_point_boost_ticket".Localize("{0} Tickets"), num);
			}
			_buttonTextUp.Commit();
		}
		if ((bool)_buttonTextDown)
		{
			if (num == 1)
			{
				_buttonTextUp.text = string.Format("ui_bigger_point_boost_01_ticket".Localize("{0} Ticket"), num);
			}
			else
			{
				_buttonTextDown.text = string.Format("ui_bigger_point_boost_ticket".Localize("{0} Tickets"), num);
			}
			_buttonTextDown.Commit();
		}
	}

	private void OnPressButton()
	{
		if (_callback != null)
		{
			if (_boostDM != null)
			{
				_callback(_boostDM.Type, _boostDM.Price.items[0].amount + 1);
			}
			else
			{
				_callback(BoostType.NoBoost, 1);
			}
		}
	}
}

using System;
using UnityEngine;

public class UnitInfoController : MonoBehaviour
{
	private UnitInfoView unitInfoView;

	private UserUnit userUnit;

	private Action onUpgradeUnitCallback;

	public UserUnit UserUnitData
	{
		get
		{
			return userUnit;
		}
	}

	private void Awake()
	{
		unitInfoView = GetComponent<UnitInfoView>();
	}

	public void ConfigureUnitInfo(UserUnit userUnit, Action callback = null)
	{
		this.userUnit = userUnit;
		onUpgradeUnitCallback = callback;
		_ConfigureUnitInfo();
	}

	private void _ConfigureUnitInfo()
	{
		if ((bool)unitInfoView)
		{
			unitInfoView.ConfigureUnitView(userUnit.UnitDataModel, userUnit.level, userUnit.partialLevel, userUnit.IsOnCooldown);
		}
	}

	public void SetState(bool state)
	{
		if ((bool)unitInfoView)
		{
			unitInfoView.SetState(state);
		}
	}

	public void OnClick()
	{
		if (!SceneTransitionManager.transitionActive)
		{
			PopupManager.ShowPopup(PopupDataModel.UpgradeUnitPopUp(userUnit, (onUpgradeUnitCallback == null) ? new Action(_ConfigureUnitInfo) : onUpgradeUnitCallback));
		}
	}
}

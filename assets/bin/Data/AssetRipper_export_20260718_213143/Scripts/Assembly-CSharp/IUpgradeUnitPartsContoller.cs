using System;
using System.Collections;
using UnityEngine;

public interface IUpgradeUnitPartsContoller
{
	void SetupScreen(UserUnit localUserUnit, Action upgradeStart, Action closeScreen, Action<bool> scrapButtonUpdate, Action<UnitDataModel, int, int> unitViewChange);

	void ToggleScreen(bool toggle);

	void SetPromoteButtonState(bool toggle);

	bool IsPromoting();

	Transform GetUnitAnchor();

	void OnPartsCommittedComplete(object input);

	IEnumerator AnimateControllerIn();

	IEnumerator AnimateControllerOut();
}

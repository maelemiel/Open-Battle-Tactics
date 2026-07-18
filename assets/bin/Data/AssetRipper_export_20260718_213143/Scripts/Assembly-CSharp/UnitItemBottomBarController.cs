using System.Collections;
using UnityEngine;

public class UnitItemBottomBarController : MonoBehaviour
{
	public UnitProxy[] unitProxies;

	private tk2dSprite tankSprite;

	public void UpdateUnitIcons(UserTeam playerTeam)
	{
		if (playerTeam != null && playerTeam.units.Count > unitProxies.Length)
		{
			Log.Error(playerTeam.units.Count + ">" + unitProxies.Length);
			Log.Error("Tanks and their icons are inconsistent", base.gameObject);
			return;
		}
		for (int i = 0; i < playerTeam.units.Count; i++)
		{
			UserUnit userUnit = playerTeam.units[i];
			if (userUnit == null)
			{
				return;
			}
			StartCoroutine(SetTankIcon(unitProxies[i], userUnit.AssetBundleID));
		}
		for (int j = playerTeam.units.Count; j < unitProxies.Length; j++)
		{
			tankSprite = unitProxies[j].GetComponentInChildren<tk2dSprite>();
			if ((bool)tankSprite)
			{
				Object.Destroy(tankSprite.gameObject);
			}
		}
	}

	private IEnumerator SetTankIcon(UnitProxy unitProxy, int assetBundleID)
	{
		if ((bool)unitProxy)
		{
			yield return StartCoroutine(unitProxy.ChangeAssetCoroutine("Prefab.prefab", assetBundleID));
		}
		tankSprite = unitProxy.GetComponentInChildren<tk2dSprite>();
		if ((bool)tankSprite)
		{
			tankSprite.SortingOrder = 7;
			tankSprite.gameObject.layer = base.gameObject.layer;
		}
	}

	public void ClearAllUnitIcons()
	{
		tk2dSprite tk2dSprite2 = null;
		for (int i = 0; i < unitProxies.Length; i++)
		{
			tk2dSprite2 = unitProxies[i].GetComponentInChildren<tk2dSprite>();
			if ((bool)tk2dSprite2)
			{
				Object.Destroy(tk2dSprite2.gameObject);
			}
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitWeaponSystem : MonoBehaviour
{
	public class UnitWeaponData
	{
		public UnitWeaponType type;

		public int order;

		public float delaySeconds;

		public Vector3 anchorPt;

		public float angle;

		public WeaponAnimationHandler handler;

		public int ArmourPiercingOrder()
		{
			switch (type)
			{
			case UnitWeaponType.CannonLow:
				return 0;
			case UnitWeaponType.CannonRare:
				return 0;
			case UnitWeaponType.CannonSR:
				return 0;
			case UnitWeaponType.MachineGunLow:
				return 1;
			case UnitWeaponType.MachineGunRare:
				return 1;
			case UnitWeaponType.MachineGunSR:
				return 1;
			case UnitWeaponType.Rockets:
				return 3;
			default:
				return 4;
			}
		}
	}

	private UnitView unitView;

	private List<UnitWeaponData> weaponList = new List<UnitWeaponData>();

	private UnitWeaponData[] weaponSlots;

	private List<UnitWeaponData> standardWeaponList = new List<UnitWeaponData>();

	public void Init(UnitView unitView, UnitWeaponType defaultWeaponType, tk2dSpriteDefinition spriteDef)
	{
		this.unitView = unitView;
		AddAttachmentPointWeapons(spriteDef, unitView);
		if (weaponList.Count == 0)
		{
			AddDefaultWeapon(defaultWeaponType);
		}
		weaponList = weaponList.OrderBy((UnitWeaponData x) => x.order).ToList();
		foreach (UnitWeaponData weapon in weaponList)
		{
			weapon.handler = WeaponAnimationHandler.CreateAnimationHandler(weapon, unitView.BattleController);
		}
		standardWeaponList = weaponList.FindAll((UnitWeaponData x) => IsStandardWeapon(x.type)).ToList();
		int[] array = null;
		if (standardWeaponList.Count == 1)
		{
			array = new int[5];
		}
		else if (standardWeaponList.Count == 2)
		{
			array = new int[5] { 0, 0, 0, 1, 1 };
		}
		else if (standardWeaponList.Count == 3)
		{
			array = new int[5] { 0, 0, 1, 1, 2 };
		}
		else if (standardWeaponList.Count == 4)
		{
			array = new int[5] { 0, 1, 1, 2, 3 };
		}
		else if (standardWeaponList.Count >= 5)
		{
			array = new int[5] { 0, 1, 2, 3, 4 };
		}
		weaponSlots = new UnitWeaponData[5];
		for (int num = 0; num < 5; num++)
		{
			weaponSlots[num] = standardWeaponList[array[num]];
		}
	}

	private void AddAttachmentPointWeapons(tk2dSpriteDefinition spriteDef, UnitView unitView)
	{
		tk2dSpriteDefinition.AttachPoint[] attachPoints = spriteDef.attachPoints;
		foreach (tk2dSpriteDefinition.AttachPoint attachPoint in attachPoints)
		{
			string[] array = attachPoint.name.Split(':');
			UnitWeaponType unitWeaponType = ParseWeaponType(array[0], unitView);
			if (unitWeaponType != UnitWeaponType.NONE)
			{
				int result = 0;
				if (array.Length > 1)
				{
					int.TryParse(array[1], out result);
				}
				result *= 10;
				result = (int)(result + unitWeaponType);
				UnitWeaponData unitWeaponData = new UnitWeaponData();
				unitWeaponData.type = unitWeaponType;
				unitWeaponData.order = result;
				unitWeaponData.angle = attachPoint.angle;
				unitWeaponData.anchorPt = attachPoint.position;
				ApplySpecialDieFace(weaponList.Count, spriteDef, unitView, attachPoint, unitWeaponData);
				weaponList.Add(unitWeaponData);
			}
		}
	}

	private void ApplySpecialDieFace(int index, tk2dSpriteDefinition spriteDef, UnitView unitView, tk2dSpriteDefinition.AttachPoint ap, UnitWeaponData data)
	{
		index = Mathf.Clamp(index, 0, unitView.DiceSides.Length);
		if (unitView.DiceSides[index] == DieFaceType.ArmourPiercing)
		{
			data.type = UnitWeaponType.ArmourPiercing;
			tk2dSpriteDefinition.AttachPoint attachPoint = FindAnchorPointForCustom(Constants.APAnchor1, Constants.APAnchor2, spriteDef, ap);
			data.angle = attachPoint.angle;
			data.anchorPt = attachPoint.position;
		}
		else if (unitView.DiceSides[index] == DieFaceType.AcidStrike)
		{
			data.type = UnitWeaponType.AcidStrike;
			tk2dSpriteDefinition.AttachPoint attachPoint2 = FindAnchorPointForCustom(Constants.ASAnchor1, Constants.ASAnchor2, spriteDef, ap);
			data.angle = attachPoint2.angle;
			data.anchorPt = attachPoint2.position;
		}
	}

	private tk2dSpriteDefinition.AttachPoint FindAnchorPointForCustom(string first, string second, tk2dSpriteDefinition def, tk2dSpriteDefinition.AttachPoint original)
	{
		tk2dSpriteDefinition.AttachPoint result = original;
		for (int i = 0; i < def.attachPoints.Length; i++)
		{
			if (def.attachPoints[i].name.StartsWith(first))
			{
				return def.attachPoints[i];
			}
			if (def.attachPoints[i].name.StartsWith(second))
			{
				result = def.attachPoints[i];
			}
		}
		return result;
	}

	private void AddDefaultWeapon(UnitWeaponType type)
	{
		UnitWeaponData unitWeaponData = new UnitWeaponData();
		unitWeaponData.type = type;
		unitWeaponData.order = 0;
		unitWeaponData.angle = 0f;
		unitWeaponData.anchorPt = new Vector2(0f, 0f);
		weaponList.Add(unitWeaponData);
	}

	public IEnumerator FiringAnimation(UnitView unit)
	{
		UnitWeaponData weapon = GetCurrentWeaponData(unitView);
		yield return StartCoroutine(weapon.handler.FiringAnimation(unit));
	}

	public IEnumerator HitAnimation(UnitView source, UnitView target)
	{
		UnitWeaponData weapon = GetCurrentWeaponData(unitView);
		yield return StartCoroutine(weapon.handler.HitAnimation(source, target));
	}

	private UnitWeaponType ParseWeaponType(string weaponName, UnitView unitView)
	{
		switch (weaponName)
		{
		case "machinegun":
			if (unitView.Rarity == 4)
			{
				return UnitWeaponType.MachineGunSR;
			}
			if (unitView.Rarity == 3)
			{
				return UnitWeaponType.MachineGunRare;
			}
			return UnitWeaponType.MachineGunLow;
		case "cannon":
			if (unitView.Rarity == 4)
			{
				return UnitWeaponType.CannonSR;
			}
			if (unitView.Rarity == 3)
			{
				return UnitWeaponType.CannonRare;
			}
			return UnitWeaponType.CannonLow;
		case "missile":
			return UnitWeaponType.Missile;
		case "rocket":
			return UnitWeaponType.Rockets;
		default:
			return UnitWeaponType.NONE;
		}
	}

	public UnitWeaponData GetCurrentWeaponData(UnitView unit)
	{
		if (unit.CurrentRollAction == DieFaceType.ArmourPiercing)
		{
			List<UnitWeaponData> list = standardWeaponList.OrderBy((UnitWeaponData x) => x.ArmourPiercingOrder()).ToList();
			return list[0];
		}
		return weaponSlots[unit.state.currentRoll];
	}

	public UnitWeaponData GetWeaponDataWithType(UnitWeaponType weaponType)
	{
		return weaponList.Find((UnitWeaponData x) => x.type == weaponType);
	}

	public bool IsStandardWeapon(UnitWeaponType weaponType)
	{
		return weaponType != UnitWeaponType.Missile;
	}
}

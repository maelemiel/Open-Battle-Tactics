using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BasicWeaponController : MonoBehaviour
{
	public bool InitReady;

	private List<UnitWeaponSystem.UnitWeaponData> weaponList = new List<UnitWeaponSystem.UnitWeaponData>();

	private UnitWeaponSystem.UnitWeaponData[] weaponSlots;

	private List<UnitWeaponSystem.UnitWeaponData> standardWeaponList = new List<UnitWeaponSystem.UnitWeaponData>();

	private static Dictionary<UnitWeaponType, EffectType> registeredAnim = new Dictionary<UnitWeaponType, EffectType>
	{
		{
			UnitWeaponType.MachineGunLow,
			EffectType.MACHINEGUN_LOW_FIRE
		},
		{
			UnitWeaponType.MachineGunRare,
			EffectType.MACHINEGUN_RARE_FIRE
		},
		{
			UnitWeaponType.MachineGunSR,
			EffectType.MACHINEGUN_SR_FIRE
		},
		{
			UnitWeaponType.CannonLow,
			EffectType.CANNON_LOW_FIRE
		},
		{
			UnitWeaponType.CannonRare,
			EffectType.CANNON_RARE_FIRE
		},
		{
			UnitWeaponType.CannonSR,
			EffectType.CANNON_SR_FIRE
		},
		{
			UnitWeaponType.Missile,
			EffectType.MISSILE_FIRE
		},
		{
			UnitWeaponType.Rockets,
			EffectType.ROCKET_FIRE
		}
	};

	public void Init(int rarity, tk2dSpriteDefinition spriteDef, UnitWeaponType defaultWeaponType = UnitWeaponType.CannonLow)
	{
		AddAttachmentPointWeapons(spriteDef, rarity);
		if (weaponList.Count == 0)
		{
			AddDefaultWeapon(defaultWeaponType);
		}
		weaponList = weaponList.OrderBy((UnitWeaponSystem.UnitWeaponData x) => x.order).ToList();
		standardWeaponList = weaponList.FindAll((UnitWeaponSystem.UnitWeaponData x) => IsStandardWeapon(x.type)).ToList();
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
		weaponSlots = new UnitWeaponSystem.UnitWeaponData[5];
		for (int num = 0; num < 5; num++)
		{
			weaponSlots[num] = standardWeaponList[array[num]];
		}
		InitReady = true;
	}

	private void AddAttachmentPointWeapons(tk2dSpriteDefinition spriteDef, int rarity)
	{
		tk2dSpriteDefinition.AttachPoint[] attachPoints = spriteDef.attachPoints;
		foreach (tk2dSpriteDefinition.AttachPoint attachPoint in attachPoints)
		{
			string[] array = attachPoint.name.Split(':');
			UnitWeaponType unitWeaponType = ParseWeaponType(array[0], rarity);
			if (unitWeaponType != UnitWeaponType.NONE)
			{
				int result = 0;
				if (array.Length > 1)
				{
					int.TryParse(array[1], out result);
				}
				result *= 10;
				result = (int)(result + unitWeaponType);
				UnitWeaponSystem.UnitWeaponData unitWeaponData = new UnitWeaponSystem.UnitWeaponData();
				unitWeaponData.type = unitWeaponType;
				unitWeaponData.order = result;
				unitWeaponData.angle = attachPoint.angle;
				unitWeaponData.anchorPt = attachPoint.position;
				weaponList.Add(unitWeaponData);
			}
		}
	}

	private void AddDefaultWeapon(UnitWeaponType type)
	{
		UnitWeaponSystem.UnitWeaponData unitWeaponData = new UnitWeaponSystem.UnitWeaponData();
		unitWeaponData.type = type;
		unitWeaponData.order = 0;
		unitWeaponData.angle = 0f;
		unitWeaponData.anchorPt = new Vector2(0f, 0f);
		weaponList.Add(unitWeaponData);
	}

	public IEnumerator FiringAnimation(int currentRoll, Transform tankSpritesTransform, bool isRotated = false)
	{
		UnitWeaponSystem.UnitWeaponData weapon = GetCurrentWeaponData(currentRoll);
		EffectInstance fireEffect = null;
		if (weapon.type == UnitWeaponType.Rockets || weapon.type == UnitWeaponType.Missile)
		{
			yield return StartCoroutine(GlobalEffectsManager.CreateCoroutine(registeredAnim[weapon.type], tankSpritesTransform.position, tankSpritesTransform.gameObject, delegate(EffectInstance effectInstance)
			{
				fireEffect = effectInstance;
			}));
		}
		else
		{
			fireEffect = GlobalEffectsManager.Create(registeredAnim[weapon.type], tankSpritesTransform.position, tankSpritesTransform);
		}
		fireEffect.AutoDestroy();
		fireEffect.transform.Translate(0f, 0f, -2f);
		fireEffect.transform.Rotate(Vector3.forward, weapon.angle);
		Vector3 anchorOffset = weapon.anchorPt;
		if (isRotated)
		{
			anchorOffset = Vector3.Scale(anchorOffset, new Vector3(-1f, 1f, 1f));
		}
		fireEffect.transform.Translate(anchorOffset);
		fireEffect.SpineAnimation.AnimationName = fireEffect.SpineAnimation.GetAnimationNames()[0];
		yield return new WaitForSeconds(0.1f);
	}

	private UnitWeaponType ParseWeaponType(string weaponName, int rarity)
	{
		switch (weaponName)
		{
		case "machinegun":
			switch (rarity)
			{
			case 4:
				return UnitWeaponType.MachineGunSR;
			case 3:
				return UnitWeaponType.MachineGunRare;
			default:
				return UnitWeaponType.MachineGunLow;
			}
		case "cannon":
			switch (rarity)
			{
			case 4:
				return UnitWeaponType.CannonSR;
			case 3:
				return UnitWeaponType.CannonRare;
			default:
				return UnitWeaponType.CannonLow;
			}
		case "missile":
			return UnitWeaponType.Missile;
		case "rocket":
			return UnitWeaponType.Rockets;
		default:
			return UnitWeaponType.NONE;
		}
	}

	private UnitWeaponSystem.UnitWeaponData GetCurrentWeaponData(int currentRoll)
	{
		return weaponSlots[currentRoll];
	}

	private UnitWeaponSystem.UnitWeaponData GetWeaponDataWithType(UnitWeaponType weaponType)
	{
		return weaponList.Find((UnitWeaponSystem.UnitWeaponData x) => x.type == weaponType);
	}

	public bool IsStandardWeapon(UnitWeaponType weaponType)
	{
		return weaponType != UnitWeaponType.Missile;
	}
}

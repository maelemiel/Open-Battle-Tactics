using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBuffEffects : MonoBehaviour
{
	private const string ANIMATION_NAME_DAMAGE_BUFFED_DIEFACE = "Buffed Red";

	private const string ANIMATION_NAME_FIRST_STRIKE_BUFFED_DIEFACE = "Buffed Blue";

	private const string ANIMATION_NAME_SPECIAL_BUFFED_DIEFACE = "Buffed White";

	private const string ANIMATION_NAME_DAMAGE_BUFFING_DIEFACE = "Buffing Red";

	private const string ANIMATION_NAME_FIRST_STRIKE_BUFFING_DIEFACE = "Buffing Blue";

	private const string ANIMATION_NAME_SPECIAL_BUFFING_DIEFACE = "Buffing White";

	[SerializeField]
	private UnitView unit;

	private Dictionary<DieFaceType, int> localBuffValues;

	private Dictionary<DieFaceType, bool> localBuffStates;

	private Dictionary<DieFaceType, EffectInstance> localBuffEffects;

	private Dictionary<DieFaceType, List<BuffType>> localBuffTypes;

	private Vector3 effectOffset = Vector3.forward;

	private void Awake()
	{
		localBuffValues = new Dictionary<DieFaceType, int>
		{
			{
				DieFaceType.DirectDamage,
				0
			},
			{
				DieFaceType.Initiative,
				0
			},
			{
				DieFaceType.Special,
				0
			},
			{
				DieFaceType.None,
				0
			},
			{
				DieFaceType.ArmourPiercing,
				0
			},
			{
				DieFaceType.AcidStrike,
				0
			}
		};
		localBuffStates = new Dictionary<DieFaceType, bool>
		{
			{
				DieFaceType.DirectDamage,
				false
			},
			{
				DieFaceType.Initiative,
				false
			},
			{
				DieFaceType.Special,
				false
			},
			{
				DieFaceType.None,
				false
			},
			{
				DieFaceType.ArmourPiercing,
				false
			},
			{
				DieFaceType.AcidStrike,
				false
			}
		};
		localBuffEffects = new Dictionary<DieFaceType, EffectInstance>
		{
			{
				DieFaceType.DirectDamage,
				null
			},
			{
				DieFaceType.Initiative,
				null
			},
			{
				DieFaceType.Special,
				null
			},
			{
				DieFaceType.None,
				null
			},
			{
				DieFaceType.ArmourPiercing,
				null
			},
			{
				DieFaceType.AcidStrike,
				null
			}
		};
		localBuffTypes = new Dictionary<DieFaceType, List<BuffType>>
		{
			{
				DieFaceType.DirectDamage,
				new List<BuffType>()
			},
			{
				DieFaceType.Initiative,
				new List<BuffType>()
			},
			{
				DieFaceType.Special,
				new List<BuffType>()
			},
			{
				DieFaceType.None,
				new List<BuffType>()
			},
			{
				DieFaceType.ArmourPiercing,
				new List<BuffType>()
			},
			{
				DieFaceType.AcidStrike,
				new List<BuffType>()
			}
		};
	}

	public void UpdateBuffEffects()
	{
		DieFaceSimple activeFace = unit.PossibleRollsSimple.ActiveFace;
		if (localBuffValues[activeFace.RollAction] > 0 && !localBuffStates[unit.CurrentRollAction] && unit.RolledDice)
		{
			CreateBuffEffectForFaceType(unit.CurrentRollAction);
		}
		UpdatePlusNumberOnDieFace();
	}

	public void UpdatePlusNumberOnDieFace()
	{
		DieFaceSimple activeFace = unit.PossibleRollsSimple.ActiveFace;
		if ((bool)activeFace)
		{
			activeFace.UpdatePlusText(localBuffValues[activeFace.RollAction]);
		}
	}

	private void CreateBuffEffectForFaceType(DieFaceType dieFaceType)
	{
		localBuffStates[dieFaceType] = true;
		CreateDiceBuffEffectOnActiveFace();
	}

	private void RemoveBuffEffectForFaceType(DieFaceType dieFaceType)
	{
		localBuffStates[dieFaceType] = false;
		if (localBuffEffects[dieFaceType] != null)
		{
			localBuffEffects[dieFaceType].Destroy();
		}
	}

	public int GetBuffValue(DieFaceType dieFaceType)
	{
		return localBuffValues[dieFaceType];
	}

	public void PushBuffValue(DieFaceType dieFaceType, int buffValue, BuffType buffType)
	{
		List<BuffType> list = localBuffTypes[dieFaceType];
		Dictionary<DieFaceType, int> dictionary2;
		Dictionary<DieFaceType, int> dictionary = (dictionary2 = localBuffValues);
		DieFaceType key2;
		DieFaceType key = (key2 = dieFaceType);
		int num = dictionary2[key2];
		dictionary[key] = num + buffValue;
		if (!list.Contains(buffType))
		{
			list.Add(buffType);
		}
		UpdateBuffEffects();
	}

	public void PopBuffValue(DieFaceType dieFaceType, int buffValue, BuffType buffType)
	{
		localBuffValues[dieFaceType] = Mathf.Max(0, localBuffValues[dieFaceType] - buffValue);
		List<BuffType> list = localBuffTypes[dieFaceType];
		if (!unit.HasAttacked)
		{
			UpdatePlusNumberOnDieFace();
		}
		if (localBuffValues[dieFaceType] <= 0)
		{
			RemoveBuffEffectForFaceType(dieFaceType);
			StopAllCoroutines();
		}
		if (list.Contains(buffType))
		{
			list.Remove(buffType);
		}
	}

	public void ResetBuffEffects()
	{
		List<DieFaceType> list = new List<DieFaceType>(localBuffStates.Keys);
		foreach (DieFaceType item in list)
		{
			localBuffStates[item] = false;
		}
		foreach (EffectInstance value in localBuffEffects.Values)
		{
			if (value != null)
			{
				value.Destroy();
			}
		}
		StopAllCoroutines();
	}

	public void CreateDiceBuffEffectOnActiveFace()
	{
		string animationName = string.Empty;
		string animationName2 = string.Empty;
		UnitPossibleRollsSimple possibleRollsSimple = unit.PossibleRollsSimple;
		switch (possibleRollsSimple.ActiveFace.RollAction)
		{
		case DieFaceType.DirectDamage:
			animationName = "Buffed Red";
			animationName2 = "Buffing Red";
			break;
		case DieFaceType.Initiative:
			animationName = "Buffed Blue";
			animationName2 = "Buffing Blue";
			break;
		case DieFaceType.ArmourPiercing:
			animationName = "Buffed Red";
			animationName2 = "Buffing Red";
			break;
		case DieFaceType.AcidStrike:
			animationName = "Buffed Red";
			animationName2 = "Buffing Red";
			break;
		case DieFaceType.Special:
			animationName = "Buffed White";
			animationName2 = "Buffing White";
			break;
		}
		List<BuffType> list = localBuffTypes[possibleRollsSimple.ActiveFace.RollAction];
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i] == BuffType.BossDamage)
			{
				animationName = "Buffed White";
				animationName2 = "Buffing White";
			}
		}
		Vector3 worldPosition = unit.PossibleRollsSimple.ActiveFace.transform.position + effectOffset;
		EffectInstance effectInstance = GlobalEffectsManager.Create(EffectType.DIEFACE_BUFFING_EFFECT, worldPosition, possibleRollsSimple.ActiveFace.gameObject);
		effectInstance.SpineAnimation.AnimationName = animationName2;
		localBuffEffects[possibleRollsSimple.ActiveFace.RollAction] = effectInstance;
		worldPosition = possibleRollsSimple.ActiveFace.transform.position + effectOffset;
		EffectInstance effectInstance2 = GlobalEffectsManager.Create(EffectType.DIEFACE_BUFFED_EFFECT, worldPosition, possibleRollsSimple.ActiveFace.gameObject);
		effectInstance2.AutoDestroy();
		tk2dSpineAnimation component = effectInstance2.GetComponent<tk2dSpineAnimation>();
		if ((bool)component)
		{
			component.AnimationName = animationName;
			StartCoroutine(PlayBuffingEffectWithDelay(effectInstance, component));
		}
	}

	public void RemoveBuffEffectOnActiveFace()
	{
		RemoveBuffEffect(unit.CurrentRollAction);
	}

	public void RemoveBuffEffect(DieFaceType dieFaceType)
	{
		EffectInstance effectInstance = localBuffEffects[dieFaceType];
		if (effectInstance != null)
		{
			effectInstance.Destroy();
		}
		StopAllCoroutines();
	}

	public void RemoveAllBuffEffects()
	{
		foreach (KeyValuePair<DieFaceType, EffectInstance> localBuffEffect in localBuffEffects)
		{
			if (localBuffEffect.Value != null)
			{
				localBuffEffect.Value.Destroy();
			}
		}
	}

	private IEnumerator PlayBuffingEffectWithDelay(EffectInstance buffingEffect, tk2dSpineAnimation previousAnimation)
	{
		while (!previousAnimation.IsComplete)
		{
			yield return 0;
		}
		buffingEffect.gameObject.SetActive(true);
	}
}

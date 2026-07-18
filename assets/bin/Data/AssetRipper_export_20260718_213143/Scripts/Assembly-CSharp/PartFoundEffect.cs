using System.Collections;
using System.Collections.Generic;
using Holoville.HOTween;
using Holoville.HOTween.Core;
using UnityEngine;

public class PartFoundEffect : MonoBehaviour
{
	[SerializeField]
	private tk2dTextMesh partFoundText;

	[SerializeField]
	private float delayBetweenParts = 0.1f;

	private List<UnitPartTypesDataModel> partList;

	private UnitView unit;

	private List<Sequence> partSequence;

	public int rowWidth = 4;

	public int SortingOrder = 10;

	public bool animating;

	public void PlayAnimation(List<UnitPartTypesDataModel> partList, UnitView unit)
	{
		this.partList = partList;
		partSequence = new List<Sequence>(partList.Count);
		this.unit = unit;
		partFoundText.SortingOrder = SortingOrder + 1;
		if (partList.Count > 1)
		{
			partFoundText.text = LocalizationManager.GetString("game_battle_partsfound", "Parts Found");
		}
		else
		{
			partFoundText.text = LocalizationManager.GetString("game_battle_partfound", "Part Found");
		}
		StartCoroutine(AnimationSequence());
	}

	private void Awake()
	{
		partFoundText.Alpha = 0f;
	}

	private IEnumerator AnimationSequence()
	{
		animating = true;
		yield return 0;
		BattleField battlefield = null;
		if (unit != null)
		{
			battlefield = unit.BattleField;
		}
		Vector3 offscreenPos = ((!(battlefield != null)) ? new Vector3(0f, 500f, 0f) : battlefield.unityCamera.ViewportToWorldPoint(new Vector3(0.5f, 1.1f, 0f)));
		offscreenPos.z = base.transform.position.z;
		List<Vector3> offsetPositions = GenerateOffsetPositions();
		int i = 0;
		int effectsDestroyed = 0;
		foreach (UnitPartTypesDataModel part in partList)
		{
			if (part == null || part.AssetLinkage == null)
			{
				continue;
			}
			GameObject partObj = PrefabProxy.CreateFromAssetLinkage(part.AssetLinkage).gameObject;
			partObj.transform.SetParent(base.transform);
			partObj.transform.position = base.transform.position;
			partObj.SetSortingOrder(SortingOrder);
			PrefabProxy partPrefabProxy = partObj.GetComponent<PrefabProxy>();
			if ((bool)partPrefabProxy)
			{
				partPrefabProxy.SortingOrder = SortingOrder;
			}
			Transform effectsGameObject = partObj.transform.GetChildRecursively("Effect");
			if ((bool)effectsGameObject)
			{
				effectsGameObject.gameObject.SetActive(true);
			}
			Vector3 offsetPos = partObj.transform.position + offsetPositions[i];
			Sequence s = new Sequence();
			partSequence.Add(s);
			s.AppendInterval((float)i * delayBetweenParts);
			EffectType type = GetDropEffectById(part.id);
			if (type != EffectType.NONE)
			{
				s.AppendCallback(CreateDropEffectCallback(partObj.transform.position, partObj.transform, type));
			}
			s.Append(HOTween.To(partObj.transform, 0.5f, new TweenParms().Prop("position", offsetPos).Ease(EaseType.EaseOutSine)));
			s.AppendCallback(CreateShineCallback(offsetPos));
			s.AppendInterval(1.5f);
			s.Append(HOTween.To(partObj.transform, 0.5f, new TweenParms().Prop("position", offscreenPos).Ease(EaseType.EaseInBack)));
			s.AppendCallback((TweenDelegate.TweenCallback)delegate
			{
				if (type == EffectType.DROP_SUPER_RARE)
				{
					AudioTrigger.SuperRarePartEarned.Play();
					AudioTrigger.SuperRarePartEarned.Play();
					AudioTrigger.SuperRarePartEarned.Play();
				}
				else
				{
					AudioTrigger.PartEarned.Play();
				}
			});
			s.AppendCallback((TweenDelegate.TweenCallback)delegate
			{
				Object.Destroy(partObj);
				effectsDestroyed++;
				if (effectsDestroyed >= partList.Count)
				{
					animating = false;
					if (base.gameObject != null)
					{
						GlobalEffectsManager.Return(base.gameObject);
					}
				}
			});
			s.Play();
			i++;
		}
		SimpleTween.Start(0f, 1f, 0.5f, EaseType.EaseOutExpo, delegate(float val)
		{
			partFoundText.Alpha = val;
		});
		Sequence textSequence = new Sequence();
		textSequence.Append(HOTween.To(partFoundText.transform, 0.5f, new TweenParms().Prop("position", base.transform.position + new Vector3(0f, -30f, 0f)).Ease(EaseType.EaseOutExpo)));
		textSequence.AppendInterval(1f + (float)(partList.Count - 1) * delayBetweenParts - 0.25f);
		textSequence.AppendCallback((TweenDelegate.TweenCallback)delegate
		{
			SimpleTween.Start(1f, 0f, 0.25f, EaseType.EaseOutExpo, delegate(float val)
			{
				partFoundText.Alpha = val;
			});
		});
		textSequence.Play();
	}

	private TweenDelegate.TweenCallback CreateShineCallback(Vector3 pos)
	{
		return delegate
		{
			EffectInstance effectInstance = GlobalEffectsManager.Create(EffectType.SHINE, pos, base.transform).AutoDestroy();
			effectInstance.gameObject.SetSortingOrder(SortingOrder + 1);
			effectInstance.gameObject.SetActive(true);
		};
	}

	private TweenDelegate.TweenCallback CreateDropEffectCallback(Vector3 pos, Transform parent, EffectType dropType = EffectType.DROP_RARE)
	{
		return delegate
		{
			EffectInstance effectInstance = GlobalEffectsManager.Create(dropType, pos + new Vector3(0f, 0f, 1f), parent).AutoDestroy();
			effectInstance.gameObject.SetSortingOrder(SortingOrder - 1);
			effectInstance.gameObject.SetActive(true);
		};
	}

	private List<Vector3> GenerateOffsetPositions()
	{
		List<Vector3> list = new List<Vector3>();
		Vector3 vector = new Vector3(0f, 10f, 0f);
		float num = 50f;
		int num2 = Mathf.FloorToInt(partList.Count / rowWidth);
		for (int i = 0; i < partList.Count; i++)
		{
			int num3 = Mathf.FloorToInt(i / rowWidth);
			Vector3 item = vector;
			item.x += ((float)(i % rowWidth) - ((float)rowWidth - 1f) / 2f) * num;
			item.y += (float)num3 * num;
			if (num3 == num2)
			{
				item.x += (float)(rowWidth - partList.Count % rowWidth) * num * 0.5f;
			}
			list.Add(item);
		}
		return list;
	}

	private EffectType GetDropEffectById(string id)
	{
		switch (id)
		{
		case "1":
		case "2":
			return EffectType.NONE;
		case "11":
		case "12":
		case "13":
			return EffectType.DROP_RARE;
		default:
			return EffectType.DROP_SUPER_RARE;
		}
	}

	private void OnDestroy()
	{
		if (partSequence != null)
		{
			for (int i = 0; i < partSequence.Count; i++)
			{
				partSequence[i].Kill();
			}
		}
	}
}

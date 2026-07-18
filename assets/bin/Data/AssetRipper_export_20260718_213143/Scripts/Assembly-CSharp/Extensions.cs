using System;
using System.Collections.Generic;
using System.Linq;
using Holoville.HOTween;
using UnityEngine;
using tk2dRuntime;

public static class Extensions
{
	public static string ToNiceString(this string[] stringArray)
	{
		return "[" + string.Join(",", stringArray) + "]";
	}

	public static string ToNiceString(this Array objectArray)
	{
		string[] array = new string[objectArray.Length];
		for (int i = 0; i < objectArray.Length; i++)
		{
			object value = objectArray.GetValue(i);
			array[i] = ((value == null) ? "null" : value.ToString());
		}
		return array.ToNiceString();
	}

	public static void MakeChild(this Transform transform, Transform child, bool inheritLayer = true)
	{
		Vector3 localPosition = child.localPosition;
		Vector3 localScale = child.localScale;
		Quaternion localRotation = child.localRotation;
		child.transform.parent = transform;
		child.localScale = localScale;
		child.localPosition = localPosition;
		child.localRotation = localRotation;
		if (inheritLayer)
		{
			child.gameObject.SetLayerRecursively(transform.gameObject.layer);
		}
	}

	public static void SetParent(this Transform transform, Transform parent, bool inheritLayer = true)
	{
		parent.MakeChild(transform);
	}

	public static void AddLocalPosition(this Transform transform, Vector3 delta)
	{
		Vector3 localPosition = transform.localPosition;
		localPosition += delta;
		transform.localPosition = localPosition;
	}

	public static void MultiplyScale(this Transform transform, float x, float y, float z)
	{
		Vector3 localScale = transform.localScale;
		localScale.x *= x;
		localScale.y *= y;
		localScale.z *= z;
		transform.localScale = localScale;
	}

	public static void SetChildrenActiveRecursively(this GameObject gameObject, bool active)
	{
		gameObject.SetActive(active);
		foreach (Transform item in gameObject.transform)
		{
			item.gameObject.SetChildrenActiveRecursively(active);
		}
	}

	public static Transform GetChildRecursively(this Transform transform, string name)
	{
		Transform[] componentsInChildren = transform.GetComponentsInChildren<Transform>(true);
		foreach (Transform transform2 in componentsInChildren)
		{
			if (transform2.name.Equals(name))
			{
				return transform2;
			}
		}
		return null;
	}

	public static void SetSortingOrder(this GameObject gameObject, int order)
	{
		Renderer[] componentsInChildren = gameObject.GetComponentsInChildren<Renderer>();
		foreach (Renderer renderer in componentsInChildren)
		{
			renderer.sortingOrder = order;
		}
	}

	public static void SetColor(this GameObject gameObject, Color color)
	{
		tk2dBaseSprite[] componentsInChildren = gameObject.GetComponentsInChildren<tk2dBaseSprite>();
		foreach (tk2dBaseSprite tk2dBaseSprite2 in componentsInChildren)
		{
			tk2dBaseSprite2.color = color;
		}
	}

	public static void AddSortingOrder(this GameObject gameObject, int order)
	{
		Renderer[] componentsInChildren = gameObject.GetComponentsInChildren<Renderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].sortingOrder += order;
		}
	}

	public static void SubtractSortingOrder(this GameObject gameObject, int order)
	{
		Renderer[] componentsInChildren = gameObject.GetComponentsInChildren<Renderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].sortingOrder -= order;
		}
	}

	public static void FitWithinBounds(this GameObject gameObject, Bounds bounds)
	{
		gameObject.transform.position = Vector3.zero;
		Bounds bounds2 = default(Bounds);
		MeshRenderer[] componentsInChildren = gameObject.GetComponentsInChildren<MeshRenderer>();
		foreach (MeshRenderer meshRenderer in componentsInChildren)
		{
			bounds2.Encapsulate(meshRenderer.bounds);
		}
		float num = Mathf.Min(bounds.size.x / bounds2.size.x, bounds.size.y / bounds2.size.y);
		Vector3 vector = bounds2.center - bounds.center;
		gameObject.transform.Translate(-vector);
		gameObject.transform.MultiplyScale(num, num, num);
	}

	public static bool Find(this string[] array, string str)
	{
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] == str)
			{
				return true;
			}
		}
		return false;
	}

	public static bool OnScreen(this Camera camera)
	{
		if (camera.rect.x + camera.rect.width <= 0f)
		{
			return false;
		}
		if (camera.rect.y + camera.rect.height <= 0f)
		{
			return false;
		}
		if (camera.rect.x >= 1f)
		{
			return false;
		}
		if (camera.rect.y >= 1f)
		{
			return false;
		}
		if ((double)camera.rect.width <= 0.01)
		{
			return false;
		}
		if ((double)camera.rect.height <= 0.01)
		{
			return false;
		}
		return true;
	}

	public static string UppercaseFirst(this string s)
	{
		if (string.IsNullOrEmpty(s))
		{
			return s;
		}
		char[] array = s.ToCharArray();
		array[0] = char.ToUpper(array[0]);
		return new string(array);
	}

	public static void SetLayerRecursively(this GameObject obj, int newLayer)
	{
		if (obj == null)
		{
			return;
		}
		obj.layer = newLayer;
		foreach (Transform item in obj.transform)
		{
			if (!(item == null))
			{
				item.gameObject.SetLayerRecursively(newLayer);
			}
		}
	}

	public static void ShuffleList<T>(this IList<T> list)
	{
		int num = list.Count;
		while (num > 1)
		{
			num--;
			int index = UnityEngine.Random.Range(0, list.Count);
			T value = list[index];
			list[index] = list[num];
			list[num] = value;
		}
	}

	public static tk2dSpriteDefinition.AttachPoint GetAttachPointByName(this tk2dBaseSprite sprite, string attachPointName)
	{
		if (sprite.CurrentSprite != null)
		{
			return sprite.CurrentSprite.GetAttachPointByName(attachPointName);
		}
		return null;
	}

	public static tk2dSpriteDefinition.AttachPoint GetAttachPointByName(this tk2dSpriteDefinition spriteDef, string attachPointName)
	{
		tk2dSpriteDefinition.AttachPoint[] attachPoints = spriteDef.attachPoints;
		foreach (tk2dSpriteDefinition.AttachPoint attachPoint in attachPoints)
		{
			if (attachPoint.name == attachPointName)
			{
				return attachPoint;
			}
		}
		return null;
	}

	public static bool CompareIfEqualTo<T>(this IList<T> listOne, IList<T> listTwo)
	{
		bool result = false;
		if (listOne == null || listTwo == null)
		{
			return result;
		}
		result = listOne.Count == listTwo.Count;
		return result & !listOne.Except(listTwo).Any();
	}

	public static Vector2 ToVector2(this Vector3 vec3)
	{
		return new Vector2(vec3.x, vec3.y);
	}

	public static Vector3 UnclampedLerp(this Vector3 from, Vector3 to, float t)
	{
		return from + (to - from) * t;
	}

	public static string Localize(this string str)
	{
		return LocalizationManager.GetString(str, str);
	}

	public static string Localize(this string str, string defaultString)
	{
		return LocalizationManager.GetString(str, defaultString);
	}

	public static void SetLocalXPosition(this Transform transform, float xPosition)
	{
		Vector3 localPosition = transform.localPosition;
		localPosition.x = xPosition;
		transform.localPosition = localPosition;
	}

	public static void SetLocalYPosition(this Transform transform, float yPosition)
	{
		Vector3 localPosition = transform.localPosition;
		localPosition.y = yPosition;
		transform.localPosition = localPosition;
	}

	public static void SetLocalZPosition(this Transform transform, float zPosition)
	{
		Vector3 localPosition = transform.localPosition;
		localPosition.z = zPosition;
		transform.localPosition = localPosition;
	}

	public static void SetXPosition(this Transform transform, float xPosition)
	{
		Vector3 position = transform.position;
		position.x = xPosition;
		transform.position = position;
	}

	public static void SetYPosition(this Transform transform, float yPosition)
	{
		Vector3 position = transform.position;
		position.y = yPosition;
		transform.position = position;
	}

	public static void SetZPosition(this Transform transform, float zPosition)
	{
		Vector3 position = transform.position;
		position.z = zPosition;
		transform.position = position;
	}

	public static void SetLocalXScale(this Transform transform, float xScale)
	{
		Vector3 localScale = transform.localScale;
		localScale.x = xScale;
		transform.localScale = localScale;
	}

	public static void SetLocalYScale(this Transform transform, float yScale)
	{
		Vector3 localScale = transform.localScale;
		localScale.y = yScale;
		transform.localScale = localScale;
	}

	public static void SetLocalZScale(this Transform transform, float zScale)
	{
		Vector3 localScale = transform.localScale;
		localScale.z = zScale;
		transform.localScale = localScale;
	}

	public static Tweener TweenLocalScale(this Transform transform, float scaleValue, float duration, EaseType easeType = EaseType.EaseOutExpo)
	{
		return transform.TweenLocalScaleVec(new Vector3(scaleValue, scaleValue, scaleValue), duration, easeType);
	}

	public static Tweener TweenLocalScaleVec(this Transform transform, Vector3 scaleValue, float duration, EaseType easeType = EaseType.EaseOutExpo)
	{
		return _Tween(transform, duration, "localScale", scaleValue, easeType);
	}

	public static Tweener TweenLocalPosition(this Transform transform, Vector3 newLocalPosition, float duration, EaseType easeType = EaseType.EaseOutExpo, Action onComplete = null)
	{
		return _Tween(transform, duration, "localPosition", newLocalPosition, easeType, onComplete);
	}

	public static Tweener TweenLocalXPosition(this Transform transform, float newLocalXPosition, float duration, EaseType easeType = EaseType.EaseOutExpo, Action onComplete = null)
	{
		return _Tween(transform, duration, "localPosition", new Vector3(newLocalXPosition, transform.localPosition.y, transform.localPosition.z), easeType, onComplete);
	}

	public static Tweener TweenLocalYPosition(this Transform transform, float newLocalYPosition, float duration, EaseType easeType = EaseType.EaseOutExpo, Action onComplete = null)
	{
		return _Tween(transform, duration, "localPosition", new Vector3(transform.localPosition.x, newLocalYPosition, transform.localPosition.z), easeType, onComplete);
	}

	public static Tweener TweenLocalZPosition(this Transform transform, float newLocalZPosition, float duration, EaseType easeType = EaseType.EaseOutExpo, Action onComplete = null)
	{
		return _Tween(transform, duration, "localPosition", new Vector3(transform.localPosition.x, transform.localPosition.y, newLocalZPosition), easeType, onComplete);
	}

	public static Tweener TweenLocalXScale(this Transform transform, float newLocalXScale, float duration, EaseType easeType = EaseType.EaseOutExpo, Action onComplete = null)
	{
		return _Tween(transform, duration, "localScale", new Vector3(newLocalXScale, transform.localScale.y, transform.localScale.z), easeType, onComplete);
	}

	public static Tweener TweenLocalYScale(this Transform transform, float newLocalYScale, float duration, EaseType easeType = EaseType.EaseOutExpo, Action onComplete = null)
	{
		return _Tween(transform, duration, "localScale", new Vector3(transform.localScale.x, newLocalYScale, transform.localScale.z), easeType, onComplete);
	}

	public static Tweener TweenLocalZScale(this Transform transform, float newLocalZScale, float duration, EaseType easeType = EaseType.EaseOutExpo, Action onComplete = null)
	{
		return _Tween(transform, duration, "localScale", new Vector3(transform.localScale.x, transform.localScale.y, newLocalZScale), easeType, onComplete);
	}

	public static Tweener TweenPosition(this Transform transform, Vector3 newPosition, float duration, EaseType easeType = EaseType.EaseOutExpo, Action onComplete = null)
	{
		return _Tween(transform, duration, "position", newPosition, easeType, onComplete);
	}

	public static Tweener TweenAlpha(this IRenderable renderable, float newAlpha, float duration, EaseType easeType = EaseType.EaseOutExpo)
	{
		return _Tween(renderable, duration, "Alpha", newAlpha, easeType);
	}

	public static Tweener TweenColor(this IRenderable renderable, Color newColor, float duration, EaseType easeType = EaseType.EaseOutExpo)
	{
		return _Tween(renderable, duration, "color", newColor, easeType);
	}

	public static Tweener TweenScale(this IRenderable renderable, Vector3 newScale, float duration, EaseType easeType = EaseType.EaseOutExpo)
	{
		return _Tween(renderable, duration, "scale", newScale, easeType);
	}

	public static Tweener TweenScale(this IRenderable renderable, float newScale, float duration, EaseType easeType = EaseType.EaseOutExpo)
	{
		return _Tween(renderable, duration, "scale", new Vector3(newScale, newScale, newScale), easeType);
	}

	public static Tweener TweenValue(this tk2dUIScrollableArea scrollableArea, float newValue, float duration, EaseType easeType = EaseType.EaseOutExpo)
	{
		return _Tween(scrollableArea, duration, "Value", newValue, easeType);
	}

	public static Tweener TweenAlpha(this tk2dSpineAnimation animation, float newAlpha, float duration, EaseType easeType = EaseType.EaseOutExpo, Action onComplete = null)
	{
		return _Tween(animation.Skeleton.skeleton, duration, "A", newAlpha, easeType, onComplete);
	}

	private static Tweener _Tween(object target, float duration, string propName, object propValue, EaseType easeType, Action onComplete = null)
	{
		TweenParms tweenParms = new TweenParms();
		tweenParms.NewProp(propName, propValue);
		tweenParms.Ease(easeType);
		if (onComplete != null)
		{
			tweenParms.OnComplete(delegate
			{
				onComplete();
			}, new object[0]);
		}
		return HOTween.To(target, duration, tweenParms);
	}

	public static void EnableButton(this tk2dUIUpDownButton button, bool enable)
	{
		button.enabled = enable;
		button.upStateGO.GetComponent<tk2dBaseSprite>().Alpha = ((!enable) ? 0.5f : 1f);
		button.GetComponent<tk2dUIItem>().enabled = enable;
	}

	public static string InlineStyleCode(this Color color)
	{
		Color32 color2 = color;
		string text = string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", color2.r, color2.g, color2.b, color2.a);
		return "^C" + text;
	}

	public static string GetEmoticonLinePrefix(this EmoticonTypes type)
	{
		switch (type)
		{
		case EmoticonTypes.HAPPY:
			return "ui_battle_taunts_happy_";
		case EmoticonTypes.SAD:
			return "ui_battle_taunts_sad_";
		case EmoticonTypes.ANGRY:
			return "ui_battle_taunts_angry_";
		default:
			return string.Empty;
		}
	}

	public static EffectType GetEffectType(this InBattleMessageType battleMessageType)
	{
		switch (battleMessageType)
		{
		case InBattleMessageType.FIRST_STRIKE:
			return EffectType.IN_BATTLE_FIRST_STRIKE;
		case InBattleMessageType.COUNTER_ATTACK:
			return EffectType.IN_BATTLE_COUNTER_ATTACK;
		case InBattleMessageType.BOSS_MOMENT:
			return EffectType.IN_BATTLE_BOSS;
		default:
			return EffectType.NONE;
		}
	}

	public static string GetLocalizedName(this ClubTypes teamType)
	{
		switch (teamType)
		{
		case ClubTypes.PUBLIC:
			return "ui_clubs_create_club_public".Localize("PUBLIC");
		case ClubTypes.PRIVATE:
			return "ui_clubs_create_club_private".Localize("PRIVATE");
		default:
			return "NONE";
		}
	}

	public static string GetAnimationName(this ExplosionTypes explosionType)
	{
		switch (explosionType)
		{
		case ExplosionTypes.BIG_A:
			return "Large A";
		case ExplosionTypes.BIG_B:
			return "Large B";
		case ExplosionTypes.SMALL_A:
			return "Small A";
		case ExplosionTypes.SMALL_B:
			return "Small B";
		default:
			return string.Empty;
		}
	}

	public static string GetUnitIconName(this UnitType unitType)
	{
		switch (unitType)
		{
		case UnitType.ASSAULT:
			return "Icon_Assault";
		case UnitType.COMMAND:
			return "Icon_Command";
		case UnitType.OPERATIVE:
			return "Icon_Ops";
		case UnitType.AIR:
			return "Icon_Air";
		case UnitType.EVENT_ASSAULT:
			return "Icon_AssaultEvent";
		case UnitType.EVENT_COMMAND:
			return "Icon_CommandEvent";
		case UnitType.EVENT_OPERATIVE:
			return "Icon_OpsEvent";
		case UnitType.EVENT_AIR:
			return "Icon_AirEvent";
		case UnitType.EXCLUSIVE_ASSAULT:
			return "Icon_AssaultExclusive";
		case UnitType.EXCLUSIVE_COMMAND:
			return "Icon_CommandExclusive";
		case UnitType.EXCLUSIVE_OPERATIVE:
			return "Icon_OpsExclusive";
		case UnitType.EXCLUSIVE_AIR:
			return "Icon_AirExclusive";
		case UnitType.BOSS:
			return "Icon_Boss";
		case UnitType.RAID_BOSS:
			return "Icon_Boss";
		case UnitType.TITAN_ASSAULT:
			return "Icon_Assault";
		case UnitType.TITAN_CHOPPER:
			return "Icon_Air";
		case UnitType.TITAN_COMMAND:
			return "Icon_Command";
		case UnitType.TITAN_OPERATIVE:
			return "Icon_Ops";
		default:
			return string.Empty;
		}
	}

	public static bool IsExclusive(this UnitType unitType)
	{
		return unitType == UnitType.EXCLUSIVE_ASSAULT || unitType == UnitType.EXCLUSIVE_AIR || unitType == UnitType.EXCLUSIVE_COMMAND || unitType == UnitType.EXCLUSIVE_OPERATIVE;
	}

	public static bool IsEvent(this UnitType unitType)
	{
		return unitType == UnitType.EVENT_ASSAULT || unitType == UnitType.EVENT_AIR || unitType == UnitType.EVENT_COMMAND || unitType == UnitType.EVENT_OPERATIVE;
	}

	public static string GetRaidBossStateString(this BountyBoardDataEntry.RaidBossState raidBossState)
	{
		switch (raidBossState)
		{
		case BountyBoardDataEntry.RaidBossState.ALIVE:
			return "alive";
		case BountyBoardDataEntry.RaidBossState.DEAD:
			return "dead";
		case BountyBoardDataEntry.RaidBossState.EXPIRED:
			return "expired";
		default:
			return string.Empty;
		}
	}
}

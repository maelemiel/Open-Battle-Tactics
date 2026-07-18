using System;
using System.Collections.Generic;
using Holoville.HOTween;
using UnityEngine;

public class Announcer : MonoBehaviour
{
	private Transform cachedTransform;

	private Sequence seq1;

	private Dictionary<AnnouncerType, EffectInstance> cachedAnnouncers = new Dictionary<AnnouncerType, EffectInstance>();

	private void Awake()
	{
		seq1 = new Sequence();
		DeactivateAllAnnouncers();
		cachedTransform = base.transform;
	}

	public void Move(AnnouncerType announcerType, Vector3 init, Vector3 to)
	{
		Move(announcerType, init, to, Vector3.one, 2f);
	}

	public void Move(AnnouncerType announcerType, Vector3 init, Vector3 to, Vector3 scale)
	{
		Move(announcerType, init, to, scale, 2f);
	}

	public void Move(AnnouncerType announcerType, Vector3 init, Vector3 to, Vector3 scale, float animTime, int sortingOrder = 0)
	{
		ActivateAnnouncer(announcerType, sortingOrder);
		animTime -= 0.5f;
		base.transform.localPosition = init;
		base.transform.localScale = scale;
		seq1.Kill();
		seq1 = new Sequence();
		seq1.Append(HOTween.To(base.transform, 0.25f, new TweenParms().Prop("localPosition", to)));
		seq1.AppendInterval(animTime);
		seq1.Append(HOTween.To(base.transform, 0.25f, new TweenParms().Prop("localPosition", init)));
		seq1.AppendCallback(DeactivateAllAnnouncers);
		seq1.Play();
	}

	public void MoveAndStay(AnnouncerType announcerType, Vector3 init, Vector3 to, int sortingOrder = 0)
	{
		MoveAndStay(announcerType, init, to, Vector3.one, 2f, sortingOrder);
	}

	public void MoveAndStay(AnnouncerType announcerType, Vector3 init, Vector3 to, Vector3 scale, int sortingOrder = 0)
	{
		MoveAndStay(announcerType, init, to, scale, 2f, sortingOrder);
	}

	public void MoveAndStay(AnnouncerType announcerType, Vector3 init, Vector3 to, Vector3 scale, float animTime, int sortingOrder = 0)
	{
		ActivateAnnouncer(announcerType, sortingOrder);
		base.transform.localPosition = init;
		base.transform.localScale = scale;
		seq1.Kill();
		seq1 = new Sequence();
		seq1.Append(HOTween.To(base.transform, animTime, new TweenParms().Prop("localPosition", to)));
		seq1.Play();
	}

	private void ActivateAnnouncer(AnnouncerType type, int sortingOrder = 0)
	{
		foreach (EffectInstance value in cachedAnnouncers.Values)
		{
			value.gameObject.SetActive(false);
		}
		if (cachedAnnouncers.ContainsKey(type))
		{
			if ((bool)cachedAnnouncers[type])
			{
				cachedAnnouncers[type].gameObject.SetActive(true);
			}
			return;
		}
		GetAnnouncer(type, delegate(EffectInstance createdAnnouncer)
		{
			if (createdAnnouncer == null)
			{
				Log.Error("Announcer with type: " + type.ToString() + " couldn't be created");
			}
			else
			{
				if ((bool)createdAnnouncer.SpineAnimation)
				{
					createdAnnouncer.SpineAnimation.Skeleton.SortOrder = sortingOrder;
				}
				createdAnnouncer.gameObject.SetActive(true);
			}
		});
	}

	private void DeactivateAllAnnouncers()
	{
		foreach (EffectInstance value in cachedAnnouncers.Values)
		{
			value.gameObject.SetActive(false);
		}
	}

	public void GetAnnouncer(AnnouncerType type, Action<EffectInstance> callback)
	{
		EffectInstance createdAnnouncer = null;
		if (cachedAnnouncers.ContainsKey(type))
		{
			createdAnnouncer = cachedAnnouncers[type];
			return;
		}
		EffectType effectType = type.GetEffectType();
		if (effectType == EffectType.NONE)
		{
			return;
		}
		StartCoroutine(GlobalEffectsManager.CreateCoroutine(effectType, Vector3.zero, base.gameObject, delegate(EffectInstance result)
		{
			createdAnnouncer = result;
			createdAnnouncer.transform.localPosition = Vector3.zero;
			cachedAnnouncers[type] = createdAnnouncer;
			if (callback != null)
			{
				callback(createdAnnouncer);
			}
		}));
	}

	private void OnDestroy()
	{
		if (seq1 != null)
		{
			seq1.Kill();
		}
	}
}

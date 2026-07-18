using System;
using System.Collections;
using UnityEngine;

public class AnnouncerSpeechController : MonoBehaviour
{
	[SerializeField]
	private Announcer announcer;

	[SerializeField]
	private PhaseText announcerPhaseText;

	public Vector3 announcerOffset = Vector3.zero;

	private Vector3 announcerInitialPosition = Vector3.zero;

	private void Awake()
	{
		announcerInitialPosition = announcer.transform.position;
		if ((bool)announcerPhaseText && announcerPhaseText.endPoint == null)
		{
			announcerPhaseText.initialPosition = new Vector3(0f, 600f, 150f);
		}
	}

	public void UpdateText(string localizationKey)
	{
		if ((bool)announcerPhaseText)
		{
			announcerPhaseText.SetSmallText(Singleton<LocalizationManager>.instance.Get(localizationKey));
		}
	}

	public IEnumerator ShowAnnouncerAndText(AnnouncerType announcerType, string localizationKey, Action callback, float announcerDuration, float textDuration)
	{
		if ((bool)announcer)
		{
			if (announcerDuration < 0f)
			{
				announcer.MoveAndStay(announcerType, announcerInitialPosition, announcerInitialPosition + announcerOffset, announcer.transform.localScale, 1f);
			}
			else
			{
				announcer.Move(announcerType, announcerInitialPosition, announcerInitialPosition + announcerOffset, announcer.transform.localScale, announcerDuration);
			}
		}
		if ((bool)announcerPhaseText)
		{
			announcerPhaseText.PresentSmall(Singleton<LocalizationManager>.instance.Get(localizationKey), callback, textDuration);
		}
		yield return new WaitForSeconds(Mathf.Max(announcerDuration, textDuration));
	}

	public void ShowSmallText(string localizationKey, Action callback, float textDuration)
	{
		if ((bool)announcerPhaseText)
		{
			announcerPhaseText.PresentSmall(Singleton<LocalizationManager>.instance.Get(localizationKey), callback, textDuration);
		}
	}
}

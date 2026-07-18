using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Holoville.HOTween;
using UnityEngine;

public class AnnouncerController : MonoBehaviour
{
	private static AnnouncerController instance;

	private static AnnouncerEvents events = new AnnouncerEvents();

	private static float defaultBlockerAlpha = 0.75f;

	private bool dialogInProgress;

	public float blockerAlpha;

	private float dialogStartTime;

	private AnnouncerType currentAnnouncerType;

	private AnnouncerConfiguration currentAnnouncerConfig;

	private GameObject currentAnnouncer;

	private EffectInstance currentScreen;

	private Tweener continueTextTween;

	[SerializeField]
	private List<AnnouncerConfiguration> configurations;

	[SerializeField]
	private tk2dTextMesh tapToContinue;

	[SerializeField]
	private float tapToContinueDelay = 2f;

	[SerializeField]
	private GameObject container;

	[SerializeField]
	private PhaseText speechBubble;

	[SerializeField]
	private tk2dSprite speechBubbleArrow;

	[SerializeField]
	private tk2dBaseSprite clickBlocker;

	[SerializeField]
	private Camera announcerCamera;

	[SerializeField]
	private Announcer announcer;

	[SerializeField]
	private AnnouncerSpeechController announcerSpeechController;

	[SerializeField]
	private MonoBehaviour coroutineTarget;

	[SerializeField]
	private Transform screenCenter;

	public static AnnouncerEvents Events
	{
		get
		{
			return events;
		}
	}

	public static Camera AnnouncerCamera
	{
		get
		{
			if ((bool)instance)
			{
				return instance.announcerCamera;
			}
			return null;
		}
	}

	public bool DialogVisible
	{
		get
		{
			return instance != null;
		}
	}

	private void Start()
	{
		instance = this;
		blockerAlpha = defaultBlockerAlpha;
		clickBlocker.Alpha = 0f;
		speechBubble.SmallTextMesh.Alpha = 0f;
		speechBubble.LargeTextMesh.Alpha = 0f;
		speechBubble.Background.Alpha = 0f;
		tapToContinue.Alpha = 0f;
	}

	private static IEnumerator EnsureAnnouncersLoaded()
	{
		if (!(instance != null))
		{
			Log.Debug("LoadAnnouncers");
			yield return InitializationManager.LoadLevel(SceneTransitionManager.Scene.Announcers.ToString());
			while (instance == null)
			{
				yield return 0;
			}
		}
	}

	public static IEnumerator DialogTrigger(string triggerKey)
	{
		yield return SingletonManager.StartCoroutine(DialogTrigger(triggerKey, defaultBlockerAlpha));
	}

	public static IEnumerator DialogTrigger(string triggerKey, float blockerAlpha)
	{
		List<AnnouncerDialogSequencesDataModel> sequenceData = GetSequenceData(triggerKey);
		if (sequenceData != null && sequenceData.Count > 0)
		{
			Log.Info("DialogTrigger: " + triggerKey);
			yield return SingletonManager.StartCoroutine(EnsureAnnouncersLoaded());
			UserProfile.player.dialogTriggers.SetDialogTriggered(triggerKey);
			instance.blockerAlpha = blockerAlpha;
			yield return instance.StartCoroutine(instance.StartDialogSequence(sequenceData));
		}
	}

	public static bool HasDialogTrigger(string triggerKey)
	{
		List<AnnouncerDialogSequencesDataModel> sequenceData = GetSequenceData(triggerKey);
		return sequenceData != null && sequenceData.Count > 0;
	}

	public static bool IsDialogVisible()
	{
		if ((bool)instance)
		{
			return instance.DialogVisible;
		}
		return false;
	}

	private static List<AnnouncerDialogSequencesDataModel> GetSequenceData(string triggerKey)
	{
		UserProfile player = UserProfile.player;
		if (player == null)
		{
			return null;
		}
		if (!UserProfile.player.dialogTriggers.ShouldTriggerDialog(triggerKey))
		{
			return null;
		}
		DialogScreenDataModel dialogScreenDataModelWithScreenId = DialogScreenDataModel.GetDialogScreenDataModelWithScreenId(triggerKey);
		List<AnnouncerDialogSequencesDataModel> sequenceDataForSequenceId = AnnouncerDialogSequencesDataModel.GetSequenceDataForSequenceId(dialogScreenDataModelWithScreenId.sequenceId);
		if (sequenceDataForSequenceId.Count == 0)
		{
			return null;
		}
		UserProfile.player.dialogTriggers.SetDialogTriggered(triggerKey);
		return sequenceDataForSequenceId;
	}

	private IEnumerator StartDialogSequence(List<AnnouncerDialogSequencesDataModel> sequenceModels)
	{
		clickBlocker.TweenAlpha(blockerAlpha, 1f);
		int index = 0;
		foreach (AnnouncerDialogSequencesDataModel sequenceItem in sequenceModels.OrderBy((AnnouncerDialogSequencesDataModel item) => item.subsequenceOrder))
		{
			events.SequenceAction(sequenceItem.action);
			yield return StartCoroutine(PlayDialogSequence(sequenceItem));
			index++;
			events.SequenceAdvance(index);
		}
		events.SequenceExit();
		StartCoroutine(ClearAnnouncer(0.5f));
		clickBlocker.TweenAlpha(0f, 0.5f);
		yield return new WaitForSeconds(0.5f);
		Log.Debug("UnloadAnnouncers");
		UnityEngine.Object.Destroy(base.gameObject);
		instance = null;
	}

	private IEnumerator PlayDialogSequence(AnnouncerDialogSequencesDataModel sequenceItem)
	{
		AnnouncerType type = AnnouncerTypeExtensions.GetAnnouncerType(sequenceItem.announcerType);
		AnnouncerConfiguration config = GetAnnouncerConfig(sequenceItem.orientation);
		if (currentAnnouncerType != type || currentAnnouncerConfig != config)
		{
			if (currentAnnouncer != null)
			{
				yield return StartCoroutine(ClearAnnouncer(0.5f));
			}
			if (currentScreen != null)
			{
				yield return StartCoroutine(ClearScreen(0.5f));
			}
			if (config.id == "screen")
			{
				yield return StartCoroutine(ShowScreen(sequenceItem.keyName, 0.5f));
			}
			else
			{
				speechBubble.SetSmallText(sequenceItem.keyName.Localize());
				yield return StartCoroutine(ShowAnnouncer(type, config, 0.5f));
			}
		}
		else
		{
			yield return StartCoroutine(FadeSpeechInOut(sequenceItem.keyName.Localize(), 0.6f));
		}
		dialogInProgress = true;
		dialogStartTime = Time.time;
		while (Time.time < dialogStartTime + (float)Constants.DialogTimeoutSeconds)
		{
			float elapsedTime = Time.time - dialogStartTime;
			if (elapsedTime >= tapToContinueDelay && tapToContinue.Alpha == 0f)
			{
				tapToContinue.transform.position = config.continueTextAnchor.position;
				tapToContinue.Alpha = 0.01f;
				continueTextTween = tapToContinue.TweenAlpha(1f, 0.7f, EaseType.Linear);
			}
			else if (elapsedTime >= tapToContinueDelay + 1f)
			{
				tapToContinue.Alpha = 1f - (Mathf.Cos((float)Math.PI + (elapsedTime - (tapToContinueDelay + 1f)) * 4.763f) * 0.5f + 0.5f) * 0.4f;
			}
			yield return 0;
		}
		tapToContinue.TweenAlpha(0f, 0.3f);
		dialogInProgress = false;
	}

	private IEnumerator ClearAnnouncer(float duration)
	{
		if (currentAnnouncer != null || currentAnnouncerConfig != null)
		{
			if (currentAnnouncer != null && currentAnnouncerConfig != null)
			{
				PerformMoveTweens(currentAnnouncer.transform, null, currentAnnouncerConfig.announcerStart, duration, EaseType.EaseOutExpo);
			}
			if (currentAnnouncerConfig != null)
			{
				PerformMoveTweens(speechBubble.transform, null, currentAnnouncerConfig.speechStart, duration * 0.5f, EaseType.EaseOutExpo);
			}
			speechBubble.Background.TweenAlpha(0f, duration);
			speechBubble.SmallTextMesh.TweenAlpha(0f, duration);
			yield return new WaitForSeconds(duration);
		}
		if ((bool)currentAnnouncer)
		{
			currentAnnouncer.SetActive(false);
			currentAnnouncer = null;
		}
		currentAnnouncerType = AnnouncerType.NONE;
		currentAnnouncerConfig = null;
	}

	private IEnumerator ShowAnnouncer(AnnouncerType type, AnnouncerConfiguration config, float duration)
	{
		currentAnnouncerType = type;
		currentAnnouncerConfig = config;
		announcer.GetAnnouncer(currentAnnouncerType, delegate(EffectInstance result)
		{
			currentAnnouncer = result.gameObject;
		});
		while (currentAnnouncer == null)
		{
			yield return 0;
		}
		EaseType easeType = EaseType.EaseOutBack;
		if (config.announcerStart != null && config.announcerEnd != null)
		{
			currentAnnouncer.gameObject.SetActive(true);
			PerformMoveTweens(currentAnnouncer.transform, config.announcerStart.transform, config.announcerEnd.transform, duration, easeType);
		}
		yield return new WaitForSeconds(duration * 0.5f);
		if (config.speechStart != null && config.speechEnd != null)
		{
			speechBubbleArrow.Alpha = 0f;
			speechBubbleArrow.transform.localPosition = config.bubbleArrowAnchor.localPosition;
			speechBubbleArrow.TweenAlpha(1f, duration);
			speechBubble.Background.FlipX = config.flipSpeechBubble;
			PerformMoveTweens(speechBubble.transform, config.speechStart.transform, config.speechEnd.transform, duration, easeType);
			speechBubble.Background.TweenAlpha(1f, duration);
			speechBubble.SmallTextMesh.TweenAlpha(1f, duration);
		}
		yield return new WaitForSeconds(duration * 0.5f);
	}

	private IEnumerator ClearScreen(float duration)
	{
		if (currentScreen != null)
		{
			tk2dBaseSprite spr = currentScreen.GetComponent<tk2dBaseSprite>();
			if (spr != null)
			{
				spr.TweenAlpha(0f, duration);
				yield return new WaitForSeconds(duration);
			}
			currentScreen.Destroy();
			currentScreen = null;
		}
	}

	private IEnumerator ShowScreen(string effectName, float duration)
	{
		EffectType effectType = (EffectType)(int)Enum.Parse(typeof(EffectType), effectName);
		EffectInstance effect = GlobalEffectsManager.Create(effectType, screenCenter.position);
		effect.gameObject.SetLayerRecursively(base.gameObject.layer);
		currentScreen = effect;
		tk2dBaseSprite spr = effect.GetComponent<tk2dBaseSprite>();
		if (spr != null)
		{
			spr.Alpha = 0f;
			spr.TweenAlpha(1f, duration);
			yield return new WaitForSeconds(duration);
		}
	}

	private IEnumerator FadeSpeechInOut(string newText, float duration)
	{
		speechBubble.SmallTextMesh.TweenAlpha(0f, duration * 0.5f);
		yield return new WaitForSeconds(duration * 0.5f);
		speechBubble.SetSmallText(newText);
		speechBubble.SmallTextMesh.TweenAlpha(1f, duration * 0.5f);
		yield return new WaitForSeconds(duration * 0.5f);
	}

	private AnnouncerConfiguration GetAnnouncerConfig(string configName)
	{
		foreach (AnnouncerConfiguration configuration in configurations)
		{
			if (configuration.id == configName)
			{
				return configuration;
			}
		}
		return null;
	}

	private void AdvanceDialogSequence()
	{
		if (dialogInProgress)
		{
			if (continueTextTween != null)
			{
				continueTextTween.Kill();
				continueTextTween = null;
			}
			dialogStartTime = Time.time - (float)Constants.DialogTimeoutSeconds - 1f;
			AudioTrigger.TapToContinue.Play();
		}
	}

	private void PerformMoveTweens(Transform target, Transform start, Transform end, float duration, EaseType easeType)
	{
		if (start != null)
		{
			target.position = start.position;
			target.localScale = start.localScale;
		}
		if (end != null)
		{
			target.TweenPosition(end.position, duration, easeType);
			target.TweenLocalScaleVec(end.localScale, duration, easeType);
		}
	}
}

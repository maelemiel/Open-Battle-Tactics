using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SceneController : MonoBehaviour
{
	public static bool resumeCallbackEnable = true;

	public static bool fireWorksEnable;

	public bool destroyOnTransition = true;

	public bool _inactiveBehaviourEnable = true;

	public bool _appOnResumeEnabled = true;

	private bool destroyed;

	private List<SceneTransition> allTransitions;

	private List<SceneTransition> pendingTransitions;

	private SceneTransition.Direction direction;

	private List<GameObject> disabledGameObjects;

	protected SceneModel sceneModel;

	protected bool transitionCancelled;

	protected bool allowsBackButton;

	protected string _sectionTitle = string.Empty;

	protected bool _showTopBar = true;

	protected bool _showHomeButton = true;

	public bool Destroyed
	{
		get
		{
			return destroyed;
		}
	}

	public string SectionTitle
	{
		get
		{
			return _sectionTitle;
		}
		set
		{
			_sectionTitle = value;
			if ((bool)TopBarController.instance)
			{
				TopBarController.instance.SectionTitle = value;
			}
		}
	}

	public bool ShowTopBar
	{
		get
		{
			return _showTopBar;
		}
		set
		{
			_showTopBar = value;
			if ((bool)TopBarController.instance)
			{
				TopBarController.instance.Visible = value;
			}
		}
	}

	public bool ShowHomeButton
	{
		get
		{
			return _showHomeButton;
		}
		set
		{
			_showHomeButton = value;
			if ((bool)TopBarController.instance)
			{
				TopBarController.instance.ShowHomeButton = value;
			}
		}
	}

	public virtual void OnBackButton()
	{
		if (allowsBackButton && !SceneTransitionManager.transitionActive && PopupManager.PopupCount == 0 && !LoadingPopupManager.ShouldBlockInput)
		{
			SceneTransitionManager.PopScene();
		}
	}

	public virtual bool OnHomeButton()
	{
		return true;
	}

	private void OnApplicationPause(bool pause)
	{
		if (!pause && resumeCallbackEnable && _appOnResumeEnabled && UserProfile.player.tutorial.IsComplete && !AnnouncerController.IsDialogVisible())
		{
			if (PopupManager.PopupCount > 0)
			{
				PopupManager.DestroyAllPopups();
			}
			SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.HomeScene);
		}
	}

	public virtual void Awake()
	{
		Singleton<InitializationManager>.Instantiate();
		SceneTransitionManager.RegisterSceneRoot(this);
		sceneModel = SceneTransitionManager.CurrentSceneDM;
		if (sceneModel != null)
		{
			CrittercismUtil.LeaveBreadcrumb("EnterScene " + sceneModel._scene);
			sceneModel.controller = this;
		}
		if (GetComponent<EffectCacheModel>() == null)
		{
			GlobalEffectsManager.UnloadAll();
		}
		Singleton<InitializationManager>.instance.ExecuteOnState(InitializationManager.State.OnlineReady, delegate
		{
			BaseInit();
		});
	}

	protected virtual void BaseInit()
	{
		ShowHomeButton = ShowHomeButton;
		SectionTitle = SectionTitle;
		ShowTopBar = ShowTopBar;
		if (_inactiveBehaviourEnable && resumeCallbackEnable && UserProfile.player.tutorial.IsComplete)
		{
			StartCoroutine(InactiveCoroutine());
		}
	}

	private IEnumerator InactiveCoroutine()
	{
		for (int count = 0; count < 5; count++)
		{
			yield return new WaitForSeconds(60f);
		}
		SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.HomeScene);
	}

	private void FindChildSceneTransitions(SceneTransition.Type type)
	{
		allTransitions = new List<SceneTransition>();
		pendingTransitions = new List<SceneTransition>();
		SceneTransition[] componentsInChildren = base.gameObject.GetComponentsInChildren<SceneTransition>(true);
		SceneTransition[] array = componentsInChildren;
		foreach (SceneTransition sceneTransition in array)
		{
			if ((sceneTransition.allowedDirections == direction || sceneTransition.allowedDirections == SceneTransition.Direction.Both) && (sceneTransition.allowedTypes == type || sceneTransition.allowedTypes == SceneTransition.Type.All))
			{
				sceneTransition.SetSceneRoot(this);
				allTransitions.Add(sceneTransition);
				pendingTransitions.Add(sceneTransition);
			}
		}
	}

	public void PumpTransitions()
	{
		for (int num = pendingTransitions.Count - 1; num >= 0; num--)
		{
			pendingTransitions[num].Pump();
		}
	}

	public void OnPrepareTransitionOut(SceneTransition.Type type)
	{
		if (TopBarController.instance != null)
		{
			TopBarController.instance.OnSceneTransitionOut(this);
		}
		direction = SceneTransition.Direction.Out;
		FindChildSceneTransitions(type);
		foreach (SceneTransition allTransition in allTransitions)
		{
			allTransition.SetDirection(SceneTransition.Direction.Out);
			allTransition.PrepareTransition();
		}
	}

	public virtual void OnBeginTransitionOut()
	{
		foreach (SceneTransition allTransition in allTransitions)
		{
			allTransition.BeginTransition();
		}
		if (pendingTransitions.Count == 0)
		{
			AllTransitionsDone();
		}
	}

	public void OnEndTransitionOut()
	{
		foreach (SceneTransition allTransition in allTransitions)
		{
			if (allTransition != null)
			{
				allTransition.EndTransition();
			}
		}
		allTransitions = null;
		if (sceneModel != null)
		{
			sceneModel.controller = null;
		}
		destroyed = true;
		Object.Destroy(base.gameObject);
	}

	public void TransitionDone(SceneTransition o)
	{
		pendingTransitions.Remove(o);
		if (pendingTransitions.Count == 0)
		{
			AllTransitionsDone();
		}
	}

	public void OnPrepareTransitionIn(SceneTransition.Type type)
	{
		direction = SceneTransition.Direction.In;
		FindChildSceneTransitions(type);
		foreach (SceneTransition allTransition in allTransitions)
		{
			allTransition.SetDirection(SceneTransition.Direction.In);
			allTransition.PrepareTransition();
		}
	}

	private void FindDisabledGameObjects()
	{
		disabledGameObjects = new List<GameObject>();
		AddDisabledChildren(base.transform);
	}

	private void AddDisabledChildren(Transform parent)
	{
		foreach (Transform item in parent)
		{
			if (!item.gameObject.activeSelf)
			{
				disabledGameObjects.Add(item.gameObject);
			}
			AddDisabledChildren(item);
		}
	}

	public void OnWarmTransitionIn()
	{
	}

	public virtual void OnBeginTransitionIn()
	{
		foreach (SceneTransition allTransition in allTransitions)
		{
			allTransition.BeginTransition();
		}
		if (pendingTransitions.Count == 0)
		{
			AllTransitionsDone();
		}
		if (TopBarController.instance != null)
		{
			TopBarController.instance.OnSceneTransitionIn(this);
		}
	}

	public void OnEndTransitionIn()
	{
		foreach (SceneTransition allTransition in allTransitions)
		{
			allTransition.EndTransition();
		}
		allTransitions = null;
		_OnEndTransitionIn();
	}

	protected virtual void _OnEndTransitionIn()
	{
	}

	protected void AllTransitionsDone()
	{
		switch (direction)
		{
		case SceneTransition.Direction.In:
			SceneTransitionManager.FinishedTransitionIn(this);
			break;
		case SceneTransition.Direction.Out:
			SceneTransitionManager.FinishedTransitionOut(this);
			break;
		}
		pendingTransitions = null;
	}

	protected void CancelTransition()
	{
		transitionCancelled = true;
		int loadingId = LoadingPopupManager.ShowLoadingPopup(0f);
		SceneTransitionManager.ClearHistory();
		SceneTransitionManager.TransitionOutDone += delegate
		{
			LoadingPopupManager.ClearLoadingPopup(loadingId);
		};
		SceneTransitionManager.TransitionInDone += delegate
		{
			SceneTransitionManager.PopToScene(SceneTransitionManager.Scene.Default);
		};
		SceneTransitionManager.readyToTransitionIn = true;
	}

	public IEnumerator DownloadVideo(string videoURL, string videoFileName)
	{
		if (!File.Exists(videoFileName))
		{
			WWW www = new WWW(videoURL);
			yield return www;
			if (www != null && www.isDone && www.error == null)
			{
				FileStream stream = new FileStream(videoFileName, FileMode.OpenOrCreate);
				stream.Write(www.bytes, 0, www.bytes.Length);
				stream.Close();
				yield return null;
			}
			else if (www.error != null)
			{
				Log.Error("Downloading the intro video had an error: " + www.error);
			}
		}
		yield return null;
	}
}

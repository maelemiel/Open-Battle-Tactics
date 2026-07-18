using System;
using System.Collections.Generic;
using UnityEngine;

public class PopupManager
{
	private class PopupHolder
	{
		public PopupDataModel m;

		public PopupController pc;

		public bool destroy;

		public bool recreated;

		public PopupHolder(PopupDataModel model)
		{
			m = model;
		}
	}

	private class QueueWithScene
	{
		public PopupHolder pHolder;

		public SceneTransitionManager.Scene queuedScene;
	}

	private class PopupBackupState
	{
		public List<PopupHolder> popups = new List<PopupHolder>();

		public SceneTransitionManager.Scene currentScene = SceneTransitionManager.Scene._NULL;

		public SceneModel currentSceneModel;
	}

	private static Queue<QueueWithScene> actionWithSceneQueue = new Queue<QueueWithScene>();

	private static Queue<PopupHolder> actionQueue = new Queue<PopupHolder>();

	private static List<PopupHolder> popups = new List<PopupHolder>();

	public static bool highDepThPopup = false;

	private static PopupBackupState popupLvlUpFeatureBackup = new PopupBackupState();

	private static bool acting = false;

	public static int PopupCount
	{
		get
		{
			return Mathf.Max(popups.Count, 0);
		}
	}

	public static bool Acting
	{
		get
		{
			return acting;
		}
	}

	public static PopupDataModel CurrentPopupDM
	{
		get
		{
			return popups[popups.Count - 1].m;
		}
	}

	public static bool ShowPopup(PopupDataModel popupModel, bool force = false, int forceIndex = -1, bool recreated = false)
	{
		Log.DebugTag("Show Popup " + popupModel.popupType.ToString() + " " + recreated, null, "PopUpManager");
		if (!force && popups.Count > 0 && popups[0].m.popupType == popupModel.popupType)
		{
			Debug.LogWarning("ShowPopUpCancelled. Type: " + popupModel.popupType);
			return false;
		}
		popupModel.id = ((forceIndex >= 0) ? forceIndex : popups.Count);
		PopupHolder popupHolder = new PopupHolder(popupModel);
		popupHolder.recreated = recreated;
		if (!acting)
		{
			ShowPopupActual(popupHolder, popupModel.popUpScene);
		}
		else
		{
			actionQueue.Enqueue(popupHolder);
		}
		return true;
	}

	public static void ShowPopup(PopupDataModel popupModel, SceneTransitionManager.Scene scene)
	{
		popupModel.id = popups.Count;
		PopupHolder popupHolder = new PopupHolder(popupModel);
		Log.DebugTag("Show Popup 2 " + popupModel.popupType, null, "PopUpManager");
		if (!acting)
		{
			ShowPopupActual(popupHolder, scene);
			return;
		}
		actionWithSceneQueue.Enqueue(new QueueWithScene
		{
			pHolder = popupHolder,
			queuedScene = scene
		});
	}

	private static void ShowPopupActual(PopupHolder popupHolder, SceneTransitionManager.Scene scene)
	{
		acting = true;
		popups.Add(popupHolder);
		InitializationManager.LoadLevel(scene.ToString());
	}

	public static void DestroyAllPopups()
	{
		PopupHolder[] array = popups.ToArray();
		foreach (PopupHolder ph in array)
		{
			DestroyPopupActual(ph);
		}
	}

	public static void DestroyPopup(PopupDataModel popupModel)
	{
		PopupHolder popupHolder = popups.Find((PopupHolder item) => item.m == popupModel);
		if (popupHolder == null)
		{
			Debug.Log("PopupManager.DestroyPopup: PopupModel with id:" + popupModel.id + " has been already destroyed");
			return;
		}
		if (!acting)
		{
			DestroyPopupActual(popupHolder);
			return;
		}
		popupHolder.destroy = true;
		actionQueue.Enqueue(popupHolder);
	}

	private static void DestroyPopupActual(PopupHolder ph)
	{
		acting = true;
		popups.Remove(ph);
		if (ph != null)
		{
			ph.pc.Dispose();
		}
		if (ph.recreated && ph.m.afterRemoveAction != null && popups.Count > 0)
		{
			popups[popups.Count - 1].pc.Invoke(ph.m.afterRemoveAction.Method.Name, 0f);
		}
		else if (ph.recreated && ph.m.afterRemoveActionObject != null && popups.Count > 0)
		{
			popups[popups.Count - 1].pc.StartCoroutine(ph.m.afterRemoveActionObject.Method.Name, ph.m);
		}
		else if (ph.m.afterRemoveAction != null)
		{
			ph.m.afterRemoveAction();
		}
		else if (ph.m.afterRemoveActionObject != null)
		{
			ph.m.afterRemoveActionObject(ph.m);
		}
	}

	public static void CallRightActionObject(Action<object> action, PopupDataModel dataModel)
	{
		if (action == null)
		{
			return;
		}
		if (popups.Count > 0 && popups[popups.Count - 1].recreated)
		{
			if (popups.Count > 1)
			{
				popups[popups.Count - 2].pc.StartCoroutine(action.Method.Name, dataModel);
			}
			else
			{
				popups[popups.Count - 1].pc.StartCoroutine(action.Method.Name, dataModel);
			}
		}
		else
		{
			action(dataModel);
		}
	}

	public static void CallRightAction(Action action, PopupDataModel dataModel)
	{
		if (action == null)
		{
			return;
		}
		if (popups.Count > 0 && popups[popups.Count - 1].recreated)
		{
			if (popups.Count > 1)
			{
				popups[popups.Count - 2].pc.Invoke(action.Method.Name, 0f);
			}
		}
		else
		{
			action();
		}
	}

	public static void RegisterPopupDisposed()
	{
		acting = false;
		if (actionQueue.Count > 0 || actionWithSceneQueue.Count > 0)
		{
			ProcessActionQueue();
		}
		else if (PopupCount == 0)
		{
			SceneTransitionManager.ProcessPendingScenes();
		}
	}

	private static void ProcessActionQueue()
	{
		if (actionWithSceneQueue.Count > 0)
		{
			ProcessActionWithSceneQueue();
		}
		else if (actionQueue.Count > 0)
		{
			ProcessRegularActionQueue();
		}
	}

	private static void ProcessRegularActionQueue()
	{
		PopupHolder popupHolder = actionQueue.Dequeue();
		if (popupHolder.destroy)
		{
			DestroyPopupActual(popupHolder);
		}
		else
		{
			ShowPopupActual(popupHolder, popupHolder.m.popUpScene);
		}
	}

	private static void ProcessActionWithSceneQueue()
	{
		QueueWithScene queueWithScene = actionWithSceneQueue.Dequeue();
		if (queueWithScene.pHolder.destroy)
		{
			DestroyPopupActual(queueWithScene.pHolder);
		}
		else
		{
			ShowPopupActual(queueWithScene.pHolder, queueWithScene.queuedScene);
		}
	}

	public static void RegisterPopupController(PopupController controller)
	{
		PopupHolder popupHolder = popups[popups.Count - 1];
		Log.DebugTag("Registering Popup Controller " + controller, null, "PopUpManager");
		popupHolder.pc = controller;
		acting = false;
		if (actionQueue.Count > 0 || actionWithSceneQueue.Count > 0)
		{
			ProcessActionQueue();
		}
		else if (PopupCount == 0)
		{
			SceneTransitionManager.ProcessPendingScenes();
		}
	}

	public static void BackupState(bool backupScene)
	{
		Log.DebugTag("BackupState " + backupScene, null, "PopUpManager");
		if (backupScene)
		{
			popupLvlUpFeatureBackup.currentScene = SceneTransitionManager.CurrentSceneDM._scene;
			popupLvlUpFeatureBackup.currentSceneModel = SceneTransitionManager.CurrentSceneDM;
		}
		popupLvlUpFeatureBackup.popups.AddRange(popups);
		DestroyAllPopups();
	}

	public static void RestoreState(bool restoreScene)
	{
		if (restoreScene)
		{
			SceneTransitionManager.PushToScene(popupLvlUpFeatureBackup.currentScene, popupLvlUpFeatureBackup.currentSceneModel, false, true);
			popupLvlUpFeatureBackup.currentScene = SceneTransitionManager.Scene._NULL;
			popupLvlUpFeatureBackup.currentSceneModel = null;
		}
		Log.DebugTag("Restoring State " + restoreScene, null, "PopUpManager");
		for (int i = 0; i < popupLvlUpFeatureBackup.popups.Count; i++)
		{
			ShowPopup(popupLvlUpFeatureBackup.popups[i].m, true, popupLvlUpFeatureBackup.popups[i].m.id, true);
		}
		popupLvlUpFeatureBackup.popups.Clear();
	}

	public static bool HasBackupState()
	{
		Log.DebugTag("HasBackupState Check", null, "PopUpManager");
		return popupLvlUpFeatureBackup.currentScene != SceneTransitionManager.Scene._NULL;
	}

	public static void ClearBackupState()
	{
		if (HasBackupState())
		{
			Log.DebugTag("Clearing Backup State", null, "PopUpManager");
			popupLvlUpFeatureBackup.popups.Clear();
			popupLvlUpFeatureBackup.currentScene = SceneTransitionManager.Scene._NULL;
			popupLvlUpFeatureBackup.currentSceneModel = null;
		}
	}
}

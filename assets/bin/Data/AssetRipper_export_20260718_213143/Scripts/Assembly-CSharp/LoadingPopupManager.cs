using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingPopupManager
{
	public enum LoadingType
	{
		normal = 0,
		altPosition = 1,
		blackBackground = 2,
		loadingBarOnly = 3,
		transparent = 4
	}

	private static List<int> displayedPerId = new List<int>();

	private static List<int> canceledPerId = new List<int>();

	private static List<int> neverDisplayPerId = new List<int>();

	private static List<int> knownIds = new List<int>();

	private static LoadingPopupController loadingPopupController;

	private static bool loadingPopupDisplayed = false;

	private static bool destroyLoadingPopup = false;

	private static int sceneTransitionPopUpId = 0;

	private static int loadingPopUpId = 1;

	private static int showPopupCount = 0;

	private static LoadingType loadingType = LoadingType.normal;

	public static string textToShow = string.Empty;

	public static string tipToShow = string.Empty;

	public static bool ShouldBlockInput
	{
		get
		{
			return showPopupCount != 0;
		}
	}

	public static int ShowLoadingPopup(float delay)
	{
		int num = loadingPopUpId++;
		knownIds.Add(num);
		if (delay == float.MaxValue || float.IsInfinity(delay))
		{
			neverDisplayPerId.Add(num);
			return num;
		}
		showPopupCount++;
		if (delay <= 0f)
		{
			ShowLoadingPopupActual(num);
		}
		else
		{
			Singleton<WorkQueue>.instance.StartCoroutine(ShowLoadingPopupCoroutine(delay, num));
		}
		return num;
	}

	private static IEnumerator ShowLoadingPopupCoroutine(float delay, int id)
	{
		yield return new WaitForSeconds(delay);
		ShowLoadingPopupActual(id);
	}

	private static void ShowLoadingPopupActual(int id)
	{
		if (!canceledPerId.Remove(id))
		{
			displayedPerId.Add(id);
			if (!loadingPopupDisplayed)
			{
				loadingPopupDisplayed = true;
				InitializationManager.LoadLevel(SceneTransitionManager.Scene.LoadingPopupScene.ToString());
			}
		}
	}

	public static void ClearAllLoadingPopups()
	{
		for (int num = neverDisplayPerId.Count - 1; num >= 0; num--)
		{
			ClearLoadingPopup(neverDisplayPerId[num]);
		}
		int[] array = knownIds.ToArray();
		int[] array2 = array;
		foreach (int id in array2)
		{
			ClearLoadingPopup(id);
		}
	}

	public static void ClearLoadingPopup(int id)
	{
		if (!knownIds.Remove(id) || neverDisplayPerId.Remove(id))
		{
			return;
		}
		showPopupCount--;
		if (displayedPerId.Remove(id))
		{
			if (displayedPerId.Count == 0 && loadingPopupDisplayed)
			{
				DestroyLoadingPopupActual();
			}
		}
		else
		{
			canceledPerId.Add(id);
		}
	}

	public static void ClearAll()
	{
		Log.Info("Clearing All Loading Popups");
		Singleton<WorkQueue>.instance.StopAllCoroutines();
		DestroyLoadingPopupActual();
		displayedPerId.Clear();
		canceledPerId.Clear();
		neverDisplayPerId.Clear();
		knownIds.Clear();
		loadingPopUpId = 1;
		showPopupCount = 0;
		destroyLoadingPopup = false;
		sceneTransitionPopUpId = 0;
		loadingPopupDisplayed = false;
	}

	private static void DestroyLoadingPopupActual()
	{
		if (loadingPopupController != null)
		{
			loadingPopupController.Destroy();
			loadingPopupController = null;
			loadingPopupDisplayed = false;
		}
		else
		{
			destroyLoadingPopup = true;
		}
		loadingType = LoadingType.normal;
		textToShow = string.Empty;
	}

	public static bool RegisterLoadingPopupController(LoadingPopupController controller)
	{
		if (loadingPopupController != null)
		{
			return false;
		}
		if (destroyLoadingPopup)
		{
			controller.Destroy();
			destroyLoadingPopup = false;
			loadingPopupDisplayed = false;
		}
		else
		{
			loadingPopupController = controller;
			loadingPopupController.Init(loadingType);
		}
		loadingType = LoadingType.normal;
		return true;
	}

	public static void SetAlternativePopUp(LoadingType type)
	{
		loadingType = type;
	}

	public static void ShowSceneTransitionPopUp(string text = "")
	{
		textToShow = Singleton<LocalizationManager>.instance.Get(text);
		sceneTransitionPopUpId = ShowLoadingPopup(0f);
	}

	public static void HideSceneTransitionPopUp()
	{
		ClearLoadingPopup(sceneTransitionPopUpId);
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditTeamSortOptions : MonoBehaviour
{
	[SerializeField]
	private SceneController sceneControllerInstance;

	[SerializeField]
	private iSortableController sceneController;

	[SerializeField]
	private Transform sortButton;

	[SerializeField]
	private Transform sortOptionContainer;

	[SerializeField]
	private GameObject sortOptionPrefab;

	[SerializeField]
	private Vector3 offsetBetweenOptions = new Vector3(0f, 80f, 0f);

	public Action OnSortComplete;

	private List<GameObject> sortOptions = new List<GameObject>();

	public void AddSortOption(SortType sortType)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(sortOptionPrefab) as GameObject;
		gameObject.transform.SetParent(sortOptionContainer);
		gameObject.transform.localPosition = offsetBetweenOptions * sortOptions.Count;
		sortOptions.Add(gameObject);
		sortType.sortButton = gameObject;
		tk2dTextMesh componentInChildren = gameObject.GetComponentInChildren<tk2dTextMesh>();
		componentInChildren.text = sortType.label;
		tk2dUIItem component = gameObject.GetComponent<tk2dUIItem>();
		component.OnClick += delegate
		{
			SetCurrentSort(sortType);
		};
	}

	public void SetCurrentSort(SortType sortType)
	{
		if (sceneController == null)
		{
			sceneController = (iSortableController)sceneControllerInstance;
		}
		if (sortType != null && sortType.HasHandler)
		{
			SortByType(sortType, sceneController.UnassignedInventory());
			if (OnSortComplete != null)
			{
				OnSortComplete();
			}
			else
			{
				sceneController.ScrollableAreaControllerInstance().OnDataChanged();
				sceneController.ScrollableAreaControllerInstance().ScrollableArea.TweenValue(0f, 0.5f);
			}
		}
		StartCoroutine(HideSortOptions());
	}

	public static void SortByType(SortType sortType, IList list)
	{
		for (int i = 1; i < list.Count; i++)
		{
			object obj = list[i];
			int num = i;
			while (num > 0 && sortType.CompareData(sortType.sortHandler(list[num - 1]), sortType.sortHandler(obj)) < 0)
			{
				list[num] = list[num - 1];
				num--;
			}
			list[num] = obj;
		}
	}

	public IEnumerator ShowSortOptions()
	{
		sortButton.transform.TweenLocalXPosition(-300f, 0.2f);
		yield return new WaitForSeconds(0.2f);
		sortOptionContainer.transform.TweenLocalXPosition(0f, 0.2f);
	}

	public IEnumerator HideSortOptions()
	{
		sortOptionContainer.transform.TweenLocalXPosition(-300f, 0.2f);
		yield return new WaitForSeconds(0.2f);
		sortButton.transform.TweenLocalXPosition(131f, 0.2f);
	}
}

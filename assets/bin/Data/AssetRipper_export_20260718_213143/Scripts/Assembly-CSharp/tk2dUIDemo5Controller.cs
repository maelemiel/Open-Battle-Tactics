using System.Collections;
using UnityEngine;

public class tk2dUIDemo5Controller : tk2dUIBaseDemoController
{
	public tk2dUILayout prefabItem;

	public tk2dUIScrollableArea manualScrollableArea;

	public tk2dUILayout lastListItem;

	public tk2dUIScrollableArea autoScrollableArea;

	private void CustomizeListObject(Transform contentRoot)
	{
		string[] array = new string[10] { "Ba", "Po", "Re", "Zu", "Meh", "Ra'", "B'k", "Adam", "Ben", "George" };
		string[] array2 = new string[28]
		{
			"Hoopler", "Hysleria", "Yeinydd", "Nekmit", "Novanoid", "Toog1t", "Yboiveth", "Resaix", "Voquev", "Yimello",
			"Oleald", "Digikiki", "Nocobot", "Morath", "Toximble", "Rodrup", "Chillaid", "Brewtine", "Surogou", "Winooze",
			"Hendassa", "Ekcle", "Noelind", "Animepolis", "Tupress", "Jeren", "Yoffa", "Acaer"
		};
		string text = array[Random.Range(0, array.Length)] + " " + array2[Random.Range(0, array2.Length)];
		Color color = new Color32((byte)Random.Range(192, 255), (byte)Random.Range(192, 255), (byte)Random.Range(192, 255), byte.MaxValue);
		contentRoot.Find("Name").GetComponent<tk2dTextMesh>().text = text;
		contentRoot.Find("HP").GetComponent<tk2dTextMesh>().text = "HP: " + Random.Range(100, 512);
		contentRoot.Find("MP").GetComponent<tk2dTextMesh>().text = "MP: " + Random.Range(2, 40) * 10;
		contentRoot.Find("Portrait").GetComponent<tk2dBaseSprite>().color = color;
	}

	private void Start()
	{
		prefabItem.transform.parent = null;
		DoSetActive(prefabItem.transform, false);
		float num = 0f;
		float x = (prefabItem.GetMaxBounds() - prefabItem.GetMinBounds()).x;
		for (int i = 0; i < 10; i++)
		{
			tk2dUILayout tk2dUILayout2 = Object.Instantiate(prefabItem) as tk2dUILayout;
			tk2dUILayout2.transform.parent = manualScrollableArea.contentContainer.transform;
			tk2dUILayout2.transform.localPosition = new Vector3(num, 0f, 0f);
			DoSetActive(tk2dUILayout2.transform, true);
			CustomizeListObject(tk2dUILayout2.transform);
			num += x;
		}
		lastListItem.transform.localPosition = new Vector3(num, lastListItem.transform.localPosition.y, 0f);
		num += (lastListItem.GetMaxBounds() - lastListItem.GetMinBounds()).x;
		manualScrollableArea.ContentLength = num;
		for (int j = 0; j < 10; j++)
		{
			tk2dUILayout tk2dUILayout3 = Object.Instantiate(prefabItem) as tk2dUILayout;
			autoScrollableArea.ContentLayoutContainer.AddLayoutAtIndex(tk2dUILayout3, tk2dUILayoutItem.FixedSizeLayoutItem(), autoScrollableArea.ContentLayoutContainer.ItemCount - 1);
			DoSetActive(tk2dUILayout3.transform, true);
			CustomizeListObject(tk2dUILayout3.transform);
		}
	}

	private IEnumerator AddSomeItemsManual()
	{
		float x = lastListItem.transform.localPosition.x;
		float w = (prefabItem.GetMaxBounds() - prefabItem.GetMinBounds()).x;
		int numToAdd = Random.Range(1, 5);
		for (int i = 0; i < numToAdd; i++)
		{
			tk2dUILayout layout = Object.Instantiate(prefabItem) as tk2dUILayout;
			layout.transform.parent = manualScrollableArea.contentContainer.transform;
			layout.transform.localPosition = new Vector3(x, 0f, 0f);
			DoSetActive(layout.transform, true);
			CustomizeListObject(layout.transform);
			x += w;
			lastListItem.transform.localPosition = new Vector3(x, lastListItem.transform.localPosition.y, 0f);
			manualScrollableArea.ContentLength = x + (lastListItem.GetMaxBounds() - lastListItem.GetMinBounds()).x;
			yield return new WaitForSeconds(0.2f);
		}
	}

	private IEnumerator AddSomeItemsAuto()
	{
		int numToAdd = Random.Range(1, 5);
		for (int i = 0; i < numToAdd; i++)
		{
			tk2dUILayout layout = Object.Instantiate(prefabItem) as tk2dUILayout;
			autoScrollableArea.ContentLayoutContainer.AddLayoutAtIndex(layout, tk2dUILayoutItem.FixedSizeLayoutItem(), autoScrollableArea.ContentLayoutContainer.ItemCount - 1);
			DoSetActive(layout.transform, true);
			CustomizeListObject(layout.transform);
			yield return new WaitForSeconds(0.2f);
		}
	}
}

using System.Collections.Generic;
using UnityEngine;

public class EditTeamCellGroup : MonoBehaviour
{
	[SerializeField]
	private GameObject cellPrefab;

	public int index;

	public List<EditTeamBaseCell> cellList;

	public tk2dSprite[] slotList;

	private List<Vector3> cellStartPositions;

	private void Awake()
	{
		cellStartPositions = new List<Vector3>();
		cellList = new List<EditTeamBaseCell>();
		for (int i = 0; i < slotList.Length; i++)
		{
			GameObject gameObject = Object.Instantiate(cellPrefab, Vector3.zero, Quaternion.identity) as GameObject;
			gameObject.transform.SetParent(base.transform);
			gameObject.transform.localPosition = slotList[i].transform.localPosition;
			cellList.Add(gameObject.GetComponent<EditTeamBaseCell>());
			cellList[i].DragItem.DragMinAngle = -360f;
			cellList[i].DragItem.DragMaxAngle = 360f;
		}
		foreach (EditTeamBaseCell cell in cellList)
		{
			cellStartPositions.Add(cell.transform.localPosition);
		}
	}

	public Vector3 GetCellWorldPosition(int index)
	{
		return base.transform.TransformPoint(cellStartPositions[index]);
	}
}

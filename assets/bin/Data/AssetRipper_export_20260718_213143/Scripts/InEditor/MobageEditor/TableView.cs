using UnityEngine;

namespace MobageEditor
{
	public class TableView : MonoBehaviour
	{
		private MB_WW_FriendPickerViewController friendPickerViewController;

		private bool[] selectMe;

		public void Load(MB_WW_FriendPickerViewController friendPickerViewController)
		{
			this.friendPickerViewController = friendPickerViewController;
			selectMe = new bool[friendPickerViewController.DisplayedFriendsList.Count];
		}

		private void OnGUI()
		{
			if (friendPickerViewController.DisplayedFriendsList != null)
			{
				GUI.Window(0, new Rect(5f, 55f, 310f, 480f), DoMyWindow, "Mock UITableView");
			}
		}

		private void DoMyWindow(int id)
		{
			for (int i = 0; i < friendPickerViewController.DisplayedFriendsList.Count; i++)
			{
				drawSingleLine(i);
			}
			if (GUI.Button(new Rect(10f, 360f, 100f, 40f), "Confirm"))
			{
				Debug.Log("confirm!!");
				onFriendPickerSelection();
			}
			if (GUI.Button(new Rect(120f, 360f, 100f, 40f), "Close"))
			{
				Object.Destroy(this);
			}
		}

		private void drawSingleLine(int pos)
		{
			GUI.depth = 0;
			int num = 10;
			int num2 = 10 + pos * 32;
			GUI.Label(new Rect(num, num2, 128f, 32f), friendPickerViewController.DisplayedFriendsList[pos].UserObject.nickname);
			selectMe[pos] = GUI.Toggle(new Rect(num + 140, num2, 150f, 32f), selectMe[pos], "Select me");
		}

		private void onFriendPickerSelection()
		{
			friendPickerViewController.OnFriendPickerSelection(selectMe);
			Object.Destroy(base.gameObject);
		}

		private void OnDisable()
		{
			Debug.Log("onDisable");
		}

		private void OnMouseDown()
		{
			Debug.Log("Mouse Down " + Input.mousePosition.x + ',' + Input.mousePosition.y);
		}
	}
}

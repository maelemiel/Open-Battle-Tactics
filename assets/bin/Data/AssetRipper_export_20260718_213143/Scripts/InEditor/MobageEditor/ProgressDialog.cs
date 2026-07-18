using UnityEngine;

namespace MobageEditor
{
	public class ProgressDialog : MonoBehaviour
	{
		private const int autoClosedDialog = 20;

		private bool showCloseButton;

		private int closeTimer;

		public ProgressDialog(string title, string message, bool isAutoDismissDialog, int seconds)
		{
			showCloseButton = isAutoDismissDialog;
			closeTimer = seconds;
		}

		public ProgressDialog(string title, string message, bool showCloseButton)
			: this(title, message, false, 20)
		{
		}

		public ProgressDialog(string title, string message)
			: this(title, message, false)
		{
		}

		public void Show()
		{
			Debug.Log("ProgressDialog Show");
		}

		public void Hide()
		{
			Debug.Log("ProgressDialog Hide");
			Object.Destroy(base.gameObject);
		}
	}
}

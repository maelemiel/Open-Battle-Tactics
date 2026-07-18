using UnityEngine;

namespace MobageEditor
{
	public class Dialog
	{
		public virtual bool DisplayDialog(string title, string text, string yes, string no)
		{
			Debug.Log(title + ":" + text);
			return true;
		}
	}
}

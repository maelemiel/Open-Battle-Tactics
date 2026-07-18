using UnityEngine;

namespace LCD.Internal.Web
{
	[ExecuteInEditMode]
	internal class UnitySendMessageDispatcher
	{
		public static void Dispatch(string name, string method, string message)
		{
			GameObject gameObject = GameObject.Find(name);
			if (gameObject != null)
			{
				gameObject.SendMessage(method, message);
			}
		}

		public static bool BoolDispatch(string name, string method, string message)
		{
			GameObject gameObject = GameObject.Find(name);
			if (gameObject != null)
			{
				LCDWebView component = gameObject.GetComponent<LCDWebView>();
				return (bool)component.GetType().GetMethod(method).Invoke(component, new string[1] { message });
			}
			return false;
		}
	}
}

using System;
using UnityEngine;

namespace Mobage
{
	public class NavigationViewController
	{
		public void PushViewController(UIViewController viewController, bool animated, Action block)
		{
			Debug.Log("PushViewController");
			block();
		}
	}
}

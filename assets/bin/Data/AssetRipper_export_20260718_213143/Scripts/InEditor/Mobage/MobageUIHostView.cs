using System;
using UnityEngine;

namespace Mobage
{
	public class MobageUIHostView
	{
		public void AnimateContentIn(Action completion)
		{
			Debug.Log("in");
			completion();
		}
	}
}

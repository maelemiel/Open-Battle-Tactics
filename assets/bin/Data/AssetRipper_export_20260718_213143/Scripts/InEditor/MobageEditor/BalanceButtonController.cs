using System;
using UnityEngine;

namespace MobageEditor
{
	public class BalanceButtonController
	{
		private static readonly BalanceButtonController instance = new BalanceButtonController();

		private MobageBalanceButton button;

		public static BalanceButtonController Instance
		{
			get
			{
				return instance;
			}
		}

		private BalanceButtonController()
		{
		}

		public void ShowButton(int x, int y, int width, int height, Action<SimpleAPIStatus, Error> onComplete)
		{
			if (button == null)
			{
				button = new GameObject().AddComponent<MobageBalanceButton>();
			}
			button.Frame = new Rect(x, y, width, height);
			button.Click = onComplete;
			button.ErrorCallback = delegate(Error obj)
			{
				onComplete(SimpleAPIStatus.Error, obj);
			};
			button.UpdateBalance();
		}

		public void HideButton()
		{
			if (button != null)
			{
				UnityEngine.Object.Destroy(button.gameObject);
			}
		}
	}
}

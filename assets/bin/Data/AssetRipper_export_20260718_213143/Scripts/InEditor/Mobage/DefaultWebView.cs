using System;
using UnityEngine;

namespace Mobage
{
	public class DefaultWebView : MonoBehaviour, IWebView
	{
		public event EventHandler WebViewDidFinishLoad;

		public void LoadURL(string url)
		{
		}

		public string EvaluateJS(string js)
		{
			return string.Empty;
		}

		public void SetVisibility(bool v)
		{
		}

		public void Init(int x, int y, int width, int height, Action<string> cb = null)
		{
		}
	}
}

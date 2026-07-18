using System;

namespace Mobage
{
	public interface IWebView
	{
		event EventHandler WebViewDidFinishLoad;

		void LoadURL(string url);

		string EvaluateJS(string js);

		void SetVisibility(bool v);

		void Init(int x, int y, int width, int height, Action<string> cb = null);
	}
}

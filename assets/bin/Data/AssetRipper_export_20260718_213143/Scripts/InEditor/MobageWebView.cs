using System;
using System.Collections;
using System.Runtime.InteropServices;
using Mobage;
using MobageEditor;
using UnityEngine;

public class MobageWebView : DefaultWebView, IWebView
{
	private Action<string> callback;

	private IntPtr webView;

	private bool visibility;

	private Rect rect;

	private Texture2D texture;

	private string inputString;

	private bool delay = true;

	public new event EventHandler WebViewDidFinishLoad;

	[DllImport("MobageWebView")]
	private static extern IntPtr MobageWebViewInit(string gameObject, int width, int height, bool ineditor);

	[DllImport("MobageWebView")]
	private static extern int MobageWebViewDestroy(IntPtr instance);

	[DllImport("MobageWebView")]
	private static extern void MobageWebViewSetRect(IntPtr instance, int width, int height);

	[DllImport("MobageWebView")]
	private static extern void MobageWebViewSetVisibility(IntPtr instance, bool visibility);

	[DllImport("MobageWebView")]
	private static extern void MobageWebViewLoadURL(IntPtr instance, string url);

	[DllImport("MobageWebView")]
	private static extern IntPtr MobageWebViewEvaluateJS(IntPtr instance, string url);

	[DllImport("MobageWebView")]
	private static extern void MobageWebViewUpdate(IntPtr instance, int x, int y, float deltaY, bool down, bool press, bool release, bool keyPress, short keyCode, string keyChars, int textureId);

	private void CreateTexture(int x, int y, int width, int height)
	{
		int num = 1;
		int num2 = 1;
		while (num < width)
		{
			num <<= 1;
		}
		while (num2 < height)
		{
			num2 <<= 1;
		}
		rect = new Rect(x, y, width, height);
		texture = new Texture2D(num, num2, TextureFormat.ARGB32, false);
	}

	public new void Init(int x, int y, int width, int height, Action<string> cb = null)
	{
		callback = cb;
		CreateTexture(x, y, width, height);
		base.name += GetInstanceID();
		webView = MobageWebViewInit(base.name, width, height, true);
	}

	public void OnDestroy()
	{
		if (!(webView == IntPtr.Zero))
		{
			MobageWebViewDestroy(webView);
		}
	}

	public void SetMargins(int left, int top, int right, int bottom)
	{
		if (!(webView == IntPtr.Zero))
		{
			int width = Screen.width - (left + right);
			int height = Screen.height - (bottom + top);
			CreateTexture(left, bottom, width, height);
			MobageWebViewSetRect(webView, width, height);
		}
	}

	public new void SetVisibility(bool v)
	{
		if (!(webView == IntPtr.Zero))
		{
			visibility = v;
			MobageWebViewSetVisibility(webView, v);
		}
	}

	public void DidFinishLoad(string msg)
	{
		if (this.WebViewDidFinishLoad != null)
		{
			this.WebViewDidFinishLoad(this, EventArgs.Empty);
		}
		delay = true;
	}

	public new void LoadURL(string url)
	{
		if (!(webView == IntPtr.Zero))
		{
			MobageWebViewLoadURL(webView, url);
			delay = true;
		}
	}

	public new string EvaluateJS(string js)
	{
		if (webView == IntPtr.Zero)
		{
			return string.Empty;
		}
		IntPtr ptr = MobageWebViewEvaluateJS(webView, js);
		string text = Marshal.PtrToStringAuto(ptr);
		if (!string.IsNullOrEmpty(text) && text != "[]")
		{
			Debug.Log(js + ':' + text);
		}
		delay = false;
		return text;
	}

	public void CallFromJS(string message)
	{
		if (callback != null)
		{
			callback(message);
		}
	}

	private void Update()
	{
		inputString += Input.inputString;
	}

	private void OnGUI()
	{
		GUI.depth = -1;
		if (!(webView == IntPtr.Zero) && visibility)
		{
			Vector3 mousePosition = Input.mousePosition;
			if (Input.GetMouseButtonDown(0) && mousePosition.y > rect.height)
			{
				UnityEngine.Object.Destroy(base.transform.parent.transform.parent.gameObject);
			}
			bool button = Input.GetButton("Fire1");
			bool buttonDown = Input.GetButtonDown("Fire1");
			bool buttonUp = Input.GetButtonUp("Fire1");
			float axis = Input.GetAxis("Mouse ScrollWheel");
			bool keyPress = false;
			string keyChars = string.Empty;
			short keyCode = 0;
			if (!string.IsNullOrEmpty(inputString))
			{
				keyPress = true;
				keyChars = inputString.Substring(0, 1);
				keyCode = (short)inputString[0];
				inputString = inputString.Substring(1);
			}
			if (delay)
			{
				StartCoroutine("delayFrame");
			}
			MobageWebViewUpdate(webView, (int)(mousePosition.x - rect.x), (int)(mousePosition.y - rect.y), axis, button, buttonDown, buttonUp, keyPress, keyCode, keyChars, texture.GetNativeTextureID());
			if (GUI.Button(new Rect(-20f, -20f, Screen.width + 40, Screen.height + 40), string.Empty))
			{
				Event.current.Use();
			}
			GL.IssuePluginEvent((int)webView);
			Matrix4x4 matrix = GUI.matrix;
			GUI.matrix = Matrix4x4.TRS(new Vector3(0f, Screen.height, 0f), Quaternion.identity, new Vector3(1f, -1f, 1f));
			GUI.DrawTexture(rect, texture);
			GUI.matrix = matrix;
		}
	}

	private IEnumerator delayFrame()
	{
		yield return new WaitForSeconds(10f);
	}

	public void EvaluateJSResult(string result)
	{
		Debug.Log("EvaluateJSResult " + result);
	}

	public bool HandleRequest(string message)
	{
		return MobageWeb.Instance.HandleRequest(message, delegate(string resp)
		{
			EvaluateJS(resp);
		});
	}
}

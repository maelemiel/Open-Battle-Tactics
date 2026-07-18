using System;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using LCD.Internal.Util;
using UnityEngine;

namespace LCD.Internal.Web
{
	[ExecuteInEditMode]
	internal class LCDWebView : MonoBehaviour
	{
		private static string TAG = "LCDWebView";

		private Action<string> callback;

		private IntPtr webView;

		private bool visibility;

		private Rect rect;

		private string inputString;

		private bool delay = true;

		private bool display;

		public bool Display
		{
			get
			{
				return display;
			}
			set
			{
				display = value;
			}
		}

		public event EventHandler WebViewDidFinishLoad;

		[DllImport("LCDWebView")]
		private static extern IntPtr LCDWebViewInit(string gameObject, float x, float y, int width, int height, bool ineditor, bool display);

		[DllImport("LCDWebView")]
		private static extern int LCDWebViewDestroy(IntPtr instance);

		[DllImport("LCDWebView")]
		private static extern void LCDWebViewSetRect(IntPtr instance, float x, float y, int width, int height);

		[DllImport("LCDWebView")]
		private static extern void LCDWebViewSetVisibility(IntPtr instance, bool visibility);

		[DllImport("LCDWebView")]
		private static extern void LCDWebViewLoadURL(IntPtr instance, string url, string headers);

		[DllImport("LCDWebView")]
		private static extern IntPtr LCDWebViewEvaluateJS(IntPtr instance, string url);

		[DllImport("LCDWebView")]
		private static extern void LCDWebViewUpdate(IntPtr instance, float viewX, float viewY, int x, int y, float deltaY, bool down, bool press, bool release, bool keyPress, short keyCode, string keyChars, int textureId, bool display);

		private bool IsOSXEditor()
		{
			bool result = true;
			if (Application.platform == RuntimePlatform.WindowsEditor)
			{
				result = false;
			}
			return result;
		}

		private void CreateTexture(int x, int y, int width, int height)
		{
			rect = new Rect(x, y, width, height);
		}

		public void Init(int x, int y, int width, int height, bool displayWeb, Action<string> cb)
		{
			callback = cb;
			Rect mainGameView = GetMainGameView();
			CreateTexture(x, y, width, height);
			if (IsOSXEditor())
			{
				webView = LCDWebViewInit(base.name, mainGameView.x, mainGameView.y, width, height, true, display);
			}
		}

		public void OnDestroy()
		{
			if (!(webView == IntPtr.Zero))
			{
				if (IsOSXEditor())
				{
					LCDWebViewDestroy(webView);
				}
				webView = IntPtr.Zero;
				GameObject gameObject = GameObject.Find("LCDWebView");
				if (gameObject != null)
				{
					UnityEngine.Object.DestroyImmediate(gameObject);
				}
			}
		}

		public static Rect GetMainGameView()
		{
			Type type = Type.GetType("LCDWebViewEditor.LCDWebViewEditor,Assembly-CSharp-Editor");
			MethodInfo method = type.GetMethod("GetMainGameRect", BindingFlags.Static | BindingFlags.Public);
			object obj = method.Invoke(null, null);
			return (Rect)obj;
		}

		public void SetMargins(int left, int top, int right, int bottom)
		{
			if (!(webView == IntPtr.Zero))
			{
				int width = Screen.width - (left + right);
				int height = Screen.height - (bottom + top);
				LCDSDKLog.Debug(TAG, "Position: left=" + left + " top= " + top + " right= " + right + " bottom= " + bottom);
				CreateTexture(left, bottom, width, height);
				Rect mainGameView = GetMainGameView();
				if (IsOSXEditor())
				{
					LCDWebViewSetRect(webView, mainGameView.x, mainGameView.y, width, height);
				}
			}
		}

		public void SetVisibility(bool v)
		{
			if (!(webView == IntPtr.Zero))
			{
				visibility = v;
				if (IsOSXEditor())
				{
					LCDWebViewSetVisibility(webView, v);
				}
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

		public void LoadURL(string url, string headers)
		{
			if (!(webView == IntPtr.Zero))
			{
				if (IsOSXEditor())
				{
					LCDWebViewLoadURL(webView, url, headers);
				}
				delay = true;
			}
		}

		public string EvaluateJS(string js)
		{
			if (webView == IntPtr.Zero)
			{
				return string.Empty;
			}
			string text = string.Empty;
			if (IsOSXEditor())
			{
				IntPtr ptr = LCDWebViewEvaluateJS(webView, js);
				text = Marshal.PtrToStringAuto(ptr);
				if (!string.IsNullOrEmpty(text) && text != "[]")
				{
					LCDSDKLog.Debug(TAG, js + ':' + text);
				}
				delay = false;
			}
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
				Rect mainGameView = GetMainGameView();
				if (webView != IntPtr.Zero && IsOSXEditor())
				{
					LCDWebViewUpdate(webView, mainGameView.x, mainGameView.y, (int)(mousePosition.x - rect.x), (int)(mousePosition.y - rect.y), axis, button, buttonDown, buttonUp, keyPress, keyCode, keyChars, GetInstanceID(), display);
				}
			}
		}

		private IEnumerator delayFrame()
		{
			yield return new WaitForSeconds(10f);
		}

		public void EvaluateJSResult(string result)
		{
			LCDSDKLog.Debug(TAG, "EvaluateJSResult " + result);
		}

		public static string HandleRequestWin(string message)
		{
			string response = string.Empty;
			bool flag = false;
			try
			{
				ManualResetEvent mre = new ManualResetEvent(false);
				flag = LCDWeb.Instance.HandleRequest(message, delegate(string resp)
				{
					response = resp;
					mre.Set();
				});
				mre.WaitOne();
				return flag.ToString().ToLower() + "|" + response;
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
				return flag.ToString().ToLower();
			}
		}

		public static bool HandleRequest(string message)
		{
			LCDSDKLog.Debug(TAG, "HandleRequest: " + message);
			return LCDWeb.Instance.HandleRequest(message, delegate(string resp)
			{
				LCDWeb.Instance.WebViewObject.EvaluateJS(resp);
			});
		}
	}
}

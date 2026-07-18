using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using UnityEngine;

namespace MobageEditor
{
	public class MobageWeb
	{
		public enum Action
		{
			LOGIN = 0,
			LOGOUT = 1,
			USER_UPGRADE = 2,
			BANK = 3,
			BANK_PURCHASE = 4,
			DEBIT = 5,
			PROFILE = 6,
			COMMUNITY = 7,
			PROMOTION = 8,
			CHECK_FACEBOOK = 9
		}

		private static readonly MobageWeb instance = new MobageWeb();

		public MobageWebView WebViewObject { get; set; }

		public Action<DismissableAPIStatus, Error, Dictionary<string, string>> CloseCallback { get; set; }

		public static MobageWeb Instance
		{
			get
			{
				return instance;
			}
		}

		public string WebViewHost
		{
			get
			{
				return "sandbox.web.mobage.com";
			}
		}

		private MobageWeb()
		{
		}

		public static string encodeDictionary(Dictionary<string, string> parameters)
		{
			string text = string.Empty;
			int num = 0;
			foreach (KeyValuePair<string, string> parameter in parameters)
			{
				if (num > 0)
				{
					text += "&";
				}
				if (parameter.Key != null)
				{
					string empty = string.Empty;
					empty = ((parameter.Value == null) ? (OAuth.URLEncodeParameter(parameter.Key) + "=" + OAuth.URLEncodeParameter(string.Empty)) : (OAuth.URLEncodeParameter(parameter.Key) + "=" + OAuth.URLEncodeParameter(parameter.Value)));
					text += empty;
					num++;
				}
				else
				{
					Debug.LogWarning("Dictionary has a null value for key or value please fix");
				}
			}
			return text;
		}

		public static void OpenMobageWeb(Action action, Dictionary<string, string> parameters, Action<DismissableAPIStatus, Error, Dictionary<string, string>> closeCallback)
		{
			MobageWeb mobageWeb = Instance;
			ServicePointManager.ServerCertificateValidationCallback = (RemoteCertificateValidationCallback)Delegate.Combine(ServicePointManager.ServerCertificateValidationCallback, (RemoteCertificateValidationCallback)((object s, X509Certificate ce, X509Chain ca, SslPolicyErrors p) => true));
			Mobage sharedInstance = Mobage.sharedInstance;
			string text = OAuth.URLEncodeParameter(sharedInstance.AppId);
			string text2 = OAuth.URLEncodeParameter(sharedInstance.SDKVersion);
			string text3 = OAuth.URLEncodeParameter(UIDevice.Instance.SystemVersion);
			string text4 = OAuth.URLEncodeParameter(UIDevice.Instance.Model);
			string empty = string.Empty;
			string text5 = string.Format("https://{0}/{1}/{2}/{3}/{4}/{5}?locale={6}", mobageWeb.WebViewHost, action.GetValue(), text, text2, text3, text4, empty);
			if (parameters != null && parameters.Count > 0)
			{
				text5 = text5 + "&" + encodeDictionary(parameters);
			}
			HttpWebRequest httpWebRequest = (HttpWebRequest)System.Net.WebRequest.Create(text5);
			httpWebRequest.UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 6_1_4 like Mac OS X) AppleWebKit/536.26 (KHTML, like Gecko) Mobile/10B350";
			WebResponse response = httpWebRequest.GetResponse();
			Mobage.MobageUIVisiblePost(true);
			using (Stream stream = response.GetResponseStream())
			{
				using (StreamReader streamReader = new StreamReader(stream, Encoding.UTF8))
				{
					string text6 = streamReader.ReadToEnd();
					WebHeaderCollection headers = response.Headers;
					mobageWeb.WebViewObject = new GameObject().AddComponent<MobageWebView>();
					mobageWeb.CloseCallback = closeCallback;
					mobageWeb.WebViewObject.Init(0, 0, Screen.width, Screen.height);
					mobageWeb.WebViewObject.LoadURL(response.ResponseUri.ToString());
					mobageWeb.WebViewObject.SetVisibility(true);
				}
			}
		}

		public static void showPromotions(List<string> keys, List<string> values, Action<DismissableAPIStatus, Error> completeCb)
		{
			if ((keys != null || values != null) && (keys == null || values == null || keys.Count != values.Count))
			{
				return;
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			if (keys != null && values != null)
			{
				for (int i = 0; i < keys.Count; i++)
				{
					dictionary[keys[i]] = values[i];
				}
			}
			OpenMobageWeb(Action.PROMOTION, dictionary, delegate(DismissableAPIStatus status, Error error, Dictionary<string, string> result)
			{
				completeCb(status, error);
			});
		}

		public static void executeUserUpgradeWithParams(List<string> keys, List<string> values, Action<DismissableAPIStatus, Error> completeCb)
		{
			if ((keys != null || values != null) && (keys == null || values == null || keys.Count != values.Count))
			{
				return;
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			if (keys != null && values != null)
			{
				for (int i = 0; i < keys.Count; i++)
				{
					dictionary[keys[i]] = values[i];
				}
			}
			OpenMobageWeb(Action.USER_UPGRADE, dictionary, delegate(DismissableAPIStatus status, Error error, Dictionary<string, string> result)
			{
				completeCb(status, error);
			});
		}

		public static void executeUserUpgrade(Action<CancelableAPIStatus, Error> completeCb)
		{
			OpenMobageWeb(Action.USER_UPGRADE, null, delegate(DismissableAPIStatus status, Error error, Dictionary<string, string> result)
			{
				CancelableAPIStatus arg;
				switch (status)
				{
				case DismissableAPIStatus.Success:
					arg = CancelableAPIStatus.Success;
					break;
				case DismissableAPIStatus.Dismiss:
					arg = CancelableAPIStatus.Cancel;
					break;
				default:
					arg = CancelableAPIStatus.Error;
					break;
				}
				completeCb(arg, error);
			});
		}

		public static void executeLogout(Action<SimpleAPIStatus, Error> completeCb)
		{
			OpenMobageWeb(Action.LOGOUT, null, delegate(DismissableAPIStatus status, Error error, Dictionary<string, string> result)
			{
				switch (status)
				{
				case DismissableAPIStatus.Success:
				case DismissableAPIStatus.Dismiss:
					completeCb(SimpleAPIStatus.Success, error);
					break;
				default:
					completeCb(SimpleAPIStatus.Error, error);
					break;
				}
			});
		}

		public static void openCurrentUserProfile()
		{
			string userId = NativeAPI.Instance.UserId;
			OpenMobageWeb(Action.PROFILE, new Dictionary<string, string> { { "userId", userId } }, delegate
			{
				Debug.Log("openCurrentUserProfile");
			});
		}

		public static void openUserProfile(User user)
		{
			OpenMobageWeb(Action.PROFILE, new Dictionary<string, string> { { "userId", user.uid } }, delegate(DismissableAPIStatus status, Error error, Dictionary<string, string> result)
			{
				Debug.Log(string.Format("openUserProfile: error={0} result={1}", error, result));
			});
		}

		public static void showCommunityUI()
		{
			OpenMobageWeb(Action.COMMUNITY, null, delegate
			{
				Debug.Log("showCommunityUI");
			});
		}

		public static void openBankDialog()
		{
			OpenMobageWeb(Action.BANK, null, delegate(DismissableAPIStatus status, Error error, Dictionary<string, string> result)
			{
				Debug.Log(string.Concat("OpenBankDialog error=", error, " result=", result));
			});
		}

		public static void purchaseASCItem(ItemData item, Action<CancelableAPIStatus, Error> completeCb)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			if (item != null)
			{
				dictionary["sku"] = item.itemId;
				dictionary["value"] = item.price.ToString("d");
				dictionary["price"] = item.originPrice.ToString("f2");
				dictionary["currency"] = item.originCurrencyLabel;
				Debug.LogWarning(dictionary["sku"] + " Value: " + dictionary["value"] + " Price: " + dictionary["price"] + " Currency: " + dictionary["currency"]);
				OpenMobageWeb(Action.BANK_PURCHASE, dictionary, delegate(DismissableAPIStatus status, Error arg2, Dictionary<string, string> result)
				{
					CancelableAPIStatus arg;
					switch (status)
					{
					case DismissableAPIStatus.Success:
						arg = CancelableAPIStatus.Success;
						break;
					case DismissableAPIStatus.Dismiss:
						arg = CancelableAPIStatus.Cancel;
						break;
					default:
						arg = CancelableAPIStatus.Error;
						break;
					}
					completeCb(arg, arg2);
				});
			}
			else
			{
				Error error = new Error();
				error.code = 10003;
				error.localizedDescription = "Invalid Params";
				completeCb(CancelableAPIStatus.Error, error);
			}
		}

		public static void executeLoginWithParams(List<string> keys, List<string> values, Action<DismissableAPIStatus, Error> completeCb)
		{
			if ((keys == null && values == null) || (keys != null && values != null && keys.Count == values.Count))
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				if (keys != null && values != null)
				{
					for (int i = 0; i < keys.Count; i++)
					{
						dictionary[keys[i]] = values[i];
					}
				}
				OpenMobageWeb(Action.LOGIN, dictionary, delegate(DismissableAPIStatus status, Error arg, Dictionary<string, string> result)
				{
					completeCb(status, arg);
				});
			}
			else
			{
				Error error = new Error();
				error.code = 10003;
				error.localizedDescription = "Invalid Params";
				completeCb(DismissableAPIStatus.Error, error);
			}
		}

		public static void checkFacebookStatus(Action<SimpleAPIStatus, Error, string, string, string> onComplete)
		{
			Dictionary<string, string> parameters = new Dictionary<string, string>();
			OpenMobageWeb(Action.CHECK_FACEBOOK, parameters, delegate(DismissableAPIStatus status, Error error, Dictionary<string, string> result)
			{
				switch (status)
				{
				case DismissableAPIStatus.Success:
					onComplete(SimpleAPIStatus.Success, error, result["userId"], result["facebookId"], result["accessToken"]);
					break;
				case DismissableAPIStatus.Dismiss:
					onComplete(SimpleAPIStatus.Error, null, null, null, null);
					break;
				default:
					onComplete(SimpleAPIStatus.Error, error, null, null, null);
					break;
				}
			});
		}

		public static void executeLogin(Action<DismissableAPIStatus, Error> onComplete)
		{
			executeLoginWithParams(null, null, onComplete);
		}

		public bool HandleRequest(string url, Action<string> callback)
		{
			bool result = false;
			Uri uri = new Uri(url);
			if (uri.Scheme == "ngcore")
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				if (!string.IsNullOrEmpty(uri.Query))
				{
					string[] array = uri.Query.TrimStart('?').Split('&');
					foreach (string text in array)
					{
						string[] array2 = text.Split('=');
						dictionary[array2[0]] = array2[1];
					}
				}
				result = true;
				string requestId = uri.AbsolutePath.TrimStart('/');
				Action<JsonData> respondToWeb = delegate(JsonData resp)
				{
					string arg = resp[1].ToJson();
					string js = string.Format("javascript:respondToWeb('{0}', '{1}', {2})", requestId, resp[0], arg);
					MobageWebView webViewObject = WebViewObject;
					webViewObject.EvaluateJS(js);
				};
				if (uri.Host == "getcapabilities")
				{
					NativeAPI.Instance.GetCapabilities(delegate(JsonData resp)
					{
						respondToWeb(resp);
					});
				}
				else if (uri.Host == "launchapp")
				{
					NativeAPI.LaunchApp(dictionary["url"], respondToWeb);
				}
				else if (uri.Host == "canlaunchapp")
				{
					NativeAPI.CanLaunchApp(dictionary["url"], respondToWeb);
				}
				else if (uri.Host == "getFacebookUser")
				{
					NativeAPI.GetFacebookUser(dictionary, respondToWeb);
				}
				else if (uri.Host == "closeFacebookSession")
				{
					NativeAPI.CloseFacebookSession(respondToWeb);
				}
				else if (uri.Host == "saveauthcredential")
				{
					NativeAPI.Instance.SaveAuthCredential(dictionary["userId"], dictionary["authToken"], dictionary["oauthToken"], dictionary["oauthSecret"], dictionary["oauth2Token"], dictionary["cookie"], dictionary["facebookId"], dictionary["userNickname"], dictionary["guestNickname"], dictionary["guestPassword"], delegate(JsonData resp)
					{
						respondToWeb(resp);
					});
				}
				else if (uri.Host == "sendnotification")
				{
					NativeAPI.Instance.SendNotification(dictionary["type"], dictionary["value"], delegate(JsonData resp)
					{
						respondToWeb(resp);
					});
				}
				else if (uri.Host == "purchasecoin")
				{
					NativeAPI.PurchaseCoin(dictionary["sku"], dictionary["price"], dictionary["value"], dictionary["currency"], delegate(JsonData resp)
					{
						respondToWeb(resp);
					});
				}
				else if (uri.Host == "takepicture")
				{
					NativeAPI.TakePicture(respondToWeb);
				}
				else if (uri.Host == "openimagepicker")
				{
					NativeAPI.TakePicture(respondToWeb);
				}
				else if (uri.Host == "closewebview")
				{
					CloseWebView(dictionary);
				}
			}
			return result;
		}

		public void CloseWebView(Dictionary<string, string> parameters)
		{
			UnityEngine.Object.Destroy(WebViewObject.gameObject);
			Mobage.MobageUIVisiblePost(false);
			string value;
			if (parameters.TryGetValue("status", out value))
			{
				switch (value)
				{
				case "success":
					if (CloseCallback != null)
					{
						CloseCallback(DismissableAPIStatus.Success, null, parameters);
					}
					break;
				case "error":
					if (CloseCallback != null)
					{
						CloseCallback(DismissableAPIStatus.Error, new Error
						{
							localizedDescription = "Unknown error code",
							domain = "com.mobage.error.api",
							code = 10005
						}, parameters);
					}
					break;
				case "dismiss":
					if (CloseCallback != null)
					{
						CloseCallback(DismissableAPIStatus.Dismiss, null, parameters);
					}
					break;
				default:
					Debug.LogWarning("CloseWebView apiStatus unknown: " + value);
					break;
				}
			}
			else
			{
				Debug.LogWarning("CloseWebView could not get parameters?");
			}
		}
	}
}

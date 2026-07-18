using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace MobageEditor
{
	public class OAuth
	{
		public string ConsumerKey;

		public string ConsumerSecret;

		public string AccessSecret;

		public string ContentType;

		private static System.Random random = new System.Random();

		public string AccessToken { get; set; }

		public static string Nonce
		{
			get
			{
				string str = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss +0000") + random.Next() + random.Next();
				return Digest.Md5Hash(str);
			}
		}

		public OAuth(string key, string secret)
		{
			ConsumerKey = key;
			ConsumerSecret = secret;
		}

		public void AddSignature(HttpWebRequest request, Dictionary<string, object> POSTFields, Dictionary<string, object> queryParams)
		{
			string value = Header(request.Address, request.Method, POSTFields, queryParams);
			request.Headers["Authorization"] = value;
		}

		public string Header(Uri url, string method, Dictionary<string, object> POSTFields, Dictionary<string, object> queryParams)
		{
			DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
			DateTime now = DateTime.Now;
			int num = (int)(now - dateTime).TotalSeconds;
			SortedDictionary<string, object> sortedDictionary = new SortedDictionary<string, object>();
			sortedDictionary.Add("oauth_nonce", Nonce);
			sortedDictionary.Add("oauth_timestamp", num.ToString());
			sortedDictionary.Add("oauth_signature_method", "HMAC-SHA1");
			sortedDictionary.Add("oauth_version", "1.0");
			SortedDictionary<string, object> sortedDictionary2 = sortedDictionary;
			if (!string.IsNullOrEmpty(ConsumerKey))
			{
				sortedDictionary2["oauth_consumer_key"] = ConsumerKey;
			}
			if (!string.IsNullOrEmpty(AccessToken))
			{
				sortedDictionary2["oauth_token"] = AccessToken;
			}
			List<KeyValuePair<string, object>> list = new List<KeyValuePair<string, object>>(sortedDictionary2);
			if (ContentType == "application/x-www-form-urlencoded" && POSTFields != null)
			{
				list.AddRange(POSTFields);
			}
			if (queryParams != null)
			{
				list.AddRange(queryParams);
			}
			IEnumerable<string> source = from x in list
				orderby x.Key + " " + x.Value
				select WWW.EscapeURL(x.Key) + "=" + URLEncodeParameter((x.Value != null) ? x.Value.ToString() : string.Empty);
			string leftPart = url.GetLeftPart(UriPartial.Path);
			string text = URLEncodeParameter(method);
			string text2 = URLEncodeParameter(leftPart);
			string text3 = URLEncodeParameter(string.Join("&", source.ToArray()));
			string text4 = text + "&" + text2 + "&" + text3;
			string text5 = OAuthEncodeParameter(ConsumerSecret) + "&";
			if (!string.IsNullOrEmpty(AccessSecret))
			{
				text5 += OAuthEncodeParameter(AccessSecret);
			}
			sortedDictionary2["oauth_signature"] = URLEncodeParameter(SHA1WithKey(text5, text4));
			string result = "OAuth " + string.Join(",", sortedDictionary2.Select((KeyValuePair<string, object> x) => string.Concat(x.Key, "=\"", x.Value, "\"")).ToArray());
			MobageLogger.log("DebugOAuth: BaseString: " + text4);
			MobageLogger.log("DebugOAuth: Signing Key: " + text5);
			return result;
		}

		public static string SHA1WithKey(string key, string dataString)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(dataString);
			byte[] bytes2 = Encoding.UTF8.GetBytes(key);
			HMACSHA1 hMACSHA = new HMACSHA1(bytes2);
			byte[] inArray = hMACSHA.ComputeHash(bytes);
			return System.Convert.ToBase64String(inArray);
		}

		public static string URLEncodeParameter(string str)
		{
			Regex regex = new Regex("%[a-f0-9]{2}");
			return regex.Replace(WWW.EscapeURL(str).Replace("+", "%20").Replace("(", "%28")
				.Replace(")", "%29"), (Match m) => m.Value.ToUpperInvariant());
		}

		public static string OAuthEncodeParameter(string str)
		{
			return URLEncodeParameter(str);
		}
	}
}

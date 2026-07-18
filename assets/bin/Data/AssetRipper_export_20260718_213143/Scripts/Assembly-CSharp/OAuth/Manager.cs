using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace OAuth
{
	public static class Manager
	{
		public enum SignatureTypes
		{
			HMACSHA1 = 0,
			PLAINTEXT = 1,
			RSASHA1 = 2
		}

		private class QueryParameter
		{
			private string name;

			private string value;

			public string Name
			{
				get
				{
					return name;
				}
			}

			public string Value
			{
				get
				{
					return value;
				}
			}

			public QueryParameter(string name, string value)
			{
				this.name = name;
				this.value = value;
			}
		}

		private class QueryParameterComparer : IComparer<QueryParameter>
		{
			public int Compare(QueryParameter x, QueryParameter y)
			{
				if (x.Name == y.Name)
				{
					return string.Compare(x.Value, y.Value);
				}
				return string.Compare(x.Name, y.Name);
			}
		}

		private const string OAuthVersion = "1.0";

		private const string OAuthParameterPrefix = "oauth_";

		private const string OAuthConsumerKeyKey = "oauth_consumer_key";

		private const string OAuthCallbackKey = "oauth_callback";

		private const string OAuthVersionKey = "oauth_version";

		private const string OAuthSignatureMethodKey = "oauth_signature_method";

		private const string OAuthSignatureKey = "oauth_signature";

		private const string OAuthTimestampKey = "oauth_timestamp";

		private const string OAuthNonceKey = "oauth_nonce";

		private const string OAuthTokenKey = "oauth_token";

		private const string OAuthTokenSecretKey = "oauth_token_secret";

		private const string HMACSHA1SignatureType = "HMAC-SHA1";

		private const string PlainTextSignatureType = "PLAINTEXT";

		private const string RSASHA1SignatureType = "RSA-SHA1";

		private const string unreservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";

		private static Random random = new Random();

		private static string ComputeHash(HashAlgorithm hashAlgorithm, string data)
		{
			if (hashAlgorithm == null)
			{
				throw new ArgumentNullException("hashAlgorithm");
			}
			if (string.IsNullOrEmpty(data))
			{
				throw new ArgumentNullException("data");
			}
			byte[] bytes = Encoding.ASCII.GetBytes(data);
			byte[] inArray = hashAlgorithm.ComputeHash(bytes);
			return Convert.ToBase64String(inArray);
		}

		private static List<QueryParameter> GetQueryParameters(string parameters)
		{
			if (parameters.StartsWith("?"))
			{
				parameters = parameters.Remove(0, 1);
			}
			List<QueryParameter> list = new List<QueryParameter>();
			if (!string.IsNullOrEmpty(parameters))
			{
				string[] array = parameters.Split('&');
				string[] array2 = array;
				foreach (string text in array2)
				{
					if (!string.IsNullOrEmpty(text))
					{
						if (text.IndexOf('=') > -1)
						{
							string[] array3 = text.Split('=');
							list.Add(new QueryParameter(array3[0], array3[1]));
						}
						else
						{
							list.Add(new QueryParameter(text, string.Empty));
						}
					}
				}
			}
			return list;
		}

		public static string UrlEncode(string value)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (char c in value)
			{
				if ("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~".IndexOf(c) != -1)
				{
					stringBuilder.Append(c);
				}
				else
				{
					stringBuilder.Append('%' + string.Format("{0:X2}", (int)c));
				}
			}
			return stringBuilder.ToString();
		}

		private static string NormalizeRequestParameters(IList<QueryParameter> parameters, bool forHeader)
		{
			StringBuilder stringBuilder = new StringBuilder();
			QueryParameter queryParameter = null;
			for (int i = 0; i < parameters.Count; i++)
			{
				queryParameter = parameters[i];
				if (forHeader)
				{
					stringBuilder.AppendFormat("{0}=\"{1}\"", UrlEncode(queryParameter.Name), UrlEncode(queryParameter.Value));
				}
				else
				{
					stringBuilder.AppendFormat("{0}={1}", queryParameter.Name, queryParameter.Value);
				}
				if (i < parameters.Count - 1)
				{
					if (forHeader)
					{
						stringBuilder.Append(",");
					}
					else
					{
						stringBuilder.Append("&");
					}
				}
			}
			return stringBuilder.ToString();
		}

		public static string GenerateSignatureBase(Uri url, string consumerKey, string token, string tokenSecret, string httpMethod, string timeStamp, string nonce, string signatureType)
		{
			if (token == null)
			{
				token = string.Empty;
			}
			if (tokenSecret == null)
			{
				tokenSecret = string.Empty;
			}
			if (string.IsNullOrEmpty(consumerKey))
			{
				throw new ArgumentNullException("consumerKey");
			}
			if (string.IsNullOrEmpty(httpMethod))
			{
				throw new ArgumentNullException("httpMethod");
			}
			if (string.IsNullOrEmpty(signatureType))
			{
				throw new ArgumentNullException("signatureType");
			}
			string text = null;
			string text2 = null;
			List<QueryParameter> queryParameters = GetQueryParameters(url.Query);
			queryParameters.Add(new QueryParameter("oauth_version", "1.0"));
			queryParameters.Add(new QueryParameter("oauth_nonce", nonce));
			queryParameters.Add(new QueryParameter("oauth_timestamp", timeStamp));
			queryParameters.Add(new QueryParameter("oauth_signature_method", signatureType));
			queryParameters.Add(new QueryParameter("oauth_consumer_key", consumerKey));
			if (!string.IsNullOrEmpty(token))
			{
				queryParameters.Add(new QueryParameter("oauth_token", token));
			}
			queryParameters.Sort(new QueryParameterComparer());
			text = string.Format("{0}://{1}", url.Scheme, url.Host);
			if ((!(url.Scheme == "http") || url.Port != 80) && (!(url.Scheme == "https") || url.Port != 443))
			{
				text = text + ":" + url.Port;
			}
			text += url.AbsolutePath;
			text2 = NormalizeRequestParameters(queryParameters, false);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("{0}&", httpMethod.ToUpper());
			stringBuilder.AppendFormat("{0}&", UrlEncode(text));
			stringBuilder.AppendFormat("{0}", UrlEncode(text2));
			return stringBuilder.ToString();
		}

		public static string GenerateAuthorizationHeader(Uri url, string consumerKey, string token, string timeStamp, string nonce, string signatureType, string signature)
		{
			List<QueryParameter> list = new List<QueryParameter>();
			list.Add(new QueryParameter("oauth_version", "1.0"));
			list.Add(new QueryParameter("oauth_nonce", nonce));
			list.Add(new QueryParameter("oauth_timestamp", timeStamp));
			list.Add(new QueryParameter("oauth_signature_method", signatureType));
			list.Add(new QueryParameter("oauth_signature", signature));
			list.Add(new QueryParameter("oauth_consumer_key", consumerKey));
			if (!string.IsNullOrEmpty(token))
			{
				list.Add(new QueryParameter("oauth_token", token));
			}
			list.Sort(new QueryParameterComparer());
			return "OAuth " + NormalizeRequestParameters(list, true);
		}

		public static string GenerateSignatureUsingHash(string signatureBase, HashAlgorithm hash)
		{
			return ComputeHash(hash, signatureBase);
		}

		public static string GenerateSignature(Uri url, string consumerKey, string consumerSecret, string token, string tokenSecret, string httpMethod, string timeStamp, string nonce)
		{
			return GenerateSignature(url, consumerKey, consumerSecret, token, tokenSecret, httpMethod, timeStamp, nonce, SignatureTypes.HMACSHA1);
		}

		public static string GenerateSignature(Uri url, string consumerKey, string consumerSecret, string token, string tokenSecret, string httpMethod, string timeStamp, string nonce, SignatureTypes signatureType)
		{
			switch (signatureType)
			{
			case SignatureTypes.PLAINTEXT:
				return UrlEncode(string.Format("{0}&{1}", consumerSecret, tokenSecret));
			case SignatureTypes.HMACSHA1:
			{
				string signatureBase = GenerateSignatureBase(url, consumerKey, token, tokenSecret, httpMethod, timeStamp, nonce, "HMAC-SHA1");
				HMACSHA1 hMACSHA = new HMACSHA1();
				hMACSHA.Key = Encoding.ASCII.GetBytes(string.Format("{0}&{1}", UrlEncode(consumerSecret), (!string.IsNullOrEmpty(tokenSecret)) ? UrlEncode(tokenSecret) : string.Empty));
				return GenerateSignatureUsingHash(signatureBase, hMACSHA);
			}
			case SignatureTypes.RSASHA1:
				throw new NotImplementedException();
			default:
				throw new ArgumentException("Unknown signature type", "signatureType");
			}
		}

		public static string GenerateTimeStamp()
		{
			return Convert.ToInt64((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds).ToString();
		}

		public static string GenerateNonce()
		{
			return random.Next(123400, 9999999).ToString();
		}
	}
}

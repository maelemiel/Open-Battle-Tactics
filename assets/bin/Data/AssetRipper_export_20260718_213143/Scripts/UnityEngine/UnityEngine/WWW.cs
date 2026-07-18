using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine.Internal;

namespace UnityEngine
{
	public sealed class WWW : IDisposable
	{
		internal IntPtr m_Ptr;

		public Dictionary<string, string> responseHeaders
		{
			get
			{
				if (!isDone)
				{
					throw new UnityException("WWW is not finished downloading yet");
				}
				return ParseHTTPHeaderString(responseHeadersString);
			}
		}

		private extern string responseHeadersString
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public string text
		{
			get
			{
				if (!isDone)
				{
					throw new UnityException("WWW is not ready downloading yet");
				}
				return GetTextEncoder().GetString(bytes, 0, bytes.Length);
			}
		}

		internal static Encoding DefaultEncoding
		{
			get
			{
				return Encoding.ASCII;
			}
		}

		[Obsolete("Please use WWW.text instead")]
		public string data
		{
			get
			{
				return text;
			}
		}

		public extern byte[] bytes
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern int size
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern string error
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public Texture2D texture
		{
			get
			{
				return GetTexture(false);
			}
		}

		public Texture2D textureNonReadable
		{
			get
			{
				return GetTexture(true);
			}
		}

		public AudioClip audioClip
		{
			get
			{
				return GetAudioClip(true);
			}
		}

		public extern bool isDone
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern float progress
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern float uploadProgress
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern int bytesDownloaded
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		[Obsolete(".oggVorbis accessor is deprecated, use .audioClip or GetAudioClip() instead.")]
		public extern AudioClip oggVorbis
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern string url
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern AssetBundle assetBundle
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern ThreadPriority threadPriority
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public WWW(string url)
		{
			InitWWW(url, null, null);
		}

		public WWW(string url, WWWForm form)
		{
			string[] iHeaders = FlattenedHeadersFrom(form.headers);
			InitWWW(url, form.data, iHeaders);
		}

		public WWW(string url, byte[] postData)
		{
			InitWWW(url, postData, null);
		}

		[Obsolete("This overload is deprecated. Use the one with Dictionary argument.")]
		public WWW(string url, byte[] postData, Hashtable headers)
		{
			string[] iHeaders = FlattenedHeadersFrom(headers);
			InitWWW(url, postData, iHeaders);
		}

		public WWW(string url, byte[] postData, Dictionary<string, string> headers)
		{
			string[] iHeaders = FlattenedHeadersFrom(headers);
			InitWWW(url, postData, iHeaders);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		internal extern WWW(string url, int version, uint crc);

		private static string[] FlattenedHeadersFrom(Hashtable headers)
		{
			if (headers == null)
			{
				return null;
			}
			string[] array = new string[headers.Count * 2];
			int num = 0;
			foreach (DictionaryEntry header in headers)
			{
				array[num++] = header.Key.ToString();
				array[num++] = header.Value.ToString();
			}
			return array;
		}

		private static string[] FlattenedHeadersFrom(Dictionary<string, string> headers)
		{
			if (headers == null)
			{
				return null;
			}
			string[] array = new string[headers.Count * 2];
			int num = 0;
			foreach (KeyValuePair<string, string> header in headers)
			{
				array[num++] = header.Key.ToString();
				array[num++] = header.Value.ToString();
			}
			return array;
		}

		internal static Dictionary<string, string> ParseHTTPHeaderString(string input)
		{
			if (input == null)
			{
				throw new ArgumentException("input was null to ParseHTTPHeaderString");
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			StringReader stringReader = new StringReader(input);
			int num = 0;
			while (true)
			{
				string text = stringReader.ReadLine();
				if (text == null)
				{
					break;
				}
				if (num++ == 0 && text.StartsWith("HTTP"))
				{
					dictionary["STATUS"] = text;
					continue;
				}
				int num2 = text.IndexOf(": ");
				if (num2 != -1)
				{
					string key = text.Substring(0, num2).ToUpper();
					string value = text.Substring(num2 + 2);
					dictionary[key] = value;
				}
			}
			return dictionary;
		}

		public void Dispose()
		{
			DestroyWWW(true);
		}

		~WWW()
		{
			DestroyWWW(false);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void DestroyWWW(bool cancel);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern void InitWWW(string url, byte[] postData, string[] iHeaders);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		internal extern bool enforceWebSecurityRestrictions();

		[ExcludeFromDocs]
		public static string EscapeURL(string s)
		{
			Encoding uTF = Encoding.UTF8;
			return EscapeURL(s, uTF);
		}

		public static string EscapeURL(string s, [DefaultValue("System.Text.Encoding.UTF8")] Encoding e)
		{
			if (s == null)
			{
				return null;
			}
			if (s == string.Empty)
			{
				return string.Empty;
			}
			if (e == null)
			{
				return null;
			}
			return WWWTranscoder.URLEncode(s, e);
		}

		[ExcludeFromDocs]
		public static string UnEscapeURL(string s)
		{
			Encoding uTF = Encoding.UTF8;
			return UnEscapeURL(s, uTF);
		}

		public static string UnEscapeURL(string s, [DefaultValue("System.Text.Encoding.UTF8")] Encoding e)
		{
			if (s == null)
			{
				return null;
			}
			if (s.IndexOf('%') == -1 && s.IndexOf('+') == -1)
			{
				return s;
			}
			return WWWTranscoder.URLDecode(s, e);
		}

		private Encoding GetTextEncoder()
		{
			string value = null;
			if (responseHeaders.TryGetValue("CONTENT-TYPE", out value))
			{
				int num = value.IndexOf("charset", StringComparison.OrdinalIgnoreCase);
				if (num > -1)
				{
					int num2 = value.IndexOf('=', num);
					if (num2 > -1)
					{
						string text = value.Substring(num2 + 1).Trim().Trim('\'', '"')
							.Trim();
						int num3 = text.IndexOf(';');
						if (num3 > -1)
						{
							text = text.Substring(0, num3);
						}
						try
						{
							return Encoding.GetEncoding(text);
						}
						catch (Exception)
						{
							Debug.Log("Unsupported encoding: '" + text + "'");
						}
					}
				}
			}
			return Encoding.UTF8;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern Texture2D GetTexture(bool markNonReadable);

		public AudioClip GetAudioClip(bool threeD)
		{
			return GetAudioClip(threeD, false);
		}

		public AudioClip GetAudioClip(bool threeD, bool stream)
		{
			return GetAudioClip(threeD, stream, AudioType.UNKNOWN);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern AudioClip GetAudioClip(bool threeD, bool stream, AudioType audioType);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern void LoadImageIntoTexture(Texture2D tex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[Obsolete("All blocking WWW functions have been deprecated, please use one of the asynchronous functions instead.", true)]
		[WrapperlessIcall]
		public static extern string GetURL(string url);

		[Obsolete("All blocking WWW functions have been deprecated, please use one of the asynchronous functions instead.", true)]
		public static Texture2D GetTextureFromURL(string url)
		{
			return new WWW(url).texture;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern void LoadUnityWeb();

		[ExcludeFromDocs]
		public static WWW LoadFromCacheOrDownload(string url, int version)
		{
			uint crc = 0u;
			return LoadFromCacheOrDownload(url, version, crc);
		}

		public static WWW LoadFromCacheOrDownload(string url, int version, [DefaultValue("0")] uint crc)
		{
			return new WWW(url, version, crc);
		}
	}
}

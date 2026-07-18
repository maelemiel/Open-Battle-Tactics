using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Cache;
using ICSharpCode.SharpZipLib.GZip;
using UnityEngine;

public class DataModelFile : Singleton<DataModelFile>
{
	public delegate void DownloadCallback(string error);

	public delegate void UnbundleDynamicCallback(string error);

	private static string DynamicDataModelKey = "DynamicDataModelName";

	private KeyValueStorage kvs;

	private void Awake()
	{
		kvs = KeyValueStorage.Instance(KeyValueStorage.Storage.DATA_MODEL_FILE);
	}

	public string GetPath()
	{
		string dynamicPath = GetDynamicPath();
		if (dynamicPath != null)
		{
			return dynamicPath;
		}
		return Path.Combine(Application.streamingAssetsPath, "dataModel.db");
	}

	public void DiscardDynamic()
	{
		string dynamicPath = GetDynamicPath();
		if (dynamicPath != null)
		{
			kvs.Remove(DynamicDataModelKey);
			File.Delete(dynamicPath);
		}
	}

	public bool UnbundleNeeded()
	{
		string path = GetPath();
		if (File.Exists(path))
		{
			return false;
		}
		return true;
	}

	protected string GetDynamicPath()
	{
		string value = kvs.GetValue<string>(DynamicDataModelKey);
		if (!string.IsNullOrEmpty(value))
		{
			return Path.Combine(Application.persistentDataPath, value);
		}
		return null;
	}

	public DataModelQueue.Request DownloadDynamic(string url, DownloadCallback cb)
	{
		DataModelQueue.Request request = new DataModelQueue.Request();
		StartCoroutine(DownloadDynamicCoroutine(request, url, cb));
		return request;
	}

	private IEnumerator DownloadDynamicCoroutine(DataModelQueue.Request request, string url, DownloadCallback cb)
	{
		HttpWebRequest httpRequest = WebRequest.Create(url) as HttpWebRequest;
		httpRequest.Headers["Accept-Encoding"] = "gzip";
		httpRequest.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
		httpRequest.Timeout = AppConfig.networkTimeout;
		IAsyncResult result = null;
		bool isDone = false;
		AsyncCallback completeCallback = delegate(IAsyncResult ar)
		{
			isDone = true;
			result = ar;
		};
		httpRequest.BeginGetResponse(completeCallback, null);
		while (!isDone)
		{
			yield return 0;
		}
		using (HttpWebResponse httpResponse = httpRequest.EndGetResponse(result) as HttpWebResponse)
		{
			try
			{
				string newName = Path.GetRandomFileName();
				string newPath = Path.Combine(Application.persistentDataPath, newName);
				using (FileStream fileStream = new FileStream(newPath, FileMode.Create))
				{
					DeNAiCloudUtil.SetNoBackupFlag(newPath);
					Stream responseStream = httpResponse.GetResponseStream();
					if (httpResponse.ContentEncoding != null && httpResponse.ContentEncoding.ToLower() == "gzip")
					{
						responseStream = new GZipInputStream(httpResponse.GetResponseStream());
					}
					byte[] buffer = new byte[16384];
					while (true)
					{
						int num;
						int read = (num = responseStream.Read(buffer, 0, buffer.Length));
						if (num == 0)
						{
							break;
						}
						fileStream.Write(buffer, 0, read);
					}
					fileStream.Close();
				}
				DeNAiCloudUtil.SetNoBackupFlag(newPath);
				string oldPath = GetDynamicPath();
				kvs.SetValue(DynamicDataModelKey, newName);
				if (oldPath != null)
				{
					File.Delete(oldPath);
				}
				if (!request.cancelled)
				{
					cb(null);
				}
			}
			catch (Exception ex)
			{
				Exception e = ex;
				if (!request.cancelled)
				{
					cb("Exception: " + e.ToString());
				}
			}
			httpResponse.Close();
		}
	}

	public DataModelQueue.Request UnbundleDynamic(UnbundleDynamicCallback cb)
	{
		DataModelQueue.Request request = new DataModelQueue.Request();
		StartCoroutine(UnbundleDynamicCoroutine(request, cb));
		return request;
	}

	private IEnumerator UnbundleDynamicCoroutine(DataModelQueue.Request request, UnbundleDynamicCallback cb)
	{
		WWW download = new WWW(GetPath());
		yield return download;
		if (download.error != null)
		{
			if (!request.cancelled)
			{
				cb(download.error);
			}
			yield break;
		}
		try
		{
			string newName = Path.GetRandomFileName();
			string newPath = Path.Combine(Application.persistentDataPath, newName);
			File.WriteAllBytes(newPath, download.bytes);
			DeNAiCloudUtil.SetNoBackupFlag(newPath);
			kvs.SetValue(DynamicDataModelKey, newName);
			if (!request.cancelled)
			{
				cb(null);
			}
		}
		catch (Exception ex)
		{
			if (!request.cancelled)
			{
				cb("Exception: " + ex.ToString());
			}
		}
	}
}

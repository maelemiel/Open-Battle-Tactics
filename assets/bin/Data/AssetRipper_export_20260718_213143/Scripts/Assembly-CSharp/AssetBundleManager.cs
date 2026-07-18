using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AssetBundleManager : Singleton<AssetBundleManager>
{
	private class LoadedBundle
	{
		public AssetBundleDataModel m;

		public AssetBundle ab;
	}

	private class BundleCacheEntry
	{
		public string path;

		public string hash;

		public string[] intern;

		public BundleCacheEntry()
		{
		}

		public BundleCacheEntry(string path, string hash)
		{
			this.path = path;
			this.hash = hash;
			intern = new string[2] { path, hash };
		}

		public static BundleCacheEntry Parse(string[] values)
		{
			if (values != null)
			{
				BundleCacheEntry bundleCacheEntry = new BundleCacheEntry();
				bundleCacheEntry.path = values[0];
				bundleCacheEntry.hash = values[1];
				bundleCacheEntry.intern = values;
				return bundleCacheEntry;
			}
			return null;
		}

		public object Serialize()
		{
			intern[0] = path;
			intern[1] = hash;
			return intern;
		}
	}

	public class Request
	{
		public int assetId;

		public string assetURL;

		public string assetName;

		public string hash;

		public IList callbacks = ArrayList.Synchronized(new ArrayList());

		public int retries;

		public bool prioritized;

		public Request(int assetId, bool prioritized, Delegate cb)
		{
			this.assetId = assetId;
			this.prioritized = prioritized;
			retries = AppConfig.networkRetries;
			callbacks.Add(cb);
		}

		public string AssetKey()
		{
			return AssetBundleManager.AssetKey(assetId);
		}
	}

	public delegate void GetAssetBundleCallback(string error, AssetBundle assetBundle, AssetBundleDataModel dm);

	private delegate void DowmloadAssetBundleCallback(string error, Request abRequest, AssetBundleDataModel m, string assetPath);

	private const string TUTORIAL_AB_UNBUNDLED = "TUTORIAL_AB_UNBUNDLED";

	public bool blocked;

	private Dictionary<string, LoadedBundle> loadedBundles = new Dictionary<string, LoadedBundle>();

	private Dictionary<string, Request> downloadingBundles = new Dictionary<string, Request>();

	private Queue<Request> downloadMissingBundlesQueue = new Queue<Request>();

	private Dictionary<string, Request> queuedAssetBundles = new Dictionary<string, Request>();

	private KeyValueStorage abCache;

	private Dictionary<AssetBundleDataModel, int> assetBundleRetainCount = new Dictionary<AssetBundleDataModel, int>();

	private static bool LOG_REFCOUNTING;

	private static string AssetKey(int id)
	{
		return "AssetId_" + id;
	}

	private string CreateAssetKey(int id)
	{
		return AssetKey(id);
	}

	public IEnumerator Init()
	{
		abCache = KeyValueStorage.Instance(KeyValueStorage.Storage.ASSET_BUNDLE);
		Singleton<InitializationManager>.instance.DataModelUpdated += DataModelUpdated;
		if (!abCache.GetValue<bool>("TUTORIAL_AB_UNBUNDLED"))
		{
			bool done = false;
			abCache.SetValueAsync("TUTORIAL_AB_UNBUNDLED", true, delegate
			{
				done = true;
			});
			while (!done)
			{
				yield return 0;
			}
		}
	}

	private void DataModelUpdated()
	{
		Singleton<DataModelQueue>.instance.Enqueue(DataModelQueue.Request.AllAssetBundles(delegate(DataModelQueue.Response response)
		{
			if (response.error != null)
			{
				Log.Error("Failed to get AllAssetBundles. Error: " + response.error.description);
			}
			else
			{
				foreach (AssetBundleDataModel item in response.dataModel as IList<AssetBundleDataModel>)
				{
					string key = CreateAssetKey(item.id);
					LoadedBundle loadedBundle = ((!loadedBundles.ContainsKey(key)) ? null : loadedBundles[key]);
					if (loadedBundle != null && PlatformHash(loadedBundle.m) != PlatformHash(item))
					{
						loadedBundle.ab.Unload(false);
						loadedBundles.Remove(key);
					}
					if (downloadingBundles.ContainsKey(key))
					{
						downloadingBundles.Remove(key);
					}
				}
				foreach (LoadedBundle value in loadedBundles.Values)
				{
					value.ab.Unload(false);
				}
				loadedBundles.Clear();
				downloadingBundles.Clear();
				DownloadMissingAssetBundles();
			}
		}));
	}

	public void DownloadMissingAssetBundles()
	{
		DataModelQueue.Request request = DataModelQueue.Request.Multi(null, delegate(DMAccessManager db, object inputs, out object outputs)
		{
			outputs = null;
			List<AssetBundleDataModel> assetBundles;
			AccessBase.Error allAssetBundles = db.DataModelAccess.GetAllAssetBundles(out assetBundles);
			if (allAssetBundles != null)
			{
				Log.Warning("Couldn't retrieve assetBundle list. Error: " + allAssetBundles.description);
				return (AccessBase.Error)null;
			}
			Log.Info("DownloadMissingAssetBundles.Multi: Going to check: {0} assets.", assetBundles.Count);
			foreach (AssetBundleDataModel item in assetBundles)
			{
				int id = item.id;
				BundleCacheEntry bundleCacheEntry = BundleCacheEntry.Parse(abCache.GetValue<string[]>(CreateAssetKey(id)));
				if (bundleCacheEntry != null)
				{
					if (bundleCacheEntry.hash == PlatformHash(item))
					{
						continue;
					}
					RemoveFromDiskCacheAsync(id, CreateAssetKey(id), bundleCacheEntry.path);
				}
				lock (queuedAssetBundles)
				{
					string key = CreateAssetKey(id);
					if (!queuedAssetBundles.ContainsKey(key))
					{
						Request request2 = new Request(id, false, (GetAssetBundleCallback)delegate
						{
						});
						queuedAssetBundles.Add(key, request2);
						downloadMissingBundlesQueue.Enqueue(request2);
					}
				}
			}
			StartDownloadingMissingAssets();
			return (AccessBase.Error)null;
		}, null);
		Singleton<DataModelQueue>.instance.Enqueue(request);
	}

	private void StartDownloadingMissingAssets()
	{
		Log.Info("Starting to download missing assets", downloadMissingBundlesQueue.Count);
		try
		{
			Request request = downloadMissingBundlesQueue.Dequeue();
			DownloadAndLoad(request, delegate
			{
				StartDownloadingMissingAssets();
			});
		}
		catch (InvalidOperationException ex)
		{
			Log.Info("Queue is empty, we are done", ex);
		}
	}

	private void DownloadAndLoad(Request request, Action downloadFinishedCallback)
	{
		downloadingBundles.Add(request.AssetKey(), request);
		DownloadAssetBundle(request, delegate(string error, Request cbRequest, AssetBundleDataModel m, string assetPath)
		{
			if (error != null)
			{
				Log.Error("Couldn't download asset id: {0} in background, because of error: {1}", request.AssetKey(), error);
			}
			else if (cbRequest.callbacks.Count > 1)
			{
				Load(request.AssetKey(), m, assetPath, cbRequest.callbacks);
			}
			downloadFinishedCallback();
		});
	}

	public void GetAssetBundle(int id, GetAssetBundleCallback cb)
	{
		StartCoroutine(GetAssetBundleCoroutine(id, cb));
	}

	private IEnumerator GetAssetBundleCoroutine(int id, GetAssetBundleCallback cb)
	{
		string assetKey = CreateAssetKey(id);
		while (blocked)
		{
			yield return 0;
		}
		if (loadedBundles.ContainsKey(assetKey))
		{
			LoadedBundle loadedBundle = loadedBundles[assetKey];
			if (id != loadedBundle.m.id)
			{
				Debug.LogError("FAILURE AssetBundleManager loaded the wrong bundle from cache! Expected:" + id + " but got: " + loadedBundle.m.id);
			}
			cb(null, loadedBundle.ab, loadedBundle.m);
			yield break;
		}
		int id2 = default(int);
		GetAssetBundleCallback cb2 = default(GetAssetBundleCallback);
		DataModelQueue.Request req = DataModelQueue.Request.Multi(id, delegate(DMAccessManager db, object inputs, out object outputs)
		{
			outputs = null;
			AssetBundleDataModel m;
			AccessBase.Error error = db.DataModelAccess._GetSingleAssetBundle(id2, out m);
			if (error != null)
			{
				return error;
			}
			BundleCacheEntry bundleCacheEntry = BundleCacheEntry.Parse(abCache.GetValue<string[]>(assetKey));
			if (bundleCacheEntry == null)
			{
				return (AccessBase.Error)null;
			}
			if (bundleCacheEntry.hash == PlatformHash(m))
			{
				outputs = new object[2] { bundleCacheEntry, m };
			}
			else
			{
				Log.Info("Asset id: {0} already downloaded but with different hash, downloading it again", m.id);
				RemoveFromDiskCacheAsync(m.id, assetKey, bundleCacheEntry.path);
			}
			return (AccessBase.Error)null;
		}, delegate(DataModelQueue.Response response)
		{
			if (response.error != null)
			{
				cb2(response.error.description, null, null);
			}
			else
			{
				object[] array = response.dataModel as object[];
				if (array == null)
				{
					lock (downloadingBundles)
					{
						Request request = null;
						if (downloadingBundles.ContainsKey(assetKey))
						{
							Request request2 = downloadingBundles[assetKey];
							request2.callbacks.Add(cb2);
						}
						else
						{
							request = new Request(id2, true, cb2);
							downloadingBundles.Add(assetKey, request);
						}
						if (request != null)
						{
							DownloadAssetBundle(request, delegate(string error, Request cbRequest, AssetBundleDataModel m, string assetPath)
							{
								if (error != null)
								{
									cb2(error, null, null);
								}
								else
								{
									Load(assetKey, m, assetPath, cbRequest.callbacks);
								}
							});
						}
						return;
					}
				}
				Load(assetKey, array[1] as AssetBundleDataModel, (array[0] as BundleCacheEntry).path, new object[1] { cb2 });
			}
		});
		Singleton<DataModelQueue>.instance.Enqueue(req);
	}

	public void RetainAssetBundle(AssetBundleDataModel assetBundleDM)
	{
		int num = 0;
		if (assetBundleRetainCount.ContainsKey(assetBundleDM))
		{
			num = assetBundleRetainCount[assetBundleDM];
		}
		assetBundleRetainCount[assetBundleDM] = num + 1;
		if (LOG_REFCOUNTING)
		{
			Log.DebugTag("AssetBundleRetain: " + assetBundleDM.id + "   retainCount:" + (num + 1), null, "ABRetain");
		}
	}

	public void ReleaseAssetBundle(AssetBundleDataModel assetBundleDM)
	{
		int num = 0;
		if (assetBundleRetainCount.ContainsKey(assetBundleDM))
		{
			num = assetBundleRetainCount[assetBundleDM];
		}
		num--;
		if (LOG_REFCOUNTING)
		{
			Log.DebugTag("AssetBundleRelease: " + assetBundleDM.id + "   retainCount:" + num, null, "ABRelease");
		}
		if (num < 0)
		{
			Log.WarningTag("AssetBundle attempted double release!", null, "ABRelease");
			assetBundleRetainCount[assetBundleDM] = 0;
		}
		else
		{
			assetBundleRetainCount[assetBundleDM] = num;
		}
	}

	public bool UnloadAssetBundle(AssetBundleDataModel assetBundleDM)
	{
		if (LOG_REFCOUNTING)
		{
			Log.DebugTag("AssetBundleUnload: " + assetBundleDM.id, null, "ABUnload");
		}
		string key = CreateAssetKey(assetBundleDM.id);
		if (!loadedBundles.ContainsKey(key))
		{
			Debug.LogWarning("could not unload bundle, probably already unloaded: " + assetBundleDM.id);
			return false;
		}
		LoadedBundle loadedBundle = loadedBundles[key];
		if (!ShouldUnloadAssetBundle(loadedBundle.m.id))
		{
			return false;
		}
		UnityEngine.Object[] array = loadedBundle.ab.LoadAll(typeof(tk2dBaseSprite));
		UnityEngine.Object[] array2 = array;
		foreach (UnityEngine.Object obj in array2)
		{
			tk2dBaseSprite tk2dBaseSprite2 = obj as tk2dBaseSprite;
			if ((bool)tk2dBaseSprite2 && (bool)tk2dBaseSprite2.Collection)
			{
				tk2dBaseSprite2.Collection.DestroyEverything();
			}
		}
		loadedBundle.ab.Unload(true);
		loadedBundles.Remove(key);
		return true;
	}

	public void UnloadUnusedBundles()
	{
		List<AssetBundleDataModel> list = new List<AssetBundleDataModel>();
		foreach (KeyValuePair<AssetBundleDataModel, int> item in assetBundleRetainCount)
		{
			if (item.Value <= 0)
			{
				list.Add(item.Key);
			}
		}
		foreach (AssetBundleDataModel item2 in list)
		{
			if (UnloadAssetBundle(item2))
			{
				assetBundleRetainCount.Remove(item2);
			}
		}
	}

	public bool ShouldUnloadAssetBundle(int abID)
	{
		if (UserProfile.player != null)
		{
			if (UserProfile.player.CurrentDivision != null && UserProfile.player.CurrentDivision.BadgeLinkage != null && abID == UserProfile.player.CurrentDivision.BadgeLinkage.bundleId)
			{
				return false;
			}
			if (UserProfile.player.CurrentTeam != null)
			{
				foreach (UserUnit unit in UserProfile.player.CurrentTeam.units)
				{
					if (unit != null && unit.AssetBundleID == abID)
					{
						return false;
					}
				}
			}
		}
		return true;
	}

	private void Load(string assetKey, AssetBundleDataModel assetBundleDM, string assetPath, IList callbacks)
	{
		if (loadedBundles.ContainsKey(assetKey))
		{
			LoadedBundle loadedBundle = loadedBundles[assetKey];
			{
				foreach (GetAssetBundleCallback callback in callbacks)
				{
					callback(null, loadedBundle.ab, loadedBundle.m);
				}
				return;
			}
		}
		AssetBundle assetBundle = AssetBundle.CreateFromFile(assetPath);
		if (assetBundle == null)
		{
			bool flag = File.Exists(assetPath);
			RemoveFromDiskCacheAsync(assetBundleDM.id, assetKey, assetPath);
			{
				foreach (GetAssetBundleCallback callback2 in callbacks)
				{
					if (flag)
					{
						callback2(string.Format("AssetBundle id: {0} couldn't be loaded by the system. Uknown error. Path: {1}", assetBundleDM.id, assetPath), null, assetBundleDM);
						continue;
					}
					Log.Warning("AssetBundle id: {0} was deleted from the file system, trying downloading it. Path: {1}", assetBundleDM.id, assetPath);
					GetAssetBundle(assetBundleDM.id, callback2);
				}
				return;
			}
		}
		loadedBundles.Add(assetKey, new LoadedBundle
		{
			ab = assetBundle,
			m = assetBundleDM
		});
		foreach (GetAssetBundleCallback callback3 in callbacks)
		{
			callback3(null, assetBundle, assetBundleDM);
		}
	}

	private void RemoveFromDiskCacheAsync(int id, string assetCacheKey, string assetPath)
	{
		WorkQueue.Do(delegate
		{
			abCache.Remove(assetCacheKey);
			try
			{
				File.Delete(assetPath);
			}
			catch (Exception ex)
			{
				Log.Error("Couldn't delete assetBundle id: {0} at path: {1}. Error: {2}", id, assetPath, ex.ToString());
			}
		});
	}

	private void DownloadingDone(int assetId)
	{
		lock (downloadingBundles)
		{
			downloadingBundles.Remove(CreateAssetKey(assetId));
		}
	}

	private string PlatformHash(AssetBundleDataModel assetBundleDM)
	{
		switch (Application.platform)
		{
		case RuntimePlatform.Android:
			return assetBundleDM.androidHash;
		case RuntimePlatform.IPhonePlayer:
			return assetBundleDM.iPhoneHash;
		case RuntimePlatform.OSXEditor:
			return assetBundleDM.androidHash;
		case RuntimePlatform.WindowsEditor:
			return assetBundleDM.androidHash;
		default:
			return null;
		}
	}

	private string PlatformName(RuntimePlatform platform)
	{
		switch (platform)
		{
		case RuntimePlatform.Android:
			return "Android";
		case RuntimePlatform.IPhonePlayer:
			return "iPhone";
		case RuntimePlatform.OSXEditor:
		case RuntimePlatform.WindowsEditor:
			return "Editor";
		default:
			return null;
		}
	}

	private void DownloadAssetBundle(Request abRequest, DowmloadAssetBundleCallback cb)
	{
		DataModelQueue.Request request = DataModelQueue.Request.SingleAssetBundle(abRequest.assetId, delegate(DataModelQueue.Response response)
		{
			if (response.error != null)
			{
				DownloadingDone(abRequest.assetId);
				cb(response.error.description, abRequest, null, null);
			}
			else
			{
				AssetBundleDataModel assetBundleDM = response.dataModel as AssetBundleDataModel;
				string text = PlatformHash(assetBundleDM);
				if (text == null)
				{
					DownloadingDone(abRequest.assetId);
					cb("Platform not known!: " + Application.platform, abRequest, null, null);
				}
				else
				{
					abRequest.assetURL = Singleton<InitializationManager>.instance.dataModelAssetUrl + text + assetBundleDM.id;
					abRequest.hash = text;
					abRequest.assetName = text + assetBundleDM.id;
					Log.Info("AssetBundle id: {0} enqueued for download from: {1}", assetBundleDM.id, abRequest.assetURL);
					string uri = text + assetBundleDM.id;
					Singleton<SessionManager>.instance.DeNetwork.DownloadAsset(uri, text, delegate(DeNetworkAssetBundleDownload result)
					{
						if (result.Error != null)
						{
							cb(result.Error.Message, abRequest, null, null);
						}
						else
						{
							Log.Info("AssetBundle id: {0}, downloaded successfully to path:{1}", assetBundleDM.id, result.LocalPath);
							DeNAiCloudUtil.SetNoBackupFlag(result.LocalPath);
							abCache.SetValueAsync(CreateAssetKey(abRequest.assetId), new BundleCacheEntry(result.LocalPath, abRequest.hash).Serialize());
							DownloadingDone(abRequest.assetId);
							cb(null, abRequest, assetBundleDM, result.LocalPath);
						}
					});
				}
			}
		});
		Singleton<DataModelQueue>.instance.Enqueue(request);
	}
}

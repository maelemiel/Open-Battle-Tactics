using System;
using System.Collections;
using System.IO;
using System.Threading;
using UnityEngine;

public static class UnbundleUtility
{
	public delegate void UnbundleCallback(string error);

	public static IEnumerator UnbundleCoroutine(string from, string to, UnbundleCallback cb)
	{
		WWW download = new WWW(from);
		yield return download;
		if (download.error != null)
		{
			cb(download.error);
			yield break;
		}
		try
		{
			File.WriteAllBytes(to, download.bytes);
		}
		catch (Exception ex)
		{
			Exception e = ex;
			cb("Exception: " + e.ToString());
		}
		cb(null);
	}

	public static string UnbundleBlocking(string from, string to)
	{
		string error = null;
		IEnumerator enumerator = UnbundleCoroutine(from, to, delegate(string err)
		{
			error = err;
		});
		enumerator.MoveNext();
		WWW wWW = enumerator.Current as WWW;
		while (!wWW.isDone)
		{
			Thread.Sleep(1);
		}
		return error;
	}
}

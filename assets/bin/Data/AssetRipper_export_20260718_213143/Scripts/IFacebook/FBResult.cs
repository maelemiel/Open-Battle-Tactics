using System;
using UnityEngine;

public class FBResult : IDisposable
{
	private bool isWWWWrapper;

	private object data;

	private string error;

	public Texture2D Texture
	{
		get
		{
			return (!isWWWWrapper) ? (data as Texture2D) : ((WWW)data).texture;
		}
	}

	public string Text
	{
		get
		{
			return (!isWWWWrapper) ? (data as string) : ((WWW)data).text;
		}
	}

	public string Error
	{
		get
		{
			return (!isWWWWrapper) ? error : ((WWW)data).error;
		}
	}

	public FBResult(WWW www)
	{
		isWWWWrapper = true;
		data = www;
	}

	public FBResult(string data, string error = null)
	{
		this.data = data;
		this.error = error;
	}

	public void Dispose()
	{
		if (isWWWWrapper && data != null)
		{
			((WWW)data).Dispose();
		}
	}

	~FBResult()
	{
		Dispose();
	}
}

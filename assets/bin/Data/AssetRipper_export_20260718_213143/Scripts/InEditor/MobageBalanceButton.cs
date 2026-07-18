using System;
using System.Collections;
using System.Collections.Generic;
using MobageEditor;
using UnityEngine;

public class MobageBalanceButton : MonoBehaviour
{
	private delegate void DownloadCallback(Texture2D texture);

	private const int BalanceBaseWidth = 138;

	private Rect drawRect;

	private Rect subdrawRect;

	private Rect currencyNamedrawRect;

	private Rect balancedrawRect;

	private Rect coindrawRect;

	private int balance;

	private string currencyName;

	private Texture2D coinImg;

	private WWW www;

	public Rect Frame
	{
		get
		{
			return drawRect;
		}
		set
		{
			drawRect = value;
			subdrawRect = CalcMainRect(value);
			float ratio = subdrawRect.width / 138f;
			currencyNamedrawRect = CalcSubviewRect(subdrawRect, new Rect(2f, 5f, 100f, 17f), ratio);
			balancedrawRect = CalcSubviewRect(subdrawRect, new Rect(12f, 22f, 75f, 20f), ratio);
			coindrawRect = CalcSubviewRect(subdrawRect, new Rect(90f, 2f, 42f, 42f), ratio);
		}
	}

	public Action<SimpleAPIStatus, Error> Click { get; set; }

	public Action<Error> ErrorCallback { get; set; }

	private void Start()
	{
		coinImg = Resources.Load("mobage_gc_icon") as Texture2D;
	}

	private void OnGUI()
	{
		GUI.depth = 0;
		if (GUI.Button(Frame, string.Empty) && Click != null)
		{
			MobageWeb instance = MobageWeb.Instance;
			ShowBankUI();
			Click(SimpleAPIStatus.Success, null);
		}
		GUI.Label(currencyNamedrawRect, currencyName);
		GUI.Label(balancedrawRect, balance.ToString());
		GUI.Label(coindrawRect, coinImg);
	}

	public Rect CalcMainRect(Rect rect)
	{
		double num = rect.width;
		double num2 = rect.height;
		int num3;
		int num4;
		if (num2 * 3.0 > num)
		{
			num3 = (int)Math.Floor(num / 3.0);
			num4 = (int)Math.Floor(num);
		}
		else
		{
			num3 = (int)Math.Floor(num2);
			num4 = (int)Math.Floor(num2 * 3.0);
		}
		int num5 = (int)Math.Floor((num2 - (double)num3) / 2.0);
		num5 += (int)drawRect.y;
		int num6 = (int)Math.Floor((num - (double)num4) / 2.0);
		num6 += (int)drawRect.x;
		return new Rect(num6, num5, num4, num3);
	}

	public static Rect CalcSubviewRect(Rect mainRect, Rect rect, float ratio)
	{
		return new Rect(mainRect.x + rect.x * ratio, mainRect.y + rect.y * ratio, rect.width * ratio, rect.height * ratio);
	}

	public void UpdateBalance()
	{
		BankBalance.GetBalanceWithCallback(delegate(SimpleAPIStatus status, Error error, int balance, string currency, string currencyIcon)
		{
			if (status == SimpleAPIStatus.Success)
			{
				DidReceiveResponse(balance, currency, currencyIcon);
			}
			else
			{
				ErrorCallback(error);
				UnityEngine.Object.Destroy(base.gameObject);
			}
		});
	}

	public void DidReceiveResponse(int balance, string currency, string coinImageUrl)
	{
		MonoBehaviour.print("balance:" + balance + ", currency:" + currency + ", coinImageUrl:" + coinImageUrl);
		this.balance = balance;
		currencyName = currency;
		www = new WWW(coinImageUrl);
		DownloadCallback value = delegate(Texture2D texture)
		{
			if (texture != null)
			{
				coinImg = texture;
			}
		};
		StartCoroutine("CreateWWW", value);
	}

	private IEnumerator CreateWWW(DownloadCallback callback)
	{
		yield return www;
		if (string.IsNullOrEmpty(www.error))
		{
			callback(www.texture);
		}
	}

	public void ShowBankUI()
	{
		MobageWeb.OpenMobageWeb(MobageWeb.Action.BANK, null, delegate(DismissableAPIStatus status, Error error, Dictionary<string, string> result)
		{
			if (error == null)
			{
				MonoBehaviour.print("Bank Dialog Closed...");
			}
			else
			{
				MonoBehaviour.print("Open Bank Dialog Failed: " + error);
			}
		});
	}
}

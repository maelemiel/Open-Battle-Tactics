using System;
using System.Collections;
using System.Collections.Generic;
using LCD;
using LCD.Bank;
using UnityEngine;

public class BankService : Singleton<BankService>
{
	public enum APIStatus
	{
		Success = 0,
		Error = 1
	}

	private Wallet wallet;

	private List<VCBundle> bankItems;

	private List<ShopItem> shopItems;

	public event Action<int> BankBalanceEvent;

	public event Action BankRetrieveItemsError;

	public event Action BankRetrieveItemsAcknowledged;

	public event Action<string> BankPurchaseSuccess;

	public event Action<string> BankPurchaseError;

	public event Action BankPurchaseErrorAcknowledged;

	private void SendBankBalanceEvent(int balance)
	{
		if (this.BankBalanceEvent != null)
		{
			this.BankBalanceEvent(balance);
		}
	}

	private void SendBankRetrieveItemsError()
	{
		if (this.BankRetrieveItemsError != null)
		{
			this.BankRetrieveItemsError();
		}
	}

	public void SendBankRetrieveItemsAcknowledged()
	{
		if (this.BankRetrieveItemsAcknowledged != null)
		{
			this.BankRetrieveItemsAcknowledged();
		}
	}

	private void SendBankPurchaseSuccess(string message)
	{
		if (this.BankPurchaseSuccess != null)
		{
			this.BankPurchaseSuccess(message);
		}
	}

	private void SendBankPurchaseError(string message)
	{
		if (this.BankPurchaseError != null)
		{
			this.BankPurchaseError(message);
		}
	}

	public void SendBankPurchaseErrorAcknowledged()
	{
		if (this.BankPurchaseErrorAcknowledged != null)
		{
			this.BankPurchaseErrorAcknowledged();
		}
	}

	private void Awake()
	{
		Log.DebugTag("Awakened", null, "BankService");
		StartCoroutine(WaitForLCDSDK());
	}

	private IEnumerator WaitForLCDSDK()
	{
		while (string.IsNullOrEmpty(Singleton<LCDController>.instance.AccessToken))
		{
			yield return new WaitForSeconds(0f);
		}
		Initialize();
	}

	private void Initialize()
	{
		StartCoroutine(UpdateBankItems());
		StartCoroutine(UpdateASCItems());
		this.BankRetrieveItemsAcknowledged = (Action)Delegate.Combine(this.BankRetrieveItemsAcknowledged, new Action(OnBankRetrieveItemsAcknowledged));
		this.BankPurchaseErrorAcknowledged = (Action)Delegate.Combine(this.BankPurchaseErrorAcknowledged, new Action(OnBankPurchaseErrorAcknowledged));
	}

	private void OnDestroy()
	{
		this.BankRetrieveItemsAcknowledged = (Action)Delegate.Remove(this.BankRetrieveItemsAcknowledged, new Action(OnBankRetrieveItemsAcknowledged));
		this.BankPurchaseErrorAcknowledged = (Action)Delegate.Remove(this.BankPurchaseErrorAcknowledged, new Action(OnBankPurchaseErrorAcknowledged));
	}

	private IEnumerator UpdateBankItems()
	{
		List<VCBundle> items = null;
		bool isDone = false;
		VCBundle.GetAsList(delegate(List<VCBundle> bundles, LCDError error)
		{
			if (error == null)
			{
				items = bundles;
			}
			else
			{
				SendBankRetrieveItemsError();
			}
			isDone = true;
		});
		while (!isDone)
		{
			yield return 0;
		}
		if (items != null)
		{
			bankItems = items;
		}
		else
		{
			bankItems = new List<VCBundle>();
		}
	}

	private void OnBankRetrieveItemsAcknowledged()
	{
		StartCoroutine(UpdateBankItems());
	}

	private IEnumerator UpdateASCItems()
	{
		while (bankItems == null)
		{
			yield return new WaitForSeconds(0f);
		}
		List<ShopItem> items = new List<ShopItem>();
		foreach (VCBundle vcb in bankItems)
		{
			ShopItem item = new ShopItem(vcb.title, vcb.sku, vcb.detail, vcb.value, vcb.currency, vcb.priceCode, vcb.price, vcb.usdPrice, vcb.displayPrice);
			items.Add(item);
		}
		shopItems = items;
	}

	public void GetCurrencyBalance(Action<int> cb)
	{
		LCD.Bank.Wallet.GetCurrentBalance(delegate(LCD.Bank.Wallet wallet, LCDError error)
		{
			if (error != null)
			{
				Debug.LogError(error.errorCode + " BankService unable to get balance: " + error.errorMessage);
			}
			else
			{
				this.wallet = new Wallet(wallet.currency, wallet.balance);
				int balance = wallet.balance;
				if (cb != null)
				{
					Log.DebugTag("Current balance : " + balance, null, "BankService");
					cb(balance);
				}
			}
		});
	}

	public void GetASCItems(Action<bool, List<ShopItem>> cb)
	{
		if (shopItems != null && shopItems.Count > 0)
		{
			cb(true, shopItems);
		}
		else
		{
			cb(false, new List<ShopItem>());
		}
	}

	public void PurchaseASC(ShopItem item, Action<bool> cb)
	{
		VCBundle vCBundle = null;
		foreach (VCBundle bankItem in bankItems)
		{
			if (bankItem.sku == item.sku)
			{
				vCBundle = bankItem;
				break;
			}
		}
		if (vCBundle != null)
		{
			Purchase(vCBundle, cb);
		}
		else
		{
			cb(false);
		}
	}

	private void Purchase(VCBundle item, Action<bool> cb)
	{
		bool success = false;
		Log.DebugTag("STORE PURCHASE", null, "shop");
		item.Purchase(delegate(LCD.Bank.Wallet wallet, LCDError error)
		{
			if (error == null)
			{
				success = true;
				Log.DebugTag("STORE PURCHASE SUCCESS", null, "shop");
				string message = string.Empty;
				if (!string.IsNullOrEmpty(item.title))
				{
					message = item.title + " sku:" + item.sku + " Successfully Purchased";
				}
				Log.DebugTag(message, null, "BankService.Purchase()");
				SendBankPurchaseSuccess(message);
				if (this.wallet != null)
				{
					this.wallet.balance += item.value;
					SendBankBalanceEvent(this.wallet.balance);
				}
			}
			else if (error.errorType != LCDError.ErrorType.USER_CANCEL)
			{
				Log.ErrorTag("STORE PURCHASE ERROR: " + error.errorMessage, null, "shop");
				Debug.LogError(error.errorMessage);
				SendBankPurchaseError(error.errorMessage);
			}
			else
			{
				Log.ErrorTag("STORE PURCHASE CANCEL ", null, "shop");
			}
			cb(success);
		});
	}

	private void OnBankPurchaseErrorAcknowledged()
	{
	}

	public void GiftASC(string giftCode, Action<bool> cb)
	{
		Debug.LogError("BankService.GiftAS() Unhandled...");
	}
}

using UnityEngine;

public class BankPopupHandler : MonoBehaviour
{
	private void Start()
	{
		Singleton<BankService>.instance.BankRetrieveItemsError += OnBankRetrieveItemsError;
		Singleton<BankService>.instance.BankPurchaseSuccess += OnBankPurchaseSuccess;
		Singleton<BankService>.instance.BankPurchaseError += OnBankPurchaseError;
	}

	private void OnDestroy()
	{
		Singleton<BankService>.instance.BankRetrieveItemsError -= OnBankRetrieveItemsError;
		Singleton<BankService>.instance.BankPurchaseSuccess -= OnBankPurchaseSuccess;
		Singleton<BankService>.instance.BankPurchaseError -= OnBankPurchaseError;
	}

	private void OnBankRetrieveItemsError()
	{
		PopupManager.ShowPopup(PopupDataModel.NetworkError(delegate
		{
			Singleton<BankService>.instance.SendBankRetrieveItemsAcknowledged();
		}));
	}

	private void OnBankPurchaseSuccess(string message)
	{
	}

	private void OnBankPurchaseError(string message)
	{
		PopupManager.ShowPopup(PopupDataModel.NetworkError(delegate
		{
			Singleton<BankService>.instance.SendBankPurchaseErrorAcknowledged();
		}));
	}
}

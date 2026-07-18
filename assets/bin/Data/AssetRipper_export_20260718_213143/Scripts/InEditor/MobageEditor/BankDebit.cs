using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;

namespace MobageEditor
{
	public class BankDebit
	{
		public delegate void createTransactionForItem_onCompleteCallback(CancelableAPIStatus status, Error error, Transaction transaction);

		public delegate void openTransaction_onCompleteCallback(SimpleAPIStatus status, Error error, Transaction transaction);

		public delegate void closeTransaction_onCompleteCallback(SimpleAPIStatus status, Error error, Transaction transaction);

		public delegate void continueTransaction_onCompleteCallback(CancelableAPIStatus status, Error error, Transaction transaction);

		public delegate void cancelTransaction_onCompleteCallback(SimpleAPIStatus status, Error error, Transaction transaction);

		public delegate void getTransaction_onCompleteCallback(SimpleAPIStatus status, Error error, Transaction transaction);

		public delegate void authorizeTransaction_onCompleteCallback(SimpleAPIStatus status, Error error, Transaction transaction);

		private static int cbUidGenerator = 0;

		private static Dictionary<int, Delegate> pendingCallbacks = new Dictionary<int, Delegate>();

		public static Dialog dialog = new Dialog();

		private static BankDebit dialogHandler = new BankDebit();

		public static void createTransactionForItem(ItemData itemToPurchase, int quantity, string comment, createTransactionForItem_onCompleteCallback onComplete)
		{
			BillingItem billingItem = new BillingItem();
			billingItem.item = itemToPurchase;
			billingItem.quantity = quantity;
			BillingItem billingItem2 = billingItem;
			CreateTransaction(billingItem2, comment, delegate(Transaction transaction)
			{
				onComplete(CancelableAPIStatus.Success, null, transaction);
			}, delegate
			{
				onComplete(CancelableAPIStatus.Cancel, null, null);
			}, delegate
			{
				onComplete(CancelableAPIStatus.Error, null, null);
			});
		}

		public static void openTransaction(Transaction transaction, openTransaction_onCompleteCallback onComplete)
		{
			openTransaction(transaction.id, delegate(Transaction successTransaction)
			{
				onComplete(SimpleAPIStatus.Success, null, successTransaction);
			}, delegate(Error error)
			{
				onComplete(SimpleAPIStatus.Error, error, transaction);
			});
		}

		public static void closeTransaction(Transaction transaction, closeTransaction_onCompleteCallback onComplete)
		{
			if (transaction == null)
			{
				Error error = new Error();
				error.domain = "com.mobage.error.api";
				error.code = 20003;
				Error error2 = error;
				onComplete(SimpleAPIStatus.Error, error2, transaction);
			}
			closeTransaction(transaction.id, delegate(Transaction successTransaction)
			{
				onComplete(SimpleAPIStatus.Success, null, successTransaction);
			}, delegate(Error error3)
			{
				onComplete(SimpleAPIStatus.Error, error3, transaction);
			});
		}

		public static void continueTransaction(Transaction transaction, continueTransaction_onCompleteCallback completeCB)
		{
			if (transaction == null)
			{
				Error error = new Error();
				error.domain = "com.mobage.error.api";
				error.code = 20003;
				Error error2 = error;
				completeCB(CancelableAPIStatus.Error, error2, transaction);
			}
			continueTransaction(transaction.id, delegate(Transaction successTransaction)
			{
				completeCB(CancelableAPIStatus.Success, null, successTransaction);
			}, delegate
			{
				completeCB(CancelableAPIStatus.Cancel, null, transaction);
			}, delegate(string transactionId, Error error3)
			{
				completeCB(CancelableAPIStatus.Error, error3, transaction);
			});
		}

		public static void cancelTransaction(Transaction transaction, cancelTransaction_onCompleteCallback completeCB)
		{
			if (transaction == null)
			{
				Error error = new Error();
				error.domain = "com.mobage.error.api";
				error.code = 20003;
				Error error2 = error;
				completeCB(SimpleAPIStatus.Error, error2, transaction);
			}
			cancelTransaction(transaction.id, delegate(Transaction successTransaction)
			{
				completeCB(SimpleAPIStatus.Success, null, successTransaction);
			}, delegate(Error error3)
			{
				completeCB(SimpleAPIStatus.Error, error3, transaction);
			});
		}

		public static void getTransaction(string transactionId, getTransaction_onCompleteCallback completeCB)
		{
			if (string.IsNullOrEmpty(transactionId))
			{
				Error error = new Error();
				error.domain = "com.mobage.error.api";
				error.code = 20003;
				Error error2 = error;
				completeCB(SimpleAPIStatus.Error, error2, null);
			}
			getTransaction(transactionId, delegate(Transaction transaction)
			{
				completeCB(SimpleAPIStatus.Success, null, transaction);
			}, delegate(Error error3)
			{
				completeCB(SimpleAPIStatus.Error, error3, null);
			});
		}

		public static void authorizeTransaction(Transaction transaction, authorizeTransaction_onCompleteCallback completeCB)
		{
			if (transaction == null)
			{
				Error error = new Error();
				error.domain = "com.mobage.error.api";
				error.code = 20003;
				Error error2 = error;
				completeCB(SimpleAPIStatus.Error, error2, transaction);
			}
			authorizeTransaction(transaction.transactionId, delegate(SimpleAPIStatus status, Error error3, Transaction authTransaction)
			{
				completeCB(status, error3, authTransaction);
			});
		}

		private static void continueTransaction(string transactionId, Action<Transaction> successCB, Action cancelCB, Action<string, Error> errorCB)
		{
			if (string.IsNullOrEmpty(transactionId))
			{
				Error error = new Error();
				error.domain = "com.mobage.error.api";
				error.code = 20003;
				Error arg = error;
				errorCB(transactionId, arg);
			}
			Action<Transaction> onSuccessCB = delegate(Transaction transaction)
			{
				Debug.Log("Send bank event CONTINUETRANSACTIONOK");
				JsonData jsonData = new JsonData();
				jsonData[BankEvent.Evpi_transactionid] = transactionId;
				jsonData[BankEvent.Evpi_sku] = string.Empty;
				BankEvent analyticsEvent = new BankEvent("CONTINUETRANSACTIONOK", jsonData);
				Mobage.sharedInstance.AnalyticsSession.Report(analyticsEvent);
				unlockTransactionLock();
				successCB(transaction);
			};
			Action<string, Error> onErrorCB = delegate(string tid, Error error2)
			{
				Debug.LogError("Mobage: BankDebit continueTransaction error " + error2.localizedDescription);
				errorCB(tid, error2);
			};
			Action onCancelCB = delegate
			{
				Debug.Log("Send bank event CONTINUETRANSACTIONCANCEL");
				JsonData jsonData = new JsonData();
				jsonData[BankEvent.Evpi_transactionid] = transactionId;
				jsonData[BankEvent.Evpi_sku] = string.Empty;
				BankEvent analyticsEvent = new BankEvent("CONTINUETRANSACTIONCANCEL", jsonData);
				Mobage.sharedInstance.AnalyticsSession.Report(analyticsEvent);
				unlockTransactionLock();
				cancelCB();
			};
			Action<int, string, string> getBalanceSuccessCb = delegate(int balance, string currency, string currencyIcon)
			{
				getTransaction(transactionId, delegate(Transaction transaction)
				{
					BillingItem billingItem = transaction.items[0];
					ItemData item = billingItem.item;
					string name = item.name;
					int price = item.price;
					Debug.Log(string.Format("item / price {0} {1}", name, price));
					string empty = string.Empty;
					if (string.IsNullOrEmpty(name))
					{
						Debug.Log("Mobage/Debit: cb for continueTxn unavailableitem = " + billingItem.item.itemId);
						Error arg2 = new Error
						{
							domain = "com.mobage.error.api",
							code = 20003
						};
						errorCB(transactionId, arg2);
					}
					int num = item.price * billingItem.quantity;
					if (billingItem.quantity == 1)
					{
						empty = item.name;
					}
					else
					{
						empty = empty + " " + billingItem.quantity + " " + item.name;
					}
					if (balance >= num)
					{
						onSuccessCB(transaction);
					}
					else
					{
						Debug.Log(string.Format("DebitAPI: insufficient balance: {0} requested: {1}", balance, num));
						onCancelCB();
						BankEvent analyticsEvent = new BankEvent("BankNeedMobaDlgShow", null);
						Mobage.sharedInstance.AnalyticsSession.Report(analyticsEvent);
						unlockTransactionLock();
					}
				}, delegate(Error arg2)
				{
					unlockTransactionLock();
					onErrorCB(transactionId, arg2);
				});
			};
			Action<Error> getBalanceErrorCb = delegate(Error arg2)
			{
				unlockTransactionLock();
				onErrorCB(transactionId, arg2);
			};
			BankBalance.GetBalanceWithCallback(delegate(SimpleAPIStatus status, Error obj, int balance, string currency, string currencyIcon)
			{
				if (status == SimpleAPIStatus.Success)
				{
					getBalanceSuccessCb(balance, currency, currencyIcon);
				}
				else
				{
					getBalanceErrorCb(obj);
				}
			});
		}

		private static void cancelTransaction(string transactionId, Action<Transaction> successCB, Action<Error> errorCB)
		{
			if (string.IsNullOrEmpty(transactionId))
			{
				Error error = new Error();
				error.domain = "com.mobage.error.api";
				error.code = 20003;
				Error obj = error;
				errorCB(obj);
			}
			Action<Transaction> onSuccessCB = delegate(Transaction transaction)
			{
				Debug.Log("Send bank event CANCELTRANSACTIONOK");
				JsonData jsonData = new JsonData();
				jsonData[BankEvent.Evpi_transactionid] = transactionId;
				BillingItem billingItem = transaction.items.Last();
				jsonData[BankEvent.Evpi_sku] = billingItem.item.itemId;
				BankEvent analyticsEvent = new BankEvent("CANCELTRANSACTIONOK", jsonData);
				Mobage.sharedInstance.AnalyticsSession.Report(analyticsEvent);
				successCB(transaction);
			};
			OnErrorBlock onErrorCB = delegate(Error obj2)
			{
				errorCB(obj2);
			};
			Transaction.Cancel(transactionId, ItemType.Debit, delegate(string tid, TransactionState state)
			{
				getTransaction(tid, onSuccessCB, onErrorCB);
			}, onErrorCB);
			unlockTransactionLock();
		}

		private static void closeTransaction(string transactionId, Action<Transaction> successCB, Action<Error> errorCB)
		{
			Action<Transaction> onSuccessCB = delegate(Transaction transaction)
			{
				Debug.Log("Send bank event CLOSETRANSACTIONOK");
				JsonData jsonData = new JsonData();
				jsonData[BankEvent.Evpi_transactionid] = transaction.transactionId;
				BillingItem billingItem = transaction.items.Last();
				jsonData[BankEvent.Evpi_sku] = billingItem.item.itemId;
				BankEvent analyticsEvent = new BankEvent("CLOSETRANSACTIONOK", jsonData);
				Mobage.sharedInstance.AnalyticsSession.Report(analyticsEvent);
				successCB(transaction);
				SocialService.BalanceUpdatePost();
			};
			OnErrorBlock onErrorCB = delegate(Error error)
			{
				Debug.Log("Send bank event CLOSETRANSACTIONFAIL");
				JsonData jsonData = new JsonData();
				jsonData[BankEvent.Evpi_transactionid] = transactionId;
				jsonData[BankEvent.Evpi_sku] = string.Empty;
				jsonData[BankEvent.Evpi_err] = error.ToString();
				BankEvent analyticsEvent = new BankEvent("CLOSETRANSACTIONFAIL", jsonData);
				Mobage.sharedInstance.AnalyticsSession.Report(analyticsEvent);
				errorCB(error);
			};
			Transaction.Close(transactionId, ItemType.Debit, delegate(string tid, TransactionState state)
			{
				getTransaction(tid, onSuccessCB, onErrorCB);
			}, onErrorCB);
		}

		private static void openTransaction(string transactionId, Action<Transaction> successCB, Action<Error> errorCB)
		{
			OnErrorBlock onErrorCB = delegate(Error error)
			{
				errorCB(error);
			};
			BankBalance.GetBalanceWithCallback(delegate(SimpleAPIStatus status, Error error, int balance, string currency, string currencyIcon)
			{
				Debug.Log("originalBalance balance is " + balance);
				int originalBalance = 0;
				if (status == SimpleAPIStatus.Success)
				{
					originalBalance = balance;
				}
				Transaction.Open(transactionId, ItemType.Debit, delegate(string tid, TransactionState state)
				{
					getTransaction(tid, delegate(Transaction transaction)
					{
						Debug.Log("Send bank event OPENTRANSACTIONOK");
						JsonData jsonData = new JsonData();
						jsonData[BankEvent.Evpi_transactionid] = transaction.transactionId;
						BillingItem billingItem = transaction.items.Last();
						jsonData[BankEvent.Evpi_sku] = billingItem.item.itemId;
						BankEvent analyticsEvent = new BankEvent("OPENTRANSACTIONOK", jsonData);
						Mobage.sharedInstance.AnalyticsSession.Report(analyticsEvent);
						BankBalance.GetBalanceWithCallback(delegate(SimpleAPIStatus stat, Error err, int bal, string cur, string curIcon)
						{
							Debug.Log("new balance is " + balance);
							if (stat == SimpleAPIStatus.Success && originalBalance > bal)
							{
								SocialService.BalanceUpdatePost();
								Debug.Log("currency delivery successfully. Sending confirmation event to analytics server.");
								currencyDeliveryConfirmationAnalyticsEvent(err, tid, cur, originalBalance, bal);
							}
						});
						successCB(transaction);
					}, onErrorCB);
				}, onErrorCB);
			});
		}

		public static void CreateTransaction(BillingItem billingItem, string comment, Action<Transaction> successCB, Action cancelCB, Action<string, Error> errorCB)
		{
			BankInventory.getItemForId(billingItem.item.itemId, delegate(SimpleAPIStatus status, Error error, ItemData itemData)
			{
				if (status == SimpleAPIStatus.Success)
				{
					createTransactionGetItemSuccessCB(billingItem, itemData, comment, successCB, cancelCB, errorCB);
				}
				else
				{
					errorCB(null, error);
				}
			});
		}

		private static void createTransactionGetItemSuccessCB(BillingItem billingItem, ItemData itemData, string comment, Action<Transaction> successCB, Action cancelCB, Action<string, Error> errorCB)
		{
			BankBalance.GetBalanceWithCallback(delegate(SimpleAPIStatus status, Error error, int balance, string currency, string currencyIcon)
			{
				if (status == SimpleAPIStatus.Success)
				{
					createTransactionGetBalanceSuccessCB(billingItem, itemData, balance, comment, currency, currencyIcon, successCB, cancelCB, errorCB);
				}
				else
				{
					errorCB(null, error);
				}
			});
		}

		private static void createTransactionGetBalanceSuccessCB(BillingItem billingItem, ItemData itemData, int balance, string comment, string currency, string currencyIcon, Action<Transaction> successCB, Action cancelCB, Action<string, Error> errorCB)
		{
			string itemName = itemData.name;
			Debug.Log("Mobage/Purchase ItemName = " + itemName);
			int requestedAmt = itemData.price * billingItem.quantity;
			Debug.Log("original balance is " + requestedAmt);
			if (balance < requestedAmt)
			{
				return;
			}
			Debug.Log(string.Format("DebitAPI: sufficient balance: {0} requested: {1}", balance, requestedAmt));
			Debug.Log("Mobage/Debit: Transaction.create - invoked");
			Transaction.Create(billingItem, comment, delegate(SimpleAPIStatus status, Error error, string transactionId)
			{
				if (status == SimpleAPIStatus.Success)
				{
					string empty = string.Empty;
					if (billingItem.quantity != 1)
					{
						throw new NotImplementedException();
					}
					empty = string.Format("{0} for {1} {2}?", itemData.name, requestedAmt, currency);
					Action<Transaction> successCB2 = delegate(Transaction transaction)
					{
						createTransactionSuccessAnalyticsEvent(transactionId, itemData);
						unlockTransactionLock();
						Debug.Log("Mobage/Debit: Transaction.create success");
						successCB(transaction);
					};
					Action cancelCB2 = delegate
					{
						createTransactionCancelAnalyticsEvent(transactionId, itemData);
						unlockTransactionLock();
						cancelCB();
					};
					Action<string, Error> errorCB2 = delegate(string tid, Error err)
					{
						Debug.LogError("Mobage: Transaction.create error");
						errorCB(tid, err);
					};
					showConfirmDialog(empty, transactionId, balance, currency, itemName, requestedAmt, successCB2, cancelCB2, errorCB2);
				}
				else
				{
					errorCB(transactionId, error);
				}
			});
		}

		private static void showConfirmDialog(string text, string transactionId, int balance, string currency, string itemName, int requestedAmt, Action<Transaction> successCB, Action cancelCB, Action<string, Error> errorCB)
		{
			Action cancelCB2 = delegate
			{
				Transaction.Cancel(transactionId, ItemType.Debit, delegate
				{
					Debug.Log("Mobage/Debit: cb for createTxn userrefused");
					cancelCB();
				}, delegate(Error error)
				{
					errorCB(transactionId, error);
				});
			};
			Action confirmCB = delegate
			{
				Transaction.Authorize(transactionId, ItemType.Debit, delegate(string tid, TransactionState state)
				{
					getTransaction(tid, successCB, delegate(Error error)
					{
						errorCB(tid, error);
					});
				}, delegate(Error error)
				{
					errorCB(transactionId, error);
				});
			};
			dialogHandler.showConfirmDialog(text, transactionId, balance, currency, itemName, requestedAmt, cancelCB2, confirmCB);
		}

		private void showConfirmDialog(string text, string transactionId, int balance, string currency, string itemName, int requestedAmt, Action cancelCB, Action confirmCB)
		{
			if (balance >= requestedAmt)
			{
				string text2 = text;
				if (balance > 0 && !string.IsNullOrEmpty(currency))
				{
					text2 = string.Format("{0}\n{1} {2} {3}", text, "Balance:", balance, currency);
				}
				string title = string.Format("Are you sure you want to purchase {0} for {1} {2}?", itemName, requestedAmt, currency);
				if (dialog.DisplayDialog(title, text2, "Yes", "No"))
				{
					confirmCB();
				}
				else
				{
					cancelCB();
				}
			}
		}

		private static void getTransaction(string transactionId, Action<Transaction> successCB, OnErrorBlock errorCB)
		{
			MobageRequest mobageRequest = MobageRequest.NewBankRequestWithContext(MobageSession.CurrentSession);
			mobageRequest.APIMethod = "bank/debit/@app/" + transactionId;
			mobageRequest.HTTPMethod = "GET";
			mobageRequest.QueryString = new Dictionary<string, object> { { "fields", "id,items,comment,state,published,updated" } };
			mobageRequest.Send(delegate(Error err, JsonData data, WebHeaderCollection headers, HttpStatusCode status)
			{
				if (data != null && err == null)
				{
					Transaction obj = JsonMapper.ToObject<Transaction>(data.ToJson());
					successCB(obj);
				}
				else
				{
					errorCB(err);
				}
			});
		}

		private static void createTransactionSuccessAnalyticsEvent(string transactionId, ItemData itemData)
		{
			string text = "CREATETRANSACTIONOK";
			Debug.Log("Send bank event " + text);
			JsonData jsonData = new JsonData();
			jsonData[BankEvent.Evpi_transactionid] = transactionId;
			jsonData[BankEvent.Evpi_sku] = itemData.itemId;
			BankEvent analyticsEvent = new BankEvent(text, jsonData);
			Mobage.sharedInstance.AnalyticsSession.Report(analyticsEvent);
		}

		private static void createTransactionCancelAnalyticsEvent(string transactionId, ItemData itemData)
		{
			Debug.Log("Send bank event CREATETRANSACTIONCANCEL");
			JsonData jsonData = new JsonData();
			jsonData[BankEvent.Evpi_transactionid] = transactionId;
			jsonData[BankEvent.Evpi_sku] = itemData.itemId;
			BankEvent analyticsEvent = new BankEvent("CREATETRANSACTIONCANCEL", jsonData);
			Mobage.sharedInstance.AnalyticsSession.Report(analyticsEvent);
		}

		private static void unlockTransactionLock()
		{
			Debug.Log("Open/Unlock the transactionLock.");
		}

		private static void currencyDeliveryConfirmationAnalyticsEvent(Error error, string transactionId, string vcur, int oldBalance, int newBalance)
		{
			Debug.Log("Send bank event MOBAIAPDELIVEROK");
			JsonData jsonData = new JsonData();
			jsonData[BankEvent.Evpi_transactionid] = transactionId;
			jsonData[BankEvent.Evpi_vcur] = vcur;
			jsonData[BankEvent.Evpi_oldbalance] = oldBalance.ToString();
			jsonData[BankEvent.Evpi_newbalance] = newBalance.ToString();
			BankEvent analyticsEvent = new BankEvent("MOBAIAPDELIVEROK", jsonData);
			Mobage.sharedInstance.AnalyticsSession.Report(analyticsEvent);
		}

		private static void authorizeTransaction(string transactionId, Action<SimpleAPIStatus, Error, Transaction> successCB)
		{
			Transaction.Authorize(transactionId, ItemType.Debit, delegate(string tid, TransactionState state)
			{
				getTransaction(tid, delegate(Transaction transaction)
				{
					successCB(SimpleAPIStatus.Success, null, transaction);
				}, delegate
				{
					Error arg = new Error
					{
						domain = "com.mobage.error.api",
						code = 10004
					};
					successCB(SimpleAPIStatus.Error, arg, null);
				});
			}, delegate
			{
				Error arg = new Error
				{
					domain = "com.mobage.error.api",
					code = 20004
				};
				successCB(SimpleAPIStatus.Error, arg, null);
			});
		}
	}
}

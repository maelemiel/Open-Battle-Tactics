using System.Collections.Generic;
using LCD.Internal.Impl;
using LCD.Internal.Web;

namespace LCD.Bank
{
	public class Wallet
	{
		public delegate void WalletCallback(Wallet wallet, LCDError error);

		public readonly int balance;

		public readonly string currency;

		internal Wallet(int balance, string currency)
		{
			this.balance = balance;
			this.currency = currency;
		}

		public static void GetCurrentBalance(WalletCallback callback)
		{
			SDKWebUtil.Execute("GET", "/bank/balance", null, new Dictionary<string, object>(), new WalletCallbackImpl(callback));
		}
	}
}

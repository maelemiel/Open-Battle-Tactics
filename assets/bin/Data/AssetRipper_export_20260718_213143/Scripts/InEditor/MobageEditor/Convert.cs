using System;
using System.Runtime.InteropServices;

namespace MobageEditor
{
	public class Convert
	{
		public static bool IsMobageFacebookLoginStatus(int intFlag)
		{
			return intFlag >= 0 && intFlag <= 4;
		}

		public static int toC(MobageFacebookLoginStatus enumValue)
		{
			return (int)enumValue;
		}

		public static MobageFacebookLoginStatus toCS_MobageFacebookLoginStatus(int enumValue)
		{
			return (MobageFacebookLoginStatus)enumValue;
		}

		public static bool IsTransactionState(int intFlag)
		{
			return intFlag >= 0 && intFlag <= 6;
		}

		public static int toC(TransactionState enumValue)
		{
			return (int)enumValue;
		}

		public static TransactionState toCS_TransactionState(int enumValue)
		{
			return (TransactionState)enumValue;
		}

		public static bool IsStandardError(int intFlag)
		{
			return intFlag >= 10001 && intFlag <= 10007;
		}

		public static int toC(StandardError enumValue)
		{
			return (int)enumValue;
		}

		public static StandardError toCS_StandardError(int enumValue)
		{
			return (StandardError)enumValue;
		}

		public static bool IsHTTPError(int intFlag)
		{
			return intFlag >= 109 && intFlag <= 599;
		}

		public static int toC(HTTPError enumValue)
		{
			return (int)enumValue;
		}

		public static HTTPError toCS_HTTPError(int enumValue)
		{
			return (HTTPError)enumValue;
		}

		public static bool IsCommonAPIError(int intFlag)
		{
			return intFlag >= 20001 && intFlag <= 20005;
		}

		public static int toC(CommonAPIError enumValue)
		{
			return (int)enumValue;
		}

		public static CommonAPIError toCS_CommonAPIError(int enumValue)
		{
			return (CommonAPIError)enumValue;
		}

		public static bool IsAnalyticsServerError(int intFlag)
		{
			return intFlag >= 30001 && intFlag <= 30005;
		}

		public static int toC(AnalyticsServerError enumValue)
		{
			return (int)enumValue;
		}

		public static AnalyticsServerError toCS_AnalyticsServerError(int enumValue)
		{
			return (AnalyticsServerError)enumValue;
		}

		public static bool IsBankError(int intFlag)
		{
			return intFlag >= 40001 && intFlag <= 40001;
		}

		public static int toC(BankError enumValue)
		{
			return (int)enumValue;
		}

		public static BankError toCS_BankError(int enumValue)
		{
			return (BankError)enumValue;
		}

		public static bool IsMobageAPIErrorType(int intFlag)
		{
			return intFlag >= 109 && intFlag <= 40001;
		}

		public static int toC(MobageAPIErrorType enumValue)
		{
			return (int)enumValue;
		}

		public static MobageAPIErrorType toCS_MobageAPIErrorType(int enumValue)
		{
			return (MobageAPIErrorType)enumValue;
		}

		public static bool IsSimpleAPIStatus(int intFlag)
		{
			return intFlag >= 0 && intFlag <= 1;
		}

		public static int toC(SimpleAPIStatus enumValue)
		{
			return (int)enumValue;
		}

		public static SimpleAPIStatus toCS_SimpleAPIStatus(int enumValue)
		{
			return (SimpleAPIStatus)enumValue;
		}

		public static bool IsDismissableAPIStatus(int intFlag)
		{
			return intFlag >= 0 && intFlag <= 2;
		}

		public static int toC(DismissableAPIStatus enumValue)
		{
			return (int)enumValue;
		}

		public static DismissableAPIStatus toCS_DismissableAPIStatus(int enumValue)
		{
			return (DismissableAPIStatus)enumValue;
		}

		public static bool IsCancelableAPIStatus(int intFlag)
		{
			return intFlag >= 0 && intFlag <= 2;
		}

		public static int toC(CancelableAPIStatus enumValue)
		{
			return (int)enumValue;
		}

		public static CancelableAPIStatus toCS_CancelableAPIStatus(int enumValue)
		{
			return (CancelableAPIStatus)enumValue;
		}

		public static IntPtr toC(string str)
		{
			return MarshalPtrToUtf8.GetInstance(string.Empty).MarshalManagedToNative(str);
		}

		public static string toCS_String(IntPtr obj)
		{
			return Marshal.PtrToStringAnsi(obj);
		}

		public static double toCS_Double(double obj)
		{
			return obj;
		}

		public static int toCS_Integer(int obj)
		{
			return obj;
		}

		public static bool toCS_Bool(short obj)
		{
			return (obj != 0) ? true : false;
		}

		public static short toC_Bool(bool obj)
		{
			return (short)(obj ? 1 : 0);
		}
	}
}

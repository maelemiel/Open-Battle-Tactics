using System;

namespace MobageEditor
{
	public static class ActionExt
	{
		public static string GetValue(this MobageWeb.Action action)
		{
			switch (action)
			{
			case MobageWeb.Action.LOGIN:
				return "login";
			case MobageWeb.Action.LOGOUT:
				return "logout";
			case MobageWeb.Action.USER_UPGRADE:
				return "userUpgrade";
			case MobageWeb.Action.BANK:
				return "bank";
			case MobageWeb.Action.BANK_PURCHASE:
				return "purchase";
			case MobageWeb.Action.DEBIT:
				return "debit";
			case MobageWeb.Action.PROFILE:
				return "profile";
			case MobageWeb.Action.COMMUNITY:
				return "community";
			case MobageWeb.Action.PROMOTION:
				return "promotions";
			case MobageWeb.Action.CHECK_FACEBOOK:
				return "checkFacebook";
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
	}
}

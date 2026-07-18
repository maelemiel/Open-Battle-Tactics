namespace MobageEditor
{
	public enum MobageAPIErrorType
	{
		ServerDown = 109,
		UpgradeRequired = 110,
		UserBanned = 111,
		AgreementNeeded = 112,
		BadRequest = 400,
		RecordNotFound = 404,
		Unauthorized = 401,
		PermissionDenied = 403,
		ServerError = 10001,
		NetworkUnavailable = 10002,
		MissingData = 10003,
		InvalidData = 10004,
		UnknownError = 10005,
		ParseError = 10006,
		NoAuthToken = 10007,
		CommonAPIInvalidSessionError = 20001,
		CommonAPIMethodMissingArgumentError = 20002,
		CommonAPIMethodInvalidArgumentError = 20003,
		CommonAPIMethodNotImplementedError = 20004,
		CommonAPIMethodNotSupportedError = 20005,
		AnalyticsServerInvalidResponse = 30001,
		AnalyticsServerEventRejected = 30002,
		AnalyticsServerEventSizeTooLarge = 30003,
		AnalyticsServerEventPropertySizeTooLarge = 30004,
		AnalyticsServerEventContainsInvalidArray = 30005,
		BankErrorInvalidStateTransition = 40001
	}
}

namespace MobageEditor
{
	public enum HTTPError
	{
		ServerDown = 109,
		UpgradeRequired = 110,
		UserBanned = 111,
		AgreementNeeded = 112,
		BadRequest = 400,
		RecordNotFound = 404,
		Unauthorized = 401,
		PermissionDenied = 403,
		FirstInternalServerError = 500,
		LastHTTPError = 599
	}
}

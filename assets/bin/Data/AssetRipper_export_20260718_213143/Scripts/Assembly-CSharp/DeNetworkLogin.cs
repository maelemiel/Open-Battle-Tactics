using System;

public interface DeNetworkLogin
{
	void Login(Action<DeNetworkError> done);

	void Authorize(string requestToken, Action<DeNetworkError, string> callback);

	void TemporaryTokenRequest(RestRequest request, Action<DeNetworkError, string> tokenCallback);

	void TokenRequest(RestRequest request, string verifier, Action<DeNetworkError> callback);
}

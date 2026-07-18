using System;
using System.Collections.Generic;

public interface HTTPHandler
{
	void Body(string body);

	void Path(string path);

	void Header(string key, string value);

	Dictionary<string, string> Headers();

	void Param(string key, string value);

	Dictionary<string, string> Parameters();

	void Timeout(int timeout);

	void Retries(int retries);

	void HighPriority(bool highPriority);

	void Exec(string method, Action<object, object> callback);

	void Cancel();
}

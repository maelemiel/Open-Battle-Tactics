public interface INotificationChecker
{
	void Init();

	void CheckConditions();

	void SendNotification();
}

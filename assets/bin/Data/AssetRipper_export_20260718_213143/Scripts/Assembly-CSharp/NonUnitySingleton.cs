public class NonUnitySingleton<T> where T : class, new()
{
	protected static T _instance;

	public static T instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new T();
			}
			return _instance;
		}
	}

	public static void Instantiate()
	{
		_instance = instance;
	}
}

using UnityEngine;

namespace MobageEditor
{
	public class MobageLogger : MonoBehaviour
	{
		public static bool LoggingEnabled = true;

		public static void exceptionLog(object obj)
		{
			if (LoggingEnabled)
			{
				MonoBehaviour.print("MBUnityLogger[GameException]: " + obj);
			}
		}

		public static void log(object obj)
		{
			if (LoggingEnabled)
			{
				MonoBehaviour.print("MBUnityLogger[Log]: " + obj);
			}
		}
	}
}

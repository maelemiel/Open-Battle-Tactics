using System;

namespace UnityEngine.Events
{
	[Serializable]
	internal enum PersistentListenerMode
	{
		EventDefined = 0,
		Void = 1,
		Object = 2,
		Int = 3,
		Float = 4,
		String = 5
	}
}

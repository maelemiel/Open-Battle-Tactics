using System;
using System.Diagnostics;

namespace UnityEngine
{
	[Conditional("UNITY_WINRT")]
	[Conditional("UNITY_FLASH")]
	internal class MarshallOnlyFirstField : Attribute
	{
	}
}

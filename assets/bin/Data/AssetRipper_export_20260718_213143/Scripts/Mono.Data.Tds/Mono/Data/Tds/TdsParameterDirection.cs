using System;

namespace Mono.Data.Tds
{
	[Serializable]
	public enum TdsParameterDirection
	{
		Input = 0,
		Output = 1,
		InputOutput = 2,
		ReturnValue = 3
	}
}

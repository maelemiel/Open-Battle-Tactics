using System;
using System.Runtime.Serialization;

namespace Microsoft.SqlServer.Server
{
	[Serializable]
	public sealed class InvalidUdtException : SystemException
	{
		[System.MonoTODO]
		internal InvalidUdtException()
		{
		}

		[System.MonoTODO]
		internal InvalidUdtException(string message)
		{
		}

		[System.MonoTODO]
		internal InvalidUdtException(string message, Exception innerException)
		{
		}

		[System.MonoTODO]
		internal InvalidUdtException(Type t, string reason)
		{
		}

		[System.MonoTODO]
		public override void GetObjectData(SerializationInfo si, StreamingContext context)
		{
		}
	}
}

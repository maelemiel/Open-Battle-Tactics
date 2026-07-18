using System.Runtime.Serialization;

namespace System.Security.AccessControl
{
	[Serializable]
	public sealed class PrivilegeNotHeldException : UnauthorizedAccessException, ISerializable
	{
		public string PrivilegeName
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public PrivilegeNotHeldException()
		{
		}

		public PrivilegeNotHeldException(string privilege)
			: base(privilege)
		{
		}

		public PrivilegeNotHeldException(string privilege, Exception inner)
			: base(privilege, inner)
		{
		}

		[MonoTODO]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			throw new NotImplementedException();
		}
	}
}

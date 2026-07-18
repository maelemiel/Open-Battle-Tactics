using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[Serializable]
	[ComVisible(false)]
	public sealed class ApplicationIdentity : ISerializable
	{
		private string _fullName;

		private string _codeBase;

		public string CodeBase
		{
			get
			{
				return _codeBase;
			}
		}

		public string FullName
		{
			get
			{
				return _fullName;
			}
		}

		public ApplicationIdentity(string applicationIdentityFullName)
		{
			if (applicationIdentityFullName == null)
			{
				throw new ArgumentNullException("applicationIdentityFullName");
			}
			if (applicationIdentityFullName.IndexOf(", Culture=") == -1)
			{
				_fullName = applicationIdentityFullName + ", Culture=neutral";
			}
			else
			{
				_fullName = applicationIdentityFullName;
			}
		}

		[MonoTODO("Missing serialization")]
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
		}

		public override string ToString()
		{
			return _fullName;
		}
	}
}

using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Security.Principal
{
	[Serializable]
	[ComVisible(false)]
	public sealed class IdentityNotMappedException : SystemException
	{
		private IdentityReferenceCollection _coll;

		public IdentityReferenceCollection UnmappedIdentities
		{
			get
			{
				if (_coll == null)
				{
					_coll = new IdentityReferenceCollection();
				}
				return _coll;
			}
		}

		public IdentityNotMappedException()
			: base(Locale.GetText("Couldn't translate some identities."))
		{
		}

		public IdentityNotMappedException(string message)
			: base(message)
		{
		}

		public IdentityNotMappedException(string message, Exception inner)
			: base(message, inner)
		{
		}

		[MonoTODO("not implemented")]
		public override void GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
		}
	}
}

using System.Runtime.Serialization;

namespace System.ComponentModel
{
	[Serializable]
	public class LicenseException : SystemException
	{
		private Type type;

		public Type LicensedType
		{
			get
			{
				return type;
			}
		}

		public LicenseException(Type type)
			: this(type, null)
		{
		}

		public LicenseException(Type type, object instance)
		{
			this.type = type;
		}

		public LicenseException(Type type, object instance, string message)
			: this(type, instance, message, null)
		{
		}

		public LicenseException(Type type, object instance, string message, Exception innerException)
			: base(message, innerException)
		{
			this.type = type;
		}

		protected LicenseException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			type = (Type)info.GetValue("LicensedType", typeof(Type));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			info.AddValue("LicensedType", type);
			base.GetObjectData(info, context);
		}
	}
}

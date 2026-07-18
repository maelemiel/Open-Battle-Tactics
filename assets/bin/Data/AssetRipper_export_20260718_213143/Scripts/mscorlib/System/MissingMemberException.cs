using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[Serializable]
	[ComVisible(true)]
	public class MissingMemberException : MemberAccessException
	{
		private const int Result = -2146233070;

		protected string ClassName;

		protected string MemberName;

		protected byte[] Signature;

		public override string Message
		{
			get
			{
				if (ClassName == null)
				{
					return base.Message;
				}
				string text = Locale.GetText("Member {0}.{1} not found.");
				return string.Format(text, ClassName, MemberName);
			}
		}

		public MissingMemberException()
			: base(Locale.GetText("Cannot find the requested class member."))
		{
			base.HResult = -2146233070;
		}

		public MissingMemberException(string message)
			: base(message)
		{
			base.HResult = -2146233070;
		}

		public MissingMemberException(string message, Exception inner)
			: base(message, inner)
		{
			base.HResult = -2146233070;
		}

		protected MissingMemberException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			ClassName = info.GetString("MMClassName");
			MemberName = info.GetString("MMMemberName");
			Signature = (byte[])info.GetValue("MMSignature", typeof(byte[]));
		}

		public MissingMemberException(string className, string memberName)
		{
			ClassName = className;
			MemberName = memberName;
			base.HResult = -2146233070;
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("MMClassName", ClassName);
			info.AddValue("MMMemberName", MemberName);
			info.AddValue("MMSignature", Signature);
		}
	}
}

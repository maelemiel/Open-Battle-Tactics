using System.Runtime.Serialization;

namespace System.Runtime.CompilerServices
{
	[Serializable]
	public sealed class RuntimeWrappedException : Exception
	{
		private object wrapped_exception;

		public object WrappedException
		{
			get
			{
				return wrapped_exception;
			}
		}

		private RuntimeWrappedException()
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("WrappedException", wrapped_exception);
		}
	}
}

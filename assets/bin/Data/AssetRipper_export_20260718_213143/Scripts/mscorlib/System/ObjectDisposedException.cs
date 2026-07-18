using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[Serializable]
	[ComVisible(true)]
	public class ObjectDisposedException : InvalidOperationException
	{
		private string obj_name;

		private string msg;

		public override string Message
		{
			get
			{
				return msg;
			}
		}

		public string ObjectName
		{
			get
			{
				return obj_name;
			}
		}

		public ObjectDisposedException(string objectName)
			: base(Locale.GetText("The object was used after being disposed."))
		{
			obj_name = objectName;
			msg = Locale.GetText("The object was used after being disposed.");
		}

		public ObjectDisposedException(string objectName, string message)
			: base(message)
		{
			obj_name = objectName;
			msg = message;
		}

		public ObjectDisposedException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected ObjectDisposedException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			obj_name = info.GetString("ObjectName");
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("ObjectName", obj_name);
		}
	}
}

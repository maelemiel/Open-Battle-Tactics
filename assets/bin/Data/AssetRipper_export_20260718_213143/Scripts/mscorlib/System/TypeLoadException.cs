using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[Serializable]
	[ComVisible(true)]
	public class TypeLoadException : SystemException
	{
		private const int Result = -2146233054;

		private string className;

		private string assemblyName;

		public override string Message
		{
			get
			{
				if (className != null)
				{
					if (assemblyName != null && assemblyName != string.Empty)
					{
						return string.Format("Could not load type '{0}' from assembly '{1}'.", className, assemblyName);
					}
					return string.Format("Could not load type '{0}'.", className);
				}
				return base.Message;
			}
		}

		public string TypeName
		{
			get
			{
				if (className == null)
				{
					return string.Empty;
				}
				return className;
			}
		}

		public TypeLoadException()
			: base(Locale.GetText("A type load exception has occurred."))
		{
			base.HResult = -2146233054;
		}

		public TypeLoadException(string message)
			: base(message)
		{
			base.HResult = -2146233054;
		}

		public TypeLoadException(string message, Exception inner)
			: base(message, inner)
		{
			base.HResult = -2146233054;
		}

		internal TypeLoadException(string className, string assemblyName)
			: this()
		{
			this.className = className;
			this.assemblyName = assemblyName;
		}

		protected TypeLoadException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			className = info.GetString("TypeLoadClassName");
			assemblyName = info.GetString("TypeLoadAssemblyName");
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			base.GetObjectData(info, context);
			info.AddValue("TypeLoadClassName", className, typeof(string));
			info.AddValue("TypeLoadAssemblyName", assemblyName, typeof(string));
			info.AddValue("TypeLoadMessageArg", string.Empty, typeof(string));
			info.AddValue("TypeLoadResourceID", 0, typeof(int));
		}
	}
}

using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Reflection
{
	[Serializable]
	[ComVisible(true)]
	public sealed class ReflectionTypeLoadException : SystemException
	{
		private Exception[] loaderExceptions;

		private Type[] types;

		public Type[] Types
		{
			get
			{
				return types;
			}
		}

		public Exception[] LoaderExceptions
		{
			get
			{
				return loaderExceptions;
			}
		}

		public ReflectionTypeLoadException(Type[] classes, Exception[] exceptions)
			: base(Locale.GetText("The classes in the module cannot be loaded."))
		{
			loaderExceptions = exceptions;
			types = classes;
		}

		public ReflectionTypeLoadException(Type[] classes, Exception[] exceptions, string message)
			: base(message)
		{
			loaderExceptions = exceptions;
			types = classes;
		}

		private ReflectionTypeLoadException(SerializationInfo info, StreamingContext sc)
			: base(info, sc)
		{
			types = (Type[])info.GetValue("Types", typeof(Type[]));
			loaderExceptions = (Exception[])info.GetValue("Exceptions", typeof(Exception[]));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("Types", types);
			info.AddValue("Exceptions", loaderExceptions);
		}
	}
}

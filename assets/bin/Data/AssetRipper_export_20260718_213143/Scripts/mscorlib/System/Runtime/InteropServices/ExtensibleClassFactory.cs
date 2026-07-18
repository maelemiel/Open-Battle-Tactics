using System.Collections;
using System.Diagnostics;
using System.Reflection;

namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	public sealed class ExtensibleClassFactory
	{
		private static Hashtable hashtable;

		private ExtensibleClassFactory()
		{
		}

		static ExtensibleClassFactory()
		{
			hashtable = new Hashtable();
		}

		internal static ObjectCreationDelegate GetObjectCreationCallback(Type t)
		{
			return hashtable[t] as ObjectCreationDelegate;
		}

		public static void RegisterObjectCreationCallback(ObjectCreationDelegate callback)
		{
			int i = 1;
			for (StackTrace stackTrace = new StackTrace(false); i < stackTrace.FrameCount; i++)
			{
				StackFrame frame = stackTrace.GetFrame(i);
				MethodBase method = frame.GetMethod();
				if (method.MemberType == MemberTypes.Constructor && method.IsStatic)
				{
					hashtable.Add(method.DeclaringType, callback);
					return;
				}
			}
			throw new InvalidOperationException("RegisterObjectCreationCallback must be called from .cctor of class derived from ComImport type.");
		}
	}
}

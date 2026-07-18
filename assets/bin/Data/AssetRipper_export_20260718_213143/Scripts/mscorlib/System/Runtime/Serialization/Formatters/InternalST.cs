using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace System.Runtime.Serialization.Formatters
{
	[ComVisible(true)]
	public sealed class InternalST
	{
		private InternalST()
		{
		}

		[Conditional("_LOGGING")]
		public static void InfoSoap(params object[] messages)
		{
			throw new NotImplementedException();
		}

		public static Assembly LoadAssemblyFromString(string assemblyString)
		{
			throw new NotImplementedException();
		}

		public static void SerializationSetValue(FieldInfo fi, object target, object value)
		{
			throw new NotImplementedException();
		}

		[Conditional("SER_LOGGING")]
		public static void Soap(params object[] messages)
		{
			throw new NotImplementedException();
		}

		[Conditional("_DEBUG")]
		public static void SoapAssert(bool condition, string message)
		{
			throw new NotImplementedException();
		}

		public static bool SoapCheckEnabled()
		{
			throw new NotImplementedException();
		}
	}
}

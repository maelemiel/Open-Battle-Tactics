using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace UnityEngine
{
	internal sealed class UnityLogWriter : TextWriter
	{
		public override Encoding Encoding
		{
			get
			{
				return Encoding.UTF8;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern void WriteStringToUnityLog(string s);

		public static void Init()
		{
			Console.SetOut(new UnityLogWriter());
		}

		public override void Write(char value)
		{
			WriteStringToUnityLog(value.ToString());
		}

		public override void Write(string s)
		{
			WriteStringToUnityLog(s);
		}
	}
}

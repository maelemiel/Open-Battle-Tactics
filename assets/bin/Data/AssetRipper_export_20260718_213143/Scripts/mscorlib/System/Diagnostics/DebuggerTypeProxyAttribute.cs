using System.Runtime.InteropServices;

namespace System.Diagnostics
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
	public sealed class DebuggerTypeProxyAttribute : Attribute
	{
		private string proxy_type_name;

		private string target_type_name;

		private Type target_type;

		public string ProxyTypeName
		{
			get
			{
				return proxy_type_name;
			}
		}

		public Type Target
		{
			get
			{
				return target_type;
			}
			set
			{
				target_type = value;
				target_type_name = target_type.Name;
			}
		}

		public string TargetTypeName
		{
			get
			{
				return target_type_name;
			}
			set
			{
				target_type_name = value;
			}
		}

		public DebuggerTypeProxyAttribute(string typeName)
		{
			proxy_type_name = typeName;
		}

		public DebuggerTypeProxyAttribute(Type type)
		{
			proxy_type_name = type.Name;
		}
	}
}

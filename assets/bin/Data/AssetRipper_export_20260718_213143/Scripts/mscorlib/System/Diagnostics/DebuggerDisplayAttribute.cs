using System.Runtime.InteropServices;

namespace System.Diagnostics
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Delegate, AllowMultiple = true)]
	[ComVisible(true)]
	public sealed class DebuggerDisplayAttribute : Attribute
	{
		private string value;

		private string type;

		private string name;

		private string target_type_name;

		private Type target_type;

		public string Value
		{
			get
			{
				return value;
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
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				target_type = value;
				target_type_name = target_type.AssemblyQualifiedName;
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

		public string Type
		{
			get
			{
				return type;
			}
			set
			{
				type = value;
			}
		}

		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
			}
		}

		public DebuggerDisplayAttribute(string value)
		{
			if (value == null)
			{
				value = string.Empty;
			}
			this.value = value;
			type = string.Empty;
			name = string.Empty;
		}
	}
}

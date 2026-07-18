using System.Runtime.InteropServices;

namespace System.Security.Permissions
{
	[Serializable]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	[ComVisible(true)]
	public sealed class PermissionSetAttribute : CodeAccessSecurityAttribute
	{
		private string file;

		private string name;

		private bool isUnicodeEncoded;

		private string xml;

		private string hex;

		public string File
		{
			get
			{
				return file;
			}
			set
			{
				file = value;
			}
		}

		public string Hex
		{
			get
			{
				return hex;
			}
			set
			{
				hex = value;
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

		public bool UnicodeEncoded
		{
			get
			{
				return isUnicodeEncoded;
			}
			set
			{
				isUnicodeEncoded = value;
			}
		}

		public string XML
		{
			get
			{
				return xml;
			}
			set
			{
				xml = value;
			}
		}

		public PermissionSetAttribute(SecurityAction action)
			: base(action)
		{
		}

		public override IPermission CreatePermission()
		{
			return null;
		}

		private PermissionSet CreateFromXml(string xml)
		{
			return null;
		}

		public PermissionSet CreatePermissionSet()
		{
			return null;
		}
	}
}

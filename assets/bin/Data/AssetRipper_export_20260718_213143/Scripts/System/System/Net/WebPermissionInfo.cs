using System.Text.RegularExpressions;

namespace System.Net
{
	internal class WebPermissionInfo
	{
		private System.Net.WebPermissionInfoType _type;

		private object _info;

		public string Info
		{
			get
			{
				if (_type == System.Net.WebPermissionInfoType.InfoRegex)
				{
					return null;
				}
				return (string)_info;
			}
		}

		public WebPermissionInfo(System.Net.WebPermissionInfoType type, string info)
		{
			_type = type;
			_info = info;
		}

		public WebPermissionInfo(Regex regex)
		{
			_type = System.Net.WebPermissionInfoType.InfoRegex;
			_info = regex;
		}
	}
}

using System.Globalization;
using System.Security;
using System.Security.Permissions;

namespace System.Data.Common
{
	internal sealed class PermissionHelper
	{
		internal static SecurityElement Element(Type type, int version)
		{
			SecurityElement securityElement = new SecurityElement("IPermission");
			securityElement.AddAttribute("class", type.FullName + ", " + type.Assembly.ToString().Replace('"', '\''));
			securityElement.AddAttribute("version", version.ToString());
			return securityElement;
		}

		internal static PermissionState CheckPermissionState(PermissionState state, bool allowUnrestricted)
		{
			switch (state)
			{
			case PermissionState.Unrestricted:
				if (!allowUnrestricted)
				{
					string paramName = global::Locale.GetText("Unrestricted isn't not allowed for identity permissions.");
					throw new ArgumentException(paramName, "state");
				}
				break;
			default:
			{
				string paramName = string.Format(global::Locale.GetText("Invalid enum {0}"), state);
				throw new ArgumentOutOfRangeException(paramName, "state");
			}
			case PermissionState.None:
				break;
			}
			return state;
		}

		internal static int CheckSecurityElement(SecurityElement se, string parameterName, int minimumVersion, int maximumVersion)
		{
			if (se == null)
			{
				throw new ArgumentNullException(parameterName);
			}
			if (se.Tag != "IPermission")
			{
				string text = global::Locale.GetText("Invalid tag '{0}' expected 'IPermission'.");
				throw new ArgumentException(string.Format(text, se.Tag), parameterName);
			}
			int num = minimumVersion;
			string text2 = se.Attribute("version");
			if (text2 != null)
			{
				try
				{
					num = int.Parse(text2);
				}
				catch (Exception innerException)
				{
					string text3 = global::Locale.GetText("Couldn't parse version from '{0}'.");
					text3 = string.Format(text3, text2);
					throw new ArgumentException(text3, parameterName, innerException);
				}
			}
			if (num < minimumVersion || num > maximumVersion)
			{
				string text4 = global::Locale.GetText("Unknown version '{0}', expected versions between ['{1}','{2}'].");
				text4 = string.Format(text4, num, minimumVersion, maximumVersion);
				throw new ArgumentException(text4, parameterName);
			}
			return num;
		}

		internal static bool IsUnrestricted(SecurityElement se)
		{
			string text = se.Attribute("Unrestricted");
			if (text == null)
			{
				return false;
			}
			return string.Compare(text, bool.TrueString, true, CultureInfo.InvariantCulture) == 0;
		}

		internal static void ThrowInvalidPermission(IPermission target, Type expected)
		{
			string text = global::Locale.GetText("Invalid permission type '{0}', expected type '{1}'.");
			text = string.Format(text, target.GetType(), expected);
			throw new ArgumentException(text, "target");
		}
	}
}

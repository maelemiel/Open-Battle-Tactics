using System.Globalization;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace System.Security
{
	[Serializable]
	[MonoTODO("CAS support is experimental (and unsupported).")]
	[ComVisible(true)]
	public abstract class CodeAccessPermission : IPermission, ISecurityEncodable, IStackWalk
	{
		[MonoTODO("CAS support is experimental (and unsupported). Imperative mode is not implemented.")]
		public void Assert()
		{
		}

		internal bool CheckAssert(CodeAccessPermission asserted)
		{
			if (asserted == null)
			{
				return false;
			}
			if (asserted.GetType() != GetType())
			{
				return false;
			}
			return IsSubsetOf(asserted);
		}

		internal bool CheckDemand(CodeAccessPermission target)
		{
			if (target == null)
			{
				return false;
			}
			if (target.GetType() != GetType())
			{
				return false;
			}
			return IsSubsetOf(target);
		}

		internal bool CheckDeny(CodeAccessPermission denied)
		{
			if (denied == null)
			{
				return true;
			}
			Type type = denied.GetType();
			if (type != GetType())
			{
				return true;
			}
			IPermission permission = Intersect(denied);
			if (permission == null)
			{
				return true;
			}
			return denied.IsSubsetOf(PermissionBuilder.Create(type));
		}

		internal bool CheckPermitOnly(CodeAccessPermission target)
		{
			if (target == null)
			{
				return false;
			}
			if (target.GetType() != GetType())
			{
				return false;
			}
			return IsSubsetOf(target);
		}

		public abstract IPermission Copy();

		public void Demand()
		{
		}

		[MonoTODO("CAS support is experimental (and unsupported). Imperative mode is not implemented.")]
		public void Deny()
		{
		}

		[ComVisible(false)]
		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (obj.GetType() != GetType())
			{
				return false;
			}
			CodeAccessPermission codeAccessPermission = obj as CodeAccessPermission;
			return IsSubsetOf(codeAccessPermission) && codeAccessPermission.IsSubsetOf(this);
		}

		public abstract void FromXml(SecurityElement elem);

		[ComVisible(false)]
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public abstract IPermission Intersect(IPermission target);

		public abstract bool IsSubsetOf(IPermission target);

		public override string ToString()
		{
			SecurityElement securityElement = ToXml();
			return securityElement.ToString();
		}

		public abstract SecurityElement ToXml();

		public virtual IPermission Union(IPermission other)
		{
			if (other != null)
			{
				throw new NotSupportedException();
			}
			return null;
		}

		[MonoTODO("CAS support is experimental (and unsupported). Imperative mode is not implemented.")]
		public void PermitOnly()
		{
		}

		[MonoTODO("CAS support is experimental (and unsupported). Imperative mode is not implemented.")]
		public static void RevertAll()
		{
		}

		[MonoTODO("CAS support is experimental (and unsupported). Imperative mode is not implemented.")]
		public static void RevertAssert()
		{
		}

		[MonoTODO("CAS support is experimental (and unsupported). Imperative mode is not implemented.")]
		public static void RevertDeny()
		{
		}

		[MonoTODO("CAS support is experimental (and unsupported). Imperative mode is not implemented.")]
		public static void RevertPermitOnly()
		{
		}

		internal SecurityElement Element(int version)
		{
			SecurityElement securityElement = new SecurityElement("IPermission");
			Type type = GetType();
			securityElement.AddAttribute("class", type.FullName + ", " + type.Assembly.ToString().Replace('"', '\''));
			securityElement.AddAttribute("version", version.ToString());
			return securityElement;
		}

		internal static PermissionState CheckPermissionState(PermissionState state, bool allowUnrestricted)
		{
			if (state != PermissionState.None && state != PermissionState.Unrestricted)
			{
				string message = string.Format(Locale.GetText("Invalid enum {0}"), state);
				throw new ArgumentException(message, "state");
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
				string message = string.Format(Locale.GetText("Invalid tag {0}"), se.Tag);
				throw new ArgumentException(message, parameterName);
			}
			int num = minimumVersion;
			string text = se.Attribute("version");
			if (text != null)
			{
				try
				{
					num = int.Parse(text);
				}
				catch (Exception innerException)
				{
					string text2 = Locale.GetText("Couldn't parse version from '{0}'.");
					text2 = string.Format(text2, text);
					throw new ArgumentException(text2, parameterName, innerException);
				}
			}
			if (num < minimumVersion || num > maximumVersion)
			{
				string text3 = Locale.GetText("Unknown version '{0}', expected versions between ['{1}','{2}'].");
				text3 = string.Format(text3, num, minimumVersion, maximumVersion);
				throw new ArgumentException(text3, parameterName);
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

		internal bool ProcessFrame(SecurityFrame frame)
		{
			if (frame.PermitOnly != null)
			{
				bool flag = frame.PermitOnly.IsUnrestricted();
				if (!flag)
				{
					foreach (IPermission item in frame.PermitOnly)
					{
						if (CheckPermitOnly(item as CodeAccessPermission))
						{
							flag = true;
							break;
						}
					}
				}
				if (!flag)
				{
					ThrowSecurityException(this, "PermitOnly", frame, SecurityAction.Demand, null);
				}
			}
			if (frame.Deny != null)
			{
				if (frame.Deny.IsUnrestricted())
				{
					ThrowSecurityException(this, "Deny", frame, SecurityAction.Demand, null);
				}
				foreach (IPermission item2 in frame.Deny)
				{
					if (!CheckDeny(item2 as CodeAccessPermission))
					{
						ThrowSecurityException(this, "Deny", frame, SecurityAction.Demand, item2);
					}
				}
			}
			if (frame.Assert != null)
			{
				if (frame.Assert.IsUnrestricted())
				{
					return true;
				}
				foreach (IPermission item3 in frame.Assert)
				{
					if (CheckAssert(item3 as CodeAccessPermission))
					{
						return true;
					}
				}
			}
			return false;
		}

		internal static void ThrowInvalidPermission(IPermission target, Type expected)
		{
			string text = Locale.GetText("Invalid permission type '{0}', expected type '{1}'.");
			text = string.Format(text, target.GetType(), expected);
			throw new ArgumentException(text, "target");
		}

		internal static void ThrowExecutionEngineException(SecurityAction stackmod)
		{
			string text = Locale.GetText("No {0} modifier is present on the current stack frame.");
			text = text + Environment.NewLine + "Currently only declarative stack modifiers are supported.";
			throw new ExecutionEngineException(string.Format(text, stackmod));
		}

		internal static void ThrowSecurityException(object demanded, string message, SecurityFrame frame, SecurityAction action, IPermission failed)
		{
			throw new SecurityException(message);
		}
	}
}

using System.Collections;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Security.Permissions
{
	[Serializable]
	[ComVisible(true)]
	public sealed class EnvironmentPermission : CodeAccessPermission, IBuiltInPermission, IUnrestrictedPermission
	{
		private const int version = 1;

		private PermissionState _state;

		private ArrayList readList;

		private ArrayList writeList;

		public EnvironmentPermission(PermissionState state)
		{
			_state = CodeAccessPermission.CheckPermissionState(state, true);
			readList = new ArrayList();
			writeList = new ArrayList();
		}

		public EnvironmentPermission(EnvironmentPermissionAccess flag, string pathList)
		{
			readList = new ArrayList();
			writeList = new ArrayList();
			SetPathList(flag, pathList);
		}

		int IBuiltInPermission.GetTokenIndex()
		{
			return 0;
		}

		public void AddPathList(EnvironmentPermissionAccess flag, string pathList)
		{
			if (pathList == null)
			{
				throw new ArgumentNullException("pathList");
			}
			switch (flag)
			{
			case EnvironmentPermissionAccess.AllAccess:
			{
				string[] array = pathList.Split(';');
				string[] array3 = array;
				foreach (string text2 in array3)
				{
					if (!readList.Contains(text2))
					{
						readList.Add(text2);
					}
					if (!writeList.Contains(text2))
					{
						writeList.Add(text2);
					}
				}
				break;
			}
			case EnvironmentPermissionAccess.NoAccess:
				break;
			case EnvironmentPermissionAccess.Read:
			{
				string[] array = pathList.Split(';');
				string[] array4 = array;
				foreach (string text3 in array4)
				{
					if (!readList.Contains(text3))
					{
						readList.Add(text3);
					}
				}
				break;
			}
			case EnvironmentPermissionAccess.Write:
			{
				string[] array = pathList.Split(';');
				string[] array2 = array;
				foreach (string text in array2)
				{
					if (!writeList.Contains(text))
					{
						writeList.Add(text);
					}
				}
				break;
			}
			default:
				ThrowInvalidFlag(flag, false);
				break;
			}
		}

		public override IPermission Copy()
		{
			EnvironmentPermission environmentPermission = new EnvironmentPermission(_state);
			string pathList = GetPathList(EnvironmentPermissionAccess.Read);
			if (pathList != null)
			{
				environmentPermission.SetPathList(EnvironmentPermissionAccess.Read, pathList);
			}
			pathList = GetPathList(EnvironmentPermissionAccess.Write);
			if (pathList != null)
			{
				environmentPermission.SetPathList(EnvironmentPermissionAccess.Write, pathList);
			}
			return environmentPermission;
		}

		public override void FromXml(SecurityElement esd)
		{
			CodeAccessPermission.CheckSecurityElement(esd, "esd", 1, 1);
			if (CodeAccessPermission.IsUnrestricted(esd))
			{
				_state = PermissionState.Unrestricted;
			}
			string text = esd.Attribute("Read");
			if (text != null && text.Length > 0)
			{
				SetPathList(EnvironmentPermissionAccess.Read, text);
			}
			string text2 = esd.Attribute("Write");
			if (text2 != null && text2.Length > 0)
			{
				SetPathList(EnvironmentPermissionAccess.Write, text2);
			}
		}

		public string GetPathList(EnvironmentPermissionAccess flag)
		{
			switch (flag)
			{
			case EnvironmentPermissionAccess.NoAccess:
			case EnvironmentPermissionAccess.AllAccess:
				ThrowInvalidFlag(flag, true);
				break;
			case EnvironmentPermissionAccess.Read:
				return GetPathList(readList);
			case EnvironmentPermissionAccess.Write:
				return GetPathList(writeList);
			default:
				ThrowInvalidFlag(flag, false);
				break;
			}
			return null;
		}

		public override IPermission Intersect(IPermission target)
		{
			EnvironmentPermission environmentPermission = Cast(target);
			if (environmentPermission == null)
			{
				return null;
			}
			if (IsUnrestricted())
			{
				return environmentPermission.Copy();
			}
			if (environmentPermission.IsUnrestricted())
			{
				return Copy();
			}
			int num = 0;
			EnvironmentPermission environmentPermission2 = new EnvironmentPermission(PermissionState.None);
			string pathList = environmentPermission.GetPathList(EnvironmentPermissionAccess.Read);
			if (pathList != null)
			{
				string[] array = pathList.Split(';');
				string[] array2 = array;
				foreach (string text in array2)
				{
					if (readList.Contains(text))
					{
						environmentPermission2.AddPathList(EnvironmentPermissionAccess.Read, text);
						num++;
					}
				}
			}
			string pathList2 = environmentPermission.GetPathList(EnvironmentPermissionAccess.Write);
			if (pathList2 != null)
			{
				string[] array3 = pathList2.Split(';');
				string[] array4 = array3;
				foreach (string text2 in array4)
				{
					if (writeList.Contains(text2))
					{
						environmentPermission2.AddPathList(EnvironmentPermissionAccess.Write, text2);
						num++;
					}
				}
			}
			return (num <= 0) ? null : environmentPermission2;
		}

		public override bool IsSubsetOf(IPermission target)
		{
			EnvironmentPermission environmentPermission = Cast(target);
			if (environmentPermission == null)
			{
				return false;
			}
			if (IsUnrestricted())
			{
				return environmentPermission.IsUnrestricted();
			}
			if (environmentPermission.IsUnrestricted())
			{
				return true;
			}
			foreach (string read in readList)
			{
				if (!environmentPermission.readList.Contains(read))
				{
					return false;
				}
			}
			foreach (string write in writeList)
			{
				if (!environmentPermission.writeList.Contains(write))
				{
					return false;
				}
			}
			return true;
		}

		public bool IsUnrestricted()
		{
			return _state == PermissionState.Unrestricted;
		}

		public void SetPathList(EnvironmentPermissionAccess flag, string pathList)
		{
			if (pathList == null)
			{
				throw new ArgumentNullException("pathList");
			}
			switch (flag)
			{
			case EnvironmentPermissionAccess.AllAccess:
			{
				readList.Clear();
				writeList.Clear();
				string[] array = pathList.Split(';');
				string[] array3 = array;
				foreach (string value2 in array3)
				{
					readList.Add(value2);
					writeList.Add(value2);
				}
				break;
			}
			case EnvironmentPermissionAccess.NoAccess:
				break;
			case EnvironmentPermissionAccess.Read:
			{
				readList.Clear();
				string[] array = pathList.Split(';');
				string[] array4 = array;
				foreach (string value3 in array4)
				{
					readList.Add(value3);
				}
				break;
			}
			case EnvironmentPermissionAccess.Write:
			{
				writeList.Clear();
				string[] array = pathList.Split(';');
				string[] array2 = array;
				foreach (string value in array2)
				{
					writeList.Add(value);
				}
				break;
			}
			default:
				ThrowInvalidFlag(flag, false);
				break;
			}
		}

		public override SecurityElement ToXml()
		{
			SecurityElement securityElement = Element(1);
			if (_state == PermissionState.Unrestricted)
			{
				securityElement.AddAttribute("Unrestricted", "true");
			}
			else
			{
				string pathList = GetPathList(EnvironmentPermissionAccess.Read);
				if (pathList != null)
				{
					securityElement.AddAttribute("Read", pathList);
				}
				pathList = GetPathList(EnvironmentPermissionAccess.Write);
				if (pathList != null)
				{
					securityElement.AddAttribute("Write", pathList);
				}
			}
			return securityElement;
		}

		public override IPermission Union(IPermission other)
		{
			EnvironmentPermission environmentPermission = Cast(other);
			if (environmentPermission == null)
			{
				return Copy();
			}
			if (IsUnrestricted() || environmentPermission.IsUnrestricted())
			{
				return new EnvironmentPermission(PermissionState.Unrestricted);
			}
			if (IsEmpty() && environmentPermission.IsEmpty())
			{
				return null;
			}
			EnvironmentPermission environmentPermission2 = (EnvironmentPermission)Copy();
			string pathList = environmentPermission.GetPathList(EnvironmentPermissionAccess.Read);
			if (pathList != null)
			{
				environmentPermission2.AddPathList(EnvironmentPermissionAccess.Read, pathList);
			}
			pathList = environmentPermission.GetPathList(EnvironmentPermissionAccess.Write);
			if (pathList != null)
			{
				environmentPermission2.AddPathList(EnvironmentPermissionAccess.Write, pathList);
			}
			return environmentPermission2;
		}

		private bool IsEmpty()
		{
			return _state == PermissionState.None && readList.Count == 0 && writeList.Count == 0;
		}

		private EnvironmentPermission Cast(IPermission target)
		{
			if (target == null)
			{
				return null;
			}
			EnvironmentPermission environmentPermission = target as EnvironmentPermission;
			if (environmentPermission == null)
			{
				CodeAccessPermission.ThrowInvalidPermission(target, typeof(EnvironmentPermission));
			}
			return environmentPermission;
		}

		internal void ThrowInvalidFlag(EnvironmentPermissionAccess flag, bool context)
		{
			string text = null;
			text = ((!context) ? Locale.GetText("Invalid flag '{0}' in this context.") : Locale.GetText("Unknown flag '{0}'."));
			throw new ArgumentException(string.Format(text, flag), "flag");
		}

		private string GetPathList(ArrayList list)
		{
			if (IsUnrestricted())
			{
				return string.Empty;
			}
			if (list.Count == 0)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string item in list)
			{
				stringBuilder.Append(item);
				stringBuilder.Append(";");
			}
			string text = stringBuilder.ToString();
			int length = text.Length;
			if (length > 0)
			{
				return text.Substring(0, length - 1);
			}
			return string.Empty;
		}
	}
}

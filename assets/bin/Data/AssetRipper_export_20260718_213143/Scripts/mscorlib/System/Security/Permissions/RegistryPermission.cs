using System.Collections;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Text;

namespace System.Security.Permissions
{
	[Serializable]
	[ComVisible(true)]
	public sealed class RegistryPermission : CodeAccessPermission, IBuiltInPermission, IUnrestrictedPermission
	{
		private const int version = 1;

		private PermissionState _state;

		private ArrayList createList;

		private ArrayList readList;

		private ArrayList writeList;

		public RegistryPermission(PermissionState state)
		{
			_state = CodeAccessPermission.CheckPermissionState(state, true);
			createList = new ArrayList();
			readList = new ArrayList();
			writeList = new ArrayList();
		}

		public RegistryPermission(RegistryPermissionAccess access, string pathList)
		{
			_state = PermissionState.None;
			createList = new ArrayList();
			readList = new ArrayList();
			writeList = new ArrayList();
			AddPathList(access, pathList);
		}

		public RegistryPermission(RegistryPermissionAccess access, AccessControlActions control, string pathList)
		{
			if (!Enum.IsDefined(typeof(AccessControlActions), control))
			{
				string message = string.Format(Locale.GetText("Invalid enum {0}"), control);
				throw new ArgumentException(message, "AccessControlActions");
			}
			_state = PermissionState.None;
			AddPathList(access, control, pathList);
		}

		int IBuiltInPermission.GetTokenIndex()
		{
			return 5;
		}

		public void AddPathList(RegistryPermissionAccess access, string pathList)
		{
			if (pathList == null)
			{
				throw new ArgumentNullException("pathList");
			}
			switch (access)
			{
			case RegistryPermissionAccess.AllAccess:
				AddWithUnionKey(createList, pathList);
				AddWithUnionKey(readList, pathList);
				AddWithUnionKey(writeList, pathList);
				break;
			case RegistryPermissionAccess.NoAccess:
				break;
			case RegistryPermissionAccess.Create:
				AddWithUnionKey(createList, pathList);
				break;
			case RegistryPermissionAccess.Read:
				AddWithUnionKey(readList, pathList);
				break;
			case RegistryPermissionAccess.Write:
				AddWithUnionKey(writeList, pathList);
				break;
			default:
				ThrowInvalidFlag(access, false);
				break;
			}
		}

		[MonoTODO("(2.0) Access Control isn't implemented")]
		public void AddPathList(RegistryPermissionAccess access, AccessControlActions control, string pathList)
		{
			throw new NotImplementedException();
		}

		public string GetPathList(RegistryPermissionAccess access)
		{
			switch (access)
			{
			case RegistryPermissionAccess.NoAccess:
			case RegistryPermissionAccess.AllAccess:
				ThrowInvalidFlag(access, true);
				break;
			case RegistryPermissionAccess.Create:
				return GetPathList(createList);
			case RegistryPermissionAccess.Read:
				return GetPathList(readList);
			case RegistryPermissionAccess.Write:
				return GetPathList(writeList);
			default:
				ThrowInvalidFlag(access, false);
				break;
			}
			return null;
		}

		public void SetPathList(RegistryPermissionAccess access, string pathList)
		{
			if (pathList == null)
			{
				throw new ArgumentNullException("pathList");
			}
			switch (access)
			{
			case RegistryPermissionAccess.AllAccess:
			{
				createList.Clear();
				readList.Clear();
				writeList.Clear();
				string[] array = pathList.Split(';');
				string[] array5 = array;
				foreach (string value4 in array5)
				{
					createList.Add(value4);
					readList.Add(value4);
					writeList.Add(value4);
				}
				break;
			}
			case RegistryPermissionAccess.NoAccess:
				break;
			case RegistryPermissionAccess.Create:
			{
				createList.Clear();
				string[] array = pathList.Split(';');
				string[] array3 = array;
				foreach (string value2 in array3)
				{
					createList.Add(value2);
				}
				break;
			}
			case RegistryPermissionAccess.Read:
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
			case RegistryPermissionAccess.Write:
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
				ThrowInvalidFlag(access, false);
				break;
			}
		}

		public override IPermission Copy()
		{
			RegistryPermission registryPermission = new RegistryPermission(_state);
			string pathList = GetPathList(RegistryPermissionAccess.Create);
			if (pathList != null)
			{
				registryPermission.SetPathList(RegistryPermissionAccess.Create, pathList);
			}
			pathList = GetPathList(RegistryPermissionAccess.Read);
			if (pathList != null)
			{
				registryPermission.SetPathList(RegistryPermissionAccess.Read, pathList);
			}
			pathList = GetPathList(RegistryPermissionAccess.Write);
			if (pathList != null)
			{
				registryPermission.SetPathList(RegistryPermissionAccess.Write, pathList);
			}
			return registryPermission;
		}

		public override void FromXml(SecurityElement esd)
		{
			CodeAccessPermission.CheckSecurityElement(esd, "esd", 1, 1);
			CodeAccessPermission.CheckSecurityElement(esd, "esd", 1, 1);
			if (CodeAccessPermission.IsUnrestricted(esd))
			{
				_state = PermissionState.Unrestricted;
			}
			string text = esd.Attribute("Create");
			if (text != null && text.Length > 0)
			{
				SetPathList(RegistryPermissionAccess.Create, text);
			}
			string text2 = esd.Attribute("Read");
			if (text2 != null && text2.Length > 0)
			{
				SetPathList(RegistryPermissionAccess.Read, text2);
			}
			string text3 = esd.Attribute("Write");
			if (text3 != null && text3.Length > 0)
			{
				SetPathList(RegistryPermissionAccess.Write, text3);
			}
		}

		public override IPermission Intersect(IPermission target)
		{
			RegistryPermission registryPermission = Cast(target);
			if (registryPermission == null)
			{
				return null;
			}
			if (IsUnrestricted())
			{
				return registryPermission.Copy();
			}
			if (registryPermission.IsUnrestricted())
			{
				return Copy();
			}
			RegistryPermission registryPermission2 = new RegistryPermission(PermissionState.None);
			IntersectKeys(createList, registryPermission.createList, registryPermission2.createList);
			IntersectKeys(readList, registryPermission.readList, registryPermission2.readList);
			IntersectKeys(writeList, registryPermission.writeList, registryPermission2.writeList);
			return (!registryPermission2.IsEmpty()) ? registryPermission2 : null;
		}

		public override bool IsSubsetOf(IPermission target)
		{
			RegistryPermission registryPermission = Cast(target);
			if (registryPermission == null)
			{
				return false;
			}
			if (registryPermission.IsEmpty())
			{
				return IsEmpty();
			}
			if (IsUnrestricted())
			{
				return registryPermission.IsUnrestricted();
			}
			if (registryPermission.IsUnrestricted())
			{
				return true;
			}
			if (!KeyIsSubsetOf(createList, registryPermission.createList))
			{
				return false;
			}
			if (!KeyIsSubsetOf(readList, registryPermission.readList))
			{
				return false;
			}
			if (!KeyIsSubsetOf(writeList, registryPermission.writeList))
			{
				return false;
			}
			return true;
		}

		public bool IsUnrestricted()
		{
			return _state == PermissionState.Unrestricted;
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
				string pathList = GetPathList(RegistryPermissionAccess.Create);
				if (pathList != null)
				{
					securityElement.AddAttribute("Create", pathList);
				}
				pathList = GetPathList(RegistryPermissionAccess.Read);
				if (pathList != null)
				{
					securityElement.AddAttribute("Read", pathList);
				}
				pathList = GetPathList(RegistryPermissionAccess.Write);
				if (pathList != null)
				{
					securityElement.AddAttribute("Write", pathList);
				}
			}
			return securityElement;
		}

		public override IPermission Union(IPermission other)
		{
			RegistryPermission registryPermission = Cast(other);
			if (registryPermission == null)
			{
				return Copy();
			}
			if (IsUnrestricted() || registryPermission.IsUnrestricted())
			{
				return new RegistryPermission(PermissionState.Unrestricted);
			}
			if (IsEmpty() && registryPermission.IsEmpty())
			{
				return null;
			}
			RegistryPermission registryPermission2 = (RegistryPermission)Copy();
			string pathList = registryPermission.GetPathList(RegistryPermissionAccess.Create);
			if (pathList != null)
			{
				registryPermission2.AddPathList(RegistryPermissionAccess.Create, pathList);
			}
			pathList = registryPermission.GetPathList(RegistryPermissionAccess.Read);
			if (pathList != null)
			{
				registryPermission2.AddPathList(RegistryPermissionAccess.Read, pathList);
			}
			pathList = registryPermission.GetPathList(RegistryPermissionAccess.Write);
			if (pathList != null)
			{
				registryPermission2.AddPathList(RegistryPermissionAccess.Write, pathList);
			}
			return registryPermission2;
		}

		private bool IsEmpty()
		{
			return _state == PermissionState.None && createList.Count == 0 && readList.Count == 0 && writeList.Count == 0;
		}

		private RegistryPermission Cast(IPermission target)
		{
			if (target == null)
			{
				return null;
			}
			RegistryPermission registryPermission = target as RegistryPermission;
			if (registryPermission == null)
			{
				CodeAccessPermission.ThrowInvalidPermission(target, typeof(RegistryPermission));
			}
			return registryPermission;
		}

		internal void ThrowInvalidFlag(RegistryPermissionAccess flag, bool context)
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

		internal bool KeyIsSubsetOf(IList local, IList target)
		{
			bool flag = false;
			foreach (string item in local)
			{
				foreach (string item2 in target)
				{
					if (item.StartsWith(item2))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return false;
				}
			}
			return true;
		}

		internal void AddWithUnionKey(IList list, string pathList)
		{
			string[] array = pathList.Split(';');
			string[] array2 = array;
			foreach (string text in array2)
			{
				int count = list.Count;
				if (count == 0)
				{
					list.Add(text);
					continue;
				}
				for (int j = 0; j < count; j++)
				{
					string text2 = (string)list[j];
					if (text2.StartsWith(text))
					{
						list[j] = text;
					}
					else if (!text.StartsWith(text2))
					{
						list.Add(text);
					}
				}
			}
		}

		internal void IntersectKeys(IList local, IList target, IList result)
		{
			foreach (string item in local)
			{
				foreach (string item2 in target)
				{
					if (item2.Length > item.Length)
					{
						if (item2.StartsWith(item))
						{
							result.Add(item2);
						}
					}
					else if (item.StartsWith(item2))
					{
						result.Add(item);
					}
				}
			}
		}
	}
}

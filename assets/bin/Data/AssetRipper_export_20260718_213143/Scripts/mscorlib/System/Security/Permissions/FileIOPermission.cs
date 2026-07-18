using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.AccessControl;

namespace System.Security.Permissions
{
	[Serializable]
	[ComVisible(true)]
	public sealed class FileIOPermission : CodeAccessPermission, IBuiltInPermission, IUnrestrictedPermission
	{
		private const int version = 1;

		private static char[] BadPathNameCharacters;

		private static char[] BadFileNameCharacters;

		private bool m_Unrestricted;

		private FileIOPermissionAccess m_AllFilesAccess;

		private FileIOPermissionAccess m_AllLocalFilesAccess;

		private ArrayList readList;

		private ArrayList writeList;

		private ArrayList appendList;

		private ArrayList pathList;

		public FileIOPermissionAccess AllFiles
		{
			get
			{
				return m_AllFilesAccess;
			}
			set
			{
				if (!m_Unrestricted)
				{
					m_AllFilesAccess = value;
				}
			}
		}

		public FileIOPermissionAccess AllLocalFiles
		{
			get
			{
				return m_AllLocalFilesAccess;
			}
			set
			{
				if (!m_Unrestricted)
				{
					m_AllLocalFilesAccess = value;
				}
			}
		}

		public FileIOPermission(PermissionState state)
		{
			if (CodeAccessPermission.CheckPermissionState(state, true) == PermissionState.Unrestricted)
			{
				m_Unrestricted = true;
				m_AllFilesAccess = FileIOPermissionAccess.AllAccess;
				m_AllLocalFilesAccess = FileIOPermissionAccess.AllAccess;
			}
			CreateLists();
		}

		public FileIOPermission(FileIOPermissionAccess access, string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			CreateLists();
			AddPathList(access, path);
		}

		public FileIOPermission(FileIOPermissionAccess access, string[] pathList)
		{
			if (pathList == null)
			{
				throw new ArgumentNullException("pathList");
			}
			CreateLists();
			AddPathList(access, pathList);
		}

		[MonoTODO("(2.0) Access Control isn't implemented")]
		public FileIOPermission(FileIOPermissionAccess access, AccessControlActions control, string path)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("(2.0) Access Control isn't implemented")]
		public FileIOPermission(FileIOPermissionAccess access, AccessControlActions control, string[] pathList)
		{
			throw new NotImplementedException();
		}

		static FileIOPermission()
		{
			BadPathNameCharacters = Path.GetInvalidPathChars();
			BadFileNameCharacters = Path.GetInvalidFileNameChars();
		}

		int IBuiltInPermission.GetTokenIndex()
		{
			return 2;
		}

		internal void CreateLists()
		{
			readList = new ArrayList();
			writeList = new ArrayList();
			appendList = new ArrayList();
			pathList = new ArrayList();
		}

		public void AddPathList(FileIOPermissionAccess access, string path)
		{
			if ((FileIOPermissionAccess.AllAccess & access) != access)
			{
				ThrowInvalidFlag(access, true);
			}
			ThrowIfInvalidPath(path);
			AddPathInternal(access, path);
		}

		public void AddPathList(FileIOPermissionAccess access, string[] pathList)
		{
			if ((FileIOPermissionAccess.AllAccess & access) != access)
			{
				ThrowInvalidFlag(access, true);
			}
			ThrowIfInvalidPath(pathList);
			foreach (string path in pathList)
			{
				AddPathInternal(access, path);
			}
		}

		internal void AddPathInternal(FileIOPermissionAccess access, string path)
		{
			path = Path.InsecureGetFullPath(path);
			if ((access & FileIOPermissionAccess.Read) == FileIOPermissionAccess.Read)
			{
				readList.Add(path);
			}
			if ((access & FileIOPermissionAccess.Write) == FileIOPermissionAccess.Write)
			{
				writeList.Add(path);
			}
			if ((access & FileIOPermissionAccess.Append) == FileIOPermissionAccess.Append)
			{
				appendList.Add(path);
			}
			if ((access & FileIOPermissionAccess.PathDiscovery) == FileIOPermissionAccess.PathDiscovery)
			{
				pathList.Add(path);
			}
		}

		public override IPermission Copy()
		{
			if (m_Unrestricted)
			{
				return new FileIOPermission(PermissionState.Unrestricted);
			}
			FileIOPermission fileIOPermission = new FileIOPermission(PermissionState.None);
			fileIOPermission.readList = (ArrayList)readList.Clone();
			fileIOPermission.writeList = (ArrayList)writeList.Clone();
			fileIOPermission.appendList = (ArrayList)appendList.Clone();
			fileIOPermission.pathList = (ArrayList)pathList.Clone();
			fileIOPermission.m_AllFilesAccess = m_AllFilesAccess;
			fileIOPermission.m_AllLocalFilesAccess = m_AllLocalFilesAccess;
			return fileIOPermission;
		}

		public override void FromXml(SecurityElement esd)
		{
			CodeAccessPermission.CheckSecurityElement(esd, "esd", 1, 1);
			if (CodeAccessPermission.IsUnrestricted(esd))
			{
				m_Unrestricted = true;
				return;
			}
			m_Unrestricted = false;
			string text = esd.Attribute("Read");
			if (text != null)
			{
				string[] array = text.Split(';');
				AddPathList(FileIOPermissionAccess.Read, array);
			}
			text = esd.Attribute("Write");
			if (text != null)
			{
				string[] array = text.Split(';');
				AddPathList(FileIOPermissionAccess.Write, array);
			}
			text = esd.Attribute("Append");
			if (text != null)
			{
				string[] array = text.Split(';');
				AddPathList(FileIOPermissionAccess.Append, array);
			}
			text = esd.Attribute("PathDiscovery");
			if (text != null)
			{
				string[] array = text.Split(';');
				AddPathList(FileIOPermissionAccess.PathDiscovery, array);
			}
		}

		public string[] GetPathList(FileIOPermissionAccess access)
		{
			if ((FileIOPermissionAccess.AllAccess & access) != access)
			{
				ThrowInvalidFlag(access, true);
			}
			ArrayList arrayList = new ArrayList();
			switch (access)
			{
			case FileIOPermissionAccess.Read:
				arrayList.AddRange(readList);
				break;
			case FileIOPermissionAccess.Write:
				arrayList.AddRange(writeList);
				break;
			case FileIOPermissionAccess.Append:
				arrayList.AddRange(appendList);
				break;
			case FileIOPermissionAccess.PathDiscovery:
				arrayList.AddRange(pathList);
				break;
			default:
				ThrowInvalidFlag(access, false);
				break;
			case FileIOPermissionAccess.NoAccess:
				break;
			}
			return (arrayList.Count <= 0) ? null : ((string[])arrayList.ToArray(typeof(string)));
		}

		public override IPermission Intersect(IPermission target)
		{
			FileIOPermission fileIOPermission = Cast(target);
			if (fileIOPermission == null)
			{
				return null;
			}
			if (IsUnrestricted())
			{
				return fileIOPermission.Copy();
			}
			if (fileIOPermission.IsUnrestricted())
			{
				return Copy();
			}
			FileIOPermission fileIOPermission2 = new FileIOPermission(PermissionState.None);
			fileIOPermission2.AllFiles = m_AllFilesAccess & fileIOPermission.AllFiles;
			fileIOPermission2.AllLocalFiles = m_AllLocalFilesAccess & fileIOPermission.AllLocalFiles;
			IntersectKeys(readList, fileIOPermission.readList, fileIOPermission2.readList);
			IntersectKeys(writeList, fileIOPermission.writeList, fileIOPermission2.writeList);
			IntersectKeys(appendList, fileIOPermission.appendList, fileIOPermission2.appendList);
			IntersectKeys(pathList, fileIOPermission.pathList, fileIOPermission2.pathList);
			return (!fileIOPermission2.IsEmpty()) ? fileIOPermission2 : null;
		}

		public override bool IsSubsetOf(IPermission target)
		{
			FileIOPermission fileIOPermission = Cast(target);
			if (fileIOPermission == null)
			{
				return false;
			}
			if (fileIOPermission.IsEmpty())
			{
				return IsEmpty();
			}
			if (IsUnrestricted())
			{
				return fileIOPermission.IsUnrestricted();
			}
			if (fileIOPermission.IsUnrestricted())
			{
				return true;
			}
			if ((m_AllFilesAccess & fileIOPermission.AllFiles) != m_AllFilesAccess)
			{
				return false;
			}
			if ((m_AllLocalFilesAccess & fileIOPermission.AllLocalFiles) != m_AllLocalFilesAccess)
			{
				return false;
			}
			if (!KeyIsSubsetOf(appendList, fileIOPermission.appendList))
			{
				return false;
			}
			if (!KeyIsSubsetOf(readList, fileIOPermission.readList))
			{
				return false;
			}
			if (!KeyIsSubsetOf(writeList, fileIOPermission.writeList))
			{
				return false;
			}
			if (!KeyIsSubsetOf(pathList, fileIOPermission.pathList))
			{
				return false;
			}
			return true;
		}

		public bool IsUnrestricted()
		{
			return m_Unrestricted;
		}

		public void SetPathList(FileIOPermissionAccess access, string path)
		{
			if ((FileIOPermissionAccess.AllAccess & access) != access)
			{
				ThrowInvalidFlag(access, true);
			}
			ThrowIfInvalidPath(path);
			Clear(access);
			AddPathInternal(access, path);
		}

		public void SetPathList(FileIOPermissionAccess access, string[] pathList)
		{
			if ((FileIOPermissionAccess.AllAccess & access) != access)
			{
				ThrowInvalidFlag(access, true);
			}
			ThrowIfInvalidPath(pathList);
			Clear(access);
			foreach (string path in pathList)
			{
				AddPathInternal(access, path);
			}
		}

		public override SecurityElement ToXml()
		{
			SecurityElement securityElement = Element(1);
			if (m_Unrestricted)
			{
				securityElement.AddAttribute("Unrestricted", "true");
			}
			else
			{
				string[] array = GetPathList(FileIOPermissionAccess.Append);
				if (array != null && array.Length > 0)
				{
					securityElement.AddAttribute("Append", string.Join(";", array));
				}
				array = GetPathList(FileIOPermissionAccess.Read);
				if (array != null && array.Length > 0)
				{
					securityElement.AddAttribute("Read", string.Join(";", array));
				}
				array = GetPathList(FileIOPermissionAccess.Write);
				if (array != null && array.Length > 0)
				{
					securityElement.AddAttribute("Write", string.Join(";", array));
				}
				array = GetPathList(FileIOPermissionAccess.PathDiscovery);
				if (array != null && array.Length > 0)
				{
					securityElement.AddAttribute("PathDiscovery", string.Join(";", array));
				}
			}
			return securityElement;
		}

		public override IPermission Union(IPermission other)
		{
			FileIOPermission fileIOPermission = Cast(other);
			if (fileIOPermission == null)
			{
				return Copy();
			}
			if (IsUnrestricted() || fileIOPermission.IsUnrestricted())
			{
				return new FileIOPermission(PermissionState.Unrestricted);
			}
			if (IsEmpty() && fileIOPermission.IsEmpty())
			{
				return null;
			}
			FileIOPermission fileIOPermission2 = (FileIOPermission)Copy();
			fileIOPermission2.AllFiles |= fileIOPermission.AllFiles;
			fileIOPermission2.AllLocalFiles |= fileIOPermission.AllLocalFiles;
			string[] array = fileIOPermission.GetPathList(FileIOPermissionAccess.Read);
			if (array != null)
			{
				UnionKeys(fileIOPermission2.readList, array);
			}
			array = fileIOPermission.GetPathList(FileIOPermissionAccess.Write);
			if (array != null)
			{
				UnionKeys(fileIOPermission2.writeList, array);
			}
			array = fileIOPermission.GetPathList(FileIOPermissionAccess.Append);
			if (array != null)
			{
				UnionKeys(fileIOPermission2.appendList, array);
			}
			array = fileIOPermission.GetPathList(FileIOPermissionAccess.PathDiscovery);
			if (array != null)
			{
				UnionKeys(fileIOPermission2.pathList, array);
			}
			return fileIOPermission2;
		}

		[MonoTODO("(2.0)")]
		[ComVisible(false)]
		public override bool Equals(object obj)
		{
			return false;
		}

		[ComVisible(false)]
		[MonoTODO("(2.0)")]
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		private bool IsEmpty()
		{
			return !m_Unrestricted && appendList.Count == 0 && readList.Count == 0 && writeList.Count == 0 && pathList.Count == 0;
		}

		private static FileIOPermission Cast(IPermission target)
		{
			if (target == null)
			{
				return null;
			}
			FileIOPermission fileIOPermission = target as FileIOPermission;
			if (fileIOPermission == null)
			{
				CodeAccessPermission.ThrowInvalidPermission(target, typeof(FileIOPermission));
			}
			return fileIOPermission;
		}

		internal static void ThrowInvalidFlag(FileIOPermissionAccess access, bool context)
		{
			string text = null;
			text = ((!context) ? Locale.GetText("Invalid flag '{0}' in this context.") : Locale.GetText("Unknown flag '{0}'."));
			throw new ArgumentException(string.Format(text, access), "access");
		}

		internal static void ThrowIfInvalidPath(string path)
		{
			string directoryName = Path.GetDirectoryName(path);
			if (directoryName != null && directoryName.LastIndexOfAny(BadPathNameCharacters) >= 0)
			{
				string message = string.Format(Locale.GetText("Invalid path characters in path: '{0}'"), path);
				throw new ArgumentException(message, "path");
			}
			string fileName = Path.GetFileName(path);
			if (fileName != null && fileName.LastIndexOfAny(BadFileNameCharacters) >= 0)
			{
				string message2 = string.Format(Locale.GetText("Invalid filename characters in path: '{0}'"), path);
				throw new ArgumentException(message2, "path");
			}
			if (!Path.IsPathRooted(path))
			{
				string text = Locale.GetText("Absolute path information is required.");
				throw new ArgumentException(text, "path");
			}
		}

		internal static void ThrowIfInvalidPath(string[] paths)
		{
			foreach (string path in paths)
			{
				ThrowIfInvalidPath(path);
			}
		}

		internal void Clear(FileIOPermissionAccess access)
		{
			if ((access & FileIOPermissionAccess.Read) == FileIOPermissionAccess.Read)
			{
				readList.Clear();
			}
			if ((access & FileIOPermissionAccess.Write) == FileIOPermissionAccess.Write)
			{
				writeList.Clear();
			}
			if ((access & FileIOPermissionAccess.Append) == FileIOPermissionAccess.Append)
			{
				appendList.Clear();
			}
			if ((access & FileIOPermissionAccess.PathDiscovery) == FileIOPermissionAccess.PathDiscovery)
			{
				pathList.Clear();
			}
		}

		internal static bool KeyIsSubsetOf(IList local, IList target)
		{
			bool flag = false;
			foreach (string item in local)
			{
				foreach (string item2 in target)
				{
					if (Path.IsPathSubsetOf(item2, item))
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

		internal static void UnionKeys(IList list, string[] paths)
		{
			foreach (string text in paths)
			{
				int count = list.Count;
				if (count == 0)
				{
					list.Add(text);
					continue;
				}
				int j;
				for (j = 0; j < count; j++)
				{
					string text2 = (string)list[j];
					if (Path.IsPathSubsetOf(text, text2))
					{
						list[j] = text;
						break;
					}
					if (Path.IsPathSubsetOf(text2, text))
					{
						break;
					}
				}
				if (j == count)
				{
					list.Add(text);
				}
			}
		}

		internal static void IntersectKeys(IList local, IList target, IList result)
		{
			foreach (string item in local)
			{
				foreach (string item2 in target)
				{
					if (item2.Length > item.Length)
					{
						if (Path.IsPathSubsetOf(item, item2))
						{
							result.Add(item2);
						}
					}
					else if (Path.IsPathSubsetOf(item2, item))
					{
						result.Add(item);
					}
				}
			}
		}
	}
}

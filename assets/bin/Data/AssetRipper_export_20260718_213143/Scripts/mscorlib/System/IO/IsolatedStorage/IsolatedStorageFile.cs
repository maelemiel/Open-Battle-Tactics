using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Security.Policy;
using System.Text;
using System.Threading;
using Mono.Security.Cryptography;

namespace System.IO.IsolatedStorage
{
	[ComVisible(true)]
	public sealed class IsolatedStorageFile : IsolatedStorage, IDisposable
	{
		[Serializable]
		private struct Identities
		{
			public object Application;

			public object Assembly;

			public object Domain;

			public Identities(object application, object assembly, object domain)
			{
				Application = application;
				Assembly = assembly;
				Domain = domain;
			}
		}

		private bool _resolved;

		private ulong _maxSize;

		private Evidence _fullEvidences;

		private static Mutex mutex = new Mutex();

		private DirectoryInfo directory;

		[CLSCompliant(false)]
		public override ulong CurrentSize
		{
			get
			{
				return GetDirectorySize(directory);
			}
		}

		[CLSCompliant(false)]
		public override ulong MaximumSize
		{
			get
			{
				if (!SecurityManager.SecurityEnabled)
				{
					return 9223372036854775807uL;
				}
				if (_resolved)
				{
					return _maxSize;
				}
				Evidence evidence = null;
				if (_fullEvidences != null)
				{
					evidence = _fullEvidences;
				}
				else
				{
					evidence = new Evidence();
					if (_assemblyIdentity != null)
					{
						evidence.AddHost(_assemblyIdentity);
					}
				}
				if (evidence.Count < 1)
				{
					throw new InvalidOperationException(Locale.GetText("Couldn't get the quota from the available evidences."));
				}
				PermissionSet denied = null;
				PermissionSet permissionSet = SecurityManager.ResolvePolicy(evidence, null, null, null, out denied);
				IsolatedStoragePermission permission = GetPermission(permissionSet);
				if (permission == null)
				{
					if (!permissionSet.IsUnrestricted())
					{
						throw new InvalidOperationException(Locale.GetText("No quota from the available evidences."));
					}
					_maxSize = 9223372036854775807uL;
				}
				else
				{
					_maxSize = (ulong)permission.UserQuota;
				}
				_resolved = true;
				return _maxSize;
			}
		}

		internal string Root
		{
			get
			{
				return directory.FullName;
			}
		}

		private IsolatedStorageFile(IsolatedStorageScope scope)
		{
			storage_scope = scope;
		}

		internal IsolatedStorageFile(IsolatedStorageScope scope, string location)
		{
			storage_scope = scope;
			directory = new DirectoryInfo(location);
			if (!directory.Exists)
			{
				string text = Locale.GetText("Invalid storage.");
				throw new IsolatedStorageException(text);
			}
		}

		public static IEnumerator GetEnumerator(IsolatedStorageScope scope)
		{
			Demand(scope);
			if (scope != IsolatedStorageScope.User && scope != (IsolatedStorageScope.User | IsolatedStorageScope.Roaming) && scope != IsolatedStorageScope.Machine)
			{
				string text = Locale.GetText("Invalid scope, only User, User|Roaming and Machine are valid");
				throw new ArgumentException(text);
			}
			return new IsolatedStorageFileEnumerator(scope, GetIsolatedStorageRoot(scope));
		}

		public static IsolatedStorageFile GetStore(IsolatedStorageScope scope, Evidence domainEvidence, Type domainEvidenceType, Evidence assemblyEvidence, Type assemblyEvidenceType)
		{
			Demand(scope);
			bool flag = (scope & IsolatedStorageScope.Domain) != 0;
			if (flag && domainEvidence == null)
			{
				throw new ArgumentNullException("domainEvidence");
			}
			bool flag2 = (scope & IsolatedStorageScope.Assembly) != 0;
			if (flag2 && assemblyEvidence == null)
			{
				throw new ArgumentNullException("assemblyEvidence");
			}
			IsolatedStorageFile isolatedStorageFile = new IsolatedStorageFile(scope);
			if (flag)
			{
				if (domainEvidenceType == null)
				{
					isolatedStorageFile._domainIdentity = GetDomainIdentityFromEvidence(domainEvidence);
				}
				else
				{
					isolatedStorageFile._domainIdentity = GetTypeFromEvidence(domainEvidence, domainEvidenceType);
				}
				if (isolatedStorageFile._domainIdentity == null)
				{
					throw new IsolatedStorageException(Locale.GetText("Couldn't find domain identity."));
				}
			}
			if (flag2)
			{
				if (assemblyEvidenceType == null)
				{
					isolatedStorageFile._assemblyIdentity = GetAssemblyIdentityFromEvidence(assemblyEvidence);
				}
				else
				{
					isolatedStorageFile._assemblyIdentity = GetTypeFromEvidence(assemblyEvidence, assemblyEvidenceType);
				}
				if (isolatedStorageFile._assemblyIdentity == null)
				{
					throw new IsolatedStorageException(Locale.GetText("Couldn't find assembly identity."));
				}
			}
			isolatedStorageFile.PostInit();
			return isolatedStorageFile;
		}

		public static IsolatedStorageFile GetStore(IsolatedStorageScope scope, object domainIdentity, object assemblyIdentity)
		{
			Demand(scope);
			if ((scope & IsolatedStorageScope.Domain) != IsolatedStorageScope.None && domainIdentity == null)
			{
				throw new ArgumentNullException("domainIdentity");
			}
			bool flag = (scope & IsolatedStorageScope.Assembly) != 0;
			if (flag && assemblyIdentity == null)
			{
				throw new ArgumentNullException("assemblyIdentity");
			}
			IsolatedStorageFile isolatedStorageFile = new IsolatedStorageFile(scope);
			if (flag)
			{
				isolatedStorageFile._fullEvidences = Assembly.GetCallingAssembly().UnprotectedGetEvidence();
			}
			isolatedStorageFile._domainIdentity = domainIdentity;
			isolatedStorageFile._assemblyIdentity = assemblyIdentity;
			isolatedStorageFile.PostInit();
			return isolatedStorageFile;
		}

		public static IsolatedStorageFile GetStore(IsolatedStorageScope scope, Type domainEvidenceType, Type assemblyEvidenceType)
		{
			Demand(scope);
			IsolatedStorageFile isolatedStorageFile = new IsolatedStorageFile(scope);
			if ((scope & IsolatedStorageScope.Domain) != IsolatedStorageScope.None)
			{
				if (domainEvidenceType == null)
				{
					domainEvidenceType = typeof(Url);
				}
				isolatedStorageFile._domainIdentity = GetTypeFromEvidence(AppDomain.CurrentDomain.Evidence, domainEvidenceType);
			}
			if ((scope & IsolatedStorageScope.Assembly) != IsolatedStorageScope.None)
			{
				Evidence e = (isolatedStorageFile._fullEvidences = Assembly.GetCallingAssembly().UnprotectedGetEvidence());
				if ((scope & IsolatedStorageScope.Domain) != IsolatedStorageScope.None)
				{
					if (assemblyEvidenceType == null)
					{
						assemblyEvidenceType = typeof(Url);
					}
					isolatedStorageFile._assemblyIdentity = GetTypeFromEvidence(e, assemblyEvidenceType);
				}
				else
				{
					isolatedStorageFile._assemblyIdentity = GetAssemblyIdentityFromEvidence(e);
				}
			}
			isolatedStorageFile.PostInit();
			return isolatedStorageFile;
		}

		public static IsolatedStorageFile GetStore(IsolatedStorageScope scope, object applicationIdentity)
		{
			Demand(scope);
			if (applicationIdentity == null)
			{
				throw new ArgumentNullException("applicationIdentity");
			}
			IsolatedStorageFile isolatedStorageFile = new IsolatedStorageFile(scope);
			isolatedStorageFile._applicationIdentity = applicationIdentity;
			isolatedStorageFile._fullEvidences = Assembly.GetCallingAssembly().UnprotectedGetEvidence();
			isolatedStorageFile.PostInit();
			return isolatedStorageFile;
		}

		public static IsolatedStorageFile GetStore(IsolatedStorageScope scope, Type applicationEvidenceType)
		{
			Demand(scope);
			IsolatedStorageFile isolatedStorageFile = new IsolatedStorageFile(scope);
			isolatedStorageFile.InitStore(scope, applicationEvidenceType);
			isolatedStorageFile._fullEvidences = Assembly.GetCallingAssembly().UnprotectedGetEvidence();
			isolatedStorageFile.PostInit();
			return isolatedStorageFile;
		}

		public static IsolatedStorageFile GetMachineStoreForApplication()
		{
			IsolatedStorageScope scope = IsolatedStorageScope.Machine | IsolatedStorageScope.Application;
			IsolatedStorageFile isolatedStorageFile = new IsolatedStorageFile(scope);
			isolatedStorageFile.InitStore(scope, null);
			isolatedStorageFile._fullEvidences = Assembly.GetCallingAssembly().UnprotectedGetEvidence();
			isolatedStorageFile.PostInit();
			return isolatedStorageFile;
		}

		public static IsolatedStorageFile GetMachineStoreForAssembly()
		{
			IsolatedStorageScope scope = IsolatedStorageScope.Assembly | IsolatedStorageScope.Machine;
			IsolatedStorageFile isolatedStorageFile = new IsolatedStorageFile(scope);
			isolatedStorageFile._assemblyIdentity = GetAssemblyIdentityFromEvidence(isolatedStorageFile._fullEvidences = Assembly.GetCallingAssembly().UnprotectedGetEvidence());
			isolatedStorageFile.PostInit();
			return isolatedStorageFile;
		}

		public static IsolatedStorageFile GetMachineStoreForDomain()
		{
			IsolatedStorageScope scope = IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly | IsolatedStorageScope.Machine;
			IsolatedStorageFile isolatedStorageFile = new IsolatedStorageFile(scope);
			isolatedStorageFile._domainIdentity = GetDomainIdentityFromEvidence(AppDomain.CurrentDomain.Evidence);
			isolatedStorageFile._assemblyIdentity = GetAssemblyIdentityFromEvidence(isolatedStorageFile._fullEvidences = Assembly.GetCallingAssembly().UnprotectedGetEvidence());
			isolatedStorageFile.PostInit();
			return isolatedStorageFile;
		}

		public static IsolatedStorageFile GetUserStoreForApplication()
		{
			IsolatedStorageScope scope = IsolatedStorageScope.User | IsolatedStorageScope.Application;
			IsolatedStorageFile isolatedStorageFile = new IsolatedStorageFile(scope);
			isolatedStorageFile.InitStore(scope, null);
			isolatedStorageFile._fullEvidences = Assembly.GetCallingAssembly().UnprotectedGetEvidence();
			isolatedStorageFile.PostInit();
			return isolatedStorageFile;
		}

		public static IsolatedStorageFile GetUserStoreForAssembly()
		{
			IsolatedStorageScope scope = IsolatedStorageScope.User | IsolatedStorageScope.Assembly;
			IsolatedStorageFile isolatedStorageFile = new IsolatedStorageFile(scope);
			isolatedStorageFile._assemblyIdentity = GetAssemblyIdentityFromEvidence(isolatedStorageFile._fullEvidences = Assembly.GetCallingAssembly().UnprotectedGetEvidence());
			isolatedStorageFile.PostInit();
			return isolatedStorageFile;
		}

		public static IsolatedStorageFile GetUserStoreForDomain()
		{
			IsolatedStorageScope scope = IsolatedStorageScope.User | IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly;
			IsolatedStorageFile isolatedStorageFile = new IsolatedStorageFile(scope);
			isolatedStorageFile._domainIdentity = GetDomainIdentityFromEvidence(AppDomain.CurrentDomain.Evidence);
			isolatedStorageFile._assemblyIdentity = GetAssemblyIdentityFromEvidence(isolatedStorageFile._fullEvidences = Assembly.GetCallingAssembly().UnprotectedGetEvidence());
			isolatedStorageFile.PostInit();
			return isolatedStorageFile;
		}

		public static void Remove(IsolatedStorageScope scope)
		{
			string isolatedStorageRoot = GetIsolatedStorageRoot(scope);
			Directory.Delete(isolatedStorageRoot, true);
		}

		internal static string GetIsolatedStorageRoot(IsolatedStorageScope scope)
		{
			string text = null;
			if ((scope & IsolatedStorageScope.User) != IsolatedStorageScope.None)
			{
				text = (((scope & IsolatedStorageScope.Roaming) == 0) ? Environment.InternalGetFolderPath(Environment.SpecialFolder.ApplicationData) : Environment.InternalGetFolderPath(Environment.SpecialFolder.LocalApplicationData));
			}
			else if ((scope & IsolatedStorageScope.Machine) != IsolatedStorageScope.None)
			{
				text = Environment.InternalGetFolderPath(Environment.SpecialFolder.CommonApplicationData);
			}
			if (text == null)
			{
				string text2 = Locale.GetText("Couldn't access storage location for '{0}'.");
				throw new IsolatedStorageException(string.Format(text2, scope));
			}
			return Path.Combine(text, ".isolated-storage");
		}

		private static void Demand(IsolatedStorageScope scope)
		{
			if (SecurityManager.SecurityEnabled)
			{
				IsolatedStorageFilePermission isolatedStorageFilePermission = new IsolatedStorageFilePermission(PermissionState.None);
				isolatedStorageFilePermission.UsageAllowed = ScopeToContainment(scope);
				isolatedStorageFilePermission.Demand();
			}
		}

		private static IsolatedStorageContainment ScopeToContainment(IsolatedStorageScope scope)
		{
			switch (scope)
			{
			case IsolatedStorageScope.User | IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly:
				return IsolatedStorageContainment.DomainIsolationByUser;
			case IsolatedStorageScope.User | IsolatedStorageScope.Assembly:
				return IsolatedStorageContainment.AssemblyIsolationByUser;
			case IsolatedStorageScope.User | IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly | IsolatedStorageScope.Roaming:
				return IsolatedStorageContainment.DomainIsolationByRoamingUser;
			case IsolatedStorageScope.User | IsolatedStorageScope.Assembly | IsolatedStorageScope.Roaming:
				return IsolatedStorageContainment.AssemblyIsolationByRoamingUser;
			case IsolatedStorageScope.User | IsolatedStorageScope.Application:
				return IsolatedStorageContainment.ApplicationIsolationByUser;
			case IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly | IsolatedStorageScope.Machine:
				return IsolatedStorageContainment.DomainIsolationByMachine;
			case IsolatedStorageScope.Assembly | IsolatedStorageScope.Machine:
				return IsolatedStorageContainment.AssemblyIsolationByMachine;
			case IsolatedStorageScope.Machine | IsolatedStorageScope.Application:
				return IsolatedStorageContainment.ApplicationIsolationByMachine;
			case IsolatedStorageScope.User | IsolatedStorageScope.Roaming | IsolatedStorageScope.Application:
				return IsolatedStorageContainment.ApplicationIsolationByRoamingUser;
			default:
				return IsolatedStorageContainment.UnrestrictedIsolatedStorage;
			}
		}

		internal static ulong GetDirectorySize(DirectoryInfo di)
		{
			ulong num = 0uL;
			FileInfo[] files = di.GetFiles();
			foreach (FileInfo fileInfo in files)
			{
				num += (ulong)fileInfo.Length;
			}
			DirectoryInfo[] directories = di.GetDirectories();
			foreach (DirectoryInfo di2 in directories)
			{
				num += GetDirectorySize(di2);
			}
			return num;
		}

		~IsolatedStorageFile()
		{
		}

		private void PostInit()
		{
			string isolatedStorageRoot = GetIsolatedStorageRoot(base.Scope);
			string text = null;
			if (_applicationIdentity != null)
			{
				text = string.Format("a{0}{1}", SeparatorInternal, GetNameFromIdentity(_applicationIdentity));
			}
			else if (_domainIdentity != null)
			{
				text = string.Format("d{0}{1}{0}{2}", SeparatorInternal, GetNameFromIdentity(_domainIdentity), GetNameFromIdentity(_assemblyIdentity));
			}
			else
			{
				if (_assemblyIdentity == null)
				{
					throw new IsolatedStorageException(Locale.GetText("No code identity available."));
				}
				text = string.Format("d{0}none{0}{1}", SeparatorInternal, GetNameFromIdentity(_assemblyIdentity));
			}
			isolatedStorageRoot = Path.Combine(isolatedStorageRoot, text);
			directory = new DirectoryInfo(isolatedStorageRoot);
			if (!directory.Exists)
			{
				try
				{
					directory.Create();
					SaveIdentities(isolatedStorageRoot);
				}
				catch (IOException)
				{
				}
			}
		}

		public void Close()
		{
		}

		public void CreateDirectory(string dir)
		{
			if (dir == null)
			{
				throw new ArgumentNullException("dir");
			}
			if (dir.IndexOfAny(Path.PathSeparatorChars) < 0)
			{
				if (directory.GetFiles(dir).Length > 0)
				{
					throw new IOException(Locale.GetText("Directory name already exists as a file."));
				}
				directory.CreateSubdirectory(dir);
				return;
			}
			string[] array = dir.Split(Path.PathSeparatorChars);
			DirectoryInfo directoryInfo = directory;
			for (int i = 0; i < array.Length; i++)
			{
				if (directoryInfo.GetFiles(array[i]).Length > 0)
				{
					throw new IOException(Locale.GetText("Part of the directory name already exists as a file."));
				}
				directoryInfo = directoryInfo.CreateSubdirectory(array[i]);
			}
		}

		public void DeleteDirectory(string dir)
		{
			try
			{
				DirectoryInfo directoryInfo = directory.CreateSubdirectory(dir);
				directoryInfo.Delete();
			}
			catch
			{
				throw new IsolatedStorageException(Locale.GetText("Could not delete directory '{0}'", dir));
			}
		}

		public void DeleteFile(string file)
		{
			File.Delete(Path.Combine(directory.FullName, file));
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);
		}

		public string[] GetDirectoryNames(string searchPattern)
		{
			if (searchPattern == null)
			{
				throw new ArgumentNullException("searchPattern");
			}
			string directoryName = Path.GetDirectoryName(searchPattern);
			string fileName = Path.GetFileName(searchPattern);
			DirectoryInfo[] array = null;
			if (directoryName == null || directoryName.Length == 0)
			{
				array = directory.GetDirectories(searchPattern);
			}
			else
			{
				DirectoryInfo[] directories = directory.GetDirectories(directoryName);
				if (directories.Length != 1 || !(directories[0].Name == directoryName) || directories[0].FullName.IndexOf(directory.FullName) < 0)
				{
					throw new SecurityException();
				}
				array = directories[0].GetDirectories(fileName);
			}
			return GetNames(array);
		}

		private string[] GetNames(FileSystemInfo[] afsi)
		{
			string[] array = new string[afsi.Length];
			for (int i = 0; i != afsi.Length; i++)
			{
				array[i] = afsi[i].Name;
			}
			return array;
		}

		public string[] GetFileNames(string searchPattern)
		{
			if (searchPattern == null)
			{
				throw new ArgumentNullException("searchPattern");
			}
			string directoryName = Path.GetDirectoryName(searchPattern);
			string fileName = Path.GetFileName(searchPattern);
			FileInfo[] array = null;
			if (directoryName == null || directoryName.Length == 0)
			{
				array = directory.GetFiles(searchPattern);
			}
			else
			{
				DirectoryInfo[] directories = directory.GetDirectories(directoryName);
				if (directories.Length != 1 || !(directories[0].Name == directoryName) || directories[0].FullName.IndexOf(directory.FullName) < 0)
				{
					throw new SecurityException();
				}
				array = directories[0].GetFiles(fileName);
			}
			return GetNames(array);
		}

		public override void Remove()
		{
			directory.Delete(true);
		}

		protected override IsolatedStoragePermission GetPermission(PermissionSet ps)
		{
			if (ps == null)
			{
				return null;
			}
			return (IsolatedStoragePermission)ps.GetPermission(typeof(IsolatedStorageFilePermission));
		}

		private string GetNameFromIdentity(object identity)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(identity.ToString());
			SHA1 sHA = SHA1.Create();
			byte[] src = sHA.ComputeHash(bytes, 0, bytes.Length);
			byte[] array = new byte[10];
			Buffer.BlockCopy(src, 0, array, 0, array.Length);
			return CryptoConvert.ToHex(array);
		}

		private static object GetTypeFromEvidence(Evidence e, Type t)
		{
			foreach (object item in e)
			{
				if (item.GetType() == t)
				{
					return item;
				}
			}
			return null;
		}

		internal static object GetAssemblyIdentityFromEvidence(Evidence e)
		{
			object typeFromEvidence = GetTypeFromEvidence(e, typeof(Publisher));
			if (typeFromEvidence != null)
			{
				return typeFromEvidence;
			}
			typeFromEvidence = GetTypeFromEvidence(e, typeof(StrongName));
			if (typeFromEvidence != null)
			{
				return typeFromEvidence;
			}
			return GetTypeFromEvidence(e, typeof(Url));
		}

		internal static object GetDomainIdentityFromEvidence(Evidence e)
		{
			object typeFromEvidence = GetTypeFromEvidence(e, typeof(ApplicationDirectory));
			if (typeFromEvidence != null)
			{
				return typeFromEvidence;
			}
			return GetTypeFromEvidence(e, typeof(Url));
		}

		private void SaveIdentities(string root)
		{
			Identities identities = new Identities(_applicationIdentity, _assemblyIdentity, _domainIdentity);
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			mutex.WaitOne();
			try
			{
				using (FileStream serializationStream = File.Create(root + ".storage"))
				{
					binaryFormatter.Serialize(serializationStream, identities);
				}
			}
			finally
			{
				mutex.ReleaseMutex();
			}
		}
	}
}

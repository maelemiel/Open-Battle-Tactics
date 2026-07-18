using System.Collections;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace System.Resources
{
	[Serializable]
	[ComVisible(true)]
	public class ResourceManager
	{
		private static Hashtable ResourceCache = new Hashtable();

		private static Hashtable NonExistent = Hashtable.Synchronized(new Hashtable());

		public static readonly int HeaderVersionNumber = 1;

		public static readonly int MagicNumber = -1091581234;

		protected string BaseNameField;

		protected Assembly MainAssembly;

		protected Hashtable ResourceSets;

		private bool ignoreCase;

		private Type resourceSource;

		private Type resourceSetType = typeof(RuntimeResourceSet);

		private string resourceDir;

		private CultureInfo neutral_culture;

		private UltimateResourceFallbackLocation fallbackLocation;

		public virtual string BaseName
		{
			get
			{
				return BaseNameField;
			}
		}

		public virtual bool IgnoreCase
		{
			get
			{
				return ignoreCase;
			}
			set
			{
				ignoreCase = value;
			}
		}

		public virtual Type ResourceSetType
		{
			get
			{
				return resourceSetType;
			}
		}

		[MonoTODO("the property exists but is not respected")]
		protected UltimateResourceFallbackLocation FallbackLocation
		{
			get
			{
				return fallbackLocation;
			}
			set
			{
				fallbackLocation = value;
			}
		}

		protected ResourceManager()
		{
		}

		public ResourceManager(Type resourceSource)
		{
			if (resourceSource == null)
			{
				throw new ArgumentNullException("resourceSource");
			}
			this.resourceSource = resourceSource;
			BaseNameField = resourceSource.Name;
			MainAssembly = resourceSource.Assembly;
			ResourceSets = GetResourceSets(MainAssembly, BaseNameField);
			neutral_culture = GetNeutralResourcesLanguage(MainAssembly);
		}

		public ResourceManager(string baseName, Assembly assembly)
		{
			if (baseName == null)
			{
				throw new ArgumentNullException("baseName");
			}
			if (assembly == null)
			{
				throw new ArgumentNullException("assembly");
			}
			BaseNameField = baseName;
			MainAssembly = assembly;
			ResourceSets = GetResourceSets(MainAssembly, BaseNameField);
			neutral_culture = GetNeutralResourcesLanguage(MainAssembly);
		}

		public ResourceManager(string baseName, Assembly assembly, Type usingResourceSet)
		{
			if (baseName == null)
			{
				throw new ArgumentNullException("baseName");
			}
			if (assembly == null)
			{
				throw new ArgumentNullException("assembly");
			}
			BaseNameField = baseName;
			MainAssembly = assembly;
			ResourceSets = GetResourceSets(MainAssembly, BaseNameField);
			resourceSetType = CheckResourceSetType(usingResourceSet, true);
			neutral_culture = GetNeutralResourcesLanguage(MainAssembly);
		}

		private ResourceManager(string baseName, string resourceDir, Type usingResourceSet)
		{
			if (baseName == null)
			{
				throw new ArgumentNullException("baseName");
			}
			if (resourceDir == null)
			{
				throw new ArgumentNullException("resourceDir");
			}
			BaseNameField = baseName;
			this.resourceDir = resourceDir;
			resourceSetType = CheckResourceSetType(usingResourceSet, false);
			ResourceSets = GetResourceSets(MainAssembly, BaseNameField);
		}

		private static Hashtable GetResourceSets(Assembly assembly, string basename)
		{
			lock (ResourceCache)
			{
				string empty = string.Empty;
				empty = ((assembly == null) ? (basename.GetHashCode() + "@@") : assembly.FullName);
				empty = ((basename == null || !(basename != string.Empty)) ? (empty + "!" + empty.GetHashCode()) : (empty + "!" + basename));
				Hashtable hashtable = ResourceCache[empty] as Hashtable;
				if (hashtable == null)
				{
					hashtable = Hashtable.Synchronized(new Hashtable());
					ResourceCache[empty] = hashtable;
				}
				return hashtable;
			}
		}

		private Type CheckResourceSetType(Type usingResourceSet, bool verifyType)
		{
			if (usingResourceSet == null)
			{
				return resourceSetType;
			}
			if (verifyType && !typeof(ResourceSet).IsAssignableFrom(usingResourceSet))
			{
				throw new ArgumentException("Type parameter must refer to a subclass of ResourceSet.", "usingResourceSet");
			}
			return usingResourceSet;
		}

		public static ResourceManager CreateFileBasedResourceManager(string baseName, string resourceDir, Type usingResourceSet)
		{
			return new ResourceManager(baseName, resourceDir, usingResourceSet);
		}

		public virtual object GetObject(string name)
		{
			return GetObject(name, null);
		}

		public virtual object GetObject(string name, CultureInfo culture)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (culture == null)
			{
				culture = CultureInfo.CurrentUICulture;
			}
			lock (this)
			{
				ResourceSet resourceSet = InternalGetResourceSet(culture, true, true);
				object obj = null;
				if (resourceSet != null)
				{
					obj = resourceSet.GetObject(name, ignoreCase);
					if (obj != null)
					{
						return obj;
					}
				}
				do
				{
					culture = culture.Parent;
					resourceSet = InternalGetResourceSet(culture, true, true);
					if (resourceSet != null)
					{
						obj = resourceSet.GetObject(name, ignoreCase);
						if (obj != null)
						{
							return obj;
						}
					}
				}
				while (!culture.Equals(neutral_culture) && !culture.Equals(CultureInfo.InvariantCulture));
			}
			return null;
		}

		public virtual ResourceSet GetResourceSet(CultureInfo culture, bool createIfNotExists, bool tryParents)
		{
			if (culture == null)
			{
				throw new ArgumentNullException("culture");
			}
			lock (this)
			{
				return InternalGetResourceSet(culture, createIfNotExists, tryParents);
			}
		}

		public virtual string GetString(string name)
		{
			return GetString(name, null);
		}

		public virtual string GetString(string name, CultureInfo culture)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (culture == null)
			{
				culture = CultureInfo.CurrentUICulture;
			}
			lock (this)
			{
				ResourceSet resourceSet = InternalGetResourceSet(culture, true, true);
				string text = null;
				if (resourceSet != null)
				{
					text = resourceSet.GetString(name, ignoreCase);
					if (text != null)
					{
						return text;
					}
				}
				do
				{
					culture = culture.Parent;
					resourceSet = InternalGetResourceSet(culture, true, true);
					if (resourceSet != null)
					{
						text = resourceSet.GetString(name, ignoreCase);
						if (text != null)
						{
							return text;
						}
					}
				}
				while (!culture.Equals(neutral_culture) && !culture.Equals(CultureInfo.InvariantCulture));
			}
			return null;
		}

		protected virtual string GetResourceFileName(CultureInfo culture)
		{
			if (culture.Equals(CultureInfo.InvariantCulture))
			{
				return BaseNameField + ".resources";
			}
			return BaseNameField + "." + culture.Name + ".resources";
		}

		private string GetResourceFilePath(CultureInfo culture)
		{
			if (resourceDir != null)
			{
				return Path.Combine(resourceDir, GetResourceFileName(culture));
			}
			return GetResourceFileName(culture);
		}

		private Stream GetManifestResourceStreamNoCase(Assembly ass, string fn)
		{
			string manifestResourceName = GetManifestResourceName(fn);
			string[] manifestResourceNames = ass.GetManifestResourceNames();
			foreach (string text in manifestResourceNames)
			{
				if (string.Compare(manifestResourceName, text, true, CultureInfo.InvariantCulture) == 0)
				{
					return ass.GetManifestResourceStream(text);
				}
			}
			return null;
		}

		[ComVisible(false)]
		[CLSCompliant(false)]
		public UnmanagedMemoryStream GetStream(string name)
		{
			return GetStream(name, null);
		}

		[ComVisible(false)]
		[CLSCompliant(false)]
		public UnmanagedMemoryStream GetStream(string name, CultureInfo culture)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (culture == null)
			{
				culture = CultureInfo.CurrentUICulture;
			}
			ResourceSet resourceSet = InternalGetResourceSet(culture, true, true);
			return resourceSet.GetStream(name, ignoreCase);
		}

		protected virtual ResourceSet InternalGetResourceSet(CultureInfo culture, bool createIfNotExists, bool tryParents)
		{
			if (culture == null)
			{
				throw new ArgumentNullException("key");
			}
			ResourceSet resourceSet = (ResourceSet)ResourceSets[culture];
			if (resourceSet != null)
			{
				return resourceSet;
			}
			if (NonExistent.Contains(culture))
			{
				return null;
			}
			if (MainAssembly != null)
			{
				CultureInfo cultureInfo = culture;
				if (culture.Equals(neutral_culture))
				{
					cultureInfo = CultureInfo.InvariantCulture;
				}
				Stream stream = null;
				string resourceFileName = GetResourceFileName(cultureInfo);
				if (!cultureInfo.Equals(CultureInfo.InvariantCulture))
				{
					Version satelliteContractVersion = GetSatelliteContractVersion(MainAssembly);
					try
					{
						Assembly satelliteAssemblyNoThrow = MainAssembly.GetSatelliteAssemblyNoThrow(cultureInfo, satelliteContractVersion);
						if (satelliteAssemblyNoThrow != null)
						{
							stream = satelliteAssemblyNoThrow.GetManifestResourceStream(resourceFileName);
							if (stream == null)
							{
								stream = GetManifestResourceStreamNoCase(satelliteAssemblyNoThrow, resourceFileName);
							}
						}
					}
					catch (Exception)
					{
					}
				}
				else
				{
					stream = MainAssembly.GetManifestResourceStream(resourceSource, resourceFileName);
					if (stream == null)
					{
						stream = GetManifestResourceStreamNoCase(MainAssembly, resourceFileName);
					}
				}
				if (stream != null && createIfNotExists)
				{
					object[] args = new object[1] { stream };
					resourceSet = (ResourceSet)Activator.CreateInstance(resourceSetType, args);
				}
				else if (cultureInfo.Equals(CultureInfo.InvariantCulture))
				{
					throw AssemblyResourceMissing(resourceFileName);
				}
			}
			else if (resourceDir != null || BaseNameField != null)
			{
				string resourceFilePath = GetResourceFilePath(culture);
				if (createIfNotExists && File.Exists(resourceFilePath))
				{
					object[] args2 = new object[1] { resourceFilePath };
					resourceSet = (ResourceSet)Activator.CreateInstance(resourceSetType, args2);
				}
				else if (culture.Equals(CultureInfo.InvariantCulture))
				{
					string message = string.Format("Could not find any resources appropriate for the specified culture (or the neutral culture) on disk.{0}baseName: {1}  locationInfo: {2}  fileName: {3}", Environment.NewLine, BaseNameField, "<null>", GetResourceFileName(culture));
					throw new MissingManifestResourceException(message);
				}
			}
			if (resourceSet == null && tryParents && !culture.Equals(CultureInfo.InvariantCulture))
			{
				resourceSet = InternalGetResourceSet(culture.Parent, createIfNotExists, tryParents);
			}
			if (resourceSet != null)
			{
				ResourceSets[culture] = resourceSet;
			}
			else
			{
				NonExistent[culture] = culture;
			}
			return resourceSet;
		}

		public virtual void ReleaseAllResources()
		{
			lock (this)
			{
				foreach (ResourceSet value in ResourceSets.Values)
				{
					value.Close();
				}
				ResourceSets.Clear();
			}
		}

		protected static CultureInfo GetNeutralResourcesLanguage(Assembly a)
		{
			object[] customAttributes = a.GetCustomAttributes(typeof(NeutralResourcesLanguageAttribute), false);
			if (customAttributes.Length == 0)
			{
				return CultureInfo.InvariantCulture;
			}
			NeutralResourcesLanguageAttribute neutralResourcesLanguageAttribute = (NeutralResourcesLanguageAttribute)customAttributes[0];
			return new CultureInfo(neutralResourcesLanguageAttribute.CultureName);
		}

		protected static Version GetSatelliteContractVersion(Assembly a)
		{
			object[] customAttributes = a.GetCustomAttributes(typeof(SatelliteContractVersionAttribute), false);
			if (customAttributes.Length == 0)
			{
				return null;
			}
			SatelliteContractVersionAttribute satelliteContractVersionAttribute = (SatelliteContractVersionAttribute)customAttributes[0];
			return new Version(satelliteContractVersionAttribute.Version);
		}

		private MissingManifestResourceException AssemblyResourceMissing(string fileName)
		{
			AssemblyName assemblyName = ((MainAssembly == null) ? null : MainAssembly.GetName());
			string manifestResourceName = GetManifestResourceName(fileName);
			string message = string.Format("Could not find any resources appropriate for the specified culture or the neutral culture.  Make sure \"{0}\" was correctly embedded or linked into assembly \"{1}\" at compile time, or that all the satellite assemblies required are loadable and fully signed.", manifestResourceName, (assemblyName == null) ? string.Empty : assemblyName.Name);
			throw new MissingManifestResourceException(message);
		}

		private string GetManifestResourceName(string fn)
		{
			string text = null;
			if (resourceSource != null)
			{
				if (resourceSource.Namespace != null && resourceSource.Namespace.Length > 0)
				{
					return resourceSource.Namespace + "." + fn;
				}
				return fn;
			}
			return fn;
		}
	}
}

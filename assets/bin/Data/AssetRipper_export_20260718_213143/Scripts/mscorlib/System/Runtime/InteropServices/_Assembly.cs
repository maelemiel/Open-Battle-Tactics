using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;

namespace System.Runtime.InteropServices
{
	[Guid("17156360-2F1A-384A-BC52-FDE93C215C5B")]
	[TypeLibImportClass(typeof(Assembly))]
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[CLSCompliant(false)]
	[ComVisible(true)]
	public interface _Assembly
	{
		string CodeBase { get; }

		string EscapedCodeBase { get; }

		string FullName { get; }

		MethodInfo EntryPoint { get; }

		string Location { get; }

		event ModuleResolveEventHandler ModuleResolve;

		new string ToString();

		new bool Equals(object other);

		new int GetHashCode();

		new Type GetType();

		AssemblyName GetName();

		AssemblyName GetName(bool copiedName);

		Type GetType(string name);

		Type GetType(string name, bool throwOnError);

		Type[] GetExportedTypes();

		Type[] GetTypes();

		Stream GetManifestResourceStream(Type type, string name);

		Stream GetManifestResourceStream(string name);

		FileStream GetFile(string name);

		FileStream[] GetFiles();

		FileStream[] GetFiles(bool getResourceModules);

		string[] GetManifestResourceNames();

		ManifestResourceInfo GetManifestResourceInfo(string resourceName);

		object[] GetCustomAttributes(Type attributeType, bool inherit);

		object[] GetCustomAttributes(bool inherit);

		bool IsDefined(Type attributeType, bool inherit);

		void GetObjectData(SerializationInfo info, StreamingContext context);

		Type GetType(string name, bool throwOnError, bool ignoreCase);

		Assembly GetSatelliteAssembly(CultureInfo culture);

		Assembly GetSatelliteAssembly(CultureInfo culture, Version version);

		Module LoadModule(string moduleName, byte[] rawModule);

		Module LoadModule(string moduleName, byte[] rawModule, byte[] rawSymbolStore);

		object CreateInstance(string typeName);

		object CreateInstance(string typeName, bool ignoreCase);

		object CreateInstance(string typeName, bool ignoreCase, BindingFlags bindingAttr, Binder binder, object[] args, CultureInfo culture, object[] activationAttributes);

		Module[] GetLoadedModules();

		Module[] GetLoadedModules(bool getResourceModules);

		Module[] GetModules();

		Module[] GetModules(bool getResourceModules);

		Module GetModule(string name);

		AssemblyName[] GetReferencedAssemblies();
	}
}

using System.Reflection;

namespace System.ComponentModel.Design
{
	public interface ITypeResolutionService
	{
		Assembly GetAssembly(AssemblyName name);

		Assembly GetAssembly(AssemblyName name, bool throwOnError);

		string GetPathOfAssembly(AssemblyName name);

		Type GetType(string name);

		Type GetType(string name, bool throwOnError);

		Type GetType(string name, bool throwOnError, bool ignoreCase);

		void ReferenceAssembly(AssemblyName name);
	}
}

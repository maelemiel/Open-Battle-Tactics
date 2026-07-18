using System.Reflection;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting
{
	[ComVisible(true)]
	public class WellKnownClientTypeEntry : TypeEntry
	{
		private Type obj_type;

		private string obj_url;

		private string app_url;

		public string ApplicationUrl
		{
			get
			{
				return app_url;
			}
			set
			{
				app_url = value;
			}
		}

		public Type ObjectType
		{
			get
			{
				return obj_type;
			}
		}

		public string ObjectUrl
		{
			get
			{
				return obj_url;
			}
		}

		public WellKnownClientTypeEntry(Type type, string objectUrl)
		{
			base.AssemblyName = type.Assembly.FullName;
			base.TypeName = type.FullName;
			obj_type = type;
			obj_url = objectUrl;
		}

		public WellKnownClientTypeEntry(string typeName, string assemblyName, string objectUrl)
		{
			obj_url = objectUrl;
			base.AssemblyName = assemblyName;
			base.TypeName = typeName;
			Assembly assembly = Assembly.Load(assemblyName);
			obj_type = assembly.GetType(typeName);
			if (obj_type == null)
			{
				throw new RemotingException("Type not found: " + typeName + ", " + assemblyName);
			}
		}

		public override string ToString()
		{
			if (ApplicationUrl != null)
			{
				return base.TypeName + base.AssemblyName + ObjectUrl + ApplicationUrl;
			}
			return base.TypeName + base.AssemblyName + ObjectUrl;
		}
	}
}

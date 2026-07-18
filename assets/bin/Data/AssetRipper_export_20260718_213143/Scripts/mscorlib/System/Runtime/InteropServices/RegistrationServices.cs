using System.Reflection;

namespace System.Runtime.InteropServices
{
	[Guid("475e398f-8afa-43a7-a3be-f4ef8d6787c9")]
	[ClassInterface(ClassInterfaceType.None)]
	[ComVisible(true)]
	public class RegistrationServices : IRegistrationServices
	{
		[MonoTODO("implement")]
		public virtual Guid GetManagedCategoryGuid()
		{
			throw new NotImplementedException();
		}

		[MonoTODO("implement")]
		public virtual string GetProgIdForType(Type type)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("implement")]
		public virtual Type[] GetRegistrableTypesInAssembly(Assembly assembly)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("implement")]
		public virtual bool RegisterAssembly(Assembly assembly, AssemblyRegistrationFlags flags)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("implement")]
		public virtual void RegisterTypeForComClients(Type type, ref Guid g)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("implement")]
		public virtual bool TypeRepresentsComType(Type type)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("implement")]
		public virtual bool TypeRequiresRegistration(Type type)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("implement")]
		public virtual bool UnregisterAssembly(Assembly assembly)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("implement")]
		[ComVisible(false)]
		public virtual int RegisterTypeForComClients(Type type, RegistrationClassContext classContext, RegistrationConnectionType flags)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("implement")]
		[ComVisible(false)]
		public virtual void UnregisterTypeForComClients(int cookie)
		{
			throw new NotImplementedException();
		}
	}
}

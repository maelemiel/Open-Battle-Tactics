using System.Runtime.InteropServices;

namespace System.Security.AccessControl
{
	public abstract class NativeObjectSecurity : CommonObjectSecurity
	{
		protected internal delegate Exception ExceptionFromErrorCode(int errorCode, string name, SafeHandle handle, object context);

		internal NativeObjectSecurity()
			: base(false)
		{
		}

		protected NativeObjectSecurity(bool isContainer, ResourceType resourceType)
			: base(isContainer)
		{
		}

		protected NativeObjectSecurity(bool isContainer, ResourceType resourceType, ExceptionFromErrorCode exceptionFromErrorCode, object exceptionContext)
			: this(isContainer, resourceType)
		{
		}

		protected NativeObjectSecurity(bool isContainer, ResourceType resourceType, SafeHandle handle, AccessControlSections includeSections)
			: this(isContainer, resourceType)
		{
		}

		protected NativeObjectSecurity(bool isContainer, ResourceType resourceType, string name, AccessControlSections includeSections)
			: this(isContainer, resourceType)
		{
		}

		protected NativeObjectSecurity(bool isContainer, ResourceType resourceType, SafeHandle handle, AccessControlSections includeSections, ExceptionFromErrorCode exceptionFromErrorCode, object exceptionContext)
			: this(isContainer, resourceType, handle, includeSections)
		{
		}

		protected NativeObjectSecurity(bool isContainer, ResourceType resourceType, string name, AccessControlSections includeSections, ExceptionFromErrorCode exceptionFromErrorCode, object exceptionContext)
			: this(isContainer, resourceType, name, includeSections)
		{
		}

		protected sealed override void Persist(SafeHandle handle, AccessControlSections includeSections)
		{
			throw new NotImplementedException();
		}

		protected sealed override void Persist(string name, AccessControlSections includeSections)
		{
			throw new NotImplementedException();
		}

		protected void Persist(SafeHandle handle, AccessControlSections includeSections, object exceptionContext)
		{
			throw new NotImplementedException();
		}

		protected void Persist(string name, AccessControlSections includeSections, object exceptionContext)
		{
			throw new NotImplementedException();
		}
	}
}

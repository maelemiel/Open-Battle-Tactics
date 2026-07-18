using System.Runtime.InteropServices;

namespace System.Security.Permissions
{
	[Serializable]
	[ComVisible(true)]
	public sealed class IsolatedStorageFilePermission : IsolatedStoragePermission, IBuiltInPermission
	{
		private const int version = 1;

		public IsolatedStorageFilePermission(PermissionState state)
			: base(state)
		{
		}

		int IBuiltInPermission.GetTokenIndex()
		{
			return 3;
		}

		public override IPermission Copy()
		{
			IsolatedStorageFilePermission isolatedStorageFilePermission = new IsolatedStorageFilePermission(PermissionState.None);
			isolatedStorageFilePermission.m_userQuota = m_userQuota;
			isolatedStorageFilePermission.m_machineQuota = m_machineQuota;
			isolatedStorageFilePermission.m_expirationDays = m_expirationDays;
			isolatedStorageFilePermission.m_permanentData = m_permanentData;
			isolatedStorageFilePermission.m_allowed = m_allowed;
			return isolatedStorageFilePermission;
		}

		public override IPermission Intersect(IPermission target)
		{
			IsolatedStorageFilePermission isolatedStorageFilePermission = Cast(target);
			if (isolatedStorageFilePermission == null)
			{
				return null;
			}
			if (IsEmpty() && isolatedStorageFilePermission.IsEmpty())
			{
				return null;
			}
			IsolatedStorageFilePermission isolatedStorageFilePermission2 = new IsolatedStorageFilePermission(PermissionState.None);
			isolatedStorageFilePermission2.m_userQuota = ((m_userQuota >= isolatedStorageFilePermission.m_userQuota) ? isolatedStorageFilePermission.m_userQuota : m_userQuota);
			isolatedStorageFilePermission2.m_machineQuota = ((m_machineQuota >= isolatedStorageFilePermission.m_machineQuota) ? isolatedStorageFilePermission.m_machineQuota : m_machineQuota);
			isolatedStorageFilePermission2.m_expirationDays = ((m_expirationDays >= isolatedStorageFilePermission.m_expirationDays) ? isolatedStorageFilePermission.m_expirationDays : m_expirationDays);
			isolatedStorageFilePermission2.m_permanentData = m_permanentData && isolatedStorageFilePermission.m_permanentData;
			isolatedStorageFilePermission2.UsageAllowed = ((m_allowed >= isolatedStorageFilePermission.m_allowed) ? isolatedStorageFilePermission.m_allowed : m_allowed);
			return isolatedStorageFilePermission2;
		}

		public override bool IsSubsetOf(IPermission target)
		{
			IsolatedStorageFilePermission isolatedStorageFilePermission = Cast(target);
			if (isolatedStorageFilePermission == null)
			{
				return IsEmpty();
			}
			if (isolatedStorageFilePermission.IsUnrestricted())
			{
				return true;
			}
			if (m_userQuota > isolatedStorageFilePermission.m_userQuota)
			{
				return false;
			}
			if (m_machineQuota > isolatedStorageFilePermission.m_machineQuota)
			{
				return false;
			}
			if (m_expirationDays > isolatedStorageFilePermission.m_expirationDays)
			{
				return false;
			}
			if (m_permanentData != isolatedStorageFilePermission.m_permanentData)
			{
				return false;
			}
			if (m_allowed > isolatedStorageFilePermission.m_allowed)
			{
				return false;
			}
			return true;
		}

		public override IPermission Union(IPermission target)
		{
			IsolatedStorageFilePermission isolatedStorageFilePermission = Cast(target);
			if (isolatedStorageFilePermission == null)
			{
				return Copy();
			}
			IsolatedStorageFilePermission isolatedStorageFilePermission2 = new IsolatedStorageFilePermission(PermissionState.None);
			isolatedStorageFilePermission2.m_userQuota = ((m_userQuota <= isolatedStorageFilePermission.m_userQuota) ? isolatedStorageFilePermission.m_userQuota : m_userQuota);
			isolatedStorageFilePermission2.m_machineQuota = ((m_machineQuota <= isolatedStorageFilePermission.m_machineQuota) ? isolatedStorageFilePermission.m_machineQuota : m_machineQuota);
			isolatedStorageFilePermission2.m_expirationDays = ((m_expirationDays <= isolatedStorageFilePermission.m_expirationDays) ? isolatedStorageFilePermission.m_expirationDays : m_expirationDays);
			isolatedStorageFilePermission2.m_permanentData = m_permanentData || isolatedStorageFilePermission.m_permanentData;
			isolatedStorageFilePermission2.UsageAllowed = ((m_allowed <= isolatedStorageFilePermission.m_allowed) ? isolatedStorageFilePermission.m_allowed : m_allowed);
			return isolatedStorageFilePermission2;
		}

		[ComVisible(false)]
		[MonoTODO("(2.0) new override - something must have been added ???")]
		public override SecurityElement ToXml()
		{
			return base.ToXml();
		}

		private IsolatedStorageFilePermission Cast(IPermission target)
		{
			if (target == null)
			{
				return null;
			}
			IsolatedStorageFilePermission isolatedStorageFilePermission = target as IsolatedStorageFilePermission;
			if (isolatedStorageFilePermission == null)
			{
				CodeAccessPermission.ThrowInvalidPermission(target, typeof(IsolatedStorageFilePermission));
			}
			return isolatedStorageFilePermission;
		}
	}
}

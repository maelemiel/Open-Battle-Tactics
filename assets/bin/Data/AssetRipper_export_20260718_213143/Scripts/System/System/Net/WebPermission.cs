using System.Collections;
using System.Security;
using System.Security.Permissions;
using System.Text.RegularExpressions;

namespace System.Net
{
	[Serializable]
	[System.MonoTODO("Most private members that include functionallity are not implemented!")]
	public sealed class WebPermission : CodeAccessPermission, IUnrestrictedPermission
	{
		private ArrayList m_acceptList = new ArrayList();

		private ArrayList m_connectList = new ArrayList();

		private bool m_noRestriction;

		public IEnumerator AcceptList
		{
			get
			{
				return m_acceptList.GetEnumerator();
			}
		}

		public IEnumerator ConnectList
		{
			get
			{
				return m_connectList.GetEnumerator();
			}
		}

		public WebPermission()
		{
		}

		public WebPermission(PermissionState state)
		{
			m_noRestriction = state == PermissionState.Unrestricted;
		}

		public WebPermission(NetworkAccess access, string uriString)
		{
			AddPermission(access, uriString);
		}

		public WebPermission(NetworkAccess access, Regex uriRegex)
		{
			AddPermission(access, uriRegex);
		}

		public void AddPermission(NetworkAccess access, string uriString)
		{
			System.Net.WebPermissionInfo info = new System.Net.WebPermissionInfo(System.Net.WebPermissionInfoType.InfoString, uriString);
			AddPermission(access, info);
		}

		public void AddPermission(NetworkAccess access, Regex uriRegex)
		{
			System.Net.WebPermissionInfo info = new System.Net.WebPermissionInfo(uriRegex);
			AddPermission(access, info);
		}

		internal void AddPermission(NetworkAccess access, System.Net.WebPermissionInfo info)
		{
			switch (access)
			{
			case NetworkAccess.Accept:
				m_acceptList.Add(info);
				break;
			case NetworkAccess.Connect:
				m_connectList.Add(info);
				break;
			default:
			{
				string text = Locale.GetText("Unknown NetworkAccess value {0}.");
				throw new ArgumentException(string.Format(text, access), "access");
			}
			}
		}

		public override IPermission Copy()
		{
			WebPermission webPermission = new WebPermission(m_noRestriction ? PermissionState.Unrestricted : PermissionState.None);
			webPermission.m_connectList = (ArrayList)m_connectList.Clone();
			webPermission.m_acceptList = (ArrayList)m_acceptList.Clone();
			return webPermission;
		}

		public override IPermission Intersect(IPermission target)
		{
			if (target == null)
			{
				return null;
			}
			WebPermission webPermission = target as WebPermission;
			if (webPermission == null)
			{
				throw new ArgumentException("Argument not of type WebPermission");
			}
			if (m_noRestriction)
			{
				IPermission result;
				if (IntersectEmpty(webPermission))
				{
					IPermission permission = null;
					result = permission;
				}
				else
				{
					result = webPermission.Copy();
				}
				return result;
			}
			if (webPermission.m_noRestriction)
			{
				IPermission result2;
				if (IntersectEmpty(this))
				{
					IPermission permission = null;
					result2 = permission;
				}
				else
				{
					result2 = Copy();
				}
				return result2;
			}
			WebPermission webPermission2 = new WebPermission(PermissionState.None);
			Intersect(m_connectList, webPermission.m_connectList, webPermission2.m_connectList);
			Intersect(m_acceptList, webPermission.m_acceptList, webPermission2.m_acceptList);
			return (!IntersectEmpty(webPermission2)) ? webPermission2 : null;
		}

		private bool IntersectEmpty(WebPermission permission)
		{
			return !permission.m_noRestriction && permission.m_connectList.Count == 0 && permission.m_acceptList.Count == 0;
		}

		[System.MonoTODO]
		private void Intersect(ArrayList list1, ArrayList list2, ArrayList result)
		{
			throw new NotImplementedException();
		}

		public override bool IsSubsetOf(IPermission target)
		{
			if (target == null)
			{
				return !m_noRestriction && m_connectList.Count == 0 && m_acceptList.Count == 0;
			}
			WebPermission webPermission = target as WebPermission;
			if (webPermission == null)
			{
				throw new ArgumentException("Parameter target must be of type WebPermission");
			}
			if (webPermission.m_noRestriction)
			{
				return true;
			}
			if (m_noRestriction)
			{
				return false;
			}
			if (m_acceptList.Count == 0 && m_connectList.Count == 0)
			{
				return true;
			}
			if (webPermission.m_acceptList.Count == 0 && webPermission.m_connectList.Count == 0)
			{
				return false;
			}
			return IsSubsetOf(m_connectList, webPermission.m_connectList) && IsSubsetOf(m_acceptList, webPermission.m_acceptList);
		}

		[System.MonoTODO]
		private bool IsSubsetOf(ArrayList list1, ArrayList list2)
		{
			throw new NotImplementedException();
		}

		public bool IsUnrestricted()
		{
			return m_noRestriction;
		}

		public override SecurityElement ToXml()
		{
			SecurityElement securityElement = new SecurityElement("IPermission");
			securityElement.AddAttribute("class", GetType().AssemblyQualifiedName);
			securityElement.AddAttribute("version", "1");
			if (m_noRestriction)
			{
				securityElement.AddAttribute("Unrestricted", "true");
				return securityElement;
			}
			if (m_connectList.Count > 0)
			{
				ToXml(securityElement, "ConnectAccess", m_connectList.GetEnumerator());
			}
			if (m_acceptList.Count > 0)
			{
				ToXml(securityElement, "AcceptAccess", m_acceptList.GetEnumerator());
			}
			return securityElement;
		}

		private void ToXml(SecurityElement root, string childName, IEnumerator enumerator)
		{
			SecurityElement securityElement = new SecurityElement(childName, null);
			root.AddChild(securityElement);
			while (enumerator.MoveNext())
			{
				System.Net.WebPermissionInfo webPermissionInfo = enumerator.Current as System.Net.WebPermissionInfo;
				if (webPermissionInfo != null)
				{
					SecurityElement securityElement2 = new SecurityElement("URI");
					securityElement2.AddAttribute("uri", webPermissionInfo.Info);
					securityElement.AddChild(securityElement2);
				}
			}
		}

		public override void FromXml(SecurityElement securityElement)
		{
			if (securityElement == null)
			{
				throw new ArgumentNullException("securityElement");
			}
			if (securityElement.Tag != "IPermission")
			{
				throw new ArgumentException("securityElement");
			}
			string text = securityElement.Attribute("Unrestricted");
			if (text != null)
			{
				m_noRestriction = string.Compare(text, "true", true) == 0;
				if (m_noRestriction)
				{
					return;
				}
			}
			m_noRestriction = false;
			m_connectList = new ArrayList();
			m_acceptList = new ArrayList();
			ArrayList children = securityElement.Children;
			foreach (SecurityElement item in children)
			{
				if (item.Tag == "ConnectAccess")
				{
					FromXml(item.Children, NetworkAccess.Connect);
				}
				else if (item.Tag == "AcceptAccess")
				{
					FromXml(item.Children, NetworkAccess.Accept);
				}
			}
		}

		private void FromXml(ArrayList endpoints, NetworkAccess access)
		{
			throw new NotImplementedException();
		}

		public override IPermission Union(IPermission target)
		{
			if (target == null)
			{
				return null;
			}
			WebPermission webPermission = target as WebPermission;
			if (webPermission == null)
			{
				throw new ArgumentException("Argument not of type WebPermission");
			}
			if (m_noRestriction || webPermission.m_noRestriction)
			{
				return new WebPermission(PermissionState.Unrestricted);
			}
			WebPermission webPermission2 = (WebPermission)webPermission.Copy();
			webPermission2.m_acceptList.InsertRange(webPermission2.m_acceptList.Count, m_acceptList);
			webPermission2.m_connectList.InsertRange(webPermission2.m_connectList.Count, m_connectList);
			return webPermission2;
		}
	}
}

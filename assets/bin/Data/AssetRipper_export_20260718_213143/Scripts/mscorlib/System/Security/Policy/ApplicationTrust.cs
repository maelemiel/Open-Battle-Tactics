using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Permissions;
using Mono.Security.Cryptography;

namespace System.Security.Policy
{
	[Serializable]
	[ComVisible(true)]
	public sealed class ApplicationTrust : ISecurityEncodable
	{
		private ApplicationIdentity _appid;

		private PolicyStatement _defaultPolicy;

		private object _xtranfo;

		private bool _trustrun;

		private bool _persist;

		private IList<StrongName> fullTrustAssemblies;

		public ApplicationIdentity ApplicationIdentity
		{
			get
			{
				return _appid;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("ApplicationIdentity");
				}
				_appid = value;
			}
		}

		public PolicyStatement DefaultGrantSet
		{
			get
			{
				if (_defaultPolicy == null)
				{
					_defaultPolicy = GetDefaultGrantSet();
				}
				return _defaultPolicy;
			}
			set
			{
				_defaultPolicy = value;
			}
		}

		public object ExtraInfo
		{
			get
			{
				return _xtranfo;
			}
			set
			{
				_xtranfo = value;
			}
		}

		public bool IsApplicationTrustedToRun
		{
			get
			{
				return _trustrun;
			}
			set
			{
				_trustrun = value;
			}
		}

		public bool Persist
		{
			get
			{
				return _persist;
			}
			set
			{
				_persist = value;
			}
		}

		public ApplicationTrust()
		{
			fullTrustAssemblies = new List<StrongName>(0);
		}

		public ApplicationTrust(ApplicationIdentity applicationIdentity)
			: this()
		{
			if (applicationIdentity == null)
			{
				throw new ArgumentNullException("applicationIdentity");
			}
			_appid = applicationIdentity;
		}

		internal ApplicationTrust(PermissionSet defaultGrantSet, IEnumerable<StrongName> fullTrustAssemblies)
		{
			if (defaultGrantSet == null)
			{
				throw new ArgumentNullException("defaultGrantSet");
			}
			_defaultPolicy = new PolicyStatement(defaultGrantSet);
			if (fullTrustAssemblies == null)
			{
				throw new ArgumentNullException("fullTrustAssemblies");
			}
			this.fullTrustAssemblies = new List<StrongName>();
			foreach (StrongName fullTrustAssembly in fullTrustAssemblies)
			{
				if (fullTrustAssembly == null)
				{
					throw new ArgumentException("fullTrustAssemblies contains an assembly that does not have a StrongName");
				}
				this.fullTrustAssemblies.Add((StrongName)fullTrustAssembly.Copy());
			}
		}

		public void FromXml(SecurityElement element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			if (element.Tag != "ApplicationTrust")
			{
				throw new ArgumentException("element");
			}
			string text = element.Attribute("FullName");
			if (text != null)
			{
				_appid = new ApplicationIdentity(text);
			}
			else
			{
				_appid = null;
			}
			_defaultPolicy = null;
			SecurityElement securityElement = element.SearchForChildByTag("DefaultGrant");
			if (securityElement != null)
			{
				for (int i = 0; i < securityElement.Children.Count; i++)
				{
					SecurityElement securityElement2 = securityElement.Children[i] as SecurityElement;
					if (securityElement2.Tag == "PolicyStatement")
					{
						DefaultGrantSet.FromXml(securityElement2, null);
						break;
					}
				}
			}
			if (!bool.TryParse(element.Attribute("TrustedToRun"), out _trustrun))
			{
				_trustrun = false;
			}
			if (!bool.TryParse(element.Attribute("Persist"), out _persist))
			{
				_persist = false;
			}
			_xtranfo = null;
			SecurityElement securityElement3 = element.SearchForChildByTag("ExtraInfo");
			if (securityElement3 == null)
			{
				return;
			}
			text = securityElement3.Attribute("Data");
			if (text != null)
			{
				byte[] buffer = CryptoConvert.FromHex(text);
				using (MemoryStream serializationStream = new MemoryStream(buffer))
				{
					BinaryFormatter binaryFormatter = new BinaryFormatter();
					_xtranfo = binaryFormatter.Deserialize(serializationStream);
				}
			}
		}

		public SecurityElement ToXml()
		{
			SecurityElement securityElement = new SecurityElement("ApplicationTrust");
			securityElement.AddAttribute("version", "1");
			if (_appid != null)
			{
				securityElement.AddAttribute("FullName", _appid.FullName);
			}
			if (_trustrun)
			{
				securityElement.AddAttribute("TrustedToRun", "true");
			}
			if (_persist)
			{
				securityElement.AddAttribute("Persist", "true");
			}
			SecurityElement securityElement2 = new SecurityElement("DefaultGrant");
			securityElement2.AddChild(DefaultGrantSet.ToXml());
			securityElement.AddChild(securityElement2);
			if (_xtranfo != null)
			{
				byte[] input = null;
				using (MemoryStream memoryStream = new MemoryStream())
				{
					BinaryFormatter binaryFormatter = new BinaryFormatter();
					binaryFormatter.Serialize(memoryStream, _xtranfo);
					input = memoryStream.ToArray();
				}
				SecurityElement securityElement3 = new SecurityElement("ExtraInfo");
				securityElement3.AddAttribute("Data", CryptoConvert.ToHex(input));
				securityElement.AddChild(securityElement3);
			}
			return securityElement;
		}

		private PolicyStatement GetDefaultGrantSet()
		{
			PermissionSet permSet = new PermissionSet(PermissionState.None);
			return new PolicyStatement(permSet);
		}
	}
}

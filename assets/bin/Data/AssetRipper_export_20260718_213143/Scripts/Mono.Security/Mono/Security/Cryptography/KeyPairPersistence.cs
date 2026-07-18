using System;
using System.Globalization;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using Mono.Xml;

namespace Mono.Security.Cryptography
{
	public class KeyPairPersistence
	{
		private static bool _userPathExists = false;

		private static string _userPath;

		private static bool _machinePathExists = false;

		private static string _machinePath;

		private CspParameters _params;

		private string _keyvalue;

		private string _filename;

		private string _container;

		private static object lockobj = new object();

		public string Filename
		{
			get
			{
				if (_filename == null)
				{
					_filename = string.Format(CultureInfo.InvariantCulture, "[{0}][{1}][{2}].xml", _params.ProviderType, ContainerName, _params.KeyNumber);
					if (UseMachineKeyStore)
					{
						_filename = Path.Combine(MachinePath, _filename);
					}
					else
					{
						_filename = Path.Combine(UserPath, _filename);
					}
				}
				return _filename;
			}
		}

		public string KeyValue
		{
			get
			{
				return _keyvalue;
			}
			set
			{
				if (CanChange)
				{
					_keyvalue = value;
				}
			}
		}

		public CspParameters Parameters
		{
			get
			{
				return Copy(_params);
			}
		}

		private static string UserPath
		{
			get
			{
				lock (lockobj)
				{
					if (_userPath == null || !_userPathExists)
					{
						_userPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".mono");
						_userPath = Path.Combine(_userPath, "keypairs");
						_userPathExists = Directory.Exists(_userPath);
						if (!_userPathExists)
						{
							try
							{
								Directory.CreateDirectory(_userPath);
								ProtectUser(_userPath);
								_userPathExists = true;
							}
							catch (Exception inner)
							{
								string text = global::Locale.GetText("Could not create user key store '{0}'.");
								throw new CryptographicException(string.Format(text, _userPath), inner);
							}
						}
					}
				}
				if (!IsUserProtected(_userPath))
				{
					string text2 = global::Locale.GetText("Improperly protected user's key pairs in '{0}'.");
					throw new CryptographicException(string.Format(text2, _userPath));
				}
				return _userPath;
			}
		}

		private static string MachinePath
		{
			get
			{
				lock (lockobj)
				{
					if (_machinePath == null || !_machinePathExists)
					{
						_machinePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), ".mono");
						_machinePath = Path.Combine(_machinePath, "keypairs");
						_machinePathExists = Directory.Exists(_machinePath);
						if (!_machinePathExists)
						{
							try
							{
								Directory.CreateDirectory(_machinePath);
								ProtectMachine(_machinePath);
								_machinePathExists = true;
							}
							catch (Exception inner)
							{
								string text = global::Locale.GetText("Could not create machine key store '{0}'.");
								throw new CryptographicException(string.Format(text, _machinePath), inner);
							}
						}
					}
				}
				if (!IsMachineProtected(_machinePath))
				{
					string text2 = global::Locale.GetText("Improperly protected machine's key pairs in '{0}'.");
					throw new CryptographicException(string.Format(text2, _machinePath));
				}
				return _machinePath;
			}
		}

		private bool CanChange
		{
			get
			{
				return _keyvalue == null;
			}
		}

		private bool UseDefaultKeyContainer
		{
			get
			{
				return (_params.Flags & CspProviderFlags.UseDefaultKeyContainer) == CspProviderFlags.UseDefaultKeyContainer;
			}
		}

		private bool UseMachineKeyStore
		{
			get
			{
				return (_params.Flags & CspProviderFlags.UseMachineKeyStore) == CspProviderFlags.UseMachineKeyStore;
			}
		}

		private string ContainerName
		{
			get
			{
				if (_container == null)
				{
					if (UseDefaultKeyContainer)
					{
						_container = "default";
					}
					else if (_params.KeyContainerName == null || _params.KeyContainerName.Length == 0)
					{
						_container = Guid.NewGuid().ToString();
					}
					else
					{
						byte[] bytes = Encoding.UTF8.GetBytes(_params.KeyContainerName);
						MD5 mD = MD5.Create();
						byte[] b = mD.ComputeHash(bytes);
						_container = new Guid(b).ToString();
					}
				}
				return _container;
			}
		}

		public KeyPairPersistence(CspParameters parameters)
			: this(parameters, null)
		{
		}

		public KeyPairPersistence(CspParameters parameters, string keyPair)
		{
			if (parameters == null)
			{
				throw new ArgumentNullException("parameters");
			}
			_params = Copy(parameters);
			_keyvalue = keyPair;
		}

		public bool Load()
		{
			bool flag = File.Exists(Filename);
			if (flag)
			{
				using (StreamReader streamReader = File.OpenText(Filename))
				{
					FromXml(streamReader.ReadToEnd());
				}
			}
			return flag;
		}

		public void Save()
		{
			using (FileStream stream = File.Open(Filename, FileMode.Create))
			{
				StreamWriter streamWriter = new StreamWriter(stream, Encoding.UTF8);
				streamWriter.Write(ToXml());
				streamWriter.Close();
			}
			if (UseMachineKeyStore)
			{
				ProtectMachine(Filename);
			}
			else
			{
				ProtectUser(Filename);
			}
		}

		public void Remove()
		{
			File.Delete(Filename);
		}

		internal static bool _CanSecure(string root)
		{
			return true;
		}

		internal static bool _ProtectUser(string path)
		{
			return true;
		}

		internal static bool _ProtectMachine(string path)
		{
			return true;
		}

		internal static bool _IsUserProtected(string path)
		{
			return true;
		}

		internal static bool _IsMachineProtected(string path)
		{
			return true;
		}

		private static bool CanSecure(string path)
		{
			int platform = (int)Environment.OSVersion.Platform;
			if (platform == 4 || platform == 128 || platform == 6)
			{
				return true;
			}
			return _CanSecure(Path.GetPathRoot(path));
		}

		private static bool ProtectUser(string path)
		{
			if (CanSecure(path))
			{
				return _ProtectUser(path);
			}
			return true;
		}

		private static bool ProtectMachine(string path)
		{
			if (CanSecure(path))
			{
				return _ProtectMachine(path);
			}
			return true;
		}

		private static bool IsUserProtected(string path)
		{
			if (CanSecure(path))
			{
				return _IsUserProtected(path);
			}
			return true;
		}

		private static bool IsMachineProtected(string path)
		{
			if (CanSecure(path))
			{
				return _IsMachineProtected(path);
			}
			return true;
		}

		private CspParameters Copy(CspParameters p)
		{
			CspParameters cspParameters = new CspParameters(p.ProviderType, p.ProviderName, p.KeyContainerName);
			cspParameters.KeyNumber = p.KeyNumber;
			cspParameters.Flags = p.Flags;
			return cspParameters;
		}

		private void FromXml(string xml)
		{
			SecurityParser securityParser = new SecurityParser();
			securityParser.LoadXml(xml);
			SecurityElement securityElement = securityParser.ToXml();
			if (securityElement.Tag == "KeyPair")
			{
				SecurityElement securityElement2 = securityElement.SearchForChildByTag("KeyValue");
				if (securityElement2.Children.Count > 0)
				{
					_keyvalue = securityElement2.Children[0].ToString();
				}
			}
		}

		private string ToXml()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("<KeyPair>{0}\t<Properties>{0}\t\t<Provider ", Environment.NewLine);
			if (_params.ProviderName != null && _params.ProviderName.Length != 0)
			{
				stringBuilder.AppendFormat("Name=\"{0}\" ", _params.ProviderName);
			}
			stringBuilder.AppendFormat("Type=\"{0}\" />{1}\t\t<Container ", _params.ProviderType, Environment.NewLine);
			stringBuilder.AppendFormat("Name=\"{0}\" />{1}\t</Properties>{1}\t<KeyValue", ContainerName, Environment.NewLine);
			if (_params.KeyNumber != -1)
			{
				stringBuilder.AppendFormat(" Id=\"{0}\" ", _params.KeyNumber);
			}
			stringBuilder.AppendFormat(">{1}\t\t{0}{1}\t</KeyValue>{1}</KeyPair>{1}", KeyValue, Environment.NewLine);
			return stringBuilder.ToString();
		}
	}
}

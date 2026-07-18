using System.Configuration.Assemblies;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using Mono.Security;
using Mono.Security.Cryptography;

namespace System.Reflection
{
	[Serializable]
	[ComDefaultInterface(typeof(_AssemblyName))]
	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.None)]
	public sealed class AssemblyName : ICloneable, ISerializable, _AssemblyName, IDeserializationCallback
	{
		private string name;

		private string codebase;

		private int major;

		private int minor;

		private int build;

		private int revision;

		private CultureInfo cultureinfo;

		private AssemblyNameFlags flags;

		private AssemblyHashAlgorithm hashalg;

		private StrongNameKeyPair keypair;

		private byte[] publicKey;

		private byte[] keyToken;

		private AssemblyVersionCompatibility versioncompat;

		private Version version;

		private ProcessorArchitecture processor_architecture;

		[MonoTODO("Not used, as the values are too limited;  Mono supports more")]
		public ProcessorArchitecture ProcessorArchitecture
		{
			get
			{
				return processor_architecture;
			}
			set
			{
				processor_architecture = value;
			}
		}

		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
			}
		}

		public string CodeBase
		{
			get
			{
				return codebase;
			}
			set
			{
				codebase = value;
			}
		}

		public string EscapedCodeBase
		{
			get
			{
				if (codebase == null)
				{
					return null;
				}
				return Uri.EscapeString(codebase, false, true, true);
			}
		}

		public CultureInfo CultureInfo
		{
			get
			{
				return cultureinfo;
			}
			set
			{
				cultureinfo = value;
			}
		}

		public AssemblyNameFlags Flags
		{
			get
			{
				return flags;
			}
			set
			{
				flags = value;
			}
		}

		public string FullName
		{
			get
			{
				if (name == null)
				{
					return string.Empty;
				}
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(name);
				if (Version != null)
				{
					stringBuilder.Append(", Version=");
					stringBuilder.Append(Version.ToString());
				}
				if (cultureinfo != null)
				{
					stringBuilder.Append(", Culture=");
					if (cultureinfo.LCID == CultureInfo.InvariantCulture.LCID)
					{
						stringBuilder.Append("neutral");
					}
					else
					{
						stringBuilder.Append(cultureinfo.Name);
					}
				}
				byte[] array = InternalGetPublicKeyToken();
				if (array != null)
				{
					if (array.Length == 0)
					{
						stringBuilder.Append(", PublicKeyToken=null");
					}
					else
					{
						stringBuilder.Append(", PublicKeyToken=");
						for (int i = 0; i < array.Length; i++)
						{
							stringBuilder.Append(array[i].ToString("x2"));
						}
					}
				}
				if ((Flags & AssemblyNameFlags.Retargetable) != AssemblyNameFlags.None)
				{
					stringBuilder.Append(", Retargetable=Yes");
				}
				return stringBuilder.ToString();
			}
		}

		public AssemblyHashAlgorithm HashAlgorithm
		{
			get
			{
				return hashalg;
			}
			set
			{
				hashalg = value;
			}
		}

		public StrongNameKeyPair KeyPair
		{
			get
			{
				return keypair;
			}
			set
			{
				keypair = value;
			}
		}

		public Version Version
		{
			get
			{
				return version;
			}
			set
			{
				version = value;
				if (value == null)
				{
					major = (minor = (build = (revision = 0)));
					return;
				}
				major = value.Major;
				minor = value.Minor;
				build = value.Build;
				revision = value.Revision;
			}
		}

		public AssemblyVersionCompatibility VersionCompatibility
		{
			get
			{
				return versioncompat;
			}
			set
			{
				versioncompat = value;
			}
		}

		private bool IsPublicKeyValid
		{
			get
			{
				if (publicKey.Length == 16)
				{
					int num = 0;
					int num2 = 0;
					while (num < publicKey.Length)
					{
						num2 += publicKey[num++];
					}
					if (num2 == 4)
					{
						return true;
					}
				}
				switch (publicKey[0])
				{
				case 0:
					if (publicKey.Length > 12 && publicKey[12] == 6)
					{
						try
						{
							CryptoConvert.FromCapiPublicKeyBlob(publicKey, 12);
							return true;
						}
						catch (CryptographicException)
						{
						}
					}
					break;
				case 6:
					try
					{
						CryptoConvert.FromCapiPublicKeyBlob(publicKey);
						return true;
					}
					catch (CryptographicException)
					{
					}
					break;
				}
				return false;
			}
		}

		public AssemblyName()
		{
			versioncompat = AssemblyVersionCompatibility.SameMachine;
		}

		public AssemblyName(string assemblyName)
		{
			if (assemblyName == null)
			{
				throw new ArgumentNullException("assemblyName");
			}
			if (assemblyName.Length < 1)
			{
				throw new ArgumentException("assemblyName cannot have zero length.");
			}
			if (!ParseName(this, assemblyName))
			{
				throw new FileLoadException("The assembly name is invalid.");
			}
		}

		internal AssemblyName(SerializationInfo si, StreamingContext sc)
		{
			name = si.GetString("_Name");
			codebase = si.GetString("_CodeBase");
			version = (Version)si.GetValue("_Version", typeof(Version));
			publicKey = (byte[])si.GetValue("_PublicKey", typeof(byte[]));
			keyToken = (byte[])si.GetValue("_PublicKeyToken", typeof(byte[]));
			hashalg = (AssemblyHashAlgorithm)(int)si.GetValue("_HashAlgorithm", typeof(AssemblyHashAlgorithm));
			keypair = (StrongNameKeyPair)si.GetValue("_StrongNameKeyPair", typeof(StrongNameKeyPair));
			versioncompat = (AssemblyVersionCompatibility)(int)si.GetValue("_VersionCompatibility", typeof(AssemblyVersionCompatibility));
			flags = (AssemblyNameFlags)(int)si.GetValue("_Flags", typeof(AssemblyNameFlags));
			int @int = si.GetInt32("_CultureInfo");
			if (@int != -1)
			{
				cultureinfo = new CultureInfo(@int);
			}
		}

		void _AssemblyName.GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId)
		{
			throw new NotImplementedException();
		}

		void _AssemblyName.GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo)
		{
			throw new NotImplementedException();
		}

		void _AssemblyName.GetTypeInfoCount(out uint pcTInfo)
		{
			throw new NotImplementedException();
		}

		void _AssemblyName.Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr)
		{
			throw new NotImplementedException();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool ParseName(AssemblyName aname, string assemblyName);

		public override string ToString()
		{
			string fullName = FullName;
			return (fullName == null) ? base.ToString() : fullName;
		}

		public byte[] GetPublicKey()
		{
			return publicKey;
		}

		public byte[] GetPublicKeyToken()
		{
			if (keyToken != null)
			{
				return keyToken;
			}
			if (publicKey == null)
			{
				return null;
			}
			if (publicKey.Length == 0)
			{
				return new byte[0];
			}
			if (!IsPublicKeyValid)
			{
				throw new SecurityException("The public key is not valid.");
			}
			keyToken = ComputePublicKeyToken();
			return keyToken;
		}

		private byte[] InternalGetPublicKeyToken()
		{
			if (keyToken != null)
			{
				return keyToken;
			}
			if (publicKey == null)
			{
				return null;
			}
			if (publicKey.Length == 0)
			{
				return new byte[0];
			}
			if (!IsPublicKeyValid)
			{
				throw new SecurityException("The public key is not valid.");
			}
			return ComputePublicKeyToken();
		}

		private byte[] ComputePublicKeyToken()
		{
			HashAlgorithm hashAlgorithm = SHA1.Create();
			byte[] array = hashAlgorithm.ComputeHash(publicKey);
			byte[] array2 = new byte[8];
			Array.Copy(array, array.Length - 8, array2, 0, 8);
			Array.Reverse(array2, 0, 8);
			return array2;
		}

		[MonoTODO]
		public static bool ReferenceMatchesDefinition(AssemblyName reference, AssemblyName definition)
		{
			if (reference == null)
			{
				throw new ArgumentNullException("reference");
			}
			if (definition == null)
			{
				throw new ArgumentNullException("definition");
			}
			if (reference.Name != definition.Name)
			{
				return false;
			}
			throw new NotImplementedException();
		}

		public void SetPublicKey(byte[] publicKey)
		{
			if (publicKey == null)
			{
				flags ^= AssemblyNameFlags.PublicKey;
			}
			else
			{
				flags |= AssemblyNameFlags.PublicKey;
			}
			this.publicKey = publicKey;
		}

		public void SetPublicKeyToken(byte[] publicKeyToken)
		{
			keyToken = publicKeyToken;
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			info.AddValue("_Name", name);
			info.AddValue("_PublicKey", publicKey);
			info.AddValue("_PublicKeyToken", keyToken);
			info.AddValue("_CultureInfo", (cultureinfo == null) ? (-1) : cultureinfo.LCID);
			info.AddValue("_CodeBase", codebase);
			info.AddValue("_Version", Version);
			info.AddValue("_HashAlgorithm", hashalg);
			info.AddValue("_HashAlgorithmForControl", AssemblyHashAlgorithm.None);
			info.AddValue("_StrongNameKeyPair", keypair);
			info.AddValue("_VersionCompatibility", versioncompat);
			info.AddValue("_Flags", flags);
			info.AddValue("_HashForControl", null);
		}

		public object Clone()
		{
			AssemblyName assemblyName = new AssemblyName();
			assemblyName.name = name;
			assemblyName.codebase = codebase;
			assemblyName.major = major;
			assemblyName.minor = minor;
			assemblyName.build = build;
			assemblyName.revision = revision;
			assemblyName.version = version;
			assemblyName.cultureinfo = cultureinfo;
			assemblyName.flags = flags;
			assemblyName.hashalg = hashalg;
			assemblyName.keypair = keypair;
			assemblyName.publicKey = publicKey;
			assemblyName.keyToken = keyToken;
			assemblyName.versioncompat = versioncompat;
			return assemblyName;
		}

		public void OnDeserialization(object sender)
		{
			Version = version;
		}

		public static AssemblyName GetAssemblyName(string assemblyFile)
		{
			if (assemblyFile == null)
			{
				throw new ArgumentNullException("assemblyFile");
			}
			AssemblyName assemblyName = new AssemblyName();
			Assembly.InternalGetAssemblyName(Path.GetFullPath(assemblyFile), assemblyName);
			return assemblyName;
		}
	}
}

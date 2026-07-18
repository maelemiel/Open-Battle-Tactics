using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;

namespace System.Security.Policy
{
	[Serializable]
	[ComVisible(true)]
	public sealed class Hash : ISerializable, IBuiltInEvidence
	{
		private Assembly assembly;

		private byte[] data;

		internal byte[] _md5;

		internal byte[] _sha1;

		public byte[] MD5
		{
			get
			{
				if (_md5 != null)
				{
					return _md5;
				}
				if (assembly == null && _sha1 != null)
				{
					string text = Locale.GetText("No assembly data. This instance was initialized with an MSHA1 digest value.");
					throw new SecurityException(text);
				}
				HashAlgorithm hashAlg = System.Security.Cryptography.MD5.Create();
				_md5 = GenerateHash(hashAlg);
				return _md5;
			}
		}

		public byte[] SHA1
		{
			get
			{
				if (_sha1 != null)
				{
					return _sha1;
				}
				if (assembly == null && _md5 != null)
				{
					string text = Locale.GetText("No assembly data. This instance was initialized with an MD5 digest value.");
					throw new SecurityException(text);
				}
				HashAlgorithm hashAlg = System.Security.Cryptography.SHA1.Create();
				_sha1 = GenerateHash(hashAlg);
				return _sha1;
			}
		}

		public Hash(Assembly assembly)
		{
			if (assembly == null)
			{
				throw new ArgumentNullException("assembly");
			}
			this.assembly = assembly;
		}

		internal Hash()
		{
		}

		internal Hash(SerializationInfo info, StreamingContext context)
		{
			data = (byte[])info.GetValue("RawData", typeof(byte[]));
		}

		int IBuiltInEvidence.GetRequiredSize(bool verbose)
		{
			return verbose ? 5 : 0;
		}

		[MonoTODO("IBuiltInEvidence")]
		int IBuiltInEvidence.InitFromBuffer(char[] buffer, int position)
		{
			return 0;
		}

		[MonoTODO("IBuiltInEvidence")]
		int IBuiltInEvidence.OutputToBuffer(char[] buffer, int position, bool verbose)
		{
			return 0;
		}

		public byte[] GenerateHash(HashAlgorithm hashAlg)
		{
			if (hashAlg == null)
			{
				throw new ArgumentNullException("hashAlg");
			}
			return hashAlg.ComputeHash(GetData());
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			info.AddValue("RawData", GetData());
		}

		public override string ToString()
		{
			SecurityElement securityElement = new SecurityElement(GetType().FullName);
			securityElement.AddAttribute("version", "1");
			StringBuilder stringBuilder = new StringBuilder();
			byte[] array = GetData();
			for (int i = 0; i < array.Length; i++)
			{
				stringBuilder.Append(array[i].ToString("X2"));
			}
			securityElement.AddChild(new SecurityElement("RawData", stringBuilder.ToString()));
			return securityElement.ToString();
		}

		private byte[] GetData()
		{
			if (assembly == null && data == null)
			{
				string text = Locale.GetText("No assembly data.");
				throw new SecurityException(text);
			}
			if (data == null)
			{
				FileStream fileStream = new FileStream(assembly.Location, FileMode.Open, FileAccess.Read);
				data = new byte[fileStream.Length];
				fileStream.Read(data, 0, (int)fileStream.Length);
			}
			return data;
		}

		public static Hash CreateMD5(byte[] md5)
		{
			if (md5 == null)
			{
				throw new ArgumentNullException("md5");
			}
			Hash hash = new Hash();
			hash._md5 = md5;
			return hash;
		}

		public static Hash CreateSHA1(byte[] sha1)
		{
			if (sha1 == null)
			{
				throw new ArgumentNullException("sha1");
			}
			Hash hash = new Hash();
			hash._sha1 = sha1;
			return hash;
		}
	}
}

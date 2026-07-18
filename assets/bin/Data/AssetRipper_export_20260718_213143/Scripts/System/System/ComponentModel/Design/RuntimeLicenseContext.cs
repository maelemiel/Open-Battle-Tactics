using System.Collections;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization.Formatters.Binary;

namespace System.ComponentModel.Design
{
	internal class RuntimeLicenseContext : LicenseContext
	{
		private Hashtable extraassemblies;

		private Hashtable keys;

		private void LoadKeys()
		{
			if (keys != null)
			{
				return;
			}
			keys = new Hashtable();
			Assembly entryAssembly = Assembly.GetEntryAssembly();
			if (entryAssembly != null)
			{
				LoadAssemblyLicenses(keys, entryAssembly);
				return;
			}
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (Assembly asm in assemblies)
			{
				LoadAssemblyLicenses(keys, asm);
			}
		}

		private void LoadAssemblyLicenses(Hashtable targetkeys, Assembly asm)
		{
			if (asm is AssemblyBuilder)
			{
				return;
			}
			string fileName = Path.GetFileName(asm.Location);
			string text = fileName + ".licenses";
			try
			{
				string[] manifestResourceNames = asm.GetManifestResourceNames();
				foreach (string text2 in manifestResourceNames)
				{
					if (text2 != text)
					{
						continue;
					}
					using (Stream serializationStream = asm.GetManifestResourceStream(text2))
					{
						BinaryFormatter binaryFormatter = new BinaryFormatter();
						object[] array = binaryFormatter.Deserialize(serializationStream) as object[];
						if (string.Compare((string)array[0], fileName, true) != 0)
						{
							continue;
						}
						Hashtable hashtable = (Hashtable)array[1];
						foreach (DictionaryEntry item in hashtable)
						{
							targetkeys.Add(item.Key, item.Value);
						}
					}
				}
			}
			catch (InvalidCastException)
			{
			}
		}

		public override string GetSavedLicenseKey(Type type, Assembly resourceAssembly)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (resourceAssembly != null)
			{
				if (extraassemblies == null)
				{
					extraassemblies = new Hashtable();
				}
				Hashtable hashtable = extraassemblies[resourceAssembly.FullName] as Hashtable;
				if (hashtable == null)
				{
					hashtable = new Hashtable();
					LoadAssemblyLicenses(hashtable, resourceAssembly);
					extraassemblies[resourceAssembly.FullName] = hashtable;
				}
				return (string)hashtable[type.AssemblyQualifiedName];
			}
			LoadKeys();
			return (string)keys[type.AssemblyQualifiedName];
		}

		public override void SetSavedLicenseKey(Type type, string key)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			LoadKeys();
			keys[type.AssemblyQualifiedName] = key;
		}
	}
}

using System.Collections;
using System.IO;
using System.Text;

namespace System.Resources
{
	internal class Win32VersionResource : Win32Resource
	{
		public string[] WellKnownProperties = new string[8] { "Comments", "CompanyName", "FileVersion", "InternalName", "LegalTrademarks", "OriginalFilename", "ProductName", "ProductVersion" };

		private long signature;

		private int struct_version;

		private long file_version;

		private long product_version;

		private int file_flags_mask;

		private int file_flags;

		private int file_os;

		private int file_type;

		private int file_subtype;

		private long file_date;

		private int file_lang;

		private int file_codepage;

		private Hashtable properties;

		public string Version
		{
			get
			{
				return string.Empty + (file_version >> 48) + "." + ((file_version >> 32) & 0xFFFF) + "." + ((file_version >> 16) & 0xFFFF) + "." + (file_version & 0xFFFF);
			}
			set
			{
				long[] array = new long[4];
				if (value != null)
				{
					string[] array2 = value.Split('.');
					try
					{
						for (int i = 0; i < array2.Length; i++)
						{
							if (i < array.Length)
							{
								array[i] = int.Parse(array2[i]);
							}
						}
					}
					catch (FormatException)
					{
					}
				}
				file_version = (array[0] << 48) | (array[1] << 32) | ((array[2] << 16) + array[3]);
				properties["FileVersion"] = Version;
			}
		}

		public virtual string this[string key]
		{
			set
			{
				properties[key] = value;
			}
		}

		public virtual string Comments
		{
			get
			{
				return (string)properties["Comments"];
			}
			set
			{
				properties["Comments"] = ((!(value == string.Empty)) ? value : " ");
			}
		}

		public virtual string CompanyName
		{
			get
			{
				return (string)properties["CompanyName"];
			}
			set
			{
				properties["CompanyName"] = ((!(value == string.Empty)) ? value : " ");
			}
		}

		public virtual string LegalCopyright
		{
			get
			{
				return (string)properties["LegalCopyright"];
			}
			set
			{
				properties["LegalCopyright"] = ((!(value == string.Empty)) ? value : " ");
			}
		}

		public virtual string LegalTrademarks
		{
			get
			{
				return (string)properties["LegalTrademarks"];
			}
			set
			{
				properties["LegalTrademarks"] = ((!(value == string.Empty)) ? value : " ");
			}
		}

		public virtual string OriginalFilename
		{
			get
			{
				return (string)properties["OriginalFilename"];
			}
			set
			{
				properties["OriginalFilename"] = ((!(value == string.Empty)) ? value : " ");
			}
		}

		public virtual string ProductName
		{
			get
			{
				return (string)properties["ProductName"];
			}
			set
			{
				properties["ProductName"] = ((!(value == string.Empty)) ? value : " ");
			}
		}

		public virtual string ProductVersion
		{
			get
			{
				return (string)properties["ProductVersion"];
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					value = " ";
				}
				long[] array = new long[4];
				string[] array2 = value.Split('.');
				try
				{
					for (int i = 0; i < array2.Length; i++)
					{
						if (i < array.Length)
						{
							array[i] = int.Parse(array2[i]);
						}
					}
				}
				catch (FormatException)
				{
				}
				properties["ProductVersion"] = value;
				product_version = (array[0] << 48) | (array[1] << 32) | ((array[2] << 16) + array[3]);
			}
		}

		public virtual string InternalName
		{
			get
			{
				return (string)properties["InternalName"];
			}
			set
			{
				properties["InternalName"] = ((!(value == string.Empty)) ? value : " ");
			}
		}

		public virtual string FileDescription
		{
			get
			{
				return (string)properties["FileDescription"];
			}
			set
			{
				properties["FileDescription"] = ((!(value == string.Empty)) ? value : " ");
			}
		}

		public virtual int FileLanguage
		{
			get
			{
				return file_lang;
			}
			set
			{
				file_lang = value;
			}
		}

		public virtual string FileVersion
		{
			get
			{
				return (string)properties["FileVersion"];
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					value = " ";
				}
				long[] array = new long[4];
				string[] array2 = value.Split('.');
				try
				{
					for (int i = 0; i < array2.Length; i++)
					{
						if (i < array.Length)
						{
							array[i] = int.Parse(array2[i]);
						}
					}
				}
				catch (FormatException)
				{
				}
				properties["FileVersion"] = value;
				file_version = (array[0] << 48) | (array[1] << 32) | ((array[2] << 16) + array[3]);
			}
		}

		public Win32VersionResource(int id, int language, bool compilercontext)
			: base(Win32ResourceType.RT_VERSION, id, language)
		{
			signature = 4277077181L;
			struct_version = 65536;
			file_flags_mask = 63;
			file_flags = 0;
			file_os = 4;
			file_type = 2;
			file_subtype = 0;
			file_date = 0L;
			file_lang = ((!compilercontext) ? 127 : 0);
			file_codepage = 1200;
			properties = new Hashtable();
			string value = ((!compilercontext) ? " " : string.Empty);
			string[] wellKnownProperties = WellKnownProperties;
			foreach (string key in wellKnownProperties)
			{
				properties[key] = value;
			}
			LegalCopyright = " ";
			FileDescription = " ";
		}

		private void emit_padding(BinaryWriter w)
		{
			Stream baseStream = w.BaseStream;
			if (baseStream.Position % 4 != 0L)
			{
				w.Write((short)0);
			}
		}

		private void patch_length(BinaryWriter w, long len_pos)
		{
			Stream baseStream = w.BaseStream;
			long position = baseStream.Position;
			baseStream.Position = len_pos;
			w.Write((short)(position - len_pos));
			baseStream.Position = position;
		}

		public override void WriteTo(Stream ms)
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(ms, Encoding.Unicode))
			{
				binaryWriter.Write((short)0);
				binaryWriter.Write((short)52);
				binaryWriter.Write((short)0);
				binaryWriter.Write("VS_VERSION_INFO".ToCharArray());
				binaryWriter.Write((short)0);
				emit_padding(binaryWriter);
				binaryWriter.Write((uint)signature);
				binaryWriter.Write(struct_version);
				binaryWriter.Write((int)(file_version >> 32));
				binaryWriter.Write((int)(file_version & 0xFFFFFFFFu));
				binaryWriter.Write((int)(product_version >> 32));
				binaryWriter.Write((int)(product_version & 0xFFFFFFFFu));
				binaryWriter.Write(file_flags_mask);
				binaryWriter.Write(file_flags);
				binaryWriter.Write(file_os);
				binaryWriter.Write(file_type);
				binaryWriter.Write(file_subtype);
				binaryWriter.Write((int)(file_date >> 32));
				binaryWriter.Write((int)(file_date & 0xFFFFFFFFu));
				emit_padding(binaryWriter);
				long position = ms.Position;
				binaryWriter.Write((short)0);
				binaryWriter.Write((short)0);
				binaryWriter.Write((short)1);
				binaryWriter.Write("VarFileInfo".ToCharArray());
				binaryWriter.Write((short)0);
				if (ms.Position % 4 != 0L)
				{
					binaryWriter.Write((short)0);
				}
				long position2 = ms.Position;
				binaryWriter.Write((short)0);
				binaryWriter.Write((short)4);
				binaryWriter.Write((short)0);
				binaryWriter.Write("Translation".ToCharArray());
				binaryWriter.Write((short)0);
				if (ms.Position % 4 != 0L)
				{
					binaryWriter.Write((short)0);
				}
				binaryWriter.Write((short)file_lang);
				binaryWriter.Write((short)file_codepage);
				patch_length(binaryWriter, position2);
				patch_length(binaryWriter, position);
				long position3 = ms.Position;
				binaryWriter.Write((short)0);
				binaryWriter.Write((short)0);
				binaryWriter.Write((short)1);
				binaryWriter.Write("StringFileInfo".ToCharArray());
				emit_padding(binaryWriter);
				long position4 = ms.Position;
				binaryWriter.Write((short)0);
				binaryWriter.Write((short)0);
				binaryWriter.Write((short)1);
				binaryWriter.Write(string.Format("{0:x4}{1:x4}", file_lang, file_codepage).ToCharArray());
				emit_padding(binaryWriter);
				foreach (string key in properties.Keys)
				{
					string text2 = (string)properties[key];
					long position5 = ms.Position;
					binaryWriter.Write((short)0);
					binaryWriter.Write((short)(text2.ToCharArray().Length + 1));
					binaryWriter.Write((short)1);
					binaryWriter.Write(key.ToCharArray());
					binaryWriter.Write((short)0);
					emit_padding(binaryWriter);
					binaryWriter.Write(text2.ToCharArray());
					binaryWriter.Write((short)0);
					emit_padding(binaryWriter);
					patch_length(binaryWriter, position5);
				}
				patch_length(binaryWriter, position4);
				patch_length(binaryWriter, position3);
				patch_length(binaryWriter, 0L);
			}
		}
	}
}

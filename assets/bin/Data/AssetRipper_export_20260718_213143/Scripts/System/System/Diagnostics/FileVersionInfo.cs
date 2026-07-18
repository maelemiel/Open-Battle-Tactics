using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace System.Diagnostics
{
	public sealed class FileVersionInfo
	{
		private string comments;

		private string companyname;

		private string filedescription;

		private string filename;

		private string fileversion;

		private string internalname;

		private string language;

		private string legalcopyright;

		private string legaltrademarks;

		private string originalfilename;

		private string privatebuild;

		private string productname;

		private string productversion;

		private string specialbuild;

		private bool isdebug;

		private bool ispatched;

		private bool isprerelease;

		private bool isprivatebuild;

		private bool isspecialbuild;

		private int filemajorpart;

		private int fileminorpart;

		private int filebuildpart;

		private int fileprivatepart;

		private int productmajorpart;

		private int productminorpart;

		private int productbuildpart;

		private int productprivatepart;

		public string Comments
		{
			get
			{
				return comments;
			}
		}

		public string CompanyName
		{
			get
			{
				return companyname;
			}
		}

		public int FileBuildPart
		{
			get
			{
				return filebuildpart;
			}
		}

		public string FileDescription
		{
			get
			{
				return filedescription;
			}
		}

		public int FileMajorPart
		{
			get
			{
				return filemajorpart;
			}
		}

		public int FileMinorPart
		{
			get
			{
				return fileminorpart;
			}
		}

		public string FileName
		{
			get
			{
				return filename;
			}
		}

		public int FilePrivatePart
		{
			get
			{
				return fileprivatepart;
			}
		}

		public string FileVersion
		{
			get
			{
				return fileversion;
			}
		}

		public string InternalName
		{
			get
			{
				return internalname;
			}
		}

		public bool IsDebug
		{
			get
			{
				return isdebug;
			}
		}

		public bool IsPatched
		{
			get
			{
				return ispatched;
			}
		}

		public bool IsPreRelease
		{
			get
			{
				return isprerelease;
			}
		}

		public bool IsPrivateBuild
		{
			get
			{
				return isprivatebuild;
			}
		}

		public bool IsSpecialBuild
		{
			get
			{
				return isspecialbuild;
			}
		}

		public string Language
		{
			get
			{
				return language;
			}
		}

		public string LegalCopyright
		{
			get
			{
				return legalcopyright;
			}
		}

		public string LegalTrademarks
		{
			get
			{
				return legaltrademarks;
			}
		}

		public string OriginalFilename
		{
			get
			{
				return originalfilename;
			}
		}

		public string PrivateBuild
		{
			get
			{
				return privatebuild;
			}
		}

		public int ProductBuildPart
		{
			get
			{
				return productbuildpart;
			}
		}

		public int ProductMajorPart
		{
			get
			{
				return productmajorpart;
			}
		}

		public int ProductMinorPart
		{
			get
			{
				return productminorpart;
			}
		}

		public string ProductName
		{
			get
			{
				return productname;
			}
		}

		public int ProductPrivatePart
		{
			get
			{
				return productprivatepart;
			}
		}

		public string ProductVersion
		{
			get
			{
				return productversion;
			}
		}

		public string SpecialBuild
		{
			get
			{
				return specialbuild;
			}
		}

		private FileVersionInfo()
		{
			comments = null;
			companyname = null;
			filedescription = null;
			filename = null;
			fileversion = null;
			internalname = null;
			language = null;
			legalcopyright = null;
			legaltrademarks = null;
			originalfilename = null;
			privatebuild = null;
			productname = null;
			productversion = null;
			specialbuild = null;
			isdebug = false;
			ispatched = false;
			isprerelease = false;
			isprivatebuild = false;
			isspecialbuild = false;
			filemajorpart = 0;
			fileminorpart = 0;
			filebuildpart = 0;
			fileprivatepart = 0;
			productmajorpart = 0;
			productminorpart = 0;
			productbuildpart = 0;
			productprivatepart = 0;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetVersionInfo_internal(string fileName);

		public static FileVersionInfo GetVersionInfo(string fileName)
		{
			string fullPath = Path.GetFullPath(fileName);
			if (!File.Exists(fullPath))
			{
				throw new FileNotFoundException(fileName);
			}
			FileVersionInfo fileVersionInfo = new FileVersionInfo();
			fileVersionInfo.GetVersionInfo_internal(fileName);
			return fileVersionInfo;
		}

		private static void AppendFormat(StringBuilder sb, string format, params object[] args)
		{
			sb.AppendFormat(format, args);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			AppendFormat(stringBuilder, "File:             {0}{1}", FileName, Environment.NewLine);
			AppendFormat(stringBuilder, "InternalName:     {0}{1}", internalname, Environment.NewLine);
			AppendFormat(stringBuilder, "OriginalFilename: {0}{1}", originalfilename, Environment.NewLine);
			AppendFormat(stringBuilder, "FileVersion:      {0}{1}", fileversion, Environment.NewLine);
			AppendFormat(stringBuilder, "FileDescription:  {0}{1}", filedescription, Environment.NewLine);
			AppendFormat(stringBuilder, "Product:          {0}{1}", productname, Environment.NewLine);
			AppendFormat(stringBuilder, "ProductVersion:   {0}{1}", productversion, Environment.NewLine);
			AppendFormat(stringBuilder, "Debug:            {0}{1}", isdebug, Environment.NewLine);
			AppendFormat(stringBuilder, "Patched:          {0}{1}", ispatched, Environment.NewLine);
			AppendFormat(stringBuilder, "PreRelease:       {0}{1}", isprerelease, Environment.NewLine);
			AppendFormat(stringBuilder, "PrivateBuild:     {0}{1}", isprivatebuild, Environment.NewLine);
			AppendFormat(stringBuilder, "SpecialBuild:     {0}{1}", isspecialbuild, Environment.NewLine);
			AppendFormat(stringBuilder, "Language          {0}{1}", language, Environment.NewLine);
			return stringBuilder.ToString();
		}
	}
}

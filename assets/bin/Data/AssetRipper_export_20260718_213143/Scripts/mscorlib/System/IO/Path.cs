using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace System.IO
{
	[ComVisible(true)]
	public static class Path
	{
		[Obsolete("see GetInvalidPathChars and GetInvalidFileNameChars methods.")]
		public static readonly char[] InvalidPathChars;

		public static readonly char AltDirectorySeparatorChar;

		public static readonly char DirectorySeparatorChar;

		public static readonly char PathSeparator;

		internal static readonly string DirectorySeparatorStr;

		public static readonly char VolumeSeparatorChar;

		internal static readonly char[] PathSeparatorChars;

		private static readonly bool dirEqualsVolume;

		static Path()
		{
			VolumeSeparatorChar = MonoIO.VolumeSeparatorChar;
			DirectorySeparatorChar = MonoIO.DirectorySeparatorChar;
			AltDirectorySeparatorChar = MonoIO.AltDirectorySeparatorChar;
			PathSeparator = MonoIO.PathSeparator;
			InvalidPathChars = GetInvalidPathChars();
			DirectorySeparatorStr = DirectorySeparatorChar.ToString();
			PathSeparatorChars = new char[3] { DirectorySeparatorChar, AltDirectorySeparatorChar, VolumeSeparatorChar };
			dirEqualsVolume = DirectorySeparatorChar == VolumeSeparatorChar;
		}

		public static string ChangeExtension(string path, string extension)
		{
			if (path == null)
			{
				return null;
			}
			if (path.IndexOfAny(InvalidPathChars) != -1)
			{
				throw new ArgumentException("Illegal characters in path.");
			}
			int num = findExtension(path);
			if (extension == null)
			{
				return (num >= 0) ? path.Substring(0, num) : path;
			}
			if (extension.Length == 0)
			{
				return (num >= 0) ? path.Substring(0, num + 1) : (path + '.');
			}
			if (path.Length != 0)
			{
				if (extension.Length > 0 && extension[0] != '.')
				{
					extension = "." + extension;
				}
			}
			else
			{
				extension = string.Empty;
			}
			if (num < 0)
			{
				return path + extension;
			}
			if (num > 0)
			{
				string text = path.Substring(0, num);
				return text + extension;
			}
			return extension;
		}

		public static string Combine(string path1, string path2)
		{
			if (path1 == null)
			{
				throw new ArgumentNullException("path1");
			}
			if (path2 == null)
			{
				throw new ArgumentNullException("path2");
			}
			if (path1.Length == 0)
			{
				return path2;
			}
			if (path2.Length == 0)
			{
				return path1;
			}
			if (path1.IndexOfAny(InvalidPathChars) != -1)
			{
				throw new ArgumentException("Illegal characters in path.");
			}
			if (path2.IndexOfAny(InvalidPathChars) != -1)
			{
				throw new ArgumentException("Illegal characters in path.");
			}
			if (IsPathRooted(path2))
			{
				return path2;
			}
			char c = path1[path1.Length - 1];
			if (c != DirectorySeparatorChar && c != AltDirectorySeparatorChar && c != VolumeSeparatorChar)
			{
				return path1 + DirectorySeparatorStr + path2;
			}
			return path1 + path2;
		}

		internal static string CleanPath(string s)
		{
			int length = s.Length;
			int num = 0;
			int num2 = 0;
			char c = s[0];
			if (length > 2 && c == '\\' && s[1] == '\\')
			{
				num2 = 2;
			}
			if (length == 1 && (c == DirectorySeparatorChar || c == AltDirectorySeparatorChar))
			{
				return s;
			}
			for (int i = num2; i < length; i++)
			{
				char c2 = s[i];
				if (c2 != DirectorySeparatorChar && c2 != AltDirectorySeparatorChar)
				{
					continue;
				}
				if (i + 1 == length)
				{
					num++;
					continue;
				}
				c2 = s[i + 1];
				if (c2 == DirectorySeparatorChar || c2 == AltDirectorySeparatorChar)
				{
					num++;
				}
			}
			if (num == 0)
			{
				return s;
			}
			char[] array = new char[length - num];
			if (num2 != 0)
			{
				array[0] = '\\';
				array[1] = '\\';
			}
			int j = num2;
			int num3 = num2;
			for (; j < length; j++)
			{
				if (num3 >= array.Length)
				{
					break;
				}
				char c3 = s[j];
				if (c3 != DirectorySeparatorChar && c3 != AltDirectorySeparatorChar)
				{
					array[num3++] = c3;
				}
				else
				{
					if (num3 + 1 == array.Length)
					{
						continue;
					}
					array[num3++] = DirectorySeparatorChar;
					for (; j < length - 1; j++)
					{
						c3 = s[j + 1];
						if (c3 != DirectorySeparatorChar && c3 != AltDirectorySeparatorChar)
						{
							break;
						}
					}
				}
			}
			return new string(array);
		}

		public static string GetDirectoryName(string path)
		{
			if (path == string.Empty)
			{
				throw new ArgumentException("Invalid path");
			}
			if (path == null || GetPathRoot(path) == path)
			{
				return null;
			}
			if (path.Trim().Length == 0)
			{
				throw new ArgumentException("Argument string consists of whitespace characters only.");
			}
			if (path.IndexOfAny(InvalidPathChars) > -1)
			{
				throw new ArgumentException("Path contains invalid characters");
			}
			int num = path.LastIndexOfAny(PathSeparatorChars);
			if (num == 0)
			{
				num++;
			}
			if (num > 0)
			{
				string text = path.Substring(0, num);
				int length = text.Length;
				if (length >= 2 && DirectorySeparatorChar == '\\' && text[length - 1] == VolumeSeparatorChar)
				{
					return text + DirectorySeparatorChar;
				}
				return CleanPath(text);
			}
			return string.Empty;
		}

		public static string GetExtension(string path)
		{
			if (path == null)
			{
				return null;
			}
			if (path.IndexOfAny(InvalidPathChars) != -1)
			{
				throw new ArgumentException("Illegal characters in path.");
			}
			int num = findExtension(path);
			if (num > -1 && num < path.Length - 1)
			{
				return path.Substring(num);
			}
			return string.Empty;
		}

		public static string GetFileName(string path)
		{
			if (path == null || path.Length == 0)
			{
				return path;
			}
			if (path.IndexOfAny(InvalidPathChars) != -1)
			{
				throw new ArgumentException("Illegal characters in path.");
			}
			int num = path.LastIndexOfAny(PathSeparatorChars);
			if (num >= 0)
			{
				return path.Substring(num + 1);
			}
			return path;
		}

		public static string GetFileNameWithoutExtension(string path)
		{
			return ChangeExtension(GetFileName(path), null);
		}

		public static string GetFullPath(string path)
		{
			return InsecureGetFullPath(path);
		}

		internal static string WindowsDriveAdjustment(string path)
		{
			if (path.Length < 2)
			{
				return path;
			}
			if (path[1] != ':' || !char.IsLetter(path[0]))
			{
				return path;
			}
			string currentDirectory = Directory.GetCurrentDirectory();
			if (path.Length == 2)
			{
				path = ((currentDirectory[0] != path[0]) ? (path + '\\') : currentDirectory);
			}
			else if (path[2] != DirectorySeparatorChar && path[2] != AltDirectorySeparatorChar)
			{
				path = ((currentDirectory[0] != path[0]) ? (path.Substring(0, 2) + DirectorySeparatorStr + path.Substring(2, path.Length - 2)) : Combine(currentDirectory, path.Substring(2, path.Length - 2)));
			}
			return path;
		}

		internal static string InsecureGetFullPath(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (path.Trim().Length == 0)
			{
				string text = Locale.GetText("The specified path is not of a legal form (empty).");
				throw new ArgumentException(text);
			}
			if (Environment.IsRunningOnWindows)
			{
				path = WindowsDriveAdjustment(path);
			}
			char c = path[path.Length - 1];
			if (path.Length >= 2 && IsDsc(path[0]) && IsDsc(path[1]))
			{
				if (path.Length == 2 || path.IndexOf(path[0], 2) < 0)
				{
					throw new ArgumentException("UNC paths should be of the form \\\\server\\share.");
				}
				if (path[0] != DirectorySeparatorChar)
				{
					path = path.Replace(AltDirectorySeparatorChar, DirectorySeparatorChar);
				}
				path = CanonicalizePath(path);
			}
			else
			{
				if (!IsPathRooted(path))
				{
					path = Directory.GetCurrentDirectory() + DirectorySeparatorStr + path;
				}
				else if (DirectorySeparatorChar == '\\' && path.Length >= 2 && IsDsc(path[0]) && !IsDsc(path[1]))
				{
					string currentDirectory = Directory.GetCurrentDirectory();
					path = ((currentDirectory[1] != VolumeSeparatorChar) ? currentDirectory.Substring(0, currentDirectory.IndexOf('\\', currentDirectory.IndexOf("\\\\") + 1)) : (currentDirectory.Substring(0, 2) + path));
				}
				path = CanonicalizePath(path);
			}
			if (IsDsc(c) && path[path.Length - 1] != DirectorySeparatorChar)
			{
				path += DirectorySeparatorChar;
			}
			return path;
		}

		private static bool IsDsc(char c)
		{
			return c == DirectorySeparatorChar || c == AltDirectorySeparatorChar;
		}

		public static string GetPathRoot(string path)
		{
			if (path == null)
			{
				return null;
			}
			if (path.Trim().Length == 0)
			{
				throw new ArgumentException("The specified path is not of a legal form.");
			}
			if (!IsPathRooted(path))
			{
				return string.Empty;
			}
			if (DirectorySeparatorChar == '/')
			{
				return (!IsDsc(path[0])) ? string.Empty : DirectorySeparatorStr;
			}
			int i = 2;
			if (path.Length == 1 && IsDsc(path[0]))
			{
				return DirectorySeparatorStr;
			}
			if (path.Length < 2)
			{
				return string.Empty;
			}
			if (IsDsc(path[0]) && IsDsc(path[1]))
			{
				for (; i < path.Length && !IsDsc(path[i]); i++)
				{
				}
				if (i < path.Length)
				{
					for (i++; i < path.Length && !IsDsc(path[i]); i++)
					{
					}
				}
				return DirectorySeparatorStr + DirectorySeparatorStr + path.Substring(2, i - 2).Replace(AltDirectorySeparatorChar, DirectorySeparatorChar);
			}
			if (IsDsc(path[0]))
			{
				return DirectorySeparatorStr;
			}
			if (path[1] == VolumeSeparatorChar)
			{
				if (path.Length >= 3 && IsDsc(path[2]))
				{
					i++;
				}
				return path.Substring(0, i);
			}
			return Directory.GetCurrentDirectory().Substring(0, 2);
		}

		public static string GetTempFileName()
		{
			FileStream fileStream = null;
			int num = 0;
			Random random = new Random();
			string text;
			do
			{
				num = random.Next();
				text = Combine(GetTempPath(), "tmp" + (num + 1).ToString("x") + ".tmp");
				try
				{
					fileStream = new FileStream(text, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read, 8192, false, (FileOptions)1);
				}
				catch (SecurityException)
				{
					throw;
				}
				catch (UnauthorizedAccessException)
				{
					throw;
				}
				catch (DirectoryNotFoundException)
				{
					throw;
				}
				catch
				{
				}
			}
			while (fileStream == null);
			fileStream.Close();
			return text;
		}

		public static string GetTempPath()
		{
			string temp_path = get_temp_path();
			if (temp_path.Length > 0 && temp_path[temp_path.Length - 1] != DirectorySeparatorChar)
			{
				return temp_path + DirectorySeparatorChar;
			}
			return temp_path;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string get_temp_path();

		public static bool HasExtension(string path)
		{
			if (path == null || path.Trim().Length == 0)
			{
				return false;
			}
			if (path.IndexOfAny(InvalidPathChars) != -1)
			{
				throw new ArgumentException("Illegal characters in path.");
			}
			int num = findExtension(path);
			return 0 <= num && num < path.Length - 1;
		}

		public static bool IsPathRooted(string path)
		{
			if (path == null || path.Length == 0)
			{
				return false;
			}
			if (path.IndexOfAny(InvalidPathChars) != -1)
			{
				throw new ArgumentException("Illegal characters in path.");
			}
			char c = path[0];
			return c == DirectorySeparatorChar || c == AltDirectorySeparatorChar || (!dirEqualsVolume && path.Length > 1 && path[1] == VolumeSeparatorChar);
		}

		public static char[] GetInvalidFileNameChars()
		{
			if (!Environment.IsRunningOnWindows)
			{
				return new char[2] { '\0', '/' };
			}
			return new char[41]
			{
				'\0', '\u0001', '\u0002', '\u0003', '\u0004', '\u0005', '\u0006', '\a', '\b', '\t',
				'\n', '\v', '\f', '\r', '\u000e', '\u000f', '\u0010', '\u0011', '\u0012', '\u0013',
				'\u0014', '\u0015', '\u0016', '\u0017', '\u0018', '\u0019', '\u001a', '\u001b', '\u001c', '\u001d',
				'\u001e', '\u001f', '"', '<', '>', '|', ':', '*', '?', '\\',
				'/'
			};
		}

		public static char[] GetInvalidPathChars()
		{
			if (Environment.IsRunningOnWindows)
			{
				return new char[36]
				{
					'"', '<', '>', '|', '\0', '\u0001', '\u0002', '\u0003', '\u0004', '\u0005',
					'\u0006', '\a', '\b', '\t', '\n', '\v', '\f', '\r', '\u000e', '\u000f',
					'\u0010', '\u0011', '\u0012', '\u0013', '\u0014', '\u0015', '\u0016', '\u0017', '\u0018', '\u0019',
					'\u001a', '\u001b', '\u001c', '\u001d', '\u001e', '\u001f'
				};
			}
			return new char[1];
		}

		public static string GetRandomFileName()
		{
			StringBuilder stringBuilder = new StringBuilder(12);
			RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
			byte[] array = new byte[11];
			randomNumberGenerator.GetBytes(array);
			for (int i = 0; i < array.Length; i++)
			{
				if (stringBuilder.Length == 8)
				{
					stringBuilder.Append('.');
				}
				int num = array[i] % 36;
				char value = (char)((num >= 26) ? (num - 26 + 48) : (num + 97));
				stringBuilder.Append(value);
			}
			return stringBuilder.ToString();
		}

		private static int findExtension(string path)
		{
			if (path != null)
			{
				int num = path.LastIndexOf('.');
				int num2 = path.LastIndexOfAny(PathSeparatorChars);
				if (num > num2)
				{
					return num;
				}
			}
			return -1;
		}

		private static string GetServerAndShare(string path)
		{
			int i;
			for (i = 2; i < path.Length && !IsDsc(path[i]); i++)
			{
			}
			if (i < path.Length)
			{
				for (i++; i < path.Length && !IsDsc(path[i]); i++)
				{
				}
			}
			return path.Substring(2, i - 2).Replace(AltDirectorySeparatorChar, DirectorySeparatorChar);
		}

		private static bool SameRoot(string root, string path)
		{
			if (root.Length < 2 || path.Length < 2)
			{
				return false;
			}
			if (IsDsc(root[0]) && IsDsc(root[1]))
			{
				if (!IsDsc(path[0]) || !IsDsc(path[1]))
				{
					return false;
				}
				string serverAndShare = GetServerAndShare(root);
				string serverAndShare2 = GetServerAndShare(path);
				return string.Compare(serverAndShare, serverAndShare2, true, CultureInfo.InvariantCulture) == 0;
			}
			if (!root[0].Equals(path[0]))
			{
				return false;
			}
			if (path[1] != VolumeSeparatorChar)
			{
				return false;
			}
			if (root.Length > 2 && path.Length > 2)
			{
				return IsDsc(root[2]) && IsDsc(path[2]);
			}
			return true;
		}

		private static string CanonicalizePath(string path)
		{
			if (path == null)
			{
				return path;
			}
			if (Environment.IsRunningOnWindows)
			{
				path = path.Trim();
			}
			if (path.Length == 0)
			{
				return path;
			}
			string pathRoot = GetPathRoot(path);
			string[] array = path.Split(DirectorySeparatorChar, AltDirectorySeparatorChar);
			int num = 0;
			bool flag = Environment.IsRunningOnWindows && pathRoot.Length > 2 && IsDsc(pathRoot[0]) && IsDsc(pathRoot[1]);
			int num2 = (flag ? 3 : 0);
			for (int i = 0; i < array.Length; i++)
			{
				if (Environment.IsRunningOnWindows)
				{
					array[i] = array[i].TrimEnd();
				}
				if (array[i] == "." || (i != 0 && array[i].Length == 0))
				{
					continue;
				}
				if (array[i] == "..")
				{
					if (num > num2)
					{
						num--;
					}
				}
				else
				{
					array[num++] = array[i];
				}
			}
			if (num == 0 || (num == 1 && array[0] == string.Empty))
			{
				return pathRoot;
			}
			string text = string.Join(DirectorySeparatorStr, array, 0, num);
			if (Environment.IsRunningOnWindows)
			{
				if (flag)
				{
					text = DirectorySeparatorStr + text;
				}
				if (!SameRoot(pathRoot, text))
				{
					text = pathRoot + text;
				}
				if (flag)
				{
					return text;
				}
				if (!IsDsc(path[0]) && SameRoot(pathRoot, path))
				{
					if (text.Length <= 2 && !text.EndsWith(DirectorySeparatorStr))
					{
						text += DirectorySeparatorChar;
					}
					return text;
				}
				string currentDirectory = Directory.GetCurrentDirectory();
				if (currentDirectory.Length > 1 && currentDirectory[1] == VolumeSeparatorChar)
				{
					if (text.Length == 0 || IsDsc(text[0]))
					{
						text += '\\';
					}
					return currentDirectory.Substring(0, 2) + text;
				}
				if (IsDsc(currentDirectory[currentDirectory.Length - 1]) && IsDsc(text[0]))
				{
					return currentDirectory + text.Substring(1);
				}
				return currentDirectory + text;
			}
			return text;
		}

		internal static bool IsPathSubsetOf(string subset, string path)
		{
			if (subset.Length > path.Length)
			{
				return false;
			}
			int num = subset.LastIndexOfAny(PathSeparatorChars);
			if (string.Compare(subset, 0, path, 0, num) != 0)
			{
				return false;
			}
			num++;
			int num2 = path.IndexOfAny(PathSeparatorChars, num);
			if (num2 >= num)
			{
				return string.Compare(subset, num, path, num, path.Length - num2) == 0;
			}
			if (subset.Length != path.Length)
			{
				return false;
			}
			return string.Compare(subset, num, path, num, subset.Length - num) == 0;
		}
	}
}

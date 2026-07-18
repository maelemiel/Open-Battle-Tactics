using System.Collections;
using System.IO;

namespace NUnit.Framework.Constraints
{
	public abstract class PathConstraint : Constraint
	{
		private static readonly char[] DirectorySeparatorChars = new char[2] { '\\', '/' };

		protected string expectedPath;

		protected string actualPath;

		protected bool caseInsensitive = Path.DirectorySeparatorChar == '\\';

		public PathConstraint IgnoreCase
		{
			get
			{
				caseInsensitive = true;
				return this;
			}
		}

		public PathConstraint RespectCase
		{
			get
			{
				caseInsensitive = false;
				return this;
			}
		}

		protected PathConstraint(string expected)
			: base(expected)
		{
			expectedPath = expected;
		}

		public override bool Matches(object actual)
		{
			base.actual = actual;
			actualPath = actual as string;
			if (actualPath == null)
			{
				return false;
			}
			return IsMatch(expectedPath, actualPath);
		}

		protected abstract bool IsMatch(string expectedPath, string actualPath);

		protected override string GetStringRepresentation()
		{
			return string.Format("<{0} \"{1}\" {2}>", base.DisplayName, expectedPath, caseInsensitive ? "ignorecase" : "respectcase");
		}

		protected static string Canonicalize(string path)
		{
			bool flag = false;
			if (path.Length > 0)
			{
				char[] directorySeparatorChars = DirectorySeparatorChars;
				foreach (char c in directorySeparatorChars)
				{
					if (path[0] == c)
					{
						flag = true;
					}
				}
			}
			ArrayList arrayList = new ArrayList(path.Split(DirectorySeparatorChars));
			int num = 0;
			while (num < arrayList.Count)
			{
				switch ((string)arrayList[num])
				{
				case ".":
					arrayList.RemoveAt(num);
					break;
				case "..":
					arrayList.RemoveAt(num);
					if (num > 0)
					{
						arrayList.RemoveAt(--num);
					}
					break;
				default:
					num++;
					break;
				}
			}
			if ((string)arrayList[arrayList.Count - 1] == "")
			{
				arrayList.RemoveAt(arrayList.Count - 1);
			}
			string text = string.Join(Path.DirectorySeparatorChar.ToString(), (string[])arrayList.ToArray(typeof(string)));
			if (flag)
			{
				text = Path.DirectorySeparatorChar + text;
			}
			return text;
		}

		protected static bool IsSamePath(string path1, string path2, bool ignoreCase)
		{
			return string.Compare(path1, path2, ignoreCase) == 0;
		}

		protected static bool IsSubPath(string path1, string path2, bool ignoreCase)
		{
			int length = path1.Length;
			int length2 = path2.Length;
			if (length >= length2)
			{
				return false;
			}
			if (string.Compare(path1, path2.Substring(0, length), ignoreCase) != 0)
			{
				return false;
			}
			return path2[length - 1] == Path.DirectorySeparatorChar || (length2 > length && path2[length] == Path.DirectorySeparatorChar);
		}

		protected bool IsSamePathOrUnder(string path1, string path2)
		{
			int length = path1.Length;
			int length2 = path2.Length;
			if (length > length2)
			{
				return false;
			}
			if (length == length2)
			{
				return string.Compare(path1, path2, caseInsensitive) == 0;
			}
			if (string.Compare(path1, path2.Substring(0, length), caseInsensitive) != 0)
			{
				return false;
			}
			return path2[length - 1] == Path.DirectorySeparatorChar || path2[length] == Path.DirectorySeparatorChar;
		}
	}
}

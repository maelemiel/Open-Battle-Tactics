using System;
using System.IO;

namespace NUnit.Framework.Constraints
{
	public class EmptyDirectoryContraint : Constraint
	{
		private int files = 0;

		private int subdirs = 0;

		public override bool Matches(object actual)
		{
			base.actual = actual;
			DirectoryInfo directoryInfo = actual as DirectoryInfo;
			if (directoryInfo == null)
			{
				throw new ArgumentException("The actual value must be a DirectoryInfo", "actual");
			}
			files = directoryInfo.GetFiles().Length;
			subdirs = directoryInfo.GetDirectories().Length;
			return files == 0 && subdirs == 0;
		}

		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.Write("An empty directory");
		}

		public override void WriteActualValueTo(MessageWriter writer)
		{
			DirectoryInfo directoryInfo = actual as DirectoryInfo;
			if (directoryInfo == null)
			{
				base.WriteActualValueTo(writer);
				return;
			}
			writer.WriteActualValue(directoryInfo);
			writer.Write(" with {0} files and {1} directories", files, subdirs);
		}
	}
}

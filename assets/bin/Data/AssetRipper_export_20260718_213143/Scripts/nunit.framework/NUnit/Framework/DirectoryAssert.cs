using System;
using System.ComponentModel;
using System.IO;
using NUnit.Framework.Constraints;

namespace NUnit.Framework
{
	[Obsolete("Use Assert with constraint-based syntax")]
	public class DirectoryAssert
	{
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new static bool Equals(object a, object b)
		{
			throw new AssertionException("Assert.Equals should not be used for Assertions");
		}

		public new static void ReferenceEquals(object a, object b)
		{
			throw new AssertionException("Assert.ReferenceEquals should not be used for Assertions");
		}

		protected DirectoryAssert()
		{
		}

		public static void AreEqual(DirectoryInfo expected, DirectoryInfo actual, string message, params object[] args)
		{
			Assert.That(actual, new EqualConstraint(expected), message, args);
		}

		public static void AreEqual(DirectoryInfo expected, DirectoryInfo actual, string message)
		{
			AreEqual(actual, expected, message, null);
		}

		public static void AreEqual(DirectoryInfo expected, DirectoryInfo actual)
		{
			AreEqual(actual, expected, string.Empty, null);
		}

		public static void AreEqual(string expected, string actual, string message, params object[] args)
		{
			DirectoryInfo expected2 = new DirectoryInfo(expected);
			DirectoryInfo actual2 = new DirectoryInfo(actual);
			AreEqual(expected2, actual2, message, args);
		}

		public static void AreEqual(string expected, string actual, string message)
		{
			AreEqual(expected, actual, message, null);
		}

		public static void AreEqual(string expected, string actual)
		{
			AreEqual(expected, actual, string.Empty, null);
		}

		public static void AreNotEqual(DirectoryInfo expected, DirectoryInfo actual, string message, params object[] args)
		{
			Assert.That(actual, new NotConstraint(new EqualConstraint(expected)), message, args);
		}

		public static void AreNotEqual(DirectoryInfo expected, DirectoryInfo actual, string message)
		{
			AreNotEqual(actual, expected, message, null);
		}

		public static void AreNotEqual(DirectoryInfo expected, DirectoryInfo actual)
		{
			AreNotEqual(actual, expected, string.Empty, null);
		}

		public static void AreNotEqual(string expected, string actual, string message, params object[] args)
		{
			DirectoryInfo expected2 = new DirectoryInfo(expected);
			DirectoryInfo actual2 = new DirectoryInfo(actual);
			AreNotEqual(expected2, actual2, message, args);
		}

		public static void AreNotEqual(string expected, string actual, string message)
		{
			AreNotEqual(expected, actual, message, null);
		}

		public static void AreNotEqual(string expected, string actual)
		{
			AreNotEqual(expected, actual, string.Empty, null);
		}

		public static void IsEmpty(DirectoryInfo directory, string message, params object[] args)
		{
			Assert.That(directory, new EmptyDirectoryContraint(), message, args);
		}

		public static void IsEmpty(DirectoryInfo directory, string message)
		{
			IsEmpty(directory, message, null);
		}

		public static void IsEmpty(DirectoryInfo directory)
		{
			IsEmpty(directory, string.Empty, null);
		}

		public static void IsEmpty(string directory, string message, params object[] args)
		{
			IsEmpty(new DirectoryInfo(directory), message, args);
		}

		public static void IsEmpty(string directory, string message)
		{
			IsEmpty(directory, message, null);
		}

		public static void IsEmpty(string directory)
		{
			IsEmpty(directory, string.Empty, null);
		}

		public static void IsNotEmpty(DirectoryInfo directory, string message, params object[] args)
		{
			Assert.That(directory, new NotConstraint(new EmptyDirectoryContraint()), message, args);
		}

		public static void IsNotEmpty(DirectoryInfo directory, string message)
		{
			IsNotEmpty(directory, message, null);
		}

		public static void IsNotEmpty(DirectoryInfo directory)
		{
			IsNotEmpty(directory, string.Empty, null);
		}

		public static void IsNotEmpty(string directory, string message, params object[] args)
		{
			DirectoryInfo directory2 = new DirectoryInfo(directory);
			IsNotEmpty(directory2, message, args);
		}

		public static void IsNotEmpty(string directory, string message)
		{
			IsNotEmpty(directory, message, null);
		}

		public static void IsNotEmpty(string directory)
		{
			IsNotEmpty(directory, string.Empty, null);
		}

		public static void IsWithin(DirectoryInfo directory, DirectoryInfo actual, string message, params object[] args)
		{
			if (directory == null)
			{
				throw new ArgumentException("The directory may not be null", "directory");
			}
			if (directory == null)
			{
				throw new ArgumentException("The actual value may not be null", "actual");
			}
			IsWithin(directory.FullName, actual.FullName, message, args);
		}

		public static void IsWithin(DirectoryInfo directory, DirectoryInfo actual, string message)
		{
			if (directory == null)
			{
				throw new ArgumentException("The directory may not be null", "directory");
			}
			if (directory == null)
			{
				throw new ArgumentException("The actual value may not be null", "actual");
			}
			IsWithin(directory.FullName, actual.FullName, message, null);
		}

		public static void IsWithin(DirectoryInfo directory, DirectoryInfo actual)
		{
			if (directory == null)
			{
				throw new ArgumentException("The directory may not be null", "directory");
			}
			if (directory == null)
			{
				throw new ArgumentException("The actual value may not be null", "actual");
			}
			IsWithin(directory.FullName, actual.FullName, string.Empty, null);
		}

		public static void IsWithin(string directory, string actual, string message, params object[] args)
		{
			Assert.That(actual, new SubPathConstraint(directory), message, args);
		}

		public static void IsWithin(string directory, string actual, string message)
		{
			IsWithin(directory, actual, message, null);
		}

		public static void IsWithin(string directory, string actual)
		{
			IsWithin(directory, actual, string.Empty, null);
		}

		public static void IsNotWithin(DirectoryInfo directory, DirectoryInfo actual, string message, params object[] args)
		{
			if (directory == null)
			{
				throw new ArgumentException("The directory may not be null", "directory");
			}
			if (directory == null)
			{
				throw new ArgumentException("The actual value may not be null", "actual");
			}
			IsNotWithin(directory.FullName, actual.FullName, message, args);
		}

		public static void IsNotWithin(DirectoryInfo directory, DirectoryInfo actual, string message)
		{
			if (directory == null)
			{
				throw new ArgumentException("The directory may not be null", "directory");
			}
			if (directory == null)
			{
				throw new ArgumentException("The actual value may not be null", "actual");
			}
			IsNotWithin(directory.FullName, actual.FullName, message, null);
		}

		public static void IsNotWithin(DirectoryInfo directory, DirectoryInfo actual)
		{
			if (directory == null)
			{
				throw new ArgumentException("The directory may not be null", "directory");
			}
			if (directory == null)
			{
				throw new ArgumentException("The actual value may not be null", "actual");
			}
			IsNotWithin(directory.FullName, actual.FullName, string.Empty, null);
		}

		public static void IsNotWithin(string directory, string actual, string message, params object[] args)
		{
			Assert.That(actual, new NotConstraint(new SubPathConstraint(directory)), message, args);
		}

		public static void IsNotWithin(string directory, string actual, string message)
		{
			IsNotWithin(directory, actual, message, null);
		}

		public static void IsNotWithin(string directory, string actual)
		{
			IsNotWithin(directory, actual, string.Empty, null);
		}
	}
}

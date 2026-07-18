using System;
using System.IO;

namespace Mono.Security.X509
{
	internal class X509Stores
	{
		public class Names
		{
			public const string Personal = "My";

			public const string OtherPeople = "AddressBook";

			public const string IntermediateCA = "CA";

			public const string TrustedRoot = "Trust";

			public const string Untrusted = "Disallowed";
		}

		private string _storePath;

		private X509Store _personal;

		private X509Store _other;

		private X509Store _intermediate;

		private X509Store _trusted;

		private X509Store _untrusted;

		public X509Store Personal
		{
			get
			{
				if (_personal == null)
				{
					string path = Path.Combine(_storePath, "My");
					_personal = new X509Store(path, false);
				}
				return _personal;
			}
		}

		public X509Store OtherPeople
		{
			get
			{
				if (_other == null)
				{
					string path = Path.Combine(_storePath, "AddressBook");
					_other = new X509Store(path, false);
				}
				return _other;
			}
		}

		public X509Store IntermediateCA
		{
			get
			{
				if (_intermediate == null)
				{
					string path = Path.Combine(_storePath, "CA");
					_intermediate = new X509Store(path, true);
				}
				return _intermediate;
			}
		}

		public X509Store TrustedRoot
		{
			get
			{
				if (_trusted == null)
				{
					string path = Path.Combine(_storePath, "Trust");
					_trusted = new X509Store(path, true);
				}
				return _trusted;
			}
		}

		public X509Store Untrusted
		{
			get
			{
				if (_untrusted == null)
				{
					string path = Path.Combine(_storePath, "Disallowed");
					_untrusted = new X509Store(path, false);
				}
				return _untrusted;
			}
		}

		internal X509Stores(string path)
		{
			_storePath = path;
		}

		public void Clear()
		{
			if (_personal != null)
			{
				_personal.Clear();
			}
			_personal = null;
			if (_other != null)
			{
				_other.Clear();
			}
			_other = null;
			if (_intermediate != null)
			{
				_intermediate.Clear();
			}
			_intermediate = null;
			if (_trusted != null)
			{
				_trusted.Clear();
			}
			_trusted = null;
			if (_untrusted != null)
			{
				_untrusted.Clear();
			}
			_untrusted = null;
		}

		public X509Store Open(string storeName, bool create)
		{
			if (storeName == null)
			{
				throw new ArgumentNullException("storeName");
			}
			string path = Path.Combine(_storePath, storeName);
			if (!create && !Directory.Exists(path))
			{
				return null;
			}
			return new X509Store(path, true);
		}
	}
}

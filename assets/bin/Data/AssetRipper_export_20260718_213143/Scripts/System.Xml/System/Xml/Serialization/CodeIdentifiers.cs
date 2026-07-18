using System.Collections;
using System.Globalization;

namespace System.Xml.Serialization
{
	public class CodeIdentifiers
	{
		private bool useCamelCasing;

		private Hashtable table;

		private Hashtable reserved;

		public bool UseCamelCasing
		{
			get
			{
				return useCamelCasing;
			}
			set
			{
				useCamelCasing = value;
			}
		}

		public CodeIdentifiers()
			: this(true)
		{
		}

		public CodeIdentifiers(bool caseSensitive)
		{
			StringComparer equalityComparer = ((!caseSensitive) ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal);
			table = new Hashtable(equalityComparer);
			reserved = new Hashtable(equalityComparer);
		}

		public void Add(string identifier, object value)
		{
			table.Add(identifier, value);
		}

		public void AddReserved(string identifier)
		{
			reserved.Add(identifier, identifier);
		}

		public string AddUnique(string identifier, object value)
		{
			string text = MakeUnique(identifier);
			Add(text, value);
			return text;
		}

		public void Clear()
		{
			table.Clear();
		}

		public bool IsInUse(string identifier)
		{
			return table.ContainsKey(identifier) || reserved.ContainsKey(identifier);
		}

		public string MakeRightCase(string identifier)
		{
			if (UseCamelCasing)
			{
				return CodeIdentifier.MakeCamel(identifier);
			}
			return CodeIdentifier.MakePascal(identifier);
		}

		public string MakeUnique(string identifier)
		{
			string text = identifier;
			int num = 1;
			while (IsInUse(text))
			{
				text = string.Format(CultureInfo.InvariantCulture, "{0}{1}", identifier, num);
				num++;
			}
			return text;
		}

		public void Remove(string identifier)
		{
			table.Remove(identifier);
		}

		public void RemoveReserved(string identifier)
		{
			reserved.Remove(identifier);
		}

		public object ToArray(Type type)
		{
			Array array = Array.CreateInstance(type, table.Count);
			table.CopyTo(array, 0);
			return array;
		}
	}
}

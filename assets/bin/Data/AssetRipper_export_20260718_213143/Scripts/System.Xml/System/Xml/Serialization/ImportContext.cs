using System.Collections;
using System.Collections.Specialized;

namespace System.Xml.Serialization
{
	public class ImportContext
	{
		private bool _shareTypes;

		private CodeIdentifiers _typeIdentifiers;

		private StringCollection _warnings = new StringCollection();

		internal Hashtable MappedTypes;

		internal Hashtable DataMappedTypes;

		internal Hashtable SharedAnonymousTypes;

		public bool ShareTypes
		{
			get
			{
				return _shareTypes;
			}
		}

		public CodeIdentifiers TypeIdentifiers
		{
			get
			{
				return _typeIdentifiers;
			}
		}

		public StringCollection Warnings
		{
			get
			{
				return _warnings;
			}
		}

		public ImportContext(CodeIdentifiers identifiers, bool shareTypes)
		{
			_typeIdentifiers = identifiers;
			_shareTypes = shareTypes;
			if (shareTypes)
			{
				MappedTypes = new Hashtable();
				DataMappedTypes = new Hashtable();
				SharedAnonymousTypes = new Hashtable();
			}
		}
	}
}

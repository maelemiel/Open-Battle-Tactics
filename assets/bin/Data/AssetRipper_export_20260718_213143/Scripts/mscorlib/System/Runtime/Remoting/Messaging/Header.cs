using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Messaging
{
	[Serializable]
	[ComVisible(true)]
	public class Header
	{
		public string HeaderNamespace;

		public bool MustUnderstand;

		public string Name;

		public object Value;

		public Header(string _Name, object _Value)
			: this(_Name, _Value, true)
		{
		}

		public Header(string _Name, object _Value, bool _MustUnderstand)
			: this(_Name, _Value, _MustUnderstand, null)
		{
		}

		public Header(string _Name, object _Value, bool _MustUnderstand, string _HeaderNamespace)
		{
			Name = _Name;
			Value = _Value;
			MustUnderstand = _MustUnderstand;
			HeaderNamespace = _HeaderNamespace;
		}
	}
}

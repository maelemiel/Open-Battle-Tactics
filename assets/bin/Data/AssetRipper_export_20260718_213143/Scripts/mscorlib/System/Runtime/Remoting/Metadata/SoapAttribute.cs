using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Metadata
{
	[ComVisible(true)]
	public class SoapAttribute : Attribute
	{
		private bool _nested;

		private bool _useAttribute;

		protected string ProtXmlNamespace;

		protected object ReflectInfo;

		public virtual bool Embedded
		{
			get
			{
				return _nested;
			}
			set
			{
				_nested = value;
			}
		}

		public virtual bool UseAttribute
		{
			get
			{
				return _useAttribute;
			}
			set
			{
				_useAttribute = value;
			}
		}

		public virtual string XmlNamespace
		{
			get
			{
				return ProtXmlNamespace;
			}
			set
			{
				ProtXmlNamespace = value;
			}
		}

		internal virtual void SetReflectionObject(object reflectionObject)
		{
			ReflectInfo = reflectionObject;
		}
	}
}

using System.Runtime.Serialization;

namespace System.Xml.XPath
{
	[Serializable]
	public class XPathException : SystemException
	{
		public override string Message
		{
			get
			{
				return base.Message;
			}
		}

		public XPathException()
			: base(string.Empty)
		{
		}

		protected XPathException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public XPathException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		public XPathException(string message)
			: base(message, null)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}

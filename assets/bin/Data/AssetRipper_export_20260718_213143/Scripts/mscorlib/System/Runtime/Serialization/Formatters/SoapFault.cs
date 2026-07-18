using System.Runtime.InteropServices;
using System.Runtime.Remoting.Metadata;

namespace System.Runtime.Serialization.Formatters
{
	[Serializable]
	[SoapType]
	[ComVisible(true)]
	public sealed class SoapFault : ISerializable
	{
		private string code;

		private string actor;

		private string faultString;

		private object detail;

		public object Detail
		{
			get
			{
				return detail;
			}
			set
			{
				detail = value;
			}
		}

		public string FaultActor
		{
			get
			{
				return actor;
			}
			set
			{
				actor = value;
			}
		}

		public string FaultCode
		{
			get
			{
				return code;
			}
			set
			{
				code = value;
			}
		}

		public string FaultString
		{
			get
			{
				return faultString;
			}
			set
			{
				faultString = value;
			}
		}

		public SoapFault()
		{
		}

		private SoapFault(SerializationInfo info, StreamingContext context)
		{
			code = info.GetString("faultcode");
			faultString = info.GetString("faultstring");
			detail = info.GetValue("detail", typeof(object));
		}

		public SoapFault(string faultCode, string faultString, string faultActor, ServerFault serverFault)
		{
			code = faultCode;
			actor = faultActor;
			this.faultString = faultString;
			detail = serverFault;
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("faultcode", code, typeof(string));
			info.AddValue("faultstring", faultString, typeof(string));
			info.AddValue("detail", detail, typeof(object));
		}
	}
}

using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[Serializable]
	[ComVisible(true)]
	public class ArgumentException : SystemException
	{
		private const int Result = -2147024809;

		private string param_name;

		public virtual string ParamName
		{
			get
			{
				return param_name;
			}
		}

		public override string Message
		{
			get
			{
				if (ParamName != null && ParamName.Length != 0)
				{
					return base.Message + Environment.NewLine + Locale.GetText("Parameter name: ") + ParamName;
				}
				return base.Message;
			}
		}

		public ArgumentException()
			: base(Locale.GetText("Value does not fall within the expected range."))
		{
			base.HResult = -2147024809;
		}

		public ArgumentException(string message)
			: base(message)
		{
			base.HResult = -2147024809;
		}

		public ArgumentException(string message, Exception innerException)
			: base(message, innerException)
		{
			base.HResult = -2147024809;
		}

		public ArgumentException(string message, string paramName)
			: base(message)
		{
			param_name = paramName;
			base.HResult = -2147024809;
		}

		public ArgumentException(string message, string paramName, Exception innerException)
			: base(message, innerException)
		{
			param_name = paramName;
			base.HResult = -2147024809;
		}

		protected ArgumentException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			param_name = info.GetString("ParamName");
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("ParamName", ParamName);
		}
	}
}

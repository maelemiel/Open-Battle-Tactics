using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;

namespace System
{
	[Serializable]
	[ComVisible(true)]
	public class BadImageFormatException : SystemException
	{
		private const int Result = -2147024885;

		private string fileName;

		private string fusionLog;

		public override string Message
		{
			get
			{
				if (message == null)
				{
					return string.Format(CultureInfo.CurrentCulture, "Could not load file or assembly '{0}' or one of its dependencies. An attempt was made to load a program with an incorrect format.", fileName);
				}
				return base.Message;
			}
		}

		public string FileName
		{
			get
			{
				return fileName;
			}
		}

		[MonoTODO("Probably not entirely correct. fusionLog needs to be set somehow (we are probably missing internal constuctor)")]
		public string FusionLog
		{
			get
			{
				return fusionLog;
			}
		}

		public BadImageFormatException()
			: base(Locale.GetText("Format of the executable (.exe) or library (.dll) is invalid."))
		{
			base.HResult = -2147024885;
		}

		public BadImageFormatException(string message)
			: base(message)
		{
			base.HResult = -2147024885;
		}

		protected BadImageFormatException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			fileName = info.GetString("BadImageFormat_FileName");
			fusionLog = info.GetString("BadImageFormat_FusionLog");
		}

		public BadImageFormatException(string message, Exception inner)
			: base(message, inner)
		{
			base.HResult = -2147024885;
		}

		public BadImageFormatException(string message, string fileName)
			: base(message)
		{
			this.fileName = fileName;
			base.HResult = -2147024885;
		}

		public BadImageFormatException(string message, string fileName, Exception inner)
			: base(message, inner)
		{
			this.fileName = fileName;
			base.HResult = -2147024885;
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("BadImageFormat_FileName", fileName);
			info.AddValue("BadImageFormat_FusionLog", fusionLog);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(GetType().FullName);
			stringBuilder.AppendFormat(": {0}", Message);
			if (fileName != null && fileName.Length > 0)
			{
				stringBuilder.Append(Environment.NewLine);
				stringBuilder.AppendFormat("File name: '{0}'", fileName);
			}
			if (InnerException != null)
			{
				stringBuilder.AppendFormat(" ---> {0}", InnerException);
			}
			if (StackTrace != null)
			{
				stringBuilder.Append(Environment.NewLine);
				stringBuilder.Append(StackTrace);
			}
			return stringBuilder.ToString();
		}
	}
}

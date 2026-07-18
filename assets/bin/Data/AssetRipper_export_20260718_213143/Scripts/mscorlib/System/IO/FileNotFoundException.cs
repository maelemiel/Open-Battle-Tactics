using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;

namespace System.IO
{
	[Serializable]
	[ComVisible(true)]
	public class FileNotFoundException : IOException
	{
		private const int Result = -2146232799;

		private string fileName;

		private string fusionLog;

		public string FileName
		{
			get
			{
				return fileName;
			}
		}

		public string FusionLog
		{
			get
			{
				return fusionLog;
			}
		}

		public override string Message
		{
			get
			{
				if (message == null && fileName != null)
				{
					return string.Format(CultureInfo.CurrentCulture, "Could not load file or assembly '{0}' or one of its dependencies. The system cannot find the file specified.", fileName);
				}
				return message;
			}
		}

		public FileNotFoundException()
			: base(Locale.GetText("Unable to find the specified file."))
		{
			base.HResult = -2146232799;
		}

		public FileNotFoundException(string message)
			: base(message)
		{
			base.HResult = -2146232799;
		}

		public FileNotFoundException(string message, Exception innerException)
			: base(message, innerException)
		{
			base.HResult = -2146232799;
		}

		public FileNotFoundException(string message, string fileName)
			: base(message)
		{
			base.HResult = -2146232799;
			this.fileName = fileName;
		}

		public FileNotFoundException(string message, string fileName, Exception innerException)
			: base(message, innerException)
		{
			base.HResult = -2146232799;
			this.fileName = fileName;
		}

		protected FileNotFoundException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			fileName = info.GetString("FileNotFound_FileName");
			fusionLog = info.GetString("FileNotFound_FusionLog");
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("FileNotFound_FileName", fileName);
			info.AddValue("FileNotFound_FusionLog", fusionLog);
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

using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[Serializable]
	[ComVisible(true)]
	public sealed class TypeInitializationException : SystemException
	{
		private string type_name;

		public string TypeName
		{
			get
			{
				return type_name;
			}
		}

		public TypeInitializationException(string fullTypeName, Exception innerException)
			: base(Locale.GetText("An exception was thrown by the type initializer for ") + fullTypeName, innerException)
		{
			type_name = fullTypeName;
		}

		internal TypeInitializationException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			type_name = info.GetString("TypeName");
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("TypeName", type_name);
		}
	}
}

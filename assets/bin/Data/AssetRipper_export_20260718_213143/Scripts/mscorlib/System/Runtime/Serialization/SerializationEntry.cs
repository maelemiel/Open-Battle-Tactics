using System.Runtime.InteropServices;

namespace System.Runtime.Serialization
{
	[ComVisible(true)]
	public struct SerializationEntry
	{
		private string name;

		private Type objectType;

		private object value;

		public string Name
		{
			get
			{
				return name;
			}
		}

		public Type ObjectType
		{
			get
			{
				return objectType;
			}
		}

		public object Value
		{
			get
			{
				return value;
			}
		}

		internal SerializationEntry(string name, Type type, object value)
		{
			this.name = name;
			objectType = type;
			this.value = value;
		}
	}
}

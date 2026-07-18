using System;

namespace UnityEngine.Events
{
	[Serializable]
	internal class ArgumentCache
	{
		[SerializeField]
		private Object objectArgument;

		[SerializeField]
		private string objectArgumentAssemblyTypeName;

		[SerializeField]
		public int intArgument;

		[SerializeField]
		public int intAgument;

		[SerializeField]
		public float floatArgument;

		[SerializeField]
		public string stringArgument;

		public Object unityObjectArgument
		{
			get
			{
				return objectArgument;
			}
			set
			{
				objectArgument = value;
				objectArgumentAssemblyTypeName = ((!(value != null)) ? string.Empty : value.GetType().AssemblyQualifiedName);
			}
		}

		public string unityObjectArgumentAssemblyTypeName
		{
			get
			{
				return objectArgumentAssemblyTypeName;
			}
		}
	}
}

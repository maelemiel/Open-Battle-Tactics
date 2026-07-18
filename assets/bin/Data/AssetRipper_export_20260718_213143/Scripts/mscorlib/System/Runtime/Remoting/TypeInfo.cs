namespace System.Runtime.Remoting
{
	[Serializable]
	internal class TypeInfo : IRemotingTypeInfo
	{
		private string serverType;

		private string[] serverHierarchy;

		private string[] interfacesImplemented;

		public string TypeName
		{
			get
			{
				return serverType;
			}
			set
			{
				serverType = value;
			}
		}

		public TypeInfo(Type type)
		{
			if (type.IsInterface)
			{
				serverType = typeof(MarshalByRefObject).AssemblyQualifiedName;
				serverHierarchy = new string[0];
				interfacesImplemented = new string[1] { type.AssemblyQualifiedName };
				return;
			}
			serverType = type.AssemblyQualifiedName;
			int num = 0;
			Type baseType = type.BaseType;
			while (baseType != typeof(MarshalByRefObject) && baseType != typeof(object))
			{
				baseType = baseType.BaseType;
				num++;
			}
			serverHierarchy = new string[num];
			baseType = type.BaseType;
			for (int i = 0; i < num; i++)
			{
				serverHierarchy[i] = baseType.AssemblyQualifiedName;
				baseType = baseType.BaseType;
			}
			Type[] interfaces = type.GetInterfaces();
			interfacesImplemented = new string[interfaces.Length];
			for (int j = 0; j < interfaces.Length; j++)
			{
				interfacesImplemented[j] = interfaces[j].AssemblyQualifiedName;
			}
		}

		public bool CanCastTo(Type fromType, object o)
		{
			if (fromType == typeof(object))
			{
				return true;
			}
			if (fromType == typeof(MarshalByRefObject))
			{
				return true;
			}
			string assemblyQualifiedName = fromType.AssemblyQualifiedName;
			int num = assemblyQualifiedName.IndexOf(',');
			if (num != -1)
			{
				num = assemblyQualifiedName.IndexOf(',', num + 1);
			}
			assemblyQualifiedName = ((num == -1) ? (assemblyQualifiedName + ",") : assemblyQualifiedName.Substring(0, num + 1));
			if ((serverType + ",").StartsWith(assemblyQualifiedName))
			{
				return true;
			}
			if (serverHierarchy != null)
			{
				string[] array = serverHierarchy;
				foreach (string text in array)
				{
					if ((text + ",").StartsWith(assemblyQualifiedName))
					{
						return true;
					}
				}
			}
			if (interfacesImplemented != null)
			{
				string[] array2 = interfacesImplemented;
				foreach (string text2 in array2)
				{
					if ((text2 + ",").StartsWith(assemblyQualifiedName))
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}

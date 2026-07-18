namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
	public sealed class EditorAttribute : Attribute
	{
		private string name;

		private string basename;

		public string EditorBaseTypeName
		{
			get
			{
				return basename;
			}
		}

		public string EditorTypeName
		{
			get
			{
				return name;
			}
		}

		public override object TypeId
		{
			get
			{
				return GetType();
			}
		}

		public EditorAttribute()
		{
			name = string.Empty;
		}

		public EditorAttribute(string typeName, string baseTypeName)
		{
			name = typeName;
			basename = baseTypeName;
		}

		public EditorAttribute(string typeName, Type baseType)
			: this(typeName, baseType.AssemblyQualifiedName)
		{
		}

		public EditorAttribute(Type type, Type baseType)
			: this(type.AssemblyQualifiedName, baseType.AssemblyQualifiedName)
		{
		}

		public override bool Equals(object obj)
		{
			if (!(obj is EditorAttribute))
			{
				return false;
			}
			return ((EditorAttribute)obj).EditorBaseTypeName.Equals(basename) && ((EditorAttribute)obj).EditorTypeName.Equals(name);
		}

		public override int GetHashCode()
		{
			return (name + basename).GetHashCode();
		}
	}
}

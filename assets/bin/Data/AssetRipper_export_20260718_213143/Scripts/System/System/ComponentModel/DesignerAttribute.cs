using System.ComponentModel.Design;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = true)]
	public sealed class DesignerAttribute : Attribute
	{
		private string name;

		private string basetypename;

		public string DesignerBaseTypeName
		{
			get
			{
				return basetypename;
			}
		}

		public string DesignerTypeName
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
				string text = basetypename;
				int num = text.IndexOf(',');
				if (num != -1)
				{
					text = text.Substring(0, num);
				}
				return GetType().ToString() + text;
			}
		}

		public DesignerAttribute(string designerTypeName)
		{
			if (designerTypeName == null)
			{
				throw new NullReferenceException();
			}
			name = designerTypeName;
			basetypename = typeof(IDesigner).FullName;
		}

		public DesignerAttribute(Type designerType)
			: this(designerType.AssemblyQualifiedName)
		{
		}

		public DesignerAttribute(string designerTypeName, Type designerBaseType)
			: this(designerTypeName, designerBaseType.AssemblyQualifiedName)
		{
		}

		public DesignerAttribute(Type designerType, Type designerBaseType)
			: this(designerType.AssemblyQualifiedName, designerBaseType.AssemblyQualifiedName)
		{
		}

		public DesignerAttribute(string designerTypeName, string designerBaseTypeName)
		{
			if (designerTypeName == null)
			{
				throw new NullReferenceException();
			}
			name = designerTypeName;
			basetypename = designerBaseTypeName;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is DesignerAttribute))
			{
				return false;
			}
			return ((DesignerAttribute)obj).DesignerBaseTypeName.Equals(basetypename) && ((DesignerAttribute)obj).DesignerTypeName.Equals(name);
		}

		public override int GetHashCode()
		{
			return (name + basetypename).GetHashCode();
		}
	}
}

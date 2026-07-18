namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.All)]
	public class ToolboxItemAttribute : Attribute
	{
		private const string defaultItemType = "System.Drawing.Design.ToolboxItem, System.Drawing, Version=2.0.5.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";

		public static readonly ToolboxItemAttribute Default = new ToolboxItemAttribute("System.Drawing.Design.ToolboxItem, System.Drawing, Version=2.0.5.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");

		public static readonly ToolboxItemAttribute None = new ToolboxItemAttribute(false);

		private Type itemType;

		private string itemTypeName;

		public Type ToolboxItemType
		{
			get
			{
				if (itemType == null && itemTypeName != null)
				{
					try
					{
						itemType = Type.GetType(itemTypeName, true);
					}
					catch (Exception innerException)
					{
						throw new ArgumentException("Failed to create ToolboxItem of type: " + itemTypeName, innerException);
					}
				}
				return itemType;
			}
		}

		public string ToolboxItemTypeName
		{
			get
			{
				if (itemTypeName == null)
				{
					if (itemType == null)
					{
						return string.Empty;
					}
					itemTypeName = itemType.AssemblyQualifiedName;
				}
				return itemTypeName;
			}
		}

		public ToolboxItemAttribute(bool defaultType)
		{
			if (defaultType)
			{
				itemTypeName = "System.Drawing.Design.ToolboxItem, System.Drawing, Version=2.0.5.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";
			}
		}

		public ToolboxItemAttribute(string toolboxItemName)
		{
			itemTypeName = toolboxItemName;
		}

		public ToolboxItemAttribute(Type toolboxItemType)
		{
			itemType = toolboxItemType;
		}

		public override bool Equals(object o)
		{
			ToolboxItemAttribute toolboxItemAttribute = o as ToolboxItemAttribute;
			if (toolboxItemAttribute == null)
			{
				return false;
			}
			return toolboxItemAttribute.ToolboxItemTypeName == ToolboxItemTypeName;
		}

		public override int GetHashCode()
		{
			if (itemTypeName != null)
			{
				return itemTypeName.GetHashCode();
			}
			return base.GetHashCode();
		}

		public override bool IsDefaultAttribute()
		{
			return Equals(Default);
		}
	}
}

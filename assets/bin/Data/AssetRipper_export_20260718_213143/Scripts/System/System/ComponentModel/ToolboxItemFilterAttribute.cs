namespace System.ComponentModel
{
	[Serializable]
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	public sealed class ToolboxItemFilterAttribute : Attribute
	{
		private string Filter;

		private ToolboxItemFilterType ItemFilterType;

		public string FilterString
		{
			get
			{
				return Filter;
			}
		}

		public ToolboxItemFilterType FilterType
		{
			get
			{
				return ItemFilterType;
			}
		}

		public override object TypeId
		{
			get
			{
				return string.Concat(base.TypeId, Filter);
			}
		}

		public ToolboxItemFilterAttribute(string filterString)
		{
			Filter = filterString;
			ItemFilterType = ToolboxItemFilterType.Allow;
		}

		public ToolboxItemFilterAttribute(string filterString, ToolboxItemFilterType filterType)
		{
			Filter = filterString;
			ItemFilterType = filterType;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is ToolboxItemFilterAttribute))
			{
				return false;
			}
			if (obj == this)
			{
				return true;
			}
			return ((ToolboxItemFilterAttribute)obj).FilterString == Filter && ((ToolboxItemFilterAttribute)obj).FilterType == ItemFilterType;
		}

		public override int GetHashCode()
		{
			return ToString().GetHashCode();
		}

		public override bool Match(object obj)
		{
			if (!(obj is ToolboxItemFilterAttribute))
			{
				return false;
			}
			return ((ToolboxItemFilterAttribute)obj).FilterString == Filter;
		}

		public override string ToString()
		{
			return string.Format("{0},{1}", Filter, ItemFilterType);
		}
	}
}

using System.IO;

namespace System.Resources
{
	internal abstract class Win32Resource
	{
		private NameOrId type;

		private NameOrId name;

		private int language;

		public Win32ResourceType ResourceType
		{
			get
			{
				if (type.IsName)
				{
					return (Win32ResourceType)(-1);
				}
				return (Win32ResourceType)type.Id;
			}
		}

		public NameOrId Name
		{
			get
			{
				return name;
			}
		}

		public NameOrId Type
		{
			get
			{
				return type;
			}
		}

		public int Language
		{
			get
			{
				return language;
			}
		}

		internal Win32Resource(NameOrId type, NameOrId name, int language)
		{
			this.type = type;
			this.name = name;
			this.language = language;
		}

		internal Win32Resource(Win32ResourceType type, int name, int language)
		{
			this.type = new NameOrId((int)type);
			this.name = new NameOrId(name);
			this.language = language;
		}

		public abstract void WriteTo(Stream s);

		public override string ToString()
		{
			return string.Concat("Win32Resource (Kind=", ResourceType, ", Name=", name, ")");
		}
	}
}

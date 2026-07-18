namespace System.Resources
{
	internal class NameOrId
	{
		private string name;

		private int id;

		public bool IsName
		{
			get
			{
				return name != null;
			}
		}

		public string Name
		{
			get
			{
				return name;
			}
		}

		public int Id
		{
			get
			{
				return id;
			}
		}

		public NameOrId(string name)
		{
			this.name = name;
		}

		public NameOrId(int id)
		{
			this.id = id;
		}

		public override string ToString()
		{
			if (name != null)
			{
				return "Name(" + name + ")";
			}
			return "Id(" + id + ")";
		}
	}
}

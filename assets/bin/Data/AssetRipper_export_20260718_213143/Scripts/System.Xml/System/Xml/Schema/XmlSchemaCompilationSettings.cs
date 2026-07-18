namespace System.Xml.Schema
{
	public sealed class XmlSchemaCompilationSettings
	{
		private bool enable_upa_check = true;

		public bool EnableUpaCheck
		{
			get
			{
				return enable_upa_check;
			}
			set
			{
				enable_upa_check = value;
			}
		}
	}
}

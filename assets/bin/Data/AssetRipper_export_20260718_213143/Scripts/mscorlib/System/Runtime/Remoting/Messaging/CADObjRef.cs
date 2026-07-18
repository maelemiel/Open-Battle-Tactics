namespace System.Runtime.Remoting.Messaging
{
	internal class CADObjRef
	{
		private ObjRef objref;

		public int SourceDomain;

		public string TypeName
		{
			get
			{
				return objref.TypeInfo.TypeName;
			}
		}

		public string URI
		{
			get
			{
				return objref.URI;
			}
		}

		public CADObjRef(ObjRef o, int sourceDomain)
		{
			objref = o;
			SourceDomain = sourceDomain;
		}
	}
}

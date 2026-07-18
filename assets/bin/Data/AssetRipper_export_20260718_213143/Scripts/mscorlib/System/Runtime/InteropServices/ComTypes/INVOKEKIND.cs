namespace System.Runtime.InteropServices.ComTypes
{
	[Serializable]
	[Flags]
	public enum INVOKEKIND
	{
		INVOKE_FUNC = 1,
		INVOKE_PROPERTYGET = 2,
		INVOKE_PROPERTYPUT = 4,
		INVOKE_PROPERTYPUTREF = 8
	}
}

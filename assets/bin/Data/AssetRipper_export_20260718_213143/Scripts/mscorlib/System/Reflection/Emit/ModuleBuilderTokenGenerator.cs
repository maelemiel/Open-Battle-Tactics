namespace System.Reflection.Emit
{
	internal class ModuleBuilderTokenGenerator : TokenGenerator
	{
		private ModuleBuilder mb;

		public ModuleBuilderTokenGenerator(ModuleBuilder mb)
		{
			this.mb = mb;
		}

		public int GetToken(string str)
		{
			return mb.GetToken(str);
		}

		public int GetToken(MemberInfo member)
		{
			return mb.GetToken(member);
		}

		public int GetToken(MethodInfo method, Type[] opt_param_types)
		{
			return mb.GetToken(method, opt_param_types);
		}

		public int GetToken(SignatureHelper helper)
		{
			return mb.GetToken(helper);
		}
	}
}

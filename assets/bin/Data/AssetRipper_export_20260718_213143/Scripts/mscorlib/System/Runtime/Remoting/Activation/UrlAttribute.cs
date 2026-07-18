using System.Runtime.InteropServices;
using System.Runtime.Remoting.Contexts;

namespace System.Runtime.Remoting.Activation
{
	[Serializable]
	[ComVisible(true)]
	public sealed class UrlAttribute : ContextAttribute
	{
		private string url;

		public string UrlValue
		{
			get
			{
				return url;
			}
		}

		public UrlAttribute(string callsiteURL)
			: base(callsiteURL)
		{
			url = callsiteURL;
		}

		public override bool Equals(object o)
		{
			if (!(o is UrlAttribute))
			{
				return false;
			}
			return ((UrlAttribute)o).UrlValue == url;
		}

		public override int GetHashCode()
		{
			return url.GetHashCode();
		}

		[ComVisible(true)]
		public override void GetPropertiesForNewContext(IConstructionCallMessage ctorMsg)
		{
		}

		[ComVisible(true)]
		public override bool IsContextOK(Context ctx, IConstructionCallMessage msg)
		{
			return true;
		}
	}
}

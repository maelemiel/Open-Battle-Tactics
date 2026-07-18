using System.Collections;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Activation;

namespace System.Runtime.Remoting.Contexts
{
	[Serializable]
	[AttributeUsage(AttributeTargets.Class)]
	[ComVisible(true)]
	public class ContextAttribute : Attribute, IContextAttribute, IContextProperty
	{
		protected string AttributeName;

		public virtual string Name
		{
			get
			{
				return AttributeName;
			}
		}

		public ContextAttribute(string name)
		{
			AttributeName = name;
		}

		public override bool Equals(object o)
		{
			if (o == null)
			{
				return false;
			}
			if (!(o is ContextAttribute))
			{
				return false;
			}
			ContextAttribute contextAttribute = (ContextAttribute)o;
			if (contextAttribute.AttributeName != AttributeName)
			{
				return false;
			}
			return true;
		}

		public virtual void Freeze(Context newContext)
		{
		}

		public override int GetHashCode()
		{
			if (AttributeName == null)
			{
				return 0;
			}
			return AttributeName.GetHashCode();
		}

		public virtual void GetPropertiesForNewContext(IConstructionCallMessage ctorMsg)
		{
			if (ctorMsg == null)
			{
				throw new ArgumentNullException("ctorMsg");
			}
			IList contextProperties = ctorMsg.ContextProperties;
			contextProperties.Add(this);
		}

		public virtual bool IsContextOK(Context ctx, IConstructionCallMessage ctorMsg)
		{
			if (ctorMsg == null)
			{
				throw new ArgumentNullException("ctorMsg");
			}
			if (ctx == null)
			{
				throw new ArgumentNullException("ctx");
			}
			if (!ctorMsg.ActivationType.IsContextful)
			{
				return true;
			}
			IContextProperty property = ctx.GetProperty(AttributeName);
			if (property == null)
			{
				return false;
			}
			if (this != property)
			{
				return false;
			}
			return true;
		}

		public virtual bool IsNewContextOK(Context newCtx)
		{
			return true;
		}
	}
}

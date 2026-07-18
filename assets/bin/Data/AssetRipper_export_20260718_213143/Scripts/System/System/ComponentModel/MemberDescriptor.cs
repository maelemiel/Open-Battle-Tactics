using System.Collections;
using System.ComponentModel.Design;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;

namespace System.ComponentModel
{
	[ComVisible(true)]
	public abstract class MemberDescriptor
	{
		private class MemberDescriptorComparer : IComparer
		{
			public int Compare(object x, object y)
			{
				return string.Compare(((MemberDescriptor)x).Name, ((MemberDescriptor)y).Name, false, CultureInfo.InvariantCulture);
			}
		}

		private string name;

		private Attribute[] attrs;

		private AttributeCollection attrCollection;

		private static IComparer default_comparer;

		protected virtual Attribute[] AttributeArray
		{
			get
			{
				ArrayList arrayList = new ArrayList();
				if (attrs != null)
				{
					arrayList.AddRange(attrs);
				}
				FillAttributes(arrayList);
				Hashtable hashtable = new Hashtable();
				foreach (Attribute item in arrayList)
				{
					hashtable[item.TypeId] = item;
				}
				Attribute[] array = new Attribute[hashtable.Values.Count];
				hashtable.Values.CopyTo(array, 0);
				return array;
			}
			set
			{
				attrs = value;
			}
		}

		public virtual AttributeCollection Attributes
		{
			get
			{
				if (attrCollection == null)
				{
					attrCollection = CreateAttributeCollection();
				}
				return attrCollection;
			}
		}

		public virtual string Category
		{
			get
			{
				return ((CategoryAttribute)Attributes[typeof(CategoryAttribute)]).Category;
			}
		}

		public virtual string Description
		{
			get
			{
				Attribute[] attributeArray = AttributeArray;
				foreach (Attribute attribute in attributeArray)
				{
					if (attribute is DescriptionAttribute)
					{
						return ((DescriptionAttribute)attribute).Description;
					}
				}
				return string.Empty;
			}
		}

		public virtual bool DesignTimeOnly
		{
			get
			{
				Attribute[] attributeArray = AttributeArray;
				foreach (Attribute attribute in attributeArray)
				{
					if (attribute is DesignOnlyAttribute)
					{
						return ((DesignOnlyAttribute)attribute).IsDesignOnly;
					}
				}
				return false;
			}
		}

		public virtual string DisplayName
		{
			get
			{
				Attribute[] attributeArray = AttributeArray;
				foreach (Attribute attribute in attributeArray)
				{
					if (attribute is DisplayNameAttribute)
					{
						return ((DisplayNameAttribute)attribute).DisplayName;
					}
				}
				return name;
			}
		}

		public virtual string Name
		{
			get
			{
				return name;
			}
		}

		public virtual bool IsBrowsable
		{
			get
			{
				Attribute[] attributeArray = AttributeArray;
				foreach (Attribute attribute in attributeArray)
				{
					if (attribute is BrowsableAttribute)
					{
						return ((BrowsableAttribute)attribute).Browsable;
					}
				}
				return true;
			}
		}

		protected virtual int NameHashCode
		{
			get
			{
				return name.GetHashCode();
			}
		}

		internal static IComparer DefaultComparer
		{
			get
			{
				if (default_comparer == null)
				{
					default_comparer = new MemberDescriptorComparer();
				}
				return default_comparer;
			}
		}

		protected MemberDescriptor(string name, Attribute[] attrs)
		{
			this.name = name;
			this.attrs = attrs;
		}

		protected MemberDescriptor(MemberDescriptor reference, Attribute[] attrs)
		{
			name = reference.name;
			this.attrs = attrs;
		}

		protected MemberDescriptor(string name)
		{
			this.name = name;
		}

		protected MemberDescriptor(MemberDescriptor reference)
		{
			name = reference.name;
			attrs = reference.AttributeArray;
		}

		protected virtual void FillAttributes(IList attributeList)
		{
		}

		protected virtual AttributeCollection CreateAttributeCollection()
		{
			return new AttributeCollection(AttributeArray);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			MemberDescriptor memberDescriptor = obj as MemberDescriptor;
			if (memberDescriptor == null)
			{
				return false;
			}
			return memberDescriptor.name == name;
		}

		protected static ISite GetSite(object component)
		{
			if (component is Component)
			{
				return ((Component)component).Site;
			}
			return null;
		}

		[Obsolete("Use GetInvocationTarget")]
		protected static object GetInvokee(Type componentClass, object component)
		{
			if (component is IComponent)
			{
				ISite site = ((IComponent)component).Site;
				if (site != null && site.DesignMode)
				{
					IDesignerHost designerHost = site.GetService(typeof(IDesignerHost)) as IDesignerHost;
					if (designerHost != null)
					{
						IDesigner designer = designerHost.GetDesigner((IComponent)component);
						if (designer != null && componentClass.IsInstanceOfType(designer))
						{
							component = designer;
						}
					}
				}
			}
			return component;
		}

		protected virtual object GetInvocationTarget(Type type, object instance)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			return GetInvokee(type, instance);
		}

		protected static MethodInfo FindMethod(Type componentClass, string name, Type[] args, Type returnType)
		{
			return FindMethod(componentClass, name, args, returnType, true);
		}

		protected static MethodInfo FindMethod(Type componentClass, string name, Type[] args, Type returnType, bool publicOnly)
		{
			BindingFlags bindingAttr = ((!publicOnly) ? (BindingFlags.Public | BindingFlags.NonPublic) : BindingFlags.Public);
			return componentClass.GetMethod(name, bindingAttr, null, CallingConventions.Any, args, null);
		}
	}
}

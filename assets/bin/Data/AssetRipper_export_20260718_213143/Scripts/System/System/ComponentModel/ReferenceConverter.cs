using System.Collections;
using System.ComponentModel.Design;
using System.Globalization;

namespace System.ComponentModel
{
	public class ReferenceConverter : TypeConverter
	{
		private Type reference_type;

		public ReferenceConverter(Type type)
		{
			reference_type = type;
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (context != null && sourceType == typeof(string))
			{
				return true;
			}
			return base.CanConvertFrom(context, sourceType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (!(value is string))
			{
				return base.ConvertFrom(context, culture, value);
			}
			if (context != null)
			{
				object obj = null;
				IReferenceService referenceService = context.GetService(typeof(IReferenceService)) as IReferenceService;
				if (referenceService != null)
				{
					obj = referenceService.GetReference((string)value);
				}
				if (obj == null && context.Container != null && context.Container.Components != null)
				{
					obj = context.Container.Components[(string)value];
				}
				return obj;
			}
			return null;
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType != typeof(string))
			{
				return base.ConvertTo(context, culture, value, destinationType);
			}
			if (value == null)
			{
				return "(none)";
			}
			string text = string.Empty;
			if (context != null)
			{
				IReferenceService referenceService = context.GetService(typeof(IReferenceService)) as IReferenceService;
				if (referenceService != null)
				{
					text = referenceService.GetName(value);
				}
				if ((text == null || text.Length == 0) && value is IComponent)
				{
					IComponent component = (IComponent)value;
					if (component.Site != null && component.Site.Name != null)
					{
						text = component.Site.Name;
					}
				}
			}
			return text;
		}

		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			ArrayList arrayList = new ArrayList();
			if (context != null)
			{
				IReferenceService referenceService = context.GetService(typeof(IReferenceService)) as IReferenceService;
				if (referenceService != null)
				{
					object[] references = referenceService.GetReferences(reference_type);
					foreach (object value in references)
					{
						if (IsValueAllowed(context, value))
						{
							arrayList.Add(value);
						}
					}
				}
				else if (context.Container != null && context.Container.Components != null)
				{
					foreach (object component in context.Container.Components)
					{
						if (component != null && IsValueAllowed(context, component) && reference_type.IsInstanceOfType(component))
						{
							arrayList.Add(component);
						}
					}
				}
				arrayList.Add(null);
			}
			return new StandardValuesCollection(arrayList);
		}

		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			return true;
		}

		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		protected virtual bool IsValueAllowed(ITypeDescriptorContext context, object value)
		{
			return true;
		}
	}
}

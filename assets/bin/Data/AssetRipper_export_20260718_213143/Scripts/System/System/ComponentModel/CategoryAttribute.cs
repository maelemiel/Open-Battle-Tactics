namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.All)]
	public class CategoryAttribute : Attribute
	{
		private string category;

		private bool IsLocalized;

		private static volatile CategoryAttribute action;

		private static volatile CategoryAttribute appearance;

		private static volatile CategoryAttribute behaviour;

		private static volatile CategoryAttribute data;

		private static volatile CategoryAttribute def;

		private static volatile CategoryAttribute design;

		private static volatile CategoryAttribute drag_drop;

		private static volatile CategoryAttribute focus;

		private static volatile CategoryAttribute format;

		private static volatile CategoryAttribute key;

		private static volatile CategoryAttribute layout;

		private static volatile CategoryAttribute mouse;

		private static volatile CategoryAttribute window_style;

		private static volatile CategoryAttribute async;

		private static object lockobj = new object();

		public static CategoryAttribute Action
		{
			get
			{
				if (action != null)
				{
					return action;
				}
				lock (lockobj)
				{
					if (action == null)
					{
						action = new CategoryAttribute("Action");
					}
				}
				return action;
			}
		}

		public static CategoryAttribute Appearance
		{
			get
			{
				if (appearance != null)
				{
					return appearance;
				}
				lock (lockobj)
				{
					if (appearance == null)
					{
						appearance = new CategoryAttribute("Appearance");
					}
				}
				return appearance;
			}
		}

		public static CategoryAttribute Asynchronous
		{
			get
			{
				if (behaviour != null)
				{
					return behaviour;
				}
				lock (lockobj)
				{
					if (async == null)
					{
						async = new CategoryAttribute("Asynchronous");
					}
				}
				return async;
			}
		}

		public static CategoryAttribute Behavior
		{
			get
			{
				if (behaviour != null)
				{
					return behaviour;
				}
				lock (lockobj)
				{
					if (behaviour == null)
					{
						behaviour = new CategoryAttribute("Behavior");
					}
				}
				return behaviour;
			}
		}

		public static CategoryAttribute Data
		{
			get
			{
				if (data != null)
				{
					return data;
				}
				lock (lockobj)
				{
					if (data == null)
					{
						data = new CategoryAttribute("Data");
					}
				}
				return data;
			}
		}

		public static CategoryAttribute Default
		{
			get
			{
				if (def != null)
				{
					return def;
				}
				lock (lockobj)
				{
					if (def == null)
					{
						def = new CategoryAttribute("Default");
					}
				}
				return def;
			}
		}

		public static CategoryAttribute Design
		{
			get
			{
				if (design != null)
				{
					return design;
				}
				lock (lockobj)
				{
					if (design == null)
					{
						design = new CategoryAttribute("Design");
					}
				}
				return design;
			}
		}

		public static CategoryAttribute DragDrop
		{
			get
			{
				if (drag_drop != null)
				{
					return drag_drop;
				}
				lock (lockobj)
				{
					if (drag_drop == null)
					{
						drag_drop = new CategoryAttribute("DragDrop");
					}
				}
				return drag_drop;
			}
		}

		public static CategoryAttribute Focus
		{
			get
			{
				if (focus != null)
				{
					return focus;
				}
				lock (lockobj)
				{
					if (focus == null)
					{
						focus = new CategoryAttribute("Focus");
					}
				}
				return focus;
			}
		}

		public static CategoryAttribute Format
		{
			get
			{
				if (format != null)
				{
					return format;
				}
				lock (lockobj)
				{
					if (format == null)
					{
						format = new CategoryAttribute("Format");
					}
				}
				return format;
			}
		}

		public static CategoryAttribute Key
		{
			get
			{
				if (key != null)
				{
					return key;
				}
				lock (lockobj)
				{
					if (key == null)
					{
						key = new CategoryAttribute("Key");
					}
				}
				return key;
			}
		}

		public static CategoryAttribute Layout
		{
			get
			{
				if (layout != null)
				{
					return layout;
				}
				lock (lockobj)
				{
					if (layout == null)
					{
						layout = new CategoryAttribute("Layout");
					}
				}
				return layout;
			}
		}

		public static CategoryAttribute Mouse
		{
			get
			{
				if (mouse != null)
				{
					return mouse;
				}
				lock (lockobj)
				{
					if (mouse == null)
					{
						mouse = new CategoryAttribute("Mouse");
					}
				}
				return mouse;
			}
		}

		public static CategoryAttribute WindowStyle
		{
			get
			{
				if (window_style != null)
				{
					return window_style;
				}
				lock (lockobj)
				{
					if (window_style == null)
					{
						window_style = new CategoryAttribute("WindowStyle");
					}
				}
				return window_style;
			}
		}

		public string Category
		{
			get
			{
				if (!IsLocalized)
				{
					IsLocalized = true;
					string localizedString = GetLocalizedString(category);
					if (localizedString != null)
					{
						category = localizedString;
					}
				}
				return category;
			}
		}

		public CategoryAttribute()
		{
			category = "Misc";
		}

		public CategoryAttribute(string category)
		{
			this.category = category;
		}

		protected virtual string GetLocalizedString(string value)
		{
			return Locale.GetText(value);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is CategoryAttribute))
			{
				return false;
			}
			if (obj == this)
			{
				return true;
			}
			return ((CategoryAttribute)obj).Category == category;
		}

		public override int GetHashCode()
		{
			return category.GetHashCode();
		}

		public override bool IsDefaultAttribute()
		{
			return category == Default.Category;
		}
	}
}

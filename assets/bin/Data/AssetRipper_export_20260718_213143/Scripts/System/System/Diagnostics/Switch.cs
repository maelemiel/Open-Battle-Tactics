using System.Collections;
using System.Collections.Specialized;

namespace System.Diagnostics
{
	public abstract class Switch
	{
		private string name;

		private string description;

		private int switchSetting;

		private string value;

		private string defaultSwitchValue;

		private bool initialized;

		private StringDictionary attributes = new StringDictionary();

		public string Description
		{
			get
			{
				return description;
			}
		}

		public string DisplayName
		{
			get
			{
				return name;
			}
		}

		protected int SwitchSetting
		{
			get
			{
				if (!initialized)
				{
					initialized = true;
					GetConfigFileSetting();
					OnSwitchSettingChanged();
				}
				return switchSetting;
			}
			set
			{
				if (switchSetting != value)
				{
					switchSetting = value;
					OnSwitchSettingChanged();
				}
				initialized = true;
			}
		}

		public StringDictionary Attributes
		{
			get
			{
				return attributes;
			}
		}

		protected string Value
		{
			get
			{
				return value;
			}
			set
			{
				this.value = value;
				OnValueChanged();
			}
		}

		protected Switch(string displayName, string description)
		{
			name = displayName;
			this.description = description;
		}

		protected Switch(string displayName, string description, string defaultSwitchValue)
			: this(displayName, description)
		{
			this.defaultSwitchValue = defaultSwitchValue;
		}

		protected internal virtual string[] GetSupportedAttributes()
		{
			return null;
		}

		protected virtual void OnValueChanged()
		{
		}

		private void GetConfigFileSetting()
		{
			IDictionary dictionary = null;
			if (dictionary != null && dictionary.Contains(name))
			{
				switchSetting = (int)dictionary[name];
			}
			else if (defaultSwitchValue != null)
			{
				value = defaultSwitchValue;
				OnValueChanged();
			}
		}

		protected virtual void OnSwitchSettingChanged()
		{
		}
	}
}

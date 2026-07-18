using System.Collections;
using System.Runtime.InteropServices;

namespace System.ComponentModel.Design
{
	[ComVisible(true)]
	public class MenuCommand
	{
		private EventHandler handler;

		private CommandID command;

		private bool ischecked;

		private bool enabled = true;

		private bool issupported = true;

		private bool visible = true;

		private Hashtable properties;

		public virtual bool Checked
		{
			get
			{
				return ischecked;
			}
			set
			{
				if (ischecked != value)
				{
					ischecked = value;
					OnCommandChanged(EventArgs.Empty);
				}
			}
		}

		public virtual CommandID CommandID
		{
			get
			{
				return command;
			}
		}

		public virtual bool Enabled
		{
			get
			{
				return enabled;
			}
			set
			{
				if (enabled != value)
				{
					enabled = value;
					OnCommandChanged(EventArgs.Empty);
				}
			}
		}

		[System.MonoTODO]
		public virtual int OleStatus
		{
			get
			{
				return 3;
			}
		}

		public virtual IDictionary Properties
		{
			get
			{
				if (properties == null)
				{
					properties = new Hashtable();
				}
				return properties;
			}
		}

		public virtual bool Supported
		{
			get
			{
				return issupported;
			}
			set
			{
				issupported = value;
			}
		}

		public virtual bool Visible
		{
			get
			{
				return visible;
			}
			set
			{
				visible = value;
			}
		}

		public event EventHandler CommandChanged;

		public MenuCommand(EventHandler handler, CommandID command)
		{
			this.handler = handler;
			this.command = command;
		}

		public virtual void Invoke()
		{
			if (handler != null)
			{
				handler(this, EventArgs.Empty);
			}
		}

		public virtual void Invoke(object arg)
		{
			Invoke();
		}

		protected virtual void OnCommandChanged(EventArgs e)
		{
			if (this.CommandChanged != null)
			{
				this.CommandChanged(this, e);
			}
		}

		public override string ToString()
		{
			string text = string.Empty;
			if (command != null)
			{
				text = command.ToString();
			}
			text += " : ";
			if (Supported)
			{
				text += "Supported";
			}
			if (Enabled)
			{
				text += "|Enabled";
			}
			if (Visible)
			{
				text += "|Visible";
			}
			if (Checked)
			{
				text += "|Checked";
			}
			return text;
		}
	}
}

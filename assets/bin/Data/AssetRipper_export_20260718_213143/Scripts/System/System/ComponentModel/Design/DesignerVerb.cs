using System.Runtime.InteropServices;

namespace System.ComponentModel.Design
{
	[ComVisible(true)]
	public class DesignerVerb : MenuCommand
	{
		private string text;

		private string description;

		public string Text
		{
			get
			{
				return text;
			}
		}

		public string Description
		{
			get
			{
				return description;
			}
			set
			{
				description = value;
			}
		}

		public DesignerVerb(string text, EventHandler handler)
			: this(text, handler, StandardCommands.VerbFirst)
		{
		}

		public DesignerVerb(string text, EventHandler handler, CommandID startCommandID)
			: base(handler, startCommandID)
		{
			this.text = text;
		}

		public override string ToString()
		{
			return text + " : " + base.ToString();
		}
	}
}

using System;
using Mobage;

namespace MobageEditor
{
	public abstract class ExperienceTabViewController : ViewController
	{
		public ExperienceViewController Experience;

		public JsonData TabOptions;

		public bool Opaque;

		public bool IsStarted { get; set; }

		public string Identifier { get; set; }

		public bool CanPreload { get; set; }

		public bool IsLoaded { get; set; }

		public JsonData Config { get; set; }

		public abstract string ComponentName { get; set; }

		public event EventHandler Started;

		public abstract void TabDidHide();

		public abstract void StartLoadIfNeeded();

		public abstract void ExperiencePresented();

		protected void Start()
		{
			if (this.Started != null)
			{
				this.Started(this, EventArgs.Empty);
				IsStarted = true;
			}
		}

		public abstract void TabDidShow();
	}
}

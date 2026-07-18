using System.Runtime.InteropServices;

namespace System.ComponentModel
{
	[ClassInterface(ClassInterfaceType.AutoDispatch)]
	[DesignerCategory("Component")]
	[ComVisible(true)]
	public class Component : MarshalByRefObject, IDisposable, IComponent
	{
		private EventHandlerList event_handlers;

		private ISite mySite;

		private object disposedEvent = new object();

		protected virtual bool CanRaiseEvents
		{
			get
			{
				return false;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public virtual ISite Site
		{
			get
			{
				return mySite;
			}
			set
			{
				mySite = value;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IContainer Container
		{
			get
			{
				if (mySite == null)
				{
					return null;
				}
				return mySite.Container;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		protected bool DesignMode
		{
			get
			{
				if (mySite == null)
				{
					return false;
				}
				return mySite.DesignMode;
			}
		}

		protected EventHandlerList Events
		{
			get
			{
				if (event_handlers == null)
				{
					event_handlers = new EventHandlerList();
				}
				return event_handlers;
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public event EventHandler Disposed
		{
			add
			{
				Events.AddHandler(disposedEvent, value);
			}
			remove
			{
				Events.RemoveHandler(disposedEvent, value);
			}
		}

		public Component()
		{
			event_handlers = null;
		}

		~Component()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool release_all)
		{
			if (release_all)
			{
				if (mySite != null && mySite.Container != null)
				{
					mySite.Container.Remove(this);
				}
				EventHandler eventHandler = (EventHandler)Events[disposedEvent];
				if (eventHandler != null)
				{
					eventHandler(this, EventArgs.Empty);
				}
			}
		}

		protected virtual object GetService(Type service)
		{
			if (mySite != null)
			{
				return mySite.GetService(service);
			}
			return null;
		}

		public override string ToString()
		{
			if (mySite == null)
			{
				return GetType().ToString();
			}
			return string.Format("{0} [{1}]", mySite.Name, GetType().ToString());
		}
	}
}

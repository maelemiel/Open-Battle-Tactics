using System.ComponentModel.Design;
using System.Runtime.InteropServices;

namespace System.ComponentModel
{
	[DesignerCategory("Component")]
	[ComVisible(true)]
	[Designer("System.Windows.Forms.Design.ComponentDocumentDesigner, System.Design, Version=2.0.5.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(IRootDesigner))]
	[TypeConverter(typeof(ComponentConverter))]
	public class MarshalByValueComponent : IDisposable, IServiceProvider, IComponent
	{
		private EventHandlerList eventList;

		private ISite mySite;

		private object disposedEvent = new object();

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual IContainer Container
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
		public virtual bool DesignMode
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

		protected EventHandlerList Events
		{
			get
			{
				if (eventList == null)
				{
					eventList = new EventHandlerList();
				}
				return eventList;
			}
		}

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

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		[System.MonoTODO]
		protected virtual void Dispose(bool disposing)
		{
			if (!disposing)
			{
			}
		}

		~MarshalByValueComponent()
		{
			Dispose(false);
		}

		public virtual object GetService(Type service)
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

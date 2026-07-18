using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[Serializable]
	[ComVisible(true)]
	public class WeakReference : ISerializable
	{
		private bool isLongReference;

		private GCHandle gcHandle;

		public virtual bool IsAlive
		{
			get
			{
				return Target != null;
			}
		}

		public virtual object Target
		{
			get
			{
				return gcHandle.Target;
			}
			set
			{
				gcHandle.Target = value;
			}
		}

		public virtual bool TrackResurrection
		{
			get
			{
				return isLongReference;
			}
		}

		protected WeakReference()
		{
		}

		public WeakReference(object target)
			: this(target, false)
		{
		}

		public WeakReference(object target, bool trackResurrection)
		{
			isLongReference = trackResurrection;
			AllocateHandle(target);
		}

		protected WeakReference(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			isLongReference = info.GetBoolean("TrackResurrection");
			object value = info.GetValue("TrackedObject", typeof(object));
			AllocateHandle(value);
		}

		private void AllocateHandle(object target)
		{
			if (isLongReference)
			{
				gcHandle = GCHandle.Alloc(target, GCHandleType.WeakTrackResurrection);
			}
			else
			{
				gcHandle = GCHandle.Alloc(target, GCHandleType.Weak);
			}
		}

		~WeakReference()
		{
			gcHandle.Free();
		}

		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			info.AddValue("TrackResurrection", TrackResurrection);
			try
			{
				info.AddValue("TrackedObject", Target);
			}
			catch (Exception)
			{
				info.AddValue("TrackedObject", null);
			}
		}
	}
}

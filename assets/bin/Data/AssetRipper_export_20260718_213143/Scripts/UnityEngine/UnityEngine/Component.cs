using System;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;
using UnityEngineInternal;

namespace UnityEngine
{
	public class Component : Object
	{
		public Transform transform
		{
			get
			{
				return InternalGetTransform();
			}
		}

		public extern Rigidbody rigidbody
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern Rigidbody2D rigidbody2D
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern Camera camera
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern Light light
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern Animation animation
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern ConstantForce constantForce
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern Renderer renderer
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern AudioSource audio
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern GUIText guiText
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern NetworkView networkView
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		[Obsolete("Please use guiTexture instead")]
		public extern GUIElement guiElement
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern GUITexture guiTexture
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern Collider collider
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern Collider2D collider2D
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern HingeJoint hingeJoint
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern ParticleEmitter particleEmitter
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern ParticleSystem particleSystem
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public GameObject gameObject
		{
			get
			{
				return InternalGetGameObject();
			}
		}

		[Obsolete("the active property is deprecated on components. Please use gameObject.active instead. If you meant to enable / disable a single component use enabled instead.")]
		public extern bool active
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern string tag
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		internal extern Transform InternalGetTransform();

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		internal extern GameObject InternalGetGameObject();

		[MethodImpl(MethodImplOptions.InternalCall)]
		[TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
		[WrapperlessIcall]
		public extern Component GetComponent(Type type);

		public T GetComponent<T>() where T : Component
		{
			return GetComponent(typeof(T)) as T;
		}

		public Component GetComponent(string type)
		{
			return gameObject.GetComponent(type);
		}

		[TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
		public Component GetComponentInChildren(Type t)
		{
			return gameObject.GetComponentInChildren(t);
		}

		public T GetComponentInChildren<T>() where T : Component
		{
			return (T)GetComponentInChildren(typeof(T));
		}

		[ExcludeFromDocs]
		public Component[] GetComponentsInChildren(Type t)
		{
			bool includeInactive = false;
			return GetComponentsInChildren(t, includeInactive);
		}

		public Component[] GetComponentsInChildren(Type t, [DefaultValue("false")] bool includeInactive)
		{
			return gameObject.GetComponentsInChildren(t, includeInactive);
		}

		public T[] GetComponentsInChildren<T>(bool includeInactive) where T : Component
		{
			return gameObject.GetComponentsInChildren<T>(includeInactive);
		}

		public T[] GetComponentsInChildren<T>() where T : Component
		{
			return GetComponentsInChildren<T>(false);
		}

		[TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
		public Component GetComponentInParent(Type t)
		{
			return gameObject.GetComponentInParent(t);
		}

		public T GetComponentInParent<T>() where T : Component
		{
			return (T)GetComponentInParent(typeof(T));
		}

		[ExcludeFromDocs]
		public Component[] GetComponentsInParent(Type t)
		{
			bool includeInactive = false;
			return GetComponentsInParent(t, includeInactive);
		}

		public Component[] GetComponentsInParent(Type t, [DefaultValue("false")] bool includeInactive)
		{
			return gameObject.GetComponentsInParent(t, includeInactive);
		}

		public T[] GetComponentsInParent<T>(bool includeInactive) where T : Component
		{
			return gameObject.GetComponentsInParent<T>(includeInactive);
		}

		public T[] GetComponentsInParent<T>() where T : Component
		{
			return GetComponentsInParent<T>(false);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern Component[] GetComponents(Type type);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern Component[] GetComponentsWithCorrectReturnType(Type type);

		public T[] GetComponents<T>() where T : Component
		{
			return (T[])GetComponentsWithCorrectReturnType(typeof(T));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern bool CompareTag(string tag);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern void SendMessageUpwards(string methodName, [DefaultValue("null")] object value, [DefaultValue("SendMessageOptions.RequireReceiver")] SendMessageOptions options);

		[ExcludeFromDocs]
		public void SendMessageUpwards(string methodName, object value)
		{
			SendMessageOptions options = SendMessageOptions.RequireReceiver;
			SendMessageUpwards(methodName, value, options);
		}

		[ExcludeFromDocs]
		public void SendMessageUpwards(string methodName)
		{
			SendMessageOptions options = SendMessageOptions.RequireReceiver;
			object value = null;
			SendMessageUpwards(methodName, value, options);
		}

		public void SendMessageUpwards(string methodName, SendMessageOptions options)
		{
			SendMessageUpwards(methodName, null, options);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern void SendMessage(string methodName, [DefaultValue("null")] object value, [DefaultValue("SendMessageOptions.RequireReceiver")] SendMessageOptions options);

		[ExcludeFromDocs]
		public void SendMessage(string methodName, object value)
		{
			SendMessageOptions options = SendMessageOptions.RequireReceiver;
			SendMessage(methodName, value, options);
		}

		[ExcludeFromDocs]
		public void SendMessage(string methodName)
		{
			SendMessageOptions options = SendMessageOptions.RequireReceiver;
			object value = null;
			SendMessage(methodName, value, options);
		}

		public void SendMessage(string methodName, SendMessageOptions options)
		{
			SendMessage(methodName, null, options);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern void BroadcastMessage(string methodName, [DefaultValue("null")] object parameter, [DefaultValue("SendMessageOptions.RequireReceiver")] SendMessageOptions options);

		[ExcludeFromDocs]
		public void BroadcastMessage(string methodName, object parameter)
		{
			SendMessageOptions options = SendMessageOptions.RequireReceiver;
			BroadcastMessage(methodName, parameter, options);
		}

		[ExcludeFromDocs]
		public void BroadcastMessage(string methodName)
		{
			SendMessageOptions options = SendMessageOptions.RequireReceiver;
			object parameter = null;
			BroadcastMessage(methodName, parameter, options);
		}

		public void BroadcastMessage(string methodName, SendMessageOptions options)
		{
			BroadcastMessage(methodName, null, options);
		}
	}
}

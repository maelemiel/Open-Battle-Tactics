using System;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;
using UnityEngineInternal;

namespace UnityEngine
{
	public sealed class GameObject : Object
	{
		public extern bool isStatic
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		internal extern bool isStaticBatchable
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern Transform transform
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
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

		public extern int layer
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		[Obsolete("GameObject.active is obsolete. Use GameObject.SetActive(), GameObject.activeSelf or GameObject.activeInHierarchy.")]
		public extern bool active
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern bool activeSelf
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern bool activeInHierarchy
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
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

		public GameObject gameObject
		{
			get
			{
				return this;
			}
		}

		public GameObject(string name)
		{
			Internal_CreateGameObject(this, name);
		}

		public GameObject()
		{
			Internal_CreateGameObject(this, null);
		}

		public GameObject(string name, params Type[] components)
		{
			Internal_CreateGameObject(this, name);
			foreach (Type componentType in components)
			{
				AddComponent(componentType);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern void SampleAnimation(AnimationClip animation, float time);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern GameObject CreatePrimitive(PrimitiveType type);

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
			return GetComponentByName(type);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern Component GetComponentByName(string type);

		[TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
		public Component GetComponentInChildren(Type type)
		{
			if (activeInHierarchy)
			{
				Component component = GetComponent(type);
				if (component != null)
				{
					return component;
				}
			}
			Transform transform = this.transform;
			if (transform != null)
			{
				foreach (Transform item in transform)
				{
					Component componentInChildren = item.gameObject.GetComponentInChildren(type);
					if (componentInChildren != null)
					{
						return componentInChildren;
					}
				}
			}
			return null;
		}

		public T GetComponentInChildren<T>() where T : Component
		{
			return GetComponentInChildren(typeof(T)) as T;
		}

		[TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
		public Component GetComponentInParent(Type type)
		{
			if (activeInHierarchy)
			{
				Component component = GetComponent(type);
				if (component != null)
				{
					return component;
				}
			}
			Transform parent = transform.parent;
			if (parent != null)
			{
				while (parent != null)
				{
					if (parent.gameObject.activeInHierarchy)
					{
						Component component2 = parent.gameObject.GetComponent(type);
						if (component2 != null)
						{
							return component2;
						}
					}
					parent = parent.parent;
				}
			}
			return null;
		}

		public T GetComponentInParent<T>() where T : Component
		{
			return GetComponentInParent(typeof(T)) as T;
		}

		[CanConvertToFlash]
		public Component[] GetComponents(Type type)
		{
			return GetComponentsInternal(type, false, false, true, false);
		}

		public T[] GetComponents<T>() where T : Component
		{
			return (T[])GetComponentsInternal(typeof(T), true, false, true, false);
		}

		[ExcludeFromDocs]
		public Component[] GetComponentsInChildren(Type type)
		{
			bool includeInactive = false;
			return GetComponentsInChildren(type, includeInactive);
		}

		public Component[] GetComponentsInChildren(Type type, [DefaultValue("false")] bool includeInactive)
		{
			return GetComponentsInternal(type, false, true, includeInactive, false);
		}

		public T[] GetComponentsInChildren<T>(bool includeInactive) where T : Component
		{
			return (T[])GetComponentsInternal(typeof(T), true, true, includeInactive, false);
		}

		public T[] GetComponentsInChildren<T>() where T : Component
		{
			return GetComponentsInChildren<T>(false);
		}

		[ExcludeFromDocs]
		public Component[] GetComponentsInParent(Type type)
		{
			bool includeInactive = false;
			return GetComponentsInParent(type, includeInactive);
		}

		public Component[] GetComponentsInParent(Type type, [DefaultValue("false")] bool includeInactive)
		{
			return GetComponentsInternal(type, false, true, includeInactive, true);
		}

		public T[] GetComponentsInParent<T>(bool includeInactive) where T : Component
		{
			return (T[])GetComponentsInternal(typeof(T), true, true, includeInactive, true);
		}

		public T[] GetComponentsInParent<T>() where T : Component
		{
			return GetComponentsInParent<T>(false);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern Component[] GetComponentsInternal(Type type, bool isGenericTypeArray, bool recursive, bool includeInactive, bool reverse);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern void SetActive(bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[Obsolete("gameObject.SetActiveRecursively() is obsolete. Use GameObject.SetActive(), which is now inherited by children.")]
		[WrapperlessIcall]
		public extern void SetActiveRecursively(bool state);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern bool CompareTag(string tag);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern GameObject FindGameObjectWithTag(string tag);

		public static GameObject FindWithTag(string tag)
		{
			return FindGameObjectWithTag(tag);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern GameObject[] FindGameObjectsWithTag(string tag);

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

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern Component AddComponent(string className);

		[TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
		public Component AddComponent(Type componentType)
		{
			return Internal_AddComponentWithType(componentType);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern Component Internal_AddComponentWithType(Type componentType);

		public T AddComponent<T>() where T : Component
		{
			return AddComponent(typeof(T)) as T;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void Internal_CreateGameObject([Writable] GameObject mono, string name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		[Obsolete("gameObject.PlayAnimation is not supported anymore. Use animation.Play")]
		public extern void PlayAnimation(AnimationClip animation);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		[Obsolete("gameObject.StopAnimation is not supported anymore. Use animation.Stop")]
		public extern void StopAnimation();

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern GameObject Find(string name);
	}
}

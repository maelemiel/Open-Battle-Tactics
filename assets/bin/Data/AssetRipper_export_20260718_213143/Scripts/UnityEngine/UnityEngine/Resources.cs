using System;
using System.Runtime.CompilerServices;
using UnityEngineInternal;

namespace UnityEngine
{
	public sealed class Resources
	{
		internal static T[] ConvertObjects<T>(Object[] rawObjects) where T : Object
		{
			if (rawObjects == null)
			{
				return null;
			}
			T[] array = new T[rawObjects.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = (T)rawObjects[i];
			}
			return array;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		[TypeInferenceRule(TypeInferenceRules.ArrayOfTypeReferencedByFirstArgument)]
		public static extern Object[] FindObjectsOfTypeAll(Type type);

		public static T[] FindObjectsOfTypeAll<T>() where T : Object
		{
			return ConvertObjects<T>(FindObjectsOfTypeAll(typeof(T)));
		}

		public static Object Load(string path)
		{
			return Load(path, typeof(Object));
		}

		public static T Load<T>(string path) where T : Object
		{
			return (T)Load(path, typeof(T));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[TypeInferenceRule(TypeInferenceRules.TypeReferencedBySecondArgument)]
		[WrapperlessIcall]
		public static extern Object Load(string path, Type systemTypeInstance);

		public static ResourceRequest LoadAsync(string path)
		{
			return LoadAsync(path, typeof(Object));
		}

		public static ResourceRequest LoadAsync<T>(string path) where T : Object
		{
			return LoadAsync(path, typeof(T));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern ResourceRequest LoadAsync(string path, Type type);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern Object[] LoadAll(string path, Type systemTypeInstance);

		public static Object[] LoadAll(string path)
		{
			return LoadAll(path, typeof(Object));
		}

		public static T[] LoadAll<T>(string path) where T : Object
		{
			return ConvertObjects<T>(LoadAll(path, typeof(T)));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		[TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
		public static extern Object GetBuiltinResource(Type type, string path);

		public static T GetBuiltinResource<T>(string path) where T : Object
		{
			return (T)GetBuiltinResource(typeof(T), path);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		[TypeInferenceRule(TypeInferenceRules.TypeReferencedBySecondArgument)]
		public static extern Object LoadAssetAtPath(string assetPath, Type type);

		public static T LoadAssetAtPath<T>(string assetPath) where T : Object
		{
			return (T)LoadAssetAtPath(assetPath, typeof(T));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern void UnloadAsset(Object assetToUnload);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern AsyncOperation UnloadUnusedAssets();
	}
}

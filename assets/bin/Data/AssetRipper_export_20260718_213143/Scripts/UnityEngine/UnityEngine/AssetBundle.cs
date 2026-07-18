using System;
using System.Runtime.CompilerServices;
using UnityEngineInternal;

namespace UnityEngine
{
	public sealed class AssetBundle : Object
	{
		public extern Object mainAsset
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern AssetBundleCreateRequest CreateFromMemory(byte[] binary);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern AssetBundle CreateFromMemoryImmediate(byte[] binary);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern AssetBundle CreateFromFile(string path);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern bool Contains(string name);

		public Object Load(string name)
		{
			return Load(name, typeof(Object));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		[TypeInferenceRule(TypeInferenceRules.TypeReferencedBySecondArgument)]
		public extern Object Load(string name, Type type);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern AssetBundleRequest LoadAsync(string name, Type type);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern Object[] LoadAll(Type type);

		public Object[] LoadAll()
		{
			return LoadAll(typeof(Object));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern void Unload(bool unloadAllLoadedObjects);
	}
}

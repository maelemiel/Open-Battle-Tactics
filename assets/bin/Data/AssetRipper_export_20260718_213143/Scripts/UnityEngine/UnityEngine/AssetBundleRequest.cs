using System;
using System.Runtime.InteropServices;

namespace UnityEngine
{
	[StructLayout(LayoutKind.Sequential)]
	public sealed class AssetBundleRequest : AsyncOperation
	{
		internal AssetBundle m_AssetBundle;

		internal string m_Path;

		internal Type m_Type;

		public Object asset
		{
			get
			{
				return m_AssetBundle.Load(m_Path, m_Type);
			}
		}
	}
}

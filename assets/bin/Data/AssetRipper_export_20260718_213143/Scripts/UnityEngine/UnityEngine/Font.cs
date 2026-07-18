using System.Runtime.CompilerServices;
using UnityEngine.Internal;

namespace UnityEngine
{
	public sealed class Font : Object
	{
		public delegate void FontTextureRebuildCallback();

		public extern Material material
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern string[] fontNames
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern CharacterInfo[] characterInfo
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public FontTextureRebuildCallback textureRebuildCallback
		{
			get
			{
				return this.m_FontTextureRebuildCallback;
			}
			set
			{
				this.m_FontTextureRebuildCallback = value;
			}
		}

		public extern bool dynamic
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		private event FontTextureRebuildCallback m_FontTextureRebuildCallback;

		public Font()
		{
			Internal_CreateFont(this, null);
		}

		public Font(string name)
		{
			Internal_CreateFont(this, name);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void Internal_CreateFont([Writable] Font _font, string name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern bool HasCharacter(char c);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern void RequestCharactersInTexture(string characters, [DefaultValue("0")] int size, [DefaultValue("FontStyle.Normal")] FontStyle style);

		[ExcludeFromDocs]
		public void RequestCharactersInTexture(string characters, int size)
		{
			FontStyle style = FontStyle.Normal;
			RequestCharactersInTexture(characters, size, style);
		}

		[ExcludeFromDocs]
		public void RequestCharactersInTexture(string characters)
		{
			FontStyle style = FontStyle.Normal;
			int size = 0;
			RequestCharactersInTexture(characters, size, style);
		}

		private void InvokeFontTextureRebuildCallback_Internal()
		{
			if (this.m_FontTextureRebuildCallback != null)
			{
				this.m_FontTextureRebuildCallback();
			}
		}

		public static int GetMaxVertsForString(string str)
		{
			return str.Length * 4 + 4;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern bool GetCharacterInfo(char ch, out CharacterInfo info, [DefaultValue("0")] int size, [DefaultValue("FontStyle.Normal")] FontStyle style);

		[ExcludeFromDocs]
		public bool GetCharacterInfo(char ch, out CharacterInfo info, int size)
		{
			FontStyle style = FontStyle.Normal;
			return GetCharacterInfo(ch, out info, size, style);
		}

		[ExcludeFromDocs]
		public bool GetCharacterInfo(char ch, out CharacterInfo info)
		{
			FontStyle style = FontStyle.Normal;
			int size = 0;
			return GetCharacterInfo(ch, out info, size, style);
		}
	}
}

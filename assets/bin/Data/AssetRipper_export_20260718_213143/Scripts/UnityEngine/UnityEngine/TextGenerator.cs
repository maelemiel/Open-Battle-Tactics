using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UnityEngine
{
	[StructLayout(LayoutKind.Sequential)]
	public sealed class TextGenerator : IDisposable
	{
		private class InternalArrayCache<T> : ICollection<T>, IList<T>, IEnumerable, IEnumerable<T>
		{
			private T[] m_Buffer;

			private int m_Size;

			public T[] buffer
			{
				get
				{
					return m_Buffer;
				}
			}

			public int Count
			{
				get
				{
					return m_Size;
				}
			}

			public int Capacity
			{
				get
				{
					return m_Buffer.Length;
				}
			}

			public bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public T this[int index]
			{
				get
				{
					if (index < 0 || index >= Count)
					{
						throw new IndexOutOfRangeException(UnityString.Format("Index {0} is out of bounds", index));
					}
					return m_Buffer[index];
				}
				set
				{
					if (index < 0 || index >= Count)
					{
						throw new IndexOutOfRangeException(UnityString.Format("Index {0} is out of bounds", index));
					}
					m_Buffer[index] = value;
				}
			}

			public InternalArrayCache(int initialCapacity)
			{
				m_Buffer = new T[initialCapacity];
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			private void IntegrityCheck()
			{
				if (m_Buffer.Length < m_Size)
				{
					throw new Exception("Internal array cache is invalid. Size of internal array is LESS then cached size");
				}
			}

			private void ResizeInternalArray(int newSize, bool copyOld)
			{
				if (newSize != m_Buffer.Length)
				{
					T[] destinationArray = new T[newSize];
					if (copyOld)
					{
						Array.Copy(m_Buffer, destinationArray, Math.Min(m_Buffer.Length, newSize));
					}
					m_Buffer = destinationArray;
					m_Size = Math.Min(m_Buffer.Length, m_Size);
					IntegrityCheck();
				}
			}

			private void Resize(int newSize, bool copyExisting)
			{
				Grow(newSize, copyExisting);
				m_Size = newSize;
			}

			public void Resize(int newSize)
			{
				Resize(newSize, true);
			}

			public void ResizeNoCopy(int newSize)
			{
				Resize(newSize, false);
			}

			public void SetCapacity(int capacity)
			{
				ResizeInternalArray(capacity, true);
			}

			public void SetCapacityNoCopy(int capacity)
			{
				ResizeInternalArray(capacity, false);
			}

			public void TrimExcess()
			{
				if (m_Size < m_Buffer.Length)
				{
					ResizeInternalArray(m_Size, true);
				}
			}

			private void Grow(int minSize, bool copyExisting)
			{
				if (minSize >= m_Buffer.Length)
				{
					int num;
					for (num = m_Buffer.Length; num < minSize; num *= 2)
					{
					}
					ResizeInternalArray(num, copyExisting);
				}
			}

			public void Grow(int minSize)
			{
				Grow(minSize, true);
			}

			public void GrowNoCopy(int minSize)
			{
				Grow(minSize, false);
			}

			public void SetData(IList<T> data)
			{
				Resize(data.Count);
				for (int i = 0; i < data.Count; i++)
				{
					m_Buffer[i] = data[i];
				}
				IntegrityCheck();
			}

			public IEnumerator<T> GetEnumerator()
			{
				for (int i = 0; i < m_Size; i++)
				{
					yield return m_Buffer[i];
				}
			}

			public bool Contains(T item)
			{
				return IndexOf(item) != -1;
			}

			public int IndexOf(T item)
			{
				int num = Array.IndexOf(m_Buffer, item);
				return (num != -1 && num <= m_Size) ? num : (-1);
			}

			public void CopyTo(T[] array, int arrayIndex)
			{
				for (int i = arrayIndex; i < Count; i++)
				{
					array[i] = buffer[i];
				}
			}

			public void Insert(int index, T item)
			{
				throw new NotSupportedException();
			}

			public bool Remove(T item)
			{
				throw new NotSupportedException();
			}

			public void RemoveAt(int index)
			{
				throw new NotSupportedException();
			}

			public void Add(T item)
			{
				throw new NotSupportedException();
			}

			public void Clear()
			{
				throw new NotSupportedException();
			}
		}

		internal IntPtr m_Ptr;

		private string m_LastString;

		private TextGenerationSettings m_LastSettings;

		private bool m_HasGenerated;

		private readonly InternalArrayCache<UIVertex> m_Verts;

		private readonly InternalArrayCache<UICharInfo> m_Characters;

		private readonly InternalArrayCache<UILineInfo> m_Lines;

		private bool m_CachedVerts;

		private bool m_CachedCharacters;

		private bool m_CachedLines;

		public IList<UIVertex> verts
		{
			get
			{
				if (!m_CachedVerts)
				{
					m_Verts.ResizeNoCopy(vertexCount);
					GetVerts(m_Verts.buffer);
					m_CachedVerts = true;
				}
				return m_Verts;
			}
		}

		public IList<UICharInfo> characters
		{
			get
			{
				if (!m_CachedCharacters)
				{
					m_Characters.ResizeNoCopy(characterCount);
					GetCharacters(m_Characters.buffer);
					m_CachedCharacters = true;
				}
				return m_Characters;
			}
		}

		public IList<UILineInfo> lines
		{
			get
			{
				if (!m_CachedLines)
				{
					m_Lines.ResizeNoCopy(lineCount);
					GetLines(m_Lines.buffer);
					m_CachedLines = true;
				}
				return m_Lines;
			}
		}

		public Vector2 extents
		{
			get
			{
				Rect rect = rectExtents;
				Vector2 result = default(Vector2);
				result.x = rect.width;
				result.y = rect.height;
				return result;
			}
		}

		public extern Rect rectExtents
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern int vertexCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern int characterCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern int lineCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public TextGenerator()
			: this(50)
		{
		}

		public TextGenerator(int initialCapacity)
		{
			m_Verts = new InternalArrayCache<UIVertex>((initialCapacity + 1) * 4);
			m_Characters = new InternalArrayCache<UICharInfo>(initialCapacity + 1);
			m_Lines = new InternalArrayCache<UILineInfo>(20);
			Init();
		}

		private TextGenerationSettings ValidatedSettings(TextGenerationSettings settings)
		{
			if (settings.font != null && settings.font.dynamic)
			{
				return settings;
			}
			if (settings.size != 0 || settings.style != FontStyle.Normal)
			{
				Debug.LogWarning("Font size and style overrides are only supported for dynamic fonts.");
				settings.size = 0;
				settings.style = FontStyle.Normal;
			}
			if (settings.wrapMode == TextWrapMode.GrowText || settings.wrapMode == TextWrapMode.ShrinkText || settings.wrapMode == TextWrapMode.BestFit)
			{
				Debug.LogWarning("Grow, Shrink, and BestFit wrap modes are only suppoerted for dynamic fonts.");
				settings.wrapMode = TextWrapMode.Wrap;
			}
			return settings;
		}

		public void Invalidate()
		{
			m_HasGenerated = false;
		}

		public bool Populate(string str, TextGenerationSettings settings)
		{
			if (m_HasGenerated && str == m_LastString && settings.Equals(m_LastSettings))
			{
				return false;
			}
			return PopulateAlways(str, settings);
		}

		private bool PopulateAlways(string str, TextGenerationSettings settings)
		{
			m_LastString = str;
			m_HasGenerated = true;
			m_CachedVerts = false;
			m_CachedCharacters = false;
			m_CachedLines = false;
			m_LastSettings = settings;
			TextGenerationSettings textGenerationSettings = ValidatedSettings(settings);
			return Populate_Internal(str, textGenerationSettings.font, textGenerationSettings.color, textGenerationSettings.size, textGenerationSettings.style, textGenerationSettings.richText, textGenerationSettings.wrapMode, textGenerationSettings.anchor, textGenerationSettings.extents, textGenerationSettings.pivot);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void Init();

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern void Dispose();

		~TextGenerator()
		{
			Dispose();
		}

		internal bool Populate_Internal(string str, Font font, Color color, int size, FontStyle style, bool richText, TextWrapMode wrapMode, TextAnchor anchor, Vector2 extents, Vector2 pivot)
		{
			return INTERNAL_CALL_Populate_Internal(this, str, font, ref color, size, style, richText, wrapMode, anchor, ref extents, ref pivot);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern bool INTERNAL_CALL_Populate_Internal(TextGenerator self, string str, Font font, ref Color color, int size, FontStyle style, bool richText, TextWrapMode wrapMode, TextAnchor anchor, ref Vector2 extents, ref Vector2 pivot);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern int GetVerts(UIVertex[] verts);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern int GetCharacters(UICharInfo[] characters);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern int GetLines(UILineInfo[] lines);
	}
}

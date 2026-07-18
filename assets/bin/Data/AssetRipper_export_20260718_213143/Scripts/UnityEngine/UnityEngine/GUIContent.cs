using System;
using System.Runtime.InteropServices;

namespace UnityEngine
{
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class GUIContent
	{
		[SerializeField]
		private string m_Text = string.Empty;

		[SerializeField]
		private Texture m_Image;

		[SerializeField]
		private string m_Tooltip = string.Empty;

		public static GUIContent none = new GUIContent(string.Empty);

		private static GUIContent s_Text = new GUIContent();

		private static GUIContent s_Image = new GUIContent();

		private static GUIContent s_TextImage = new GUIContent();

		public string text
		{
			get
			{
				return m_Text;
			}
			set
			{
				m_Text = value;
			}
		}

		public Texture image
		{
			get
			{
				return m_Image;
			}
			set
			{
				m_Image = value;
			}
		}

		public string tooltip
		{
			get
			{
				return m_Tooltip;
			}
			set
			{
				m_Tooltip = value;
			}
		}

		internal int hash
		{
			get
			{
				int result = 0;
				if (m_Text != null && m_Text != string.Empty)
				{
					result = m_Text.GetHashCode() * 37;
				}
				return result;
			}
		}

		public GUIContent()
		{
		}

		public GUIContent(string text)
		{
			m_Text = text;
		}

		public GUIContent(Texture image)
		{
			m_Image = image;
		}

		public GUIContent(string text, Texture image)
		{
			m_Text = text;
			m_Image = image;
		}

		public GUIContent(string text, string tooltip)
		{
			m_Text = text;
			m_Tooltip = tooltip;
		}

		public GUIContent(Texture image, string tooltip)
		{
			m_Image = image;
			m_Tooltip = tooltip;
		}

		public GUIContent(string text, Texture image, string tooltip)
		{
			m_Text = text;
			m_Image = image;
			m_Tooltip = tooltip;
		}

		public GUIContent(GUIContent src)
		{
			m_Text = src.m_Text;
			m_Image = src.m_Image;
			m_Tooltip = src.m_Tooltip;
		}

		internal static GUIContent Temp(string t)
		{
			s_Text.m_Text = t;
			return s_Text;
		}

		internal static GUIContent Temp(Texture i)
		{
			s_Image.m_Image = i;
			return s_Image;
		}

		internal static GUIContent Temp(string t, Texture i)
		{
			s_TextImage.m_Text = t;
			s_TextImage.m_Image = i;
			return s_TextImage;
		}

		internal static void ClearStaticCache()
		{
			s_Text.m_Text = null;
			s_Image.m_Image = null;
			s_TextImage.m_Text = null;
			s_TextImage.m_Image = null;
		}

		internal static GUIContent[] Temp(string[] texts)
		{
			GUIContent[] array = new GUIContent[texts.Length];
			for (int i = 0; i < texts.Length; i++)
			{
				array[i] = new GUIContent(texts[i]);
			}
			return array;
		}

		internal static GUIContent[] Temp(Texture[] images)
		{
			GUIContent[] array = new GUIContent[images.Length];
			for (int i = 0; i < images.Length; i++)
			{
				array[i] = new GUIContent(images[i]);
			}
			return array;
		}
	}
}

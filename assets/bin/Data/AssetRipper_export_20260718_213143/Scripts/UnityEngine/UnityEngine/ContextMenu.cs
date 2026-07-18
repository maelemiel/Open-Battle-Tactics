using System;

namespace UnityEngine
{
	public sealed class ContextMenu : Attribute
	{
		private string m_ItemName;

		public string menuItem
		{
			get
			{
				return m_ItemName;
			}
		}

		public ContextMenu(string name)
		{
			m_ItemName = name;
		}
	}
}

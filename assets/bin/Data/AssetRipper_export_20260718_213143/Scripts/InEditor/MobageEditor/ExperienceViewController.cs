using System.Collections.Generic;
using Mobage;
using UnityEngine;

namespace MobageEditor
{
	public abstract class ExperienceViewController : UIViewController
	{
		public string ExperienceName;

		public JsonData ExperienceOptions;

		public ExperienceTabViewController ActiveTab;

		public List<ExperienceTabViewController> Tabs = new List<ExperienceTabViewController>();

		public bool ShouldBeTransparent
		{
			get
			{
				return true;
			}
		}

		public void PresentTab(ExperienceTabViewController tab, JsonData options, bool animated)
		{
			Debug.Log("ExperienceViewController presentTab:" + tab.GetInstanceID());
			tab.TabOptions = options;
			if (ActiveTab != null)
			{
				ActiveTab.TabDidHide();
			}
			ActiveTab = tab;
			ActiveTab.Experience = this;
			ActiveTab.TabDidShow();
			if (ShouldBeTransparent && ActiveTab != null)
			{
				ActiveTab.Opaque = false;
			}
			if (ActiveTab != null && !Tabs.Contains(ActiveTab))
			{
				Tabs.Add(ActiveTab);
			}
		}

		protected void insertTab(ExperienceTabViewController tab, int index)
		{
			Tabs.Insert(index, tab);
		}

		public virtual ExperienceTabViewController TabForIdentifier(string tabIdentifier)
		{
			foreach (ExperienceTabViewController tab in Tabs)
			{
				if (tab.Identifier == tabIdentifier)
				{
					return tab;
				}
			}
			return null;
		}
	}
}

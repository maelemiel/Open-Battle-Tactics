using System;
using UnityEngine;

namespace MobageEditor
{
	public abstract class WebExperience : ExperienceViewController
	{
		private JsonData tabTable;

		private JsonData tabOrder;

		public WebExperience(JsonData dict)
		{
			Debug.Log("WebExperience " + dict.ToJson());
			tabTable = dict["tabs"];
			tabOrder = dict["tabOrder"];
			for (int i = 0; i < tabOrder.Count; i++)
			{
				ExperienceTabViewController tab = TabForIdentifier((string)tabOrder[i]);
				Debug.Log("tab:" + tab.GetInstanceID());
				if (tab.CanPreload)
				{
					Debug.Log("canPreload");
					tab.Started += delegate
					{
						tab.StartLoadIfNeeded();
					};
				}
				Debug.Log("inserting tab");
				insertTab(tab, i);
				Debug.Log("tab inserted");
			}
			Debug.Log("WebExperience created");
		}

		private void LoadView()
		{
			PreloadTabs();
		}

		private void PreloadTabs()
		{
			foreach (ExperienceTabViewController tab in Tabs)
			{
				if (tab.CanPreload && !tab.IsLoaded)
				{
					tab.StartLoadIfNeeded();
				}
			}
		}

		public void TabFinishedLoading(ExperienceTabViewController tab)
		{
			Debug.Log("WebExperience TabFinishedLoading:" + tab.GetInstanceID());
			tab.ExperiencePresented();
			if (ActiveTab == tab)
			{
				ActiveTab.TabDidShow();
			}
		}

		public override ExperienceTabViewController TabForIdentifier(string tabIdentifier)
		{
			ExperienceTabViewController experienceTabViewController = base.TabForIdentifier(tabIdentifier);
			if (experienceTabViewController != null)
			{
				return experienceTabViewController;
			}
			if (!tabTable.Contains(tabIdentifier))
			{
				return (ExperienceTabViewController)WebSitemap.Instance.ControllerForIdentifier(tabIdentifier);
			}
			JsonData jsonData = tabTable[tabIdentifier];
			experienceTabViewController = WebSitemap.Instance.ControllerForIdentifier(getIndexKey(jsonData, tabIdentifier)) as ExperienceTabViewController;
			experienceTabViewController.ComponentName = tabIdentifier;
			try
			{
				experienceTabViewController.CanPreload = (bool)jsonData["preload"];
			}
			catch
			{
			}
			return experienceTabViewController;
		}

		private string getIndexKey(JsonData tabConfig, string tabIdentifier)
		{
			try
			{
				return (string)tabConfig["indexKey"];
			}
			catch
			{
				return tabIdentifier;
			}
		}

		public abstract void DismissAndReturnArrayToNative(JsonData arr);

		public void DismissWindow(Action finalCallback)
		{
			MobageUI.Instance.DismissAllExperiencesAnimated(true, delegate
			{
				Debug.Log("JSRequested: dismissWindow");
				finalCallback();
			});
		}
	}
}

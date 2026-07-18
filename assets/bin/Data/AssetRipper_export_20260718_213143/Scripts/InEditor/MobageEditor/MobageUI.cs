using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Mobage;
using UnityEngine;

namespace MobageEditor
{
	public class MobageUI
	{
		public MobageUIHostView View = new MobageUIHostView();

		public NavigationViewController NavController = new NavigationViewController();

		private static MobageUI instance;

		public static MobageUI Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new MobageUI();
				}
				return instance;
			}
		}

		private MobageUI()
		{
		}

		public void PresentExperienceNamed(string experienceIdentifier, JsonData options)
		{
			PresentExperienceNamed(experienceIdentifier, options, null, null);
		}

		public void PresentExperienceNamed(string experienceIdentifier, JsonData experienceOptions, string tabIdentifier, JsonData tabOptions)
		{
			Action action = delegate
			{
				Debug.Log("presentBlock in Present Experience Named");
				Action<Error> action2 = delegate(Error err)
				{
					Debug.LogError(string.Format("Error loading experience named {0}, error: {1}", experienceIdentifier, err));
				};
				Error obj = new Error
				{
					domain = "com.mobage.error.api",
					code = 10002
				};
				ExperienceViewController experience = WebSitemap.Instance.ExperienceForIdentifier(experienceIdentifier);
				if (experience == null)
				{
					action2(obj);
				}
				else
				{
					experience.ExperienceName = experienceIdentifier;
					experience.ExperienceOptions = experienceOptions;
					if (!string.IsNullOrEmpty(tabIdentifier))
					{
						ExperienceTabViewController tab = experience.TabForIdentifier(tabIdentifier);
						if (tab != null)
						{
							tab.Started += delegate
							{
								experience.PresentTab(tab, tabOptions, false);
								if (experience.ActiveTab == null && experience.Tabs.Count > 0)
								{
									experience.PresentTab(experience.Tabs[0], tabOptions, false);
								}
								PresentExperience(experience, true, delegate
								{
								});
							};
							return;
						}
					}
					if (experience.ActiveTab == null && experience.Tabs.Count > 0)
					{
						experience.Tabs[0].Started += delegate
						{
							experience.PresentTab(experience.Tabs[0], tabOptions, false);
							PresentExperience(experience, true, delegate
							{
							});
						};
					}
					else
					{
						PresentExperience(experience, true, delegate
						{
						});
					}
				}
			};
			if (WebSitemap.Instance == null || WebSitemap.Instance.ExperienceForIdentifier(experienceIdentifier) == null)
			{
				Debug.Log("no experienceIdentifier");
				UpdateSitemapWithCallback(action);
			}
			else
			{
				Debug.Log("has experienceIdentifier");
				action();
			}
		}

		private void ShowWindow()
		{
			Mobage.MobageUIVisiblePost(true);
		}

		public void PresentExperience(ExperienceViewController viewControllerToPresent, bool animated, Action completion)
		{
			Action action = delegate
			{
				Debug.Log("PresentViewController, MobageStyle!");
				ShowWindow();
				NavController.PushViewController(viewControllerToPresent, false, delegate
				{
					Debug.Log("animateContentIn");
					View.AnimateContentIn(completion);
				});
			};
			action();
		}

		public void UpdateSitemapWithCallback(Action onComplete)
		{
			UpdateSitemap(null, onComplete);
		}

		public void UpdateSitemap(List<string> filteredExperienceIdentifiers, Action onComplete)
		{
			string arg = "en";
			System.Net.WebRequest webRequest = System.Net.WebRequest.Create(string.Format("{0}{1}/sitemap.json", WebUIViewController.WebviewBaseURL, arg));
			webRequest.Method = "GET";
			JSONRequest jSONRequest = new JSONRequest((HttpWebRequest)webRequest);
			jSONRequest.Start(delegate(HttpWebResponse res)
			{
				using (Stream stream = res.GetResponseStream())
				{
					using (StreamReader streamReader = new StreamReader(stream, Encoding.UTF8))
					{
						string text = streamReader.ReadToEnd();
						WebSitemap webSitemap = null;
						webSitemap = ((filteredExperienceIdentifiers == null) ? JsonMapper.ToObject<WebSitemap>(text) : WebSitemap.Create(text, filteredExperienceIdentifiers));
						WebSitemap.Instance = webSitemap;
						WebSitemap.Instance.PrecacheSite();
						MobageLogger.log("response data collected: " + text);
					}
				}
				onComplete();
			});
		}

		public void TransitionalPresentExperienceNamed(string experienceIdentifier, JsonData experienceOptions, Action<ExperienceViewController, Action<ExperienceViewController>> checkIfShouldContinue)
		{
			Debug.Log("TransitionalPresentExperienceNamed " + experienceIdentifier + ", " + experienceOptions);
			Action<ExperienceViewController> finalPresentationBlock = delegate(ExperienceViewController finalExperience)
			{
				if (finalExperience == null)
				{
					Debug.LogError("Error Expected Experience to be passed in.");
				}
				else
				{
					PresentExperience(finalExperience, true, delegate
					{
					});
				}
			};
			Action action = delegate
			{
				Debug.Log("presentBlock in TransitionalPresentExperienceNamed");
				ExperienceViewController experience = WebSitemap.Instance.ExperienceForIdentifier(experienceIdentifier);
				experience.ExperienceName = experienceIdentifier;
				experience.ExperienceOptions = experienceOptions;
				if (experience.ActiveTab == null && experience.Tabs.Count > 0)
				{
					Debug.Log("not active " + experience.Tabs[0].GetInstanceID());
					experience.Tabs[0].Started += delegate
					{
						experience.PresentTab(experience.Tabs[0], new JsonData(), false);
						Debug.Log("checkIfShouldContinue");
						checkIfShouldContinue(experience, finalPresentationBlock);
					};
				}
				else
				{
					Debug.Log("checkIfShouldContinue");
					checkIfShouldContinue(experience, finalPresentationBlock);
				}
			};
			Debug.Log("Checking web site...");
			ExperienceViewController experienceViewController = WebSitemap.Instance.ExperienceForIdentifier(experienceIdentifier);
			if (experienceViewController == null)
			{
				Debug.Log("MBWebSitemap experienceForIdendifier " + experienceIdentifier + " not found");
				UpdateSitemapWithCallback(action);
				return;
			}
			Debug.Log("MBWebSitemap experienceForIdentifier " + experienceIdentifier + " found");
			if (experienceViewController.Tabs.Count > 0)
			{
				UnityEngine.Object.Destroy(experienceViewController.Tabs[0].gameObject);
			}
			action();
		}

		public void DismissAllExperiencesAnimated(bool flag, Action completion)
		{
			completion();
		}

		public void PopExperienceAnimated(bool flag, Action completion)
		{
			dismissViewControllerAnimated(flag, completion);
		}

		private void dismissViewControllerAnimated(bool flag, Action complete)
		{
			complete();
		}
	}
}

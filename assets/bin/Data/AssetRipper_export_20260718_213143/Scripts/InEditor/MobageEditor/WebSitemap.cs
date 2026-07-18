using System;
using System.Collections.Generic;
using Mobage;
using UnityEngine;

namespace MobageEditor
{
	public class WebSitemap
	{
		private class WebSitemapCacheFillingWebView
		{
			public void startCachingFromSitemap(WebSitemap sitemap)
			{
				sitemap.FinishedCaching = true;
			}
		}

		public JsonData experiences = new JsonData();

		public JsonData index;

		public JsonData cachingExclusions;

		public bool FinishedCaching;

		public GameObject gameObject;

		public static WebSitemap Instance { get; set; }

		public static WebSitemap Create(string result, List<string> filteredExperiences)
		{
			WebSitemap webSitemap = new WebSitemap();
			JsonData jsonData = JsonMapper.ToObject(result);
			JsonData jsonData2 = jsonData["experiences"];
			if (filteredExperiences != null)
			{
				JsonData jsonData3 = new JsonData();
				foreach (string filteredExperience in filteredExperiences)
				{
					if (jsonData2.Contains(filteredExperience))
					{
						jsonData3[filteredExperience] = jsonData2[filteredExperience];
					}
				}
				webSitemap.experiences = jsonData3;
			}
			else
			{
				webSitemap.experiences = jsonData2;
			}
			JsonData jsonData4 = jsonData["index"];
			if (filteredExperiences != null)
			{
				JsonData jsonData5 = new JsonData();
				if (jsonData4 != null)
				{
					foreach (string filteredExperience2 in filteredExperiences)
					{
						if (jsonData4.Contains(filteredExperience2))
						{
							jsonData5[filteredExperience2] = jsonData4[filteredExperience2];
						}
					}
				}
				webSitemap.index = jsonData5;
			}
			else
			{
				webSitemap.index = jsonData4;
			}
			if (jsonData.Contains("cachingExclusions"))
			{
				webSitemap.cachingExclusions = jsonData["cachingExclusions"];
			}
			if (webSitemap.experiences == null || webSitemap.index == null)
			{
				return null;
			}
			return webSitemap;
		}

		public static WebSitemap Create(string result)
		{
			return JsonMapper.ToObject<WebSitemap>(result);
		}

		private Type experienceClassForDictionary(JsonData initializer)
		{
			try
			{
				return Type.GetType("MobageEditor." + (string)initializer["native"]["ios"][0]["class"]);
			}
			catch
			{
				return Type.GetType("MobageEditor.MB_WW_Registration");
			}
		}

		public void PrecacheSite()
		{
			new WebSitemapCacheFillingWebView().startCachingFromSitemap(this);
		}

		public ExperienceViewController ExperienceForIdentifier(string identifier)
		{
			Debug.Log("ExperienceForIdentifier(" + identifier + ")");
			if (!experiences.Contains(identifier))
			{
				return null;
			}
			try
			{
				JsonData jsonData = experiences[identifier];
				if (jsonData.IsString)
				{
					return ExperienceForIdentifier((string)jsonData);
				}
				Type type = experienceClassForDictionary(jsonData);
				if (type.GetConstructor(new Type[1] { typeof(JsonData) }) != null)
				{
					return (ExperienceViewController)Activator.CreateInstance(type, jsonData);
				}
				return (ExperienceViewController)Activator.CreateInstance(type);
			}
			catch (KeyNotFoundException)
			{
				Debug.Log("return null");
				return null;
			}
		}

		public ViewController ControllerForIdentifier(string identifier)
		{
			Debug.Log("WebSitemap ControllerForIdentifier " + identifier);
			JsonData jsonData = index[identifier];
			Debug.Log(jsonData.ToJson());
			GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load("Prefab/WebTabViewController")) as GameObject;
			ExperienceTabViewController component = gameObject.GetComponent<ExperienceTabViewController>();
			component.Config = jsonData;
			component.transform.parent = gameObject.transform;
			if (this.gameObject == null)
			{
				this.gameObject = new GameObject();
			}
			gameObject.transform.parent = this.gameObject.transform;
			return component;
		}
	}
}

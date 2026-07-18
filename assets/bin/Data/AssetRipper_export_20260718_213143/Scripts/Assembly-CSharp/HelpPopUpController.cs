using System.Collections.Generic;
using UnityEngine;

public class HelpPopUpController : PopupController
{
	private const string DEFAULT_SELECTED_TOPIC = "1";

	[SerializeField]
	protected ScrollableAreaController sacHelpTopics;

	[SerializeField]
	protected ScrollableAreaController sacHelpDescriptions;

	private string _selectedTopic;

	protected override void Start()
	{
		base.Start();
		_selectedTopic = "1";
		UpdateScrollableAreas("1");
	}

	private void TopicButtonCallback(string id)
	{
		if (id == _selectedTopic)
		{
			_selectedTopic = null;
		}
		else
		{
			_selectedTopic = id;
		}
		UpdateScrollableAreas(id);
	}

	private void SubTopicButtonCallback(string id)
	{
		int num = 0;
		foreach (HelpRegistersDataModel item in CacheManager.helpRegistersCache[_selectedTopic])
		{
			if (item.id == id)
			{
				break;
			}
			num++;
		}
		sacHelpDescriptions.ContentToPosition((0f - sacHelpDescriptions.cellHeight) * (float)num);
	}

	private void UpdateScrollableAreas(string topicId)
	{
		int num = 0;
		List<HelpTopicEntryData> list = new List<HelpTopicEntryData>();
		List<HelpRegistersDataModel> list2 = new List<HelpRegistersDataModel>();
		bool flag = false;
		foreach (HelpTopicDataModel value in CacheManager.helpTopicsCache.Values)
		{
			HelpTopicEntryData helpTopicEntryData = new HelpTopicEntryData();
			helpTopicEntryData.id = value.id;
			helpTopicEntryData.isOpen = value.id == topicId && _selectedTopic != null;
			helpTopicEntryData.name = LocalizationManager.GetString(value.titleKey, value.name);
			list.Add(helpTopicEntryData);
			if (value.id == topicId)
			{
				flag = true;
				if (CacheManager.helpRegistersCache.ContainsKey(helpTopicEntryData.id) && _selectedTopic != null)
				{
					foreach (HelpRegistersDataModel item in CacheManager.helpRegistersCache[helpTopicEntryData.id])
					{
						helpTopicEntryData = new HelpTopicEntryData();
						helpTopicEntryData.id = item.id;
						helpTopicEntryData.isSubTopic = true;
						helpTopicEntryData.name = LocalizationManager.GetString(item.titleKey, item.name);
						list.Add(helpTopicEntryData);
						list2.Add(item);
					}
				}
			}
			if (!flag)
			{
				num++;
			}
		}
		if (num > 0)
		{
			num--;
		}
		sacHelpTopics.DataSource = list;
		sacHelpTopics.ContentToPosition((0f - sacHelpTopics.cellHeight) * (float)num);
		sacHelpDescriptions.DataSource = list2;
		foreach (HelpTopicEntryCell cellComponent in sacHelpTopics.GetCellComponents<HelpTopicEntryCell>())
		{
			cellComponent.topicCallback = TopicButtonCallback;
			cellComponent.subTopicCallback = SubTopicButtonCallback;
		}
	}
}

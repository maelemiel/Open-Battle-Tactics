using System;
using UnityEngine;

public class HelpTopicEntryCell : ScrollableCell
{
	[SerializeField]
	private GameObject openArrow;

	[SerializeField]
	private GameObject closeArrow;

	[SerializeField]
	private tk2dTextMesh topicTitleSelected;

	[SerializeField]
	private tk2dTextMesh topicTitleUnselected;

	[SerializeField]
	private tk2dTextMesh subTopicTitleSelected;

	[SerializeField]
	private tk2dTextMesh subTopicTitleUnselected;

	public Action<string> topicCallback;

	public Action<string> subTopicCallback;

	private string _topicId;

	private bool _isSubTopic;

	private void Awake()
	{
		topicTitleSelected.gameObject.SetActive(false);
		topicTitleUnselected.gameObject.SetActive(false);
		subTopicTitleSelected.gameObject.SetActive(false);
		subTopicTitleUnselected.gameObject.SetActive(false);
		openArrow.SetActive(false);
		closeArrow.SetActive(false);
	}

	public override void ConfigureCellData()
	{
		topicTitleSelected.gameObject.SetActive(false);
		topicTitleUnselected.gameObject.SetActive(false);
		subTopicTitleSelected.gameObject.SetActive(false);
		subTopicTitleUnselected.gameObject.SetActive(false);
		openArrow.SetActive(false);
		closeArrow.SetActive(false);
		if (base.DataObject != null)
		{
			HelpTopicEntryData helpTopicEntryData = base.DataObject as HelpTopicEntryData;
			_topicId = helpTopicEntryData.id;
			_isSubTopic = helpTopicEntryData.isSubTopic;
			if (helpTopicEntryData.isSubTopic)
			{
				subTopicTitleSelected.gameObject.SetActive(true);
				subTopicTitleUnselected.gameObject.SetActive(true);
				subTopicTitleSelected.text = helpTopicEntryData.name;
				subTopicTitleSelected.Commit();
				subTopicTitleUnselected.text = helpTopicEntryData.name;
				subTopicTitleUnselected.Commit();
			}
			else
			{
				topicTitleSelected.gameObject.SetActive(true);
				topicTitleUnselected.gameObject.SetActive(true);
				topicTitleSelected.text = helpTopicEntryData.name;
				topicTitleSelected.Commit();
				topicTitleUnselected.text = helpTopicEntryData.name;
				topicTitleUnselected.Commit();
				if (helpTopicEntryData.isOpen)
				{
					openArrow.SetActive(true);
				}
				else
				{
					closeArrow.SetActive(true);
				}
			}
		}
		base.ConfigureCellData();
	}

	private void PressTopic()
	{
		if (_isSubTopic)
		{
			subTopicCallback(_topicId);
		}
		else
		{
			topicCallback(_topicId);
		}
	}
}

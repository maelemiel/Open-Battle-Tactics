using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemsMixerSceneController : SceneController
{
	public const int SCROLLABLE_AREA_OUT_OF_SCREEN_LOCAL_X = 600;

	public const int BOARD_OUT_OF_SCREEN_LOCAL_X = -1200;

	public int initialBoardXPosition = -150;

	public int finalBoardXPosition;

	public int initialBoardYPosition = 140;

	public int finalBoardYPosition = -220;

	public int initialBackgroundYPosition = 450;

	public int finalBackgroundYPosition = 265;

	public int itemsMixerHeight = 1330;

	private GachaPlinkoStates currentState;

	public GachaRewardsSceneModel gachaResultsPayload;

	public string partFilterID;

	[SerializeField]
	private BattleText movingText;

	[SerializeField]
	private List<ItemMixerState> sceneStates;

	private Dictionary<GachaPlinkoStates, GachaPlinkoBaseState> itemMixerStatesDictionary;

	public GachaPlinkoPrizesDataModel selectedPrize;

	[NonSerialized]
	public ItemsMixerPlayerSlotViewController SelectedPlayerSlot;

	public ItemCollectionDataModel.Item ItemsMixerItemResult;

	public int ItemsMixerSlotResult;

	[NonSerialized]
	public GameObject playerSlotChip;

	[NonSerialized]
	public GameObject playerRewardCell;

	public GachaPlinkoStates CurrentState
	{
		get
		{
			return currentState;
		}
	}

	public event Action OnResultAvailable;

	public override void Awake()
	{
		_showHomeButton = false;
		_showTopBar = false;
		base.Awake();
		InitializeStatesDictionary();
		Singleton<AudioManager>.instance.StopMusic();
		allowsBackButton = true;
	}

	private void InitializeStatesDictionary()
	{
		itemMixerStatesDictionary = new Dictionary<GachaPlinkoStates, GachaPlinkoBaseState>();
		foreach (ItemMixerState sceneState in sceneStates)
		{
			sceneState.stateComponent.ItemsMixer = this;
			itemMixerStatesDictionary.Add(sceneState.stateType, sceneState.stateComponent);
		}
	}

	private void Start()
	{
		SceneTransitionManager.readyToTransitionIn = true;
		if ((bool)TopBarController.instance)
		{
			TopBarController.instance.ShowProgressBanner = false;
			TopBarController.instance.Visible = false;
		}
		Singleton<InitializationManager>.instance.ExecuteOnState(InitializationManager.State.OnlineReady, delegate
		{
			Init();
		});
	}

	private void Init()
	{
		gachaResultsPayload = null;
		if (sceneModel == null)
		{
			ItemCollectionDataModel itemCollectionDataModel = new ItemCollectionDataModel();
			itemCollectionDataModel.AddItem(new ItemCollectionDataModel.Item(UserInventory.ItemType.Parts, 919, 1));
			itemCollectionDataModel.AddItem(new ItemCollectionDataModel.Item(UserInventory.ItemType.Parts, 925, 1));
			itemCollectionDataModel.AddItem(new ItemCollectionDataModel.Item(UserInventory.ItemType.Parts, 921, 1));
			itemCollectionDataModel.AddItem(new ItemCollectionDataModel.Item(UserInventory.ItemType.Parts, 930, 1));
			itemCollectionDataModel.AddItem(new ItemCollectionDataModel.Item(UserInventory.ItemType.Parts, 926, 1));
			itemCollectionDataModel.AddItem(new ItemCollectionDataModel.Item(UserInventory.ItemType.Parts, 927, 1));
			itemCollectionDataModel.AddItem(new ItemCollectionDataModel.Item(UserInventory.ItemType.Parts, 928, 1));
			itemCollectionDataModel.AddItem(new ItemCollectionDataModel.Item(UserInventory.ItemType.Parts, 929, 1));
			gachaResultsPayload = new GachaRewardsSceneModel(GachaTypes.ITEMS_MIXER, itemCollectionDataModel, null);
		}
		else
		{
			ItemMixerSceneModel itemMixerSceneModel = (ItemMixerSceneModel)sceneModel;
			gachaResultsPayload = itemMixerSceneModel.gachaRewardsSceneModel;
			partFilterID = itemMixerSceneModel.forcePartId;
		}
		SetState(GachaPlinkoStates.INTRO);
	}

	public void SetState(GachaPlinkoStates stateToSet)
	{
		if (itemMixerStatesDictionary.ContainsKey(stateToSet))
		{
			StartCoroutine(itemMixerStatesDictionary[stateToSet].StartStateSequence());
		}
		else
		{
			Log.Error("[ItemsMixerSceneController] State not found. State: " + stateToSet);
		}
		currentState = stateToSet;
	}

	public void SetResult(ItemCollectionDataModel.Item itemResult, int slotResult)
	{
		ItemsMixerItemResult = itemResult;
		ItemsMixerSlotResult = slotResult;
		if (this.OnResultAvailable != null)
		{
			this.OnResultAvailable();
		}
	}

	public bool HasResults()
	{
		return (ItemsMixerItemResult != null) ? true : false;
	}

	public void ShowText(string text)
	{
		if ((bool)movingText)
		{
			movingText.ShowMessage(text);
		}
	}

	private void OnExit()
	{
		ItemMixerSceneModel itemMixerSceneModel = (ItemMixerSceneModel)sceneModel;
		if (itemMixerSceneModel.onExit != null)
		{
			itemMixerSceneModel.onExit();
		}
		else
		{
			SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.ShopItemsSuppliesScene);
		}
	}

	public override void OnBackButton()
	{
		if (currentState != GachaPlinkoStates.IN_PROGRESS && currentState != GachaPlinkoStates.FINISHED)
		{
			base.OnBackButton();
		}
	}
}

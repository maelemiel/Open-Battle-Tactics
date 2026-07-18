public interface IDivisionMetadata
{
	string ID { get; }

	int BaseWinCoinReward { get; }

	int BaseLoseCoinReward { get; }

	int WinPoint { get; }

	int WinEventPoint { get; }

	int EventPointPVPBonus { get; }

	int LosePoint { get; }
}

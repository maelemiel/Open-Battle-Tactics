public class BattleConstantsProvider : IBattleConstantsProvider
{
	public bool Contains(string name)
	{
		return CacheManager.ConstantExists(name);
	}

	public int GetInt(string name)
	{
		return CacheManager.GetConstantInt(name, 0);
	}
}

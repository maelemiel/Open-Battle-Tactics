public abstract class StateBehaviour
{
	public string name;

	public abstract void StartState();

	public abstract string Run();

	public abstract void EndState();
}

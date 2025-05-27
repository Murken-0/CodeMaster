namespace CodeMaster.Core;

public class Player
{
	public string Id { get; }
	public string Name { get; }
	public int Attempts { get; private set; }

	public Player(string id, string name)
	{
		Id = id;
		Name = name;
	}

	public void IncrementAttempts()
	{
		Attempts++;
	}
}
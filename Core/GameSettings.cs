namespace CodeMaster.Core;

public class GameSettings
{
	public int CodeLength { get; set; } = 4;
	public int MaxPlayers { get; set; } = 4;
	public int MinPlayers { get; set; } = 2;
	public int MaxAttempts { get; set; } = 10;
	public string AllowedCharacters { get; set; } = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
}
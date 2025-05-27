namespace CodeMaster.Server.Models;

public class GameResponse
{
	public int BlackMarkers { get; set; }
	public int WhiteMarkers { get; set; }
	public int AttemptsLeft { get; set; }
	public bool IsRoundCompleted { get; set; }
	public string? WinnerId { get; set; }
	public string? WinnerName { get; set; }
	public string Message { get; set; }
}
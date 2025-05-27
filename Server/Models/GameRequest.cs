namespace CodeMaster.Server.Models;

public class GameRequest
{
	public string PlayerId { get; set; }
	public string PlayerName { get; set; }
	public string Guess { get; set; }
}
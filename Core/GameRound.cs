using System.Numerics;

namespace CodeMaster.Core;

public class GameRound
{
	public string SecretCode { get; }
	public List<Player> Players { get; }
	public DateTime StartTime { get; }
	public DateTime? EndTime { get; private set; }
	public Player? Winner { get; private set; }
	public bool IsCompleted { get; private set; }
	public GameSettings Settings { get; }

	public GameRound(GameSettings settings, List<Player> players)
	{
		Settings = settings;
		SecretCode = GameLogic.GenerateRandomCode(settings);
		Players = players;
		StartTime = DateTime.UtcNow;
	}

	public (int blackMarkers, int whiteMarkers) ProcessGuess(Player player, string guess)
	{
		if (IsCompleted)
			throw new InvalidOperationException("Game round is already completed");

		if (guess.Length != Settings.CodeLength)
			throw new ArgumentException($"Guess must be {Settings.CodeLength} characters long");

		player.IncrementAttempts();

		var result = GameLogic.EvaluateGuess(SecretCode, guess);

		if (result.blackMarkers == Settings.CodeLength)
		{
			Winner = player;
			EndTime = DateTime.UtcNow;
			IsCompleted = true;
		}
		else if (Players.All(p => p.Attempts >= Settings.MaxAttempts))
		{
			EndTime = DateTime.UtcNow;
			IsCompleted = true;
		}

		return result;
	}
}
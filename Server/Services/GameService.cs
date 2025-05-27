using CodeMaster.Core;
using CodeMaster.Server.Models;

namespace CodeMaster.Server.Services;

public class GameService
{
	private readonly GameSettings _settings;
	private GameRound? _currentRound;
	private readonly List<Player> _waitingPlayers = new();

	public GameService(GameSettings settings)
	{
		_settings = settings;
	}

	public async Task<GameResponse> JoinGameAsync(string playerId, string playerName)
	{
		var player = new Player(playerId, playerName);

		lock (_waitingPlayers)
		{
			if (_currentRound == null || _currentRound.IsCompleted)
			{
				_waitingPlayers.Add(player);

				if (_waitingPlayers.Count >= _settings.MinPlayers)
				{
					StartNewRound();
				}

				return new GameResponse
				{
					Message = "Waiting for more players to join..."
				};
			}

			if (_currentRound.Players.Count >= _settings.MaxPlayers)
			{
				return new GameResponse
				{
					Message = "Game is full. Please try again later."
				};
			}

			_currentRound.Players.Add(player);
			return new GameResponse
			{
				Message = $"Joined game. Current players: {_currentRound.Players.Count}/{_settings.MaxPlayers}"
			};
		}
	}

	public async Task<GameResponse> SubmitGuessAsync(string playerId, string guess)
	{
		if (_currentRound == null || _currentRound.IsCompleted)
		{
			return new GameResponse
			{
				Message = "No active game round. Please join a new game."
			};
		}

		var player = _currentRound.Players.FirstOrDefault(p => p.Id == playerId);
		if (player == null)
		{
			return new GameResponse
			{
				Message = "Player not found in current game."
			};
		}

		try
		{
			var (blackMarkers, whiteMarkers) = _currentRound.ProcessGuess(player, guess);

			var response = new GameResponse
			{
				BlackMarkers = blackMarkers,
				WhiteMarkers = whiteMarkers,
				AttemptsLeft = _settings.MaxAttempts - player.Attempts,
				IsRoundCompleted = _currentRound.IsCompleted,
				Message = $"Guess processed. Black markers: {blackMarkers}, White markers: {whiteMarkers}"
			};

			if (_currentRound.IsCompleted)
			{
				response.Message = _currentRound.Winner != null
					? $"Round completed! Winner: {_currentRound.Winner.Name}"
					: "Round completed! No winner - max attempts reached.";

				if (_currentRound.Winner != null)
				{
					response.WinnerId = _currentRound.Winner.Id;
					response.WinnerName = _currentRound.Winner.Name;
				}

				// Сохраняем результаты раунда
				await SaveRoundResultsAsync(_currentRound);

				// Начинаем новый раунд, если есть ожидающие игроки
				StartNewRoundIfPossible();
			}

			return response;
		}
		catch (Exception ex)
		{
			return new GameResponse
			{
				Message = $"Error: {ex.Message}"
			};
		}
	}

	private void StartNewRound()
	{
		lock (_waitingPlayers)
		{
			if (_waitingPlayers.Count < _settings.MinPlayers) return;

			var playersToAdd = Math.Min(_waitingPlayers.Count, _settings.MaxPlayers);
			var players = _waitingPlayers.Take(playersToAdd).ToList();
			_waitingPlayers.RemoveRange(0, playersToAdd);

			_currentRound = new GameRound(_settings, players);
		}
	}

	private void StartNewRoundIfPossible()
	{
		lock (_waitingPlayers)
		{
			if (_waitingPlayers.Count >= _settings.MinPlayers)
			{
				StartNewRound();
			}
		}
	}

	private async Task SaveRoundResultsAsync(GameRound round)
	{
		var serializer = new XmlSerializerService();
		await serializer.SerializeRoundAsync(round);
	}
}
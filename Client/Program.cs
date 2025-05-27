using System.Net.Http.Json;
using System.Text;
using CodeMaster.Client.Models;

namespace CodeMaster.Client;

public class Program
{
	private static readonly HttpClient _httpClient = new();
	private static string _apiUrl = "http://localhost:5001/api/game";
	private static string _playerId = Guid.NewGuid().ToString();
	private static string _playerName = "Player_" + new Random().Next(1000);

	public static async Task Main(string[] args)
	{
		Console.OutputEncoding = Encoding.UTF8;
		Console.WriteLine($"Добро пожаловать в Код-Мастер! Вы: {_playerName}");

		var joinResponse = await JoinGameAsync();
		Console.WriteLine(joinResponse.Message);

		while (true)
		{
			Console.Write("Введите вашу догадку (4 символа A-Z или 0-9): ");
			var guess = Console.ReadLine()?.ToUpper();

			if (string.IsNullOrEmpty(guess) || guess.Length != 4 || !guess.All(c => char.IsLetterOrDigit(c)))
			{
				Console.WriteLine("Неверный формат догадки. Попробуйте еще раз.");
				continue;
			}

			var guessResponse = await SubmitGuessAsync(guess);
			Console.WriteLine(guessResponse.Message);

			if (guessResponse.IsRoundCompleted)
			{
				if (guessResponse.WinnerId == _playerId)
				{
					Console.WriteLine("Поздравляем! Вы выиграли этот раунд!");
				}
				else if (!string.IsNullOrEmpty(guessResponse.WinnerId))
				{
					Console.WriteLine($"Игрок {guessResponse.WinnerName} выиграл этот раунд.");
				}
				else
				{
					Console.WriteLine("Раунд завершен. Никто не угадал код.");
				}

				Console.WriteLine("Начинается новый раунд...");
				joinResponse = await JoinGameAsync();
				Console.WriteLine(joinResponse.Message);
			}
		}
	}

	private static async Task<GameResponse> JoinGameAsync()
	{
		var request = new GameRequest
		{
			PlayerId = _playerId,
			PlayerName = _playerName,
			Guess = ""
		};

		var response = await _httpClient.PostAsJsonAsync($"{_apiUrl}/join", request);
		return await response.Content.ReadFromJsonAsync<GameResponse>();
	}

	private static async Task<GameResponse> SubmitGuessAsync(string guess)
	{
		var request = new GameRequest
		{
			PlayerId = _playerId,
			PlayerName = _playerName,
			Guess = guess
		};

		var response = await _httpClient.PostAsJsonAsync($"{_apiUrl}/guess", request);
		return await response.Content.ReadFromJsonAsync<GameResponse>();
	}
}
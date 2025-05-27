namespace CodeMaster.Core;

public static class GameLogic
{
	public static (int blackMarkers, int whiteMarkers) EvaluateGuess(string secretCode, string guess)
	{
		if (secretCode.Length != guess.Length)
			throw new ArgumentException("Code and guess must be of the same length");

		int blackMarkers = 0;
		int whiteMarkers = 0;

		var secretCodeChars = secretCode.ToCharArray();
		var guessChars = guess.ToCharArray();

		// Сначала считаем черные маркеры (правильные позиции)
		for (int i = 0; i < secretCodeChars.Length; i++)
		{
			if (secretCodeChars[i] == guessChars[i])
			{
				blackMarkers++;
				secretCodeChars[i] = '_';
				guessChars[i] = '_';
			}
		}

		// Затем считаем белые маркеры (правильные символы на неправильных позициях)
		for (int i = 0; i < secretCodeChars.Length; i++)
		{
			if (secretCodeChars[i] == '_') continue;

			for (int j = 0; j < guessChars.Length; j++)
			{
				if (guessChars[j] == '_') continue;

				if (secretCodeChars[i] == guessChars[j])
				{
					whiteMarkers++;
					secretCodeChars[i] = '_';
					guessChars[j] = '_';
					break;
				}
			}
		}

		return (blackMarkers, whiteMarkers);
	}

	public static string GenerateRandomCode(GameSettings settings)
	{
		var random = new Random();
		var code = new char[settings.CodeLength];

		for (int i = 0; i < settings.CodeLength; i++)
		{
			code[i] = settings.AllowedCharacters[random.Next(settings.AllowedCharacters.Length)];
		}

		return new string(code);
	}
}
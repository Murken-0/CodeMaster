using CodeMaster.Server.Models;
using CodeMaster.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace CodeMaster.Server.Controllers;

[ApiController]
[Route("api/game")]
public class GameController : ControllerBase
{
	private readonly GameService _gameService;

	public GameController(GameService gameService)
	{
		_gameService = gameService;
	}

	[HttpPost("join")]
	public async Task<IActionResult> JoinGame([FromBody] GameRequest request)
	{
		var response = await _gameService.JoinGameAsync(request.PlayerId, request.PlayerName);
		return Ok(response);
	}

	[HttpPost("guess")]
	public async Task<IActionResult> SubmitGuess([FromBody] GameRequest request)
	{
		var response = await _gameService.SubmitGuessAsync(request.PlayerId, request.Guess);
		return Ok(response);
	}
}
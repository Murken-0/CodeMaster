using System.Xml.Serialization;
using CodeMaster.Core;

namespace CodeMaster.Server.Services;

public class XmlSerializerService
{
	private readonly string _resultsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "GameResults");

	public XmlSerializerService()
	{
		if (!Directory.Exists(_resultsDirectory))
		{
			Directory.CreateDirectory(_resultsDirectory);
		}
	}

	public async Task SerializeRoundAsync(GameRound round)
	{
		var model = new RoundResultModel
		{
			StartTime = round.StartTime,
			EndTime = round.EndTime ?? DateTime.UtcNow,
			SecretCode = round.SecretCode,
			Players = round.Players.Select(p => new PlayerResult
			{
				Id = p.Id,
				Name = p.Name,
				Attempts = p.Attempts,
				IsWinner = round.Winner?.Id == p.Id
			}).ToList(),
			WinnerId = round.Winner?.Id,
			WinnerName = round.Winner?.Name
		};

		var fileName = $"Round_{round.StartTime:yyyyMMddHHmmss}.xml";
		var filePath = Path.Combine(_resultsDirectory, fileName);

		var serializer = new XmlSerializer(typeof(RoundResultModel));
		await using var writer = new StreamWriter(filePath);
		serializer.Serialize(writer, model);
	}

	[XmlRoot("RoundResult")]
	public class RoundResultModel
	{
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }
		public string SecretCode { get; set; }
		public List<PlayerResult> Players { get; set; }
		public string? WinnerId { get; set; }
		public string? WinnerName { get; set; }
	}

	public class PlayerResult
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public int Attempts { get; set; }
		public bool IsWinner { get; set; }
	}
}
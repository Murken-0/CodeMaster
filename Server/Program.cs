using CodeMaster.Server.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var gameSettings = new CodeMaster.Core.GameSettings
{
	CodeLength = 4,
	MaxPlayers = 4,
	MinPlayers = 2,
	MaxAttempts = 10,
	AllowedCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"
};

builder.Services.AddSingleton(gameSettings);
builder.Services.AddSingleton<GameService>();
builder.Services.AddSingleton<XmlSerializerService>();

var app = builder.Build();

app.UseAuthorization();
app.MapControllers();

app.Run("http://localhost:5001/");
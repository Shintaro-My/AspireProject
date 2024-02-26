
using WebApi.Service.DiscordBot;

namespace WebApi.Service
{
    public class DiscordBotService: BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var bot = new Bot();
            await bot.MainAsync("bot_token", stoppingToken);
        }
    }
}

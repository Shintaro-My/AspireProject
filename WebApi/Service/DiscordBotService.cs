
using WebApi.Service.DiscordBot;

// https://dsharpplus.github.io/DSharpPlus/articles/advanced_topics/buttons.html
// https://github.com/DSharpPlus/DSharpPlus/discussions/1421

namespace WebApi.Service
{
    /// <summary>
    /// バックグラウンドタスクとして、Discord Botを起動する試み
    /// </summary>
    public class DiscordBotService: BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var bot = new Bot();
            await bot.MainAsync("bot_token", stoppingToken);
        }
    }
}

using DSharpPlus;
using DSharpPlus.CommandsNext;
using WebApi.Service.DiscordBot.Commands;

namespace WebApi.Service.DiscordBot
{
    public class Bot
    {
        public async Task MainAsync(string botToken, CancellationToken cancellationToken)
        {
            var discord = new DiscordClient(new DiscordConfiguration()
            {
                Token = botToken,
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.AllUnprivileged | DiscordIntents.MessageContents
            });
            var commands = discord.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = new[] { "!" }
            });

            // コマンドモジュールの登録（個数分だけ追加する）
            commands.RegisterCommands<SampleCommandModule>();

            await discord.ConnectAsync();
            await Task.Delay(-1, cancellationToken);
        }
    }
}

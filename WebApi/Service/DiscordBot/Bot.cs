using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Interactivity;
using DSharpPlus.SlashCommands;
using WebApi.Service.DiscordBot.Argument;
using WebApi.Service.DiscordBot.Commands;
using WebApi.Service.DiscordBot.Help;
using WebApi.Service.DiscordBot.SlashCommands;

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
            discord.UseInteractivity(new InteractivityConfiguration()
            {
                PollBehaviour = PollBehaviour.KeepEmojis,
                Timeout = TimeSpan.FromSeconds(30)
            });
            var commands = discord.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = new[] { "!" }
            });
            var slash = discord.UseSlashCommands();
            slash.RegisterCommands<SlashAdmin>();

            // コマンドモジュールなどの登録（個数分だけ追加する）
            commands.RegisterConverter(new CustomArgumentConverter());
            commands.RegisterCommands<SampleCommandModule>();
            commands.SetHelpFormatter<CustomHelpFormatter>();

            await discord.ConnectAsync();
            // 永続
            await Task.Delay(-1, cancellationToken);
        }
    }
}

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace WebApi.Service.DiscordBot.Commands
{
    public class SampleCommandModule : BaseCommandModule
    {
        [Command("greet")]
        public async Task GreetCommand(CommandContext ctx, [RemainingText] string name)
        {
            await ctx.RespondAsync($"Greetings! Thank you for executing me, {name}!");
        }
    }
}

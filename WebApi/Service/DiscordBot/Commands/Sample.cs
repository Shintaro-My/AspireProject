using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;

namespace WebApi.Service.DiscordBot.Commands
{
    public class SampleCommandModule : BaseCommandModule
    {
        [Command("react")]
        public async Task React(CommandContext ctx, DiscordMember member)
        {
            var emoji = DiscordEmoji.FromName(ctx.Client, ":ok_hand:");
            var message = await ctx.RespondAsync($"{member.Mention}, react with {emoji}.");

            var result = await message.WaitForReactionAsync(member, emoji);

            if (!result.TimedOut) await ctx.RespondAsync("Thank you!");
        }

        [Command("btn")]
        public async Task ButtonTest(CommandContext ctx)
        {
            var interactivity = ctx.Client.GetInteractivity();

            var builder = new DiscordMessageBuilder()
                .WithContent("This message has buttons! Pretty neat innit?")
                .AddComponents(new DiscordComponent[]
                {
                    new DiscordButtonComponent(ButtonStyle.Primary,   "1_top", "Blurple!"),
                    new DiscordButtonComponent(ButtonStyle.Secondary, "2_top", "Grey!"),
                    new DiscordButtonComponent(ButtonStyle.Success,   "3_top", "Green!"),
                    new DiscordButtonComponent(ButtonStyle.Danger,    "4_top", "Red!"),
                    new DiscordLinkButtonComponent("https://some-super-cool.site", "Link!")
                });

            var message = await ctx.RespondAsync(builder);
            var buttonResult = await interactivity.WaitForButtonAsync(message);

            await ctx.RespondAsync($"Clicked: {buttonResult.Result.Id}");
        }
    }
}

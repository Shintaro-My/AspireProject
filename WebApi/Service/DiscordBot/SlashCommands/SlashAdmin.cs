using DSharpPlus.Entities;
using DSharpPlus;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;

namespace WebApi.Service.DiscordBot.SlashCommands
{
    [SlashCommandGroup("admin", "Admin Tool")]
    public class SlashAdmin : ApplicationCommandModule
    {
        [SlashCommand("ban", "Bans a user")]
        [SlashRequirePermissions(Permissions.BanMembers)]
        public async Task Ban(InteractionContext ctx, [Option("user", "User to ban")] DiscordUser user,
            [Choice("None", 0)]
            [Choice("1 Day", 1)]
            [Choice("1 Week", 7)]
            [Option("deletedays", "Number of days of message history to delete")] long deleteDays = 0)
        {
            await ctx.Guild.BanMemberAsync(user.Id, (int)deleteDays);
            await ctx.CreateResponseAsync(
                InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().WithContent($"Banned {user.Username}: {deleteDays} day(s)")
            );
        }
    }
}

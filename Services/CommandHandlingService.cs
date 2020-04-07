using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SamaASP.Services
{
    public class CommandHandlingService
    {
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;
        private IServiceProvider _provider;
        private IConfiguration _config;

        public CommandHandlingService( IServiceProvider provider, DiscordSocketClient discord, CommandService commands )
        {
            _discord = discord;
            _commands = commands;
            _provider = provider;

            _discord.MessageReceived += MessageReceived;
            _discord.UserJoined += UserJoined;
            _discord.GuildMemberUpdated += GuildMemberUpdated;
            _discord.JoinedGuild += JoinedGuild;
        }

        public async Task InitializeAsync( IServiceProvider provider, IConfiguration config )
        {
            _provider = provider;
            _config = config;
            await _commands.AddModulesAsync( Assembly.GetEntryAssembly(), _provider );
            // Add additional initialization code here...
        }

        private async Task JoinedGuild( SocketGuild guild )
        {
            return;
        }

        private async Task UserJoined( SocketUser user )
            => await ( user as IGuildUser ).AddRoleAsync( ( user as IGuildUser ).Guild.Roles.FirstOrDefault( x => x.Name == "Newbie" ) );

        private async Task GuildMemberUpdated( SocketGuildUser before, SocketGuildUser after )
        {
            Tools.UpdatePlayerRole( after );
        }

        private async Task MessageReceived( SocketMessage rawMessage )
        {
            // Ignore system messages and messages from bots
            if ( !( rawMessage is SocketUserMessage message ) ) return;
            if ( message.Source != MessageSource.User ) return;

            int argPos = 0;
            if ( !message.HasStringPrefix( "sm!", ref argPos ) ) return;

            var context = new SocketCommandContext(_discord, message);
            var result  = await _commands.ExecuteAsync(context, argPos, _provider);

            if ( result.Error.HasValue && result.Error.Value != CommandError.UnknownCommand )
                await context.Channel.SendMessageAsync( result.ToString() );
            else if ( result.Error.Value == CommandError.UnknownCommand )
                await context.Channel.SendMessageAsync( "Commande Inconnue !" );
        }
    }
}
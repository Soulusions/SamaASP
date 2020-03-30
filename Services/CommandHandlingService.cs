using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Linq;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace SamaASP.Services
{
    public class CommandHandlingService
    {
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;
        private IServiceProvider _provider;

        public CommandHandlingService( IServiceProvider provider, DiscordSocketClient discord, CommandService commands )
        {
            _discord  = discord;
            _commands = commands;
            _provider = provider;

            _discord.MessageReceived    += MessageReceived;
            _discord.UserJoined         += UserJoined;
            _discord.GuildMemberUpdated += GuildMemberUpdated;
        }

        public async Task InitializeAsync( IServiceProvider provider )
        {
            _provider = provider;
            await _commands.AddModulesAsync( Assembly.GetEntryAssembly(), _provider );
            // Add additional initialization code here...
        }

        private async Task UserJoined( SocketUser user )
            => await ( user as IGuildUser ).AddRoleAsync( ( user as IGuildUser ).Guild.Roles.FirstOrDefault( x => x.Name == "Newbie" ) );

        private async Task GuildMemberUpdated( SocketGuildUser before, SocketGuildUser after )
        {
            bool inRoleplay = false;
            foreach ( var r in after.Roles )
            {
                if ( r.Name.StartsWith( "RP " ) )
                {
                    inRoleplay = true;
                    break;
                }
            }

            if ( inRoleplay )
            {
                await after.AddRoleAsync( after.Guild.Roles.FirstOrDefault( x => x.Name == "Joueurs et Joueuses" ) );
            }
            else
            {
                await after.RemoveRoleAsync( after.Guild.Roles.FirstOrDefault( x => x.Name == "Joueurs et Joueuses" ) );
            }
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
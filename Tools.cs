using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace SamaASP
{
    public static class Tools
    {
        public static async void UpdatePlayerRole( SocketGuildUser user )
        {
            if ( user.GuildPermissions.ManageChannels || user.GuildPermissions.Administrator || user.Hierarchy > user.Guild.CurrentUser.Hierarchy ) return;

            Console.WriteLine( $"Updating: {user.Username}\n" );
            SocketRole player  = user.Guild.Roles.FirstOrDefault(x => x.Name == "Joueurs et Joueuses");
            bool inRoleplay = false;
            foreach ( var r in user.Roles )
            {
                if ( r.Name.StartsWith( "RP " ) )
                {
                    inRoleplay = true;
                    break;
                }
            }

            if ( inRoleplay && !user.Roles.Contains( player ) )
            {
                await user.AddRoleAsync( player );
            }
            else if ( !inRoleplay && user.Roles.Contains( player ) )
            {
                await user.RemoveRoleAsync( player );
            }
        }
    }
}
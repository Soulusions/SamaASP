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
            if ( user.Hierarchy > user.Guild.CurrentUser.Hierarchy ) return;

            SocketRole player  = user.Guild.Roles.FirstOrDefault(x => x.Name == "Joueurs et Joueuses");
            SocketRole notPlayer = user.Guild.Roles.First(x => x.Name.StartsWith("Non "));

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
                if ( user.Roles.Contains( notPlayer ) )
                {
                    await user.RemoveRoleAsync( notPlayer );
                }
            }
            else if ( !inRoleplay )
            {
                if ( user.Roles.Contains( player ) )
                {
                    await user.RemoveRoleAsync( player );
                }
                await user.AddRoleAsync( notPlayer );
            }
        }
    }
}
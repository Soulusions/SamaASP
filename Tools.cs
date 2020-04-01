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
            bool inRoleplay = false;
            foreach ( var r in user.Roles )
            {
                if ( r.Name.StartsWith( "RP " ) )
                {
                    inRoleplay = true;
                    break;
                }
            }

            if ( inRoleplay )
            {
                await user.AddRoleAsync( user.Guild.Roles.FirstOrDefault( x => x.Name == "Joueurs et Joueuses" ) );
            }
            else
            {
                await user.RemoveRoleAsync( user.Guild.Roles.FirstOrDefault( x => x.Name == "Joueurs et Joueuses" ) );
            }
        }
    }
}

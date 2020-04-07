using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SamaASP.Modules
{
    public class UserInfoModule : ModuleBase<SocketCommandContext>
    {
        [Command( "userinfo" )]
        public async Task UserInfo( SocketUser user = null )
        {
            SocketRole guest = Context.Guild.Roles.FirstOrDefault(x => x.Name == "Invités");
            SocketRole player = Context.Guild.Roles.FirstOrDefault(x => x.Name == "Joueurs et Joueuses");

            if ( user == null ) user = Context.User;

            var guildUser = user as IGuildUser;

            var statEmbed = new EmbedBuilder().WithTitle("Rapport Statistique de Nibba-Sama")
                .WithDescription($"Statistiques de **{user.Username}**, membre depuis *{guildUser.JoinedAt}*")
                .WithAuthor("Nibba-Sama", "https://imgur.com/xtxFbE2.png")
                .WithTimestamp(DateTime.Now)
                .WithColor(new Color(255, 145, 255))
                .WithFooter("Kesturegarde ?", "https://imgur.com/xtxFbE2.png")
                .WithImageUrl($"{user.GetAvatarUrl()}");

            var status = "";
            if ( guildUser.RoleIds.Contains( guest.Id ) ) status = "Invité";
            if ( guildUser.RoleIds.Contains( player.Id ) ) status = "Joueur";
            if ( guildUser.GuildPermissions.ManageChannels ) status = "**Administrateur**";
            if ( guildUser.IsBot ) status = "Bot";

            List<EmbedFieldBuilder> statFields = new List<EmbedFieldBuilder>(
                new EmbedFieldBuilder[]
                {
                    new EmbedFieldBuilder().WithName("Id").WithValue(user.Id).WithIsInline(true),
                    new EmbedFieldBuilder().WithName("Tag").WithValue($"{user.Username}#{user.Discriminator}").WithIsInline(true),
                    new EmbedFieldBuilder().WithName("Status").WithValue(status),
                    new EmbedFieldBuilder().WithName("Roles").WithValue(guildUser.RoleIds.Count),
                    new EmbedFieldBuilder().WithName("Crée le").WithValue(user.CreatedAt),
                });

            statEmbed.WithFields( statFields );

            await ReplyAsync( embed: statEmbed.Build() );
        }
    }

    public class GuildInfoModule : ModuleBase<SocketCommandContext>
    {
        [Command( "guildinfo" )]
        public async Task GuildInfo()
        {
            var Guild = Context.Guild;

            var statEmbed = new EmbedBuilder().WithTitle("Rapport Statistique de Nibba-Sama")
                .WithDescription($"Statistiques de **{Guild.Name}** crée par **{Guild.Owner.Username}**")
                .WithAuthor("Nibba-Sama", "https://imgur.com/xtxFbE2.png")
                .WithTimestamp(DateTime.Now)
                .WithColor(new Color(255, 145, 255))
                .WithFooter("Kesturegarde ?", "https://imgur.com/xtxFbE2.png");

            int admins = 0;
            int bots   = 0;
            foreach ( var u in Context.Guild.Users ) if ( u.GuildPermissions.ManageChannels && !u.IsBot ) admins++;
            foreach ( var u in Context.Guild.Users ) if ( u.IsBot ) bots++;

            List<EmbedFieldBuilder> statFields = new List<EmbedFieldBuilder>(
                new EmbedFieldBuilder[]
                {
                    new EmbedFieldBuilder().WithName("Utilisateurs").WithValue(Guild.Users.Count),
                    new EmbedFieldBuilder().WithName("Administrateurs").WithValue(admins),
                    new EmbedFieldBuilder().WithName("Bots").WithValue(bots),
                    new EmbedFieldBuilder().WithName("Roles").WithValue(Guild.Roles.Count),
                    new EmbedFieldBuilder().WithName("Channels Texte").WithValue(Guild.TextChannels.Count),
                    new EmbedFieldBuilder().WithName("Channel Vocaux").WithValue(Guild.VoiceChannels.Count),
                    new EmbedFieldBuilder().WithName("Crée le").WithValue(Guild.CreatedAt)
                });

            statEmbed.WithFields( statFields );

            await ReplyAsync( embed: statEmbed.Build() );
        }
    }

    public class HelpModule : ModuleBase<SocketCommandContext>
    {
        [Command( "help" )]
        [RequireUserPermission( ChannelPermission.ManageChannels )]
        public async Task Help( int page = 1 )
        {
            var helpEmbed = new EmbedBuilder().WithTitle("Nibba-Sama's cook book")
                .WithAuthor("Nibba-Sama", "https://imgur.com/xtxFbE2.png")
                .WithTimestamp(DateTime.Now)
                .WithColor(new Color(255, 145, 255))
                .WithDescription("Un guide à l'utilisation de Nibba-Sama")
                .WithFooter("Kesturegarde ?", "https://imgur.com/xtxFbE2.png");

            List<EmbedFieldBuilder> helpFields = new List<EmbedFieldBuilder>(
                new EmbedFieldBuilder[] {
                     new EmbedFieldBuilder().WithName("`sm!info`")
                         .WithValue("Infos sur Nibba-Sama").WithIsInline(true),
                     new EmbedFieldBuilder().WithName("`sm!whois`").WithValue("Récupère des infos sur l'utilisateur mentionné").WithIsInline(true),
                     new EmbedFieldBuilder().WithName("\u200b").WithValue("\u200b").WithIsInline(true),
                     new EmbedFieldBuilder().WithName("`sm!guildinfo`").WithValue("Donne des stats / infos sur la guilde").WithIsInline(true),
                }
             );

            await ReplyAsync( embed: helpEmbed.WithFields( helpFields ).Build() );
        }
    }

    public class VerificationModule : ModuleBase<SocketCommandContext>
    {
        [Command( "verifier" )]
        public async Task Verify()
        {
            SocketRole newbie = Context.Guild.Roles.FirstOrDefault(x => x.Name == "Newbie");
            SocketRole guest  = Context.Guild.Roles.FirstOrDefault(x => x.Name == "Invités");

            if ( ( Context.Message.Author as IGuildUser ).RoleIds.Contains( newbie.Id ) )
            {
                await ( Context.Message.Author as IGuildUser ).AddRoleAsync( guest );
                await ( Context.Message.Author as IGuildUser ).RemoveRoleAsync( newbie );
                await Context.Channel.SendMessageAsync( $"Vous êtes désormais vérifiés {Context.Message.Author.Mention} ! Vous avez désormais accès au serveur !" );
            }
            else
            {
                await Context.Channel.SendMessageAsync( $"Vous êtes déjà vérifiés {Context.Message.Author.Mention} !" );
            }
        }
    }

    public class UtilsModule : ModuleBase<SocketCommandContext>
    {
        [Command( "forceupdate" )]
        [RequireUserPermission( ChannelPermission.ManageChannels )]
        public async Task ForceUpdate()
        {
            var timer = System.Diagnostics.Stopwatch.StartNew();

            SocketRole newbie = Context.Guild.Roles.FirstOrDefault(x => x.Name == "Newbie");
            SocketRole guest  = Context.Guild.Roles.FirstOrDefault(x => x.Name == "Invités");

            foreach ( SocketGuildUser u in Context.Guild.Users )
            {
                Tools.UpdatePlayerRole( u );

                if ( ( !u.Roles.Contains( newbie ) || !u.Roles.Contains( guest ) ) && !u.GuildPermissions.ManageChannels )
                {
                    await u.AddRoleAsync( guest );
                }
            }

            timer.Stop();

            await Context.User.SendMessageAsync( $"Beep Boop, nettoyé en {timer.ElapsedMilliseconds}ms" );
        }
    }

    public class MiscModule : ModuleBase<SocketCommandContext>
    {
        [Command( "tg" )]
        public async Task Nein()
        {
            await Context.User.SendMessageAsync( "T'es le prochain..." );
        }
    }
}
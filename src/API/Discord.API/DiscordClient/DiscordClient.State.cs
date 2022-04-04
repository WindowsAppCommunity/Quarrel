// Adam Dernis © 2022

using CommunityToolkit.Diagnostics;
using Discord.API.Models.Channels.Abstract;
using Discord.API.Models.Channels.Interfaces;
using Discord.API.Models.Guilds;
using Discord.API.Models.Json.Channels;
using Discord.API.Models.Json.Guilds;
using Discord.API.Models.Json.Settings;
using Discord.API.Models.Json.Users;
using Discord.API.Models.Settings;
using Discord.API.Models.Users;
using System.Collections.Concurrent;

namespace Discord.API
{
    public partial class DiscordClient
    {
        private SelfUser? _selfUser;
        private Settings _settings;

        private ConcurrentDictionary<ulong, Guild> _guildMap;
        private ConcurrentDictionary<ulong, Channel> _channelMap;
        private ConcurrentDictionary<ulong, User> _userMap;
        private ConcurrentDictionary<(ulong GuildId, ulong UserId), GuildMember> _guildsMemberMap;

        public DiscordClient()
        {
            _guildMap = new ConcurrentDictionary<ulong, Guild>();
            _channelMap = new ConcurrentDictionary<ulong, Channel>();
            _userMap = new ConcurrentDictionary<ulong, User>();
            _guildsMemberMap = new ConcurrentDictionary<(ulong GuildId, ulong UserId), GuildMember>();
        }

        internal SelfUser? CurrentUser => _selfUser;

        internal Guild? GetGuildInternal(ulong guildId)
        {
            if (_guildMap.TryGetValue(guildId, out Guild guild))
            {
                return guild;
            }

            return null;
        }

        internal bool AddGuild(JsonGuild jsonGuild)
        {
            Guild guild = new Guild(jsonGuild, this);
            if(_guildMap.TryAdd(guild.Id, guild))
            {
                foreach (var jsonChannel in jsonGuild.Channels)
                {
                    bool added = AddChannel(jsonChannel, jsonGuild.Id);
                    if (added)
                    {
                        guild.AddChannel(jsonChannel.Id);
                    }
                }

                foreach (var member in jsonGuild.Members)
                {
                    AddGuildMember(guild.Id, member);
                }


                return true;
            }

            return false;
        }

        internal bool UpdateGuild(JsonGuild jsonGuild)
        {
            if (_guildMap.TryGetValue(jsonGuild.Id, out Guild guild))
            {
                guild.UpdateFromRestGuild(jsonGuild);
                return true;
            }

            return false;
        }

        internal bool RemoveGuild(ulong guildId)
        {
            if (_guildMap.TryRemove(guildId, out Guild guild))
            {
                foreach (var channelId in guild.ChannelIds)
                {
                    RemoveChannel(channelId);
                }

                return true;
            }

            return false;
        }

        internal Channel? GetChannelInternal(ulong channelId)
        {
            if (_channelMap.TryGetValue(channelId, out Channel channel))
            {
                return channel;
            }

            return null;
        }

        internal bool AddChannel(JsonChannel jsonChannel, ulong? guildId = null)
        {
            guildId = jsonChannel.GuildId ?? guildId;
            Channel? channel = Channel.FromRestChannel(jsonChannel, this, guildId);
            if(channel != null && _channelMap.TryAdd(channel.Id, channel))
            {
                if (guildId.HasValue && _guildMap.TryGetValue(guildId.Value, out Guild guild))
                {
                    guild.AddChannel(channel.Id);
                }

                return true;
            }

            return false;
        }

        internal bool UpdateChannel(JsonChannel jsonChannel)
        {
            if (_channelMap.TryGetValue(jsonChannel.Id, out Channel channel))
            {
                channel.UpdateFromRestChannel(jsonChannel);
                return true;
            }

            return false;
        }

        internal bool RemoveChannel(ulong channelId)
        {
            if (_channelMap.TryGetValue(channelId, out Channel channel))
            {
                if (channel is IGuildChannel guildChannel && _guildMap.TryGetValue(guildChannel.GuildId, out Guild guild))
                {
                    guild.RemoveChannel(channelId);
                }

                return true;
            }

            return false;
        }

        internal bool AddReadState(JsonReadState jsonReadState)
        {
            Channel? channel = GetChannelInternal(jsonReadState.ChannelId);

            if (channel is IMessageChannel messageChannel)
            {
                messageChannel.MentionCount = jsonReadState.MentionCount;
                messageChannel.LastReadMessageId = jsonReadState.LastMessageId;
                return true;
            }

            return false;
        }

        internal User? GetUserInternal(ulong userId)
        {
            if (_userMap.TryGetValue(userId, out User user))
            {
                return user;
            }

            return null;
        }

        internal User? GetOrAddUserInternal(JsonUser jsonUser)
        {
            if(_userMap.TryGetValue(jsonUser.Id, out User user))
            {
                return user;
            }

            AddUser(jsonUser);
            return GetUserInternal(jsonUser.Id);
        }

        internal bool AddUser(JsonUser jsonUser)
        {
            User user = new User(jsonUser, this);
            return _userMap.TryAdd(user.Id, user);
        }

        internal bool AddSelfUser(JsonUser jsonUser)
        {
            SelfUser user = new SelfUser(jsonUser, this);
            _selfUser = user;
            return _userMap.TryAdd(user.Id, user);
        }

        internal bool UpdateUser(JsonUser jsonUser)
        {
            if (_userMap.TryGetValue(jsonUser.Id, out User user))
            {
                user.UpdateFromRestUser(jsonUser);
                return true;
            }

            return false;
        }

        internal bool RemoveUser(ulong userId)
        {
            return _userMap.TryRemove(userId, out User user);
        }

        internal GuildMember? GetGuildMemberInternal((ulong GuildId, ulong UserId) key)
        {
            if (_guildsMemberMap.TryGetValue(key, out GuildMember member))
            {
                return member;
            }

            return null;
        }

        internal bool AddGuildMember(ulong guildId, JsonGuildMember jsonGuildMember)
        {
            GuildMember member = new GuildMember(jsonGuildMember, guildId, this);
            if (_guildsMemberMap.TryAdd((guildId, member.UserId), member))
            {
                AddUser(jsonGuildMember.User);

                return true;
            }
            return false;
        }

        internal bool UpdateGuildMember(ulong guildId, JsonGuildMember jsonGuildMember)
        {
            if (_guildsMemberMap.TryGetValue((guildId, jsonGuildMember.User.Id), out GuildMember member))
            {
                member.UpdateFromJsonGuildMember(jsonGuildMember);
                return true;
            }

            return false;
        }

        internal bool RemoveGuildMember(ulong guildId, JsonGuildMember jsonGuildMember)
        {
            return _guildsMemberMap.TryRemove((guildId, jsonGuildMember.User.Id), out GuildMember member);
        }

        internal bool AddPresence(ulong? guildId, JsonPresence jsonPresence)
        {
            Guard.IsNotNull(jsonPresence.User, nameof(jsonPresence.User));

            if (guildId is null)
            {
                return AddPresence(jsonPresence);
            }

            ulong userId = jsonPresence.User.Id;
            if (_guildsMemberMap.TryGetValue((guildId.Value, userId), out GuildMember member))
            {
                member.Presence = new Presence(jsonPresence);
                return true;
            }

            return false;
        }

        internal bool AddPresence(JsonPresence jsonPresence)
        {
            Guard.IsNotNull(jsonPresence.User, nameof(jsonPresence.User));

            if (GetUserInternal(jsonPresence.User.Id) is null)
            {
                AddUser(jsonPresence.User);
            }

            if(_userMap.TryGetValue(jsonPresence.User.Id, out User user))
            {
                user.Presence = new Presence(jsonPresence);
                return true;
            }

            return false;
        }

        internal bool AddRelationship(JsonRelationship jsonRelationship)
        {
            Guard.IsNotNull(jsonRelationship.User, nameof(jsonRelationship.User));

            User? user = GetOrAddUserInternal(jsonRelationship.User);

            bool status = user is not null;
            if (user is not null)
            {
                user.RelationshipType = jsonRelationship.Type;
            }

            if (jsonRelationship.Presence is not null)
            {
                bool added = AddPresence(jsonRelationship.Presence);
                status = status && added;
            }

            return status;
        }

        internal void UpdateSettings(JsonUserSettings jsonUserSettings)
        {
            Settings settings = new Settings(jsonUserSettings, this);
            _settings = settings;
        }
    }
}

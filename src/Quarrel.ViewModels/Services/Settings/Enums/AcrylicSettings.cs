// Copyright (c) Quarrel. All rights reserved.

using System;

namespace Quarrel.ViewModels.Services.Settings.Enums
{
    /// <summary>
    /// Values are powers of 2 to avoid collison for flags.
    /// </summary>
    [Flags]
    public enum AcrylicSettings
    {
        MessageView = 0x1,
        ChannelView = 0x2,
        GuildView = 0x4,
        CommandBar = 0x8,
    }
}

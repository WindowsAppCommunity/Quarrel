// Special thanks to Sergio Pedri for the basis of this design
// Copyright (c) Quarrel. All rights reserved.

namespace Quarrel.ViewModels.Services.Settings
{
    /// <summary>
    /// An <see langword="enum"/> that holds the various setting keys used in the app.
    /// </summary>
    public enum SettingKeys
    {
        // Other
        Token,
        AdsRemoved,

        // Display
        Theme,
        Blurple,
        ServerMuteIcons,
        ExpensiveRendering,
        DerivedColor,
        AcrylicSettings,
        FontSize,
        FluentTheme,

        // Behavior
        MentionGlow,
        AuthorPresence,
        ShowNoPermssions,
        HideMuted,
        CollapseOverride,
        FilterMembers,
        TTLAttachments,
        DataCompression,

        // Voice
        OutputDevice,
        InputDevice,
    }
}

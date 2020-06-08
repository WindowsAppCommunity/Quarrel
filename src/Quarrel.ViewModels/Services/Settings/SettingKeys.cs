// Special thanks to Sergio Pedri for this design from Legere
// Sergio's GitHub: https://github.com/Sergio0694
// Legere: https://www.microsoft.com/store/apps/9PHJRVCSKVJZ

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

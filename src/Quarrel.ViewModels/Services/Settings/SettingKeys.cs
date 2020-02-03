// Special thanks to Sergio Pedri for the basis of this design

namespace Quarrel.ViewModels.Services.Settings
{
    /// <summary>
    /// An <see langword="enum"/> that holds the various setting keys used in the app
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

        // Behavior
        MentionGlow,
        ShowNoPermssions,
        HideMuted,
        CollapseOverride,
        FilterMembers,
        TTLAttachments,
        DataCompression

    }
}

// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.API.Guild.Models;
using myTube;
using System;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Data;

namespace Quarrel.Converters.AuditLog
{
    /// <summary>
    /// Converter for a change type and values to markdown text.
    /// </summary>
    public class ChangeToMarkdownConverter : IValueConverter
    {
        /// <summary>
        /// Converts a change to markdown text.
        /// </summary>
        /// <param name="value">Change.</param>
        /// <param name="targetType">Requested out type.</param>
        /// <param name="parameter">Extra info.</param>
        /// <param name="language">What language the user is using.</param>
        /// <returns>Natural text change info.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is Change change)
            {
                string format = GetFormat(change);

                switch (change.Key)
                {
                    case "name":
                    case "code":
                    case "bitrate":
                    case "region":
                        if (change.NewValue != null)
                        {
                            format = format.Replace("<new>", string.Format("**{0}**", change.NewValue));
                        }

                        if (change.OldValue != null)
                        {
                            format = format.Replace("<old>", change.OldValue.ToString());
                        }

                        return format;

                    case "nsfw":
                        if (change.NewValue != null)
                        {
                            format = format.Replace("<new>", (bool)change.NewValue ? "**NSFW**" : "**SFW**");
                        }

                        return format;

                    case "color":
                        if (change.NewValue != null)
                        {
                            format = format.Replace("<new>", string.Format("<@$QUARREL-color{0}>", change.NewValue));
                        }

                        if (change.OldValue != null)
                        {
                            format = format.Replace("<old>", string.Format("<@$QUARREL-color{0}>", change.OldValue));
                        }

                        return format;

                    case "channel_id":
                        if (change.NewValue != null)
                        {
                            format = format.Replace("<new>", string.Format("<#{0}>", change.NewValue));
                        }

                        return format;

                    case "max_age":
                        if (change.NewValue != null)
                        {
                            if ((long)change.NewValue == 0)
                            {
                                format = ResourceLoader.GetForCurrentView("AuditLog").GetString("max_age0");
                            }
                            else
                            {
                                format = format.Replace("<new>", string.Format("**{0}**", change.NewValue));
                            }
                        }

                        return format;

                    case "uses":
                        if (change.NewValue != null)
                        {
                            if ((long)change.NewValue == 0)
                            {
                                format = ResourceLoader.GetForCurrentView("AuditLog").GetString("uses0");
                            }
                            else if ((long)change.NewValue == 1)
                            {
                                format = ResourceLoader.GetForCurrentView("AuditLog").GetString("uses1");
                            }
                            else
                            {
                                format = format.Replace("<new>", string.Format("**{0}**", change.NewValue));
                            }
                        }

                        return format;

                    case "max_uses":
                        if (change.NewValue != null)
                        {
                            if ((long)change.NewValue == 0)
                            {
                                format = ResourceLoader.GetForCurrentView("AuditLog").GetString("max_uses0");
                            }
                            else if ((long)change.NewValue == 1)
                            {
                                format = ResourceLoader.GetForCurrentView("AuditLog").GetString("max_uses1");
                            }
                            else
                            {
                                format = format.Replace("<new>", string.Format("**{0}**", change.NewValue));
                            }
                        }

                        return format;

                    default:
                        format = format.Replace("<property>", change.Key);
                        if (change.NewValue != null)
                        {
                            format = format.Replace("<new>", string.Format("**{0}**", change.NewValue));
                        }

                        if (change.OldValue != null)
                        {
                            format = format.Replace("<old>", change.OldValue.ToString());
                        }

                        return format;
                }
            }

            return "Unknown change";
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets change text template from change key and OldValue.
        /// </summary>
        /// <param name="change">Change object.</param>
        /// <returns>Resource format by language and type.</returns>
        public string GetFormat(Change change)
        {
            string append = change.OldValue != null ? "Change" : "Set";
            string format = Helpers.Constants.Localization.GetLocalizedAuditLogString(change.Key + append);

            if (string.IsNullOrEmpty(format))
            {
                format = Helpers.Constants.Localization.GetLocalizedAuditLogString("Generic" + append);
            }

            return format;
        }
    }
}

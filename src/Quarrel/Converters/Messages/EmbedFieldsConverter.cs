// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Models;
using DiscordAPI.Models.Messages;
using DiscordAPI.Models.Messages.Embeds;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Data;

namespace Quarrel.Converters.Messages
{
    /// <summary>
    /// A Converters that returns a list of fields from a list of embeds.
    /// </summary>
    public class EmbedFieldsConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is EmbedField[] embedFields)
            {
                List<List<EmbedField>> inlinedFields = new List<List<EmbedField>> { new List<EmbedField>() };
                foreach (var embedField in embedFields)
                {
                    if (embedField.Inline)
                    {
                        inlinedFields[inlinedFields.Count - 1].Add(embedField);
                    }
                    else
                    {
                        inlinedFields.Add(new List<EmbedField> { embedField });
                        inlinedFields.Add(new List<EmbedField>());
                    }
                }

                if (inlinedFields.Last().Count == 0)
                {
                    inlinedFields.RemoveAt(inlinedFields.Count - 1);
                }

                return inlinedFields;
            }

            return null;
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

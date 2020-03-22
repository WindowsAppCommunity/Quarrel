using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using DiscordAPI.Models;

namespace Quarrel.Converters.Messages
{
    public class EmbedFieldsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is EmbedField[] embedFields)
            {
                List<List<EmbedField>> inlinedFields = new List<List<EmbedField>>{new List<EmbedField>()};
                foreach (var embedField in embedFields)
                {
                    if (embedField.Inline)
                    {
                        inlinedFields[inlinedFields.Count-1].Add(embedField);
                    }
                    else
                    {
                        inlinedFields.Add(new List<EmbedField>{ embedField });
                        inlinedFields.Add(new List<EmbedField>());
                    }
                }

                if (inlinedFields.Last().Count == 0) inlinedFields.RemoveAt(inlinedFields.Count-1);

                return inlinedFields;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

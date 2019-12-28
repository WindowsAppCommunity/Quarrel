using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordAPI.Interfaces
{
    public interface IPreviewableAttachment
    {
        string ImageUrl { get; }

        double ImageHeight { get; }

        double ImageWidth { get; }
    }
}

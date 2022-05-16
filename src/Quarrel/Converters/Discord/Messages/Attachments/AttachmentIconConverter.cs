// Quarrel © 2022

namespace Quarrel.Converters.Discord.Messages.Attachments
{
    public class AttachmentIconConverter
    {
        public static string Convert(string filetype)
        {
            // TODO: Replace with dictionary from file
            return filetype switch
            {
                // Text file icon
                "txt" => "",

                // Zipped file(s) icon
                "7z" or
                "gz" or
                "zip" => "",

                // Image icon
                "png" or
                "jpg" or
                "jpeg" or
                "bmp" or
                "gif" => "",

                // Audio icon
                "mp3" or 
                "wav" => "",

                // Video icon
                "mp4" or
                "mov" => "",

                // 3D icon
                "obj" or
                "blend" => "",

                // Console icon
                "ps1" or
                "batch" => "",

                // App Package icon
                "appx" or
                "appxbundle" or
                "msix" or
                "msixbundle" or
                "appinstaller" => "",

                // Certificate icon
                "cer" => "",

                // Code icon
                "json" => "",

                // Disc icon
                "iso" => "",

                // File icon
                _ => "",
            };
        }
    }
}

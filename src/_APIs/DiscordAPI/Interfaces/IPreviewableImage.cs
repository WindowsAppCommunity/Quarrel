namespace DiscordAPI.Interfaces
{
    public interface IPreviewableImage
    {
        string ImageUrl { get; }

        double ImageHeight { get; }

        double ImageWidth { get; }
    }
}

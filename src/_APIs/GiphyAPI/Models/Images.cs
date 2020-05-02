using Newtonsoft.Json;

namespace GiphyAPI.Models
{
    public struct Images
    {
        [JsonProperty("fixed_height")]
        public FixedHeight FixedHeight { get; set; }

        [JsonProperty("fixed_height_still")]
        public FixedHeightStill FixedHeightStill { get; set; }

        [JsonProperty("fixed_height_downsampled")]
        public FixedHeightDownsized fixedHeightDownsized { get; set; }

        [JsonProperty("fixed_width")]
        public FixedWidth FixedWidth { get; set; }

        [JsonProperty("fixed_width_still")]
        public FixedWidthStill FixedWidthStill { get; set; }

        [JsonProperty("fixed_width_downsampled")]
        public FixedWidthDownsized FixedWidthDownsized { get; set; }

        [JsonProperty("fixed_height_small")]
        public FixedHeightSmall FixedHeightSmall { get; set; }

        [JsonProperty("fixed_height_small_still")]
        public FixedHeightSmallStill FixedHeightSmallStill { get; set; }

        [JsonProperty("fixed_width_small")]
        public FixedWidthSmall FixedWidthSmall { get; set; }

        [JsonProperty("fixed_width_small_still")]
        public FixedWidthSmallStill FixedWidthSmallStill { get; set; }

        [JsonProperty("downsized")]
        public Downsized Downsized { get; set; }

        [JsonProperty("downsized_still")]
        public DownsizedStill DownsizedStill { get; set; }

        [JsonProperty("downsized_large")]
        public DownsizedLarge DownsizedLarge { get; set; }

        [JsonProperty("downsized_medium")]
        public DownsizedMedium DownsizedMedium { get; set; }

        [JsonProperty("downsized_small")]
        public DownsizedSmall DownsizedSmall { get; set; }

        [JsonProperty("original")]
        public Orginial Orginial { get; set; }

        [JsonProperty("original_still")]
        public OrginialStill OrginialStill { get; set; }

        [JsonProperty("looping")]
        public Looping Looping { get; set; }

        [JsonProperty("preview")]
        public Preview Preview { get; set; }

        [JsonProperty("preview_gif")]
        public PreviewGif PreviewGif { get; set; }
    }

    public struct FixedHeight
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("width")]
        public string Width { get; set; }

        [JsonProperty("height")]
        public string Height { get; set; }

        [JsonProperty("size")]
        public string Size { get; set; }

        [JsonProperty("mp4")]
        public string Mp4Url { get; set; }

        [JsonProperty("mp4_size")]
        public string Mp4Size { get; set; }

        [JsonProperty("webp")]
        public string WebpUrl { get; set; }

        [JsonProperty("webp_size")]
        public string Title { get; set; }
    }

    public struct FixedHeightSmall
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("width")]
        public string Width { get; set; }

        [JsonProperty("height")]
        public string Height { get; set; }

        [JsonProperty("size")]
        public string Size { get; set; }

        [JsonProperty("mp4")]
        public string Mp4Url { get; set; }

        [JsonProperty("mp4_size")]
        public string Mp4Size { get; set; }

        [JsonProperty("webp")]
        public string WebpUrl { get; set; }

        [JsonProperty("webp_size")]
        public string Title { get; set; }
    }

    public struct FixedHeightStill
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("width")]
        public string Width { get; set; }

        [JsonProperty("height")]
        public string Height { get; set; }
    }

    public struct FixedHeightSmallStill
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("width")]
        public string Width { get; set; }

        [JsonProperty("height")]
        public string Height { get; set; }
    }

    public struct FixedHeightDownsized
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("width")]
        public string Width { get; set; }

        [JsonProperty("height")]
        public string Height { get; set; }

        [JsonProperty("webp")]
        public string WebpUrl { get; set; }

        [JsonProperty("webp_size")]
        public string Title { get; set; }
    }

    public struct FixedWidth
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("width")]
        public string Width { get; set; }

        [JsonProperty("height")]
        public string Height { get; set; }

        [JsonProperty("size")]
        public string Size { get; set; }

        [JsonProperty("mp4")]
        public string Mp4Url { get; set; }

        [JsonProperty("mp4_size")]
        public string Mp4Size { get; set; }

        [JsonProperty("webp")]
        public string WebpUrl { get; set; }

        [JsonProperty("webp_size")]
        public string Title { get; set; }
    }

    public struct FixedWidthSmall
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("width")]
        public string Width { get; set; }

        [JsonProperty("height")]
        public string Height { get; set; }

        [JsonProperty("size")]
        public string Size { get; set; }

        [JsonProperty("mp4")]
        public string Mp4Url { get; set; }

        [JsonProperty("mp4_size")]
        public string Mp4Size { get; set; }

        [JsonProperty("webp")]
        public string WebpUrl { get; set; }

        [JsonProperty("webp_size")]
        public string Title { get; set; }
    }

    public struct FixedWidthStill
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("width")]
        public string Width { get; set; }

        [JsonProperty("height")]
        public string Height { get; set; }
    }

    public struct FixedWidthSmallStill
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("width")]
        public string Width { get; set; }

        [JsonProperty("height")]
        public string Height { get; set; }
    }

    public struct FixedWidthDownsized
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("width")]
        public string Width { get; set; }

        [JsonProperty("height")]
        public string Height { get; set; }

        [JsonProperty("webp")]
        public string WebpUrl { get; set; }

        [JsonProperty("webp_size")]
        public string Title { get; set; }
    }

    public struct Downsized
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("width")]
        public string Width { get; set; }

        [JsonProperty("height")]
        public string Height { get; set; }

        [JsonProperty("size")]
        public string String { get; set; }
    }
    public struct DownsizedStill
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("width")]
        public string Width { get; set; }

        [JsonProperty("height")]
        public string Height { get; set; }
    }
    public struct DownsizedLarge
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("width")]
        public string Width { get; set; }

        [JsonProperty("height")]
        public string Height { get; set; }

        [JsonProperty("size")]
        public string String { get; set; }
    }
    public struct DownsizedMedium
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("width")]
        public string Width { get; set; }

        [JsonProperty("height")]
        public string Height { get; set; }

        [JsonProperty("size")]
        public string String { get; set; }
    }
    public struct DownsizedSmall
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("width")]
        public string Width { get; set; }

        [JsonProperty("height")]
        public string Height { get; set; }

        [JsonProperty("size")]
        public string String { get; set; }
    }

    public struct Orginial
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("width")]
        public string Width { get; set; }

        [JsonProperty("height")]
        public string Height { get; set; }

        [JsonProperty("size")]
        public string Size { get; set; }

        [JsonProperty("frames")]
        public string Frames { get; set; }

        [JsonProperty("mp4")]
        public string Mp4Url { get; set; }

        [JsonProperty("mp4_size")]
        public string Mp4Size { get; set; }

        [JsonProperty("webp")]
        public string WebpUrl { get; set; }

        [JsonProperty("webp_size")]
        public string Title { get; set; }
    }
    public struct OrginialStill
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("width")]
        public string Width { get; set; }

        [JsonProperty("height")]
        public string Height { get; set; }
    }

    public struct Looping
    {
        [JsonProperty("mp4")]
        public string Mp4Url { get; set; }
    }

    public struct Preview
    {
        [JsonProperty("mp4")]
        public string Mp4Url { get; set; }

        [JsonProperty("mp4_size")]
        public string Mp4Size { get; set; }

        [JsonProperty("width")]
        public string Width { get; set; }

        [JsonProperty("height")]
        public string Height { get; set; }
    }
    public struct PreviewGif
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("width")]
        public string Width { get; set; }

        [JsonProperty("height")]
        public string Height { get; set; }

        [JsonProperty("size")]
        public string Size { get; set; }
    }
}

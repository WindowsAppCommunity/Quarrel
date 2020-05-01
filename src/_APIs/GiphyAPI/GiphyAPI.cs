
using Refit;

namespace GiphyAPI
{
    public static class GiphyAPI
    {
        internal const string GiphyKey = "erGe4TVabEDlDPOkHFc389gQPvx4ze9Z";

        public static IGiphyService GetGiphyService()
        {
            return RestService.For<IGiphyService>("http://api.giphy.com");
        }
    }
}

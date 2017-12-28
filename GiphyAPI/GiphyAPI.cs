using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Refit;

namespace GiphyAPI
{
    public class GiphyAPI
    {
        internal const string GiphyKey = "erGe4TVabEDlDPOkHFc389gQPvx4ze9Z";

        public IGiphyService GetGiphyService()
        {
            return RestService.For<IGiphyService>("api.giphy.com");
        }
    }
}

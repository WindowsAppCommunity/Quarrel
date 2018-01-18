using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Refit;

namespace FrenglyAPI
{
    public class FrenglyAPI
    {
        //TODO: http://frengly.com/api

        public static ITranslateService GetGiphyService()
        {
            return RestService.For<ITranslateService>("http://frengly.com/");
        }
    }
}

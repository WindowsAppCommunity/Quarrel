using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Refit;

namespace FrenglyAPI
{
    public interface ITranslateService
    {
        [Post("/frengly/data/translateREST")]
        Task<string> Translate([Body] TranlateData data);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quarrel.Converters.Base
{
    public class ContainsItemsToBoolConverter
    {
        public static bool Convert(IEnumerable<object> collection)
        {
            return collection.Any();
        }
    }
}

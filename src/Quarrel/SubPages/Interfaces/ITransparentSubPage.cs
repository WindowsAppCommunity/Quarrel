using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quarrel.SubPages.Interfaces
{
    public interface ITransparentSubPage
    {
        /// <summary>
        /// Is the background dimmed
        /// </summary>
        bool Dimmed { get; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAPI.Voice
{
    struct Sodium
    {
        [DllImport("SodiumC.dll")]
        public static extern int add(int a, int b);
    }
}

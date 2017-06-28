using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_UWP
{
    class EntryMessages
    {
        public static string GetMessage()
        {
            Random rand = new Random();
            switch (rand.Next(0, 5))
            {
                case 0:
                    return "HUZZAH";
                case 1:
                    return "Beam me up Scottie!!!";
                case 3:
                    return "";
                default:
                    return "Waiting for Ready Packet";
            }
        }
    }
}

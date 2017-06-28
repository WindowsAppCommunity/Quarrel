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
            switch (rand.Next(0, 10))
            //switch (11)
            {
                case 0:
                    return "HUZZAH";
                case 1:
                    return "Beam me up Scottie!!!";
                case 3:
                    return "Catch Phrase";
                case 4:
                    return "Microsoft's working on Windows Mobile !?!?!? (Submitted by @Dikas#8802)";
                case 5:
                    return "Splash screen* (Submitted by @ToonWK#5841)";
                case 6:
                    return "I don't mean to be rude but...";
                case 7:
                    return "Food doesn't grow on trees";
                case 8:
                    return "😬";
                case 9:
                    return "Pushing the Payload";
                case 10:
                    return "Hunting robot dinosaurs";
                case 11:
                    return "Microsoft FTW (Submitted by @dabezt31#9977)";
                default:
                    return "Waiting for Ready Packet";
            }
        }
    }
}

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
            switch (rand.Next(0, 24))
            {
                case 0: return "HUZZAH";
                case 1: return "Beam me up Scotty!!!";
                case 3: return "Aligning buttons";
                case 5: return "UWP FTW";
                case 6: return "Generating stuff";
                case 7: return "Cutting ribbons";
                case 8: return "Passing the vacuum cleaner";
                case 9: return "I'm afraid I can't do that, Dave";
                case 10: return "I'Il be back";
                case 11: return "It's alive!!";
                case 12: return "Thanking the producers";
                case 13: return "Hiding the ugly code";
                case 14: return "Writing loading messages";
                case 15: return "400: Bad joke";
                case 16: return "418: I'm a teapot";
                case 17: return "404: Joke not found";
                case 18: return "Catch Phrase";
                case 19: return "Microsoft's working on Windows Mobile !?!?!? (Submitted by @Dikas#8802)";
                case 20: return "Splash screen* (Submitted by @ToonWK#5841)";
                case 21: return "I don't mean to be rude but...";
                case 22: return "Converting caffine to code";
                case 23: return "😬";
                case 24: return "Burning evidence...";
                default: return "Waiting for Ready Packet";
            }
        }
    }
}

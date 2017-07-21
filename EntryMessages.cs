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
            switch (rand.Next(0, 39))
            {
                case 0: return "HUZZAH";
                case 1: return "Beam me up Scotty!!!";
                case 3: return "Aligning buttons";
                case 5: return "UWP FTW";
                case 6: return "Generating stuff";
                case 7: return "Cutting ribbons";
                case 8: return "Passing the vacuum cleaner";
                case 9: return "I'm afraid I can't do that, Dave";
                case 10: return "Squashing bugs";
                case 11: return "It's alive!!";
                case 12: return "Thanking the producers";
                case 13: return "Hiding the ugly code";
                case 14: return "Writing loading messages";
                case 15: return "400: Bad joke";
                case 16: return "418: I'm a teapot";
                case 17: return "404: Joke not found";
                case 18: return "<Insert catch phrase here>";
                case 19: return "Microsoft's working on Windows Mobile!?!?!? (Submitted by @Dikas#8802)";
                case 20: return "Splash screen* (Submitted by @ToonWK#5841)";
                case 21: return "I don't mean to be rude but...";
                case 22: return "Converting caffeine to code";
                case 23: return "😬";
                case 24: return "Burning evidence...";
                case 25: return "Oh, it's you";
                case 26: return "Making covfefe";
                case 27: return "Hiring lawyers";
                case 28: return "if(Loading) { EntryMessages.GetMessage(1); }";
                case 29: return "Genji on the Payload?! (Submitted by @ModProg|Roland#6987)";
                case 30: return "Waiting for opus 😪 (Submitted by @hvb#2543)";
                case 31: return "Capturing the flag... (Submitted by @Gavirlas#9973)";
                case 32: return "Does it even load (Submitted by @Gavirlas#9973)";
                case 33: return "Gotta go fast! (Submitted by @YoshiAsk#4385)";
                case 34: return "It's a Trap! (Submitted by @ModProg|Roland#6987)";
                case 35: return "It's a Trap! (Submitted by @ModProg|Roland#6987)";
                case 36: return "The sun is a deadly laser (Submitted by @ToonWK#5841)";
                case 37: return "I need healing (Submitted by @Neel#2970)";
                case 38: return "Han shot first (Submitted by @DougTheDog6#5067)";
                case 39: return "Shots fired (Submitted by @Gavirlas#9973)";
                default: return "Waiting for the Ready Packet";
            }
        }
    }
}

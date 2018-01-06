using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_UWP
{
    class EntryMessages
    {
        public static KeyValuePair<string,string> GetMessage()
        {
            if (App.AslansBullshit)
            {
                return kvp("Past og splash - waitng for Ready packet");
            }

            Random rand = new Random();
            if (rand.Next(4) % 4 != 0)
            {
                DateTime now = DateTime.Now;
                if (now.Month == 12 && now.Day == 25)
                {
                    switch (rand.Next(0, 3))
                    {
                        //Christmas day
                        case 0: return kvp("Ho ho ho");
                        case 1: return kvp("Merry Christmas!");
                        case 2: return kvp("What a beautiful christmas day...");
                        case 3: return kvp("All I want for christmas is a loading screen");
                    }
                }
                if(now.Month == 12 && now.Day == 21)
                {
                    //End of the world
                    return kvp((now.Year - 2012).ToString() + " years since the end of the world");
                    
                }
                if(now.Month == 5 && now.Day == 4)
                {
                    //Star wars day (May the fourth)
                    switch (rand.Next(0, 5))
                    {
                        case 0: return kvp("May the force be with you", "@Darth4212#5735");
                        case 1: return kvp("Using the force");
                        case 2: return kvp("A long time ago in a galaxy far, far away...");
                        case 3: return kvp("Removing Porgs from the Millenium Falcon...");
                        case 4: return kvp("Travelling through hyperspace");
                        case 5: return kvp("Murdering Jar Jar Binks");
                      
                    }
                }
            }
            
            switch (rand.Next(0, 79))
            {
                case 0:  return kvp("HUZZAH");
                case 1:  return kvp("Beam me up Scotty!!!");
                case 3:  return kvp("Aligning buttons");
                case 5:  return kvp("UWP FTW");
                case 6:  return kvp("Generating stuff");
                case 7:  return kvp("Cutting ribbons");
                case 8:  return kvp("Passing the vacuum cleaner");
                case 9:  return kvp("I'm afraid I can load that, Dave");
                case 10: return kvp("Squashing bugs");
                case 11: return kvp("It's alive!!");
                case 12: return kvp("Thanking the producers");
                case 13: return kvp("Hiding the ugly code");
                case 14: return kvp("Writing loading messages");
                case 15: return kvp("400: Bad joke");
                case 16: return kvp("418: I'm a teapot");
                case 17: return kvp("404: (Good) joke not found");
                case 18: return kvp("<Insert catch phrase here>");
                case 19: return kvp("Microsoft's working on Windows Mobile!?!?!?","@Dikas#8802");
                case 20: return kvp("Splash screen*", "@ToonWK#5841");
                case 21: return kvp("I don't mean to be rude but...");
                case 22: return kvp("Converting caffeine to code");
                case 23: return kvp("😬");
                case 24: return kvp("Burning evidence...");
                case 25: return kvp("Oh, it's you");
                case 26: return kvp("Making covfefe");
                case 27: return kvp("Hiring lawyers");
                case 28: return kvp("if(NotWorking) { MakeItWork(); }");
                case 29: return kvp("Genji on the Payload?!", "@ModProg|Roland#6987");
                case 30: return kvp("Waiting for opus 😪", "@hvb#2543");
                case 31: return kvp("Capturing the flag...", "@Gavirlas#9973");
                case 32: return kvp("Does it even load", "@Gavirlas#9973");
                case 33: return kvp("Gotta go fast!", "@YoshiAsk#4385");
                case 34: return kvp("It's a Trap!", "@ModProg|Roland#6987");
                case 35: return kvp("<Insert Wilhelm scream>", "@Aslan#9846");
                case 36: return kvp("The sun is a deadly laser", "@ToonWK#5841");
                case 37: return kvp("I need healing", "@Neel#2970");
                case 38: return kvp("Han shot first", "@DougTheDog6#5067");
                case 39: return kvp("Shots fired", "@Gavirlas#9973");
                case 40: return kvp("$100! $200! $300!", "@DougTheDog6#5067");
                case 41: return kvp("€100! €200! €300! Go Yurop!", "@ModProg|Roland#6987");
                case 42: return kvp("I should charge money for new downloads now...", "@Neel#2970");
                case 43: return kvp("Aren't you a little short to be a storm trooper?", "@DougTheDog6#5067");
                case 44: return kvp("Smooth animations warning", "@Gavirlas#9973");
                case 45: return kvp("VOICE IS NOT AVAILABLE YET!");
                case 46: return kvp("Randomly pressing buttons...", "@Gavirlas#9973");
                case 47: return kvp("System.out.println(\"Hello World\");", "@ModProg|Roland#6987");
                case 48: return kvp("The developers are great!", "@Canada Baltimore Bias#2911");
                case 49: return kvp("Ready player one?", "@ዘልኗጌልዪዕ_ርቿረጎክቿ#9883");
                case 50: return kvp("Watch the icon rollin' rollin'", "@Gavirlas#9973");
                case 51: return kvp("You spin me right round, baby right round", "omgPANTO#6232");
                case 52: return kvp("Watch the icon rollin' rollin'", "@ዘልኗጌልዪዕ_ርቿረጎክቿ#9883");
                case 53: return kvp("YOU'RE A WIZARD LARRY!", "@SamCraftRecon#9075");
                case 54: return kvp("Updating memes...", "@SamCraftRecon#9075");
                case 55: return kvp("L.O.L. ing at cats", "@SamCraftRecon#9075");
                case 56: return kvp("shaken not shtirred", "@Aslan#9846");
                case 57: return kvp("All natural!", "@SamCraftRecon#9075"); 
                case 58: return kvp("100 % sugar free", "@Aslan#9846");
                case 59: return kvp("Searching for the meaning of life");
                case 60: return kvp("You got a license for that?", "@Aslan#9846");
                case 61: return kvp("Did you bring snacks?", "Chestbeard#9806");
                case 62: return kvp("Suitable for Vegans", "@SamCraftRecon#9075");
                case 63: return kvp("I can't believe you've done this");
                case 64: return kvp("Better than iOS 11!", "@Abaan404#9892");
                case 65: return kvp("Watching Netflix", "@Aslan#9846");
                case 66: return kvp("Checking for Bugs", "@Aslan#9846");
                case 67: return kvp("FRE SHAVAC ADO");
                case 68: return kvp("I have the power of God and Anime");
                case 69: return kvp("I ain't got no sleep");
                case 70: return kvp("No ketchup, just sauce, raw sauce");
                case 71: return kvp("I said babes, loading's not hot");
                case 72: return kvp("The ting goes skrrraah");
                case 73: return kvp("Tacos incoming, prepare for pure joy", "@LuketheDuke424#2556");
                case 74: return kvp("Waiting for Senpai", "@LuketheDuke424#2556");
                case 75: return kvp("Is this even legal?", "@Smash_kirby#0966");
                case 76: return kvp("Maximizing Hype", "@Civiled#1713");
                case 77: return kvp("Preparing the Men", "@Civiled#1713");
                case 78: return kvp("Everytime you ask, it becomes a little longer");
                case 79: return kvp("And also with you", "John Mulaney");
                default: return kvp("Waiting for the Ready Packet");
            }
        }

        private static KeyValuePair<string, string> kvp(string key, string val = "")
        {
            return new KeyValuePair<string, string>(key, val);
        }
    }
}

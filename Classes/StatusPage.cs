using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Discord_UWP.Classes
{
   public static class StatusPage
   {
       public static string Url = "https://discordapp.statuspage.io/";
       public static string MetricsId = "ztt4777v23lf";

       public static async Task<StatusPageClasses.Index> GetStatus()
       {
           using (HttpClient client = new HttpClient())
           {
               Stream stream;
               try
               {
                   stream = await client.GetStreamAsync(Url + "index.json");
               }
               catch (Exception)
               {
                   return null;
               }
               using (StreamReader reader = new StreamReader(stream))
               {
                   using (var jsonTextReader = new JsonTextReader(reader))
                   {
                       JsonSerializer serializer = new JsonSerializer();
                       return serializer.Deserialize<StatusPageClasses.Index>(jsonTextReader);
                   }
                }
            }
       }
       public static async Task<StatusPageClasses.AllMetrics> GetMetrics()
       {
           using (HttpClient client = new HttpClient())
           {
               Stream stream;
               try
               {
                   stream = await client.GetStreamAsync(Url + "metrics-display/"+MetricsId+"/day.json");
               }
               catch (Exception)
               {
                   return null;
               }
               using (StreamReader reader = new StreamReader(stream))
               {
                   using (var jsonTextReader = new JsonTextReader(reader))
                   {
                       JsonSerializer serializer = new JsonSerializer();
                       return serializer.Deserialize<StatusPageClasses.AllMetrics>(jsonTextReader);
                   }
               }
           }
       }
    }
}

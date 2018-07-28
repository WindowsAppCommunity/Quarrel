using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Windows.Web.Http;
using Windows.Web.Http.Headers;

namespace Discord_UWP.Classes
{
   public static class StatusPage
   {
       public static string Url = "https://discord.statuspage.io/";
       public static string MetricsId = "ztt4777v23lf";
       public static string Token = null;
       public static string Cookies = "";

       public static async Task<StatusPageClasses.Index> GetStatus()
       {
           using (HttpClient client = new HttpClient())
           {
               IInputStream stream;
               try
               {
                   stream = await client.GetInputStreamAsync(new Uri(Url + "index.json"));
               }
               catch (Exception)
               {
                   return null;
               }
               using (StreamReader reader = new StreamReader(stream.AsStreamForRead()))
               {
                   using (var jsonTextReader = new JsonTextReader(reader))
                   {
                       JsonSerializer serializer = new JsonSerializer();
                       return serializer.Deserialize<StatusPageClasses.Index>(jsonTextReader);
                   }
                }
            }
       }
       public static async Task<StatusPageClasses.AllMetrics> GetMetrics(string duration = "day")
       {
            using (HttpClient client = new HttpClient())
           {
                IInputStream stream;
               try
               {
                   stream = await client.GetInputStreamAsync(new Uri(Url + "metrics-display/"+MetricsId+"/"+duration+".json"));
               }
               catch (Exception)
               {
                   return null;
               }
               using (StreamReader reader = new StreamReader(stream.AsStreamForRead()))
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

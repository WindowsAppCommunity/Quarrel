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

namespace Quarrel.Classes
{
    /// <summary>
    /// Independant API for discord outage metrics
    /// </summary>
   public static class StatusPage
   {
        /// <summary>
        /// Base URL of metrics API
        /// </summary>
        public static string Url = "https://discord.statuspage.io/";
        
        /// <summary>
        /// ID of metrics to view
        /// </summary>
        public static string MetricsId = "ztt4777v23lf";

        /// <summary>
        /// No token neccessary
        /// </summary>
        public static string Token = null;

        /// <summary>
        /// No cookies neccessary
        /// </summary>
        public static string Cookies = "";

        /// <summary>
        /// Get basic Status of Discord Servers
        /// </summary>
        /// <returns>Basic status of Discord Servers</returns>
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

        /// <summary>
        /// Gets the server response-time metrics
        /// </summary>
        /// <param name="duration">The time frame to look at metrics from</param>
        /// <returns>Repsonse Time Metrics</returns>
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

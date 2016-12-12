using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TravelBot.JsonDeserialization
{

    public class DestinationValue
    {
        public string id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public string urlPingSuffix { get; set; }
        public List<About> about { get; set; }
        public string displayUrl { get; set; }
        public string snippet { get; set; }
        public List<DeepLink> deepLinks { get; set; }
        public string dateLastCrawled { get; set; }
    }

    [JsonObject]
    public class DestinationResult
    {
        public string _type { get; set; }
        public Instrumentation instrumentation { get; set; }
        public WebPages webPages { get; set; }
        public Images images { get; set; }
        public News news { get; set; }
        public RelatedSearches relatedSearches { get; set; }
        public Videos videos { get; set; }
    }
}
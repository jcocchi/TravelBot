﻿using Newtonsoft.Json;
using System.Collections.Generic;

namespace TravelBot.JsonDeserialization
{
    public class Instrumentation
    {
        public string pingUrlBase { get; set; }
        public string pageLoadPingUrl { get; set; }
    }

    public class Thumbnail
    {
        public string contentUrl { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    public class Image
    {
        public Thumbnail thumbnail { get; set; }
    }

    public class About
    {
        public string readLink { get; set; }
        public string name { get; set; }
    }

    public class Provider
    {
        public string _type { get; set; }
        public string name { get; set; }
    }

    public class Value
    {
        public string name { get; set; }
        public string url { get; set; }
        public string urlPingSuffix { get; set; }
        public Image image { get; set; }
        public string description { get; set; }
        public List<About> about { get; set; }
        public List<Provider> provider { get; set; }
        public string datePublished { get; set; }
        public string category { get; set; }
    }

    [JsonObject]
    public class NewsResult
    {
        public string _type { get; set; }
        public Instrumentation instrumentation { get; set; }
        public string readLink { get; set; }
        public int totalEstimatedMatches { get; set; }
        public List<Value> value { get; set; }
    }
}
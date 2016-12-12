using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TravelBot.JsonDeserialization
{
    public class DeepLink
    {
        public string name { get; set; }
        public string url { get; set; }
        public string urlPingSuffix { get; set; }
    }

    public class WebPages
    {
        public string webSearchUrl { get; set; }
        public string webSearchUrlPingSuffix { get; set; }
        public int totalEstimatedMatches { get; set; }
        public List<DestinationValue> value { get; set; }
    }

    public class Value2
    {
        public string name { get; set; }
        public string webSearchUrl { get; set; }
        public string webSearchUrlPingSuffix { get; set; }
        public string thumbnailUrl { get; set; }
        public string datePublished { get; set; }
        public string contentUrl { get; set; }
        public string hostPageUrl { get; set; }
        public string hostPageUrlPingSuffix { get; set; }
        public string contentSize { get; set; }
        public string encodingFormat { get; set; }
        public string hostPageDisplayUrl { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public Thumbnail thumbnail { get; set; }
    }

    public class Images
    {
        public string id { get; set; }
        public string readLink { get; set; }
        public string webSearchUrl { get; set; }
        public string webSearchUrlPingSuffix { get; set; }
        public bool isFamilyFriendly { get; set; }
        public List<Value2> value { get; set; }
        public bool displayShoppingSourcesBadges { get; set; }
        public bool displayRecipeSourcesBadges { get; set; }
    }

    public class News
    {
        public string id { get; set; }
        public string readLink { get; set; }
        public List<NewsValue> value { get; set; }
    }

    public class Value4
    {
        public string text { get; set; }
        public string displayText { get; set; }
        public string webSearchUrl { get; set; }
        public string webSearchUrlPingSuffix { get; set; }
    }

    public class RelatedSearches
    {
        public string id { get; set; }
        public List<Value4> value { get; set; }
    }

    public class Publisher
    {
        public string name { get; set; }
    }

    public class Value5
    {
        public string name { get; set; }
        public string description { get; set; }
        public string webSearchUrl { get; set; }
        public string webSearchUrlPingSuffix { get; set; }
        public string thumbnailUrl { get; set; }
        public string datePublished { get; set; }
        public List<Publisher> publisher { get; set; }
        public string contentUrl { get; set; }
        public string hostPageUrl { get; set; }
        public string hostPageUrlPingSuffix { get; set; }
        public string encodingFormat { get; set; }
        public string hostPageDisplayUrl { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public string duration { get; set; }
        public string motionThumbnailUrl { get; set; }
        public string embedHtml { get; set; }
        public bool allowHttpsEmbed { get; set; }
        public int viewCount { get; set; }
        public Thumbnail thumbnail { get; set; }
    }

    public class Videos
    {
        public string id { get; set; }
        public string readLink { get; set; }
        public string webSearchUrl { get; set; }
        public string webSearchUrlPingSuffix { get; set; }
        public bool isFamilyFriendly { get; set; }
        public List<Value5> value { get; set; }
        public string scenario { get; set; }
    }

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
}
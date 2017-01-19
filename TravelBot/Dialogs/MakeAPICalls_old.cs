using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;

namespace TravelBot.Dialogs
{
    [Serializable]
    internal class MakeAPICalls
    {
        public async Task<object> CallSearchAPI(string location, string searchType)
        {
            var client = new HttpClient();   
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", WebConfigurationManager.AppSettings["CogServices:NewsSearch:ID"]);

            // Request parameters
            queryString["count"] = "4";
            queryString["offset"] = "0";
            queryString["mkt"] = "en-US";
            queryString["safeSearch"] = "Moderate";
            string uri = null;
            if (searchType == "news")
            {
                queryString["q"] = "Travel related news about " + location;
                uri = "https://api.cognitive.microsoft.com/bing/v5.0/news/search?" + queryString;

            }
            else if (searchType == "destination")
            {
                queryString["q"] = "Travel desitnations in " + location;
                uri = "https://api.cognitive.microsoft.com/bing/v5.0/search?" + queryString;
            }
            var response = await client.GetAsync(uri);

            HttpContent resultContent = response.Content;
            var result = await resultContent.ReadAsStringAsync();

            // Populate a JSON object with the results of the API call
            if (searchType == "news")
            {
                NewsResult news = new NewsResult();
                JsonConvert.PopulateObject(result, news);
                return news;
            }
            else if (searchType == "destination")
            {
                DestinationResult destination = new DestinationResult();
                JsonConvert.PopulateObject(result, destination);
                return destination;
            }

            return null;
        }

        public async Task<object> CallWeatherAPI(string location, DateTime date)
        {
            // Format the location
            var formatLoc = location.ToLower();
            formatLoc = formatLoc.Replace(" ", "_");

            // Decide which API to call
            string type;
            var uri = "http://api.wunderground.com/api/99739e85768e55e2/";
            var futureDate = DateTime.UtcNow.AddDays(10);
            if (date <= futureDate)
            {
                uri += String.Format("forecast10day/q/CA/{0}.json", formatLoc);
                type = "10day";
            }
            else
            {
                // Determine month1 and day1 format
                var month1 = date.Month.ToString();
                var day1 = date.Day.ToString();
                if (date.Month < 10)
                {
                    month1 = "0" + month1;
                }
                if (date.Day < 10)
                {
                    day1 = "0" + day1;
                }
                // Determine month2 and day2 format
                var date2 = date.AddDays(29);
                var month2 = date2.Month.ToString();
                var day2 = date2.Day.ToString();
                if (date2.Month < 10)
                {
                    month2 = "0" + month2;
                }
                if (date2.Day < 10)
                {
                    day2 = "0" + day2;
                }

                // Determine date range
                var range = String.Format("{0}{1}{2}{3}",
                                month1, day1, month2, day2);
                uri += String.Format("planner_{0}/q/CA/{1}.json", range, formatLoc);
                type = "average";
            }

            var client = new HttpClient();
            var response = await client.GetAsync(uri);

            HttpContent resultContent = response.Content;
            var result = await resultContent.ReadAsStringAsync();

            if (type == "10day")
            {
                TenDayForecastResult forecast = new TenDayForecastResult();
                JsonConvert.PopulateObject(result, forecast);
                return forecast;
            }
            else
            {
                AverageForecastResult forecast = new AverageForecastResult();
                JsonConvert.PopulateObject(result, forecast);
                return forecast;
            }
        }
    }
}
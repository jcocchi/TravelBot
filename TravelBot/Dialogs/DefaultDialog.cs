using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System.Web.Configuration;
using System.Net.Http;
using System.Web;
using TravelBot.JsonDeserialization;
using Newtonsoft.Json;

namespace TravelBot.Dialogs
{
    [Serializable]
    [LuisModel("138ad2b9-9738-4871-ba7a-93c1121aff70", "efe598955c1e4f4f93a89e018d468e0a")]
    public class DefaultDialog : LuisDialog<object>
    {
        private static string luisCountry = "builtin.geography.country";
        private static string luisState = "builtin.geography.us_state";
        private static string luisCity = "builtin.geography.city";

        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Sorry, I don't understand what you said.");
            context.Wait(MessageReceived);
        }

        [LuisIntent("Greeting")]
        public async Task Greeting(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Hello! How can I assist you with your travels today?");
            context.Wait(MessageReceived);
        }

        [LuisIntent("FindDesitnation")]
        public async Task FindDestination(IDialogContext context, LuisResult result)
        {
            var location = GetLocation(new List<EntityRecommendation>(result.Entities));

            // If the user specified a location
            if (location != null)
            {
                await context.PostAsync("You want to travel to " + location.Entity + "!");
                context.Wait(MessageReceived);
            }
            else // The user needs to enter a location before getting destination suggestions
            {
                var prompt = "What country, state, or city would you like to find travel suggestions for?";
                PromptDialog.Text(context, HandleDestinationSearch, prompt);
            }
        }

        private async Task HandleDestinationSearch(IDialogContext context, IAwaitable<string> result)
        {
            await context.PostAsync("I am handling your request to search locations near " + result.GetAwaiter().GetResult());
            context.Wait(MessageReceived);
        }

        [LuisIntent("GetNews")]
        public async Task GetNews(IDialogContext context, LuisResult result)
        {
            var location = GetLocation(new List<EntityRecommendation>(result.Entities));

            // If the user specified a location
            if (location != null)
            {
                await context.PostAsync("You want news from " + location.Entity + "!");
                // Now that we have the location to search for news about, we are ready to call the API
                HandleNewsSearch(context, location.Entity);
            }
            else // The user needs to enter a location before getting destination suggestions
            {
                var prompt = "What country, state, or city would you like to find news for?";
                PromptDialog.Text(context, GetNewsLocation, prompt);
            }
        }

        private async Task GetNewsLocation(IDialogContext context, IAwaitable<string> result)
        {
            await context.PostAsync("I am handling your request to get news from " + result.GetAwaiter().GetResult().ToString());

            // Now that we have the location to search for news about, we are ready to call the API
            HandleNewsSearch(context, result.GetAwaiter().GetResult().ToString());
        }

        private async void HandleNewsSearch(IDialogContext context, string location)
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", WebConfigurationManager.AppSettings["CogServices:NewsSearch:ID"]);

            // Request parameters
            queryString["q"] = "News about " + location;
            queryString["count"] = "10";
            queryString["offset"] = "0";
            queryString["mkt"] = "en-US";
            queryString["safeSearch"] = "Moderate";
            var uri = "https://api.cognitive.microsoft.com/bing/v5.0/news/search?" + queryString;
            var response = await client.GetAsync(uri);

            HttpContent resultContent = response.Content;
            var result = await resultContent.ReadAsStringAsync();

            // Populate a JSON object with the results of the API call
            NewsResult news = new NewsResult();
            JsonConvert.PopulateObject(result, news);

            await context.PostAsync("I have some responses to your search.");
            context.Wait(MessageReceived);
        }

        [LuisIntent("GetWeather")]
        public async Task GetWeather(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Checking the weather...");
            context.Wait(MessageReceived);
        }

        private EntityRecommendation GetLocation(List<EntityRecommendation> entities)
        {
            // Find the location the user specified
            EntityRecommendation location = null;
            if (entities.Any((entity) => entity.Type == luisCountry))
            {
                location = entities.Where((entity) => entity.Type == luisCountry).First();
            }
            else if (entities.Any((entity) => entity.Type == luisState))
            {
                location = entities.Where((entity) => entity.Type == luisState).First();
            }
            else if (entities.Any((entity) => entity.Type == luisCity))
            {
                location = entities.Where((entity) => entity.Type == luisCity).First();
            }

            return location;
        }
    }
}
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
using System.Threading;
using static TravelBot.Dialogs.WeatherDialog;

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

            await context.PostAsync("Hello! How can I assist you with your travels today?\n" +
                                        "- Search for a travel destination\n" +
                                        "- Check the weather in a location\n" +
                                        "- Check the current news in a location\n");
            context.Wait(MessageReceived);
        }

        [LuisIntent("FindDesitnation")]
        public async Task FindDestination(IDialogContext context, LuisResult result)
        {
            // We are only looking at the first intent in the array because that represents this function
            if (result.Entities.Count == 1)
            {
                // We have all entities we need because the action was triggered
                // Get the location and call the API
                var destination = await CallSearchAPI(GetLocation(new List<EntityRecommendation>(result.Entities)), "destination");

                // Create and display the news results
                await context.PostAsync(MakeDestinationCards(context, (DestinationResult)destination));
                context.Wait(MessageReceived);
            }
            else
            {
                // We need to prompt the user for a location
                var prompt = "What country, state, or city would you like to find travel suggestions for?";
                PromptDialog.Text(context, HandleDestinationSearch, prompt);
            }
        }

        private async Task HandleDestinationSearch(IDialogContext context, IAwaitable<string> result)
        {
            var location = await result;
            await context.PostAsync("I am handling your request to search for destinations near " + location + ".");

            // Now that we have the location to search for news about, call the API
            var destinations = await CallSearchAPI(location, "destination");

            // Create and display the destination results
            await context.PostAsync(MakeDestinationCards(context, (DestinationResult)destinations));
            context.Wait(MessageReceived);
        }

        private IMessageActivity MakeDestinationCards(IDialogContext context, DestinationResult dest)
        {
            var resultMessage = context.MakeMessage();
            resultMessage.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            resultMessage.Attachments = new List<Attachment>();
            foreach (var item in dest.webPages.value)
            {
                var action = new CardAction();
                action.Type = "openUrl";
                action.Value = item.url;
                HeroCard heroCard = new HeroCard()
                {
                    Title = item.name,
                    Text = item.snippet,
                    Tap = action,
                };

                resultMessage.Attachments.Add(heroCard.ToAttachment());
            }

            return resultMessage;
        }

        [LuisIntent("GetNews")]
        public async Task GetNews(IDialogContext context, LuisResult result)
        {
            // We are only looking at the first intent in the array because that represents this function
            // We know there is only one action for this function to get the news
            if (result.Intents[0].Actions[0].Triggered.Equals(true))
            {
                // We have all entities we need because the action was triggered
                // Get the location and call the API
                var news = await CallSearchAPI(GetLocation(new List<EntityRecommendation>(result.Entities)), "news");

                // Create and display the news results
                await context.PostAsync(MakeNewsCards(context, (NewsResult)news));
                context.Wait(MessageReceived);
            }
            else
            {
                // We need to prompt the user for a location
                var prompt = "What country, state, or city would you like to find news for?";
                PromptDialog.Text(context, HandleNewsSearch, prompt);
            }
        }

        private async Task HandleNewsSearch(IDialogContext context, IAwaitable<string> result)
        {
            var location = await result;
            await context.PostAsync("I am handling your request to get news from " + location + ".");

            // Now that we have the location to search for news about, call the API
            var news = await CallSearchAPI(location, "news");

            // Create and display the news results
            await context.PostAsync(MakeNewsCards(context, (NewsResult)news));
            context.Wait(MessageReceived);
        }

        private IMessageActivity MakeNewsCards(IDialogContext context, NewsResult news)
        {
            var resultMessage = context.MakeMessage();
            resultMessage.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            resultMessage.Attachments = new List<Attachment>();
            foreach (var item in news.value)
            {
                var action = new CardAction();
                action.Type = "openUrl";
                action.Value = item.url;
                HeroCard heroCard = new HeroCard()
                {
                    Title = item.name,
                    Text = item.description,
                    Images = new List<CardImage>()
                    {
                        new CardImage() { Url = item.image.thumbnail.contentUrl }
                    },
                    Tap = action,
                };

                resultMessage.Attachments.Add(heroCard.ToAttachment());
            }

            return resultMessage;
        }

        [LuisIntent("GetWeather")]
        public async Task GetWeather(IDialogContext context, LuisResult result)
        {
            var entities = new List<EntityRecommendation>(result.Entities);

            // We are only looking at the first intent in the array because that represents this function
            // We know there is only one action for this function to search the weather
            if (result.Intents[0].Actions[0].Triggered.Equals(true))
            {
                // We have all entities we need because the action was triggered
                // Get the location and call the API
                //var weather = await CallWeatherAPI(GetLocation(entities), getDate(entities), "news");

                // Create and display the news results
                //await context.PostAsync(MakeWeatherCards(context, (WeatherResult)weather));
                context.Wait(MessageReceived);
            }
            else
            {
                // We need to prompt the user for a location, a date or both
                await context.PostAsync("Let me gather some extra info for you");
                context.Call<Weather>(new WeatherDialog(entities), HandleWeatherSearch); 
            }
        }

        private async Task HandleWeatherSearch(IDialogContext context, IAwaitable<Weather> result)
        {
            var weather = await result; 
            await context.PostAsync("I am handling your weather search for weather in " + weather.Location + " on " + weather.Date);

            context.Wait(MessageReceived);
        }

        private string GetLocation(List<EntityRecommendation> entities)
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

            return location.Entity;
        }

        private async Task<object> CallSearchAPI(string location, string searchType)
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
    }
}